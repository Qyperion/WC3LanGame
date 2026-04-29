using System.Buffers.Binary;
using System.Text;

using WC3LanGame.Core.Extensions;
using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core.Warcraft3;

internal static class WarcraftPacketProcessor
{
    // All packets used in Warcraft III have 4 bytes header as below:
    //   1 / uint8  : Magic byte
    //   1 / uint8  : OP Code
    //   2 / uint16 : Packet length including this 4 bytes header

    private const byte FirstHeaderByte = 0xF7; //All LAN UDP or Game TCP packets are using this magic byte in the header

    //OP Code's
    private const byte QueryForLanGameOPCode = 0x2F; // The game sends this packet to query for LAN games
    private const byte GameInfoReplyOPCode = 0x30; // Sent when received UDP 0x2F packets, this packet contains complete game information as reply
    private const byte GameCancelledOPCode = 0x33; // This packet is broadcasted to 255.255.255.255 when a game is cancelled
    private const byte GamePlayersChangedOPCode = 0x32; // This packet is broadcasted to 255.255.255.255 when number of in-game players is changed.

    // Length of packets 
    private const ushort GameCancelledPacketLength = 8;
    private const ushort QueryForLanGamePacketLength = 16;
    private const ushort GamePlayersChangedPacketLength = 16;

    // Minimum GameInfo reply: 20 (fixed header) + 2 (name null + encoded null) + 22 (fixed footer)
    private const int MinGameInfoReplyLength = 44;

    // GameInfo reply header offsets
    private const int GameIdOffset = 12;
    private const int GameNameStartOffset = 20;

    // GameInfo reply footer offsets (from end of packet)
    private const int SlotsCountOffsetFromEnd = 22;
    private const int CurrentPlayersOffsetFromEnd = 14;
    private const int PlayerSlotsOffsetFromEnd = 10;
    private const int PortOffsetFromEnd = 2;

    // Decoded encoded data offsets
    private const int MapWidthDecodedOffset = 5;
    private const int MapHeightDecodedOffset = 7;
    private const int MapNameDecodedOffset = 13;
    private const int MinDecodedDataLength = 14;

    // Cached reversed game type identifiers (WC3 protocol stores them in reverse byte order)
    private static readonly byte[] TheFrozenThroneId = "PX3W"u8.ToArray(); // "W3XP" reversed
    private static readonly byte[] ReignOfChaosId = "3RAW"u8.ToArray();    // "WAR3" reversed

    public static byte[] GenerateGameCancelledPacket(uint gameId)
    {
        var packet = new byte[GameCancelledPacketLength];
        packet[0] = FirstHeaderByte;
        packet[1] = GameCancelledOPCode;

        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(2), GameCancelledPacketLength);
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(4), gameId);

        return packet;
    }

    public static byte[] GenerateGameAnnouncePacket(GameInfo game)
    {
        var playersCount = game.CurrentPlayersCount + game.SlotsCount - game.PlayerSlotsCount;

        var packet = new byte[GamePlayersChangedPacketLength];
        packet[0] = FirstHeaderByte;
        packet[1] = GamePlayersChangedOPCode;

        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(2), GamePlayersChangedPacketLength);
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(4), game.GameId);
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(8), playersCount);
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(12), game.SlotsCount);

        return packet;
    }

    public static byte[] GenerateQueryForLanGamesPacket(HostInfo hostInfo)
    {
        var gameTypeBytes = hostInfo.GameType switch
        {
            WarcraftType.TheFrozenThrone => TheFrozenThroneId,
            WarcraftType.ReignOfChaos => ReignOfChaosId,
            _ => throw new ArgumentOutOfRangeException(nameof(hostInfo))
        };

        var packet = new byte[QueryForLanGamePacketLength];
        packet[0] = FirstHeaderByte;
        packet[1] = QueryForLanGameOPCode;
        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(2), QueryForLanGamePacketLength);
        gameTypeBytes.CopyTo(packet.AsSpan(4));
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(8), hostInfo.Version.Id());
        // packet[12..15] = 0 (Game ID zero for broadcast) — already zeroed by new byte[]

        return packet;
    }

    public static GameInfo ParseGameInfoPacket(ReadOnlySpan<byte> replyPacket)
    {
        // Validate minimum size before accessing any indices
        if (replyPacket.Length < MinGameInfoReplyLength)
            return null;

        // Check that it is correct Warcraft packet header
        if (replyPacket[0] != FirstHeaderByte || replyPacket[1] != GameInfoReplyOPCode)
            return null;

        var gameType = ParseGameType(replyPacket[4..8]);
        if (gameType == null)
            return null;

        var gameId = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[GameIdOffset..]);
        var slotsCount = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[^SlotsCountOffsetFromEnd..]);
        var currentPlayersCount = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[^CurrentPlayersOffsetFromEnd..]);
        var playerSlotsCount = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[^PlayerSlotsOffsetFromEnd..]);
        var port = BinaryPrimitives.ReadUInt16LittleEndian(replyPacket[^PortOffsetFromEnd..]);

        var name = GetStringSegment(replyPacket[GameNameStartOffset..]);

        var encodedSegmentStartIndex = GameNameStartOffset + 2 + Encoding.UTF8.GetByteCount(name);
        if (encodedSegmentStartIndex >= replyPacket.Length)
            return null;

        var decrypted = DecodeStringPart(replyPacket[encodedSegmentStartIndex..]);

        // Decoded data must contain at least: 5 bytes prefix + 2 mapWidth + 2 mapHeight + 4 bytes + mapName null
        if (decrypted.Length < MinDecodedDataLength)
            return null;

        ReadOnlySpan<byte> decryptedSpan = decrypted;
        var mapWidth = BinaryPrimitives.ReadUInt16LittleEndian(decryptedSpan[MapWidthDecodedOffset..]);
        var mapHeight = BinaryPrimitives.ReadUInt16LittleEndian(decryptedSpan[MapHeightDecodedOffset..]);

        var mapName = GetStringSegment(decryptedSpan[MapNameDecodedOffset..]);
        var lastMapNameSegment = mapName.Split('\\').LastOrDefault();
        mapName = lastMapNameSegment ?? mapName;

        return new GameInfo(gameId, name, port, gameType.Value, slotsCount, currentPlayersCount, playerSlotsCount,
            mapName, mapWidth, mapHeight);

        // Compare directly against cached reversed bytes — avoids string allocation and Reverse()
        static WarcraftType? ParseGameType(ReadOnlySpan<byte> gameTypeSegment)
        {
            if (gameTypeSegment.SequenceEqual(TheFrozenThroneId))
                return WarcraftType.TheFrozenThrone;
            if (gameTypeSegment.SequenceEqual(ReignOfChaosId))
                return WarcraftType.ReignOfChaos;
            return null;
        }
    }

    // Get first null-terminated string from byte span 
    private static string GetStringSegment(ReadOnlySpan<byte> data)
    {
        var firstZeroIndex = data.IndexOf((byte)0);
        if (firstZeroIndex == -1)
            return "";

        return Encoding.UTF8.GetString(data[..firstZeroIndex]);
    }

    // Decode encoded string part. This algorithm is described in 3b part of GamePacketSpecs doc.
    // Calculates exact output size upfront to avoid a second allocation from slicing.
    private static byte[] DecodeStringPart(ReadOnlySpan<byte> data)
    {
        var firstZeroIndex = data.IndexOf((byte)0);
        if (firstZeroIndex == -1)
            return [];

        var dataCut = data[..firstZeroIndex];

        // Every 8th byte (starting at 0) is a mask byte, rest is data
        var maskCount = (dataCut.Length + 7) / 8;
        var decodedSize = dataCut.Length - maskCount;
        var result = new byte[decodedSize];
        var writeIndex = 0;
        byte mask = 0;

        for (var i = 0; i < dataCut.Length; i++)
        {
            var b = dataCut[i];

            if (i % 8 != 0)
            {
                result[writeIndex++] = (mask & (1 << (i % 8))) == 0
                    ? (byte)(b - 1)
                    : b;
            }
            else
                mask = b;
        }

        return result;
    }
}
