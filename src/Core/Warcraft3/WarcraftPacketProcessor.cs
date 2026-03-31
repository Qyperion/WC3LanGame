using System.Buffers.Binary;
using System.Text;

using WC3LanGame.Core.Extensions;
using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core.Warcraft3
{
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

        // Cached reversed game type identifiers (WC3 protocol stores them in reverse byte order)
        private static readonly byte[] TheFrozenThroneId = "PX3W"u8.ToArray(); // "W3XP" reversed
        private static readonly byte[] ReignOfChaosId = "3RAW"u8.ToArray();    // "WAR3" reversed

        public static byte[] GenerateGameCancelledPacket(uint gameId)
        {
            byte[] packet = new byte[GameCancelledPacketLength];
            packet[0] = FirstHeaderByte;
            packet[1] = GameCancelledOPCode;
            BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(2), GameCancelledPacketLength);
            BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(4), gameId);
            return packet;
        }

        public static byte[] GenerateGameAnnouncePacket(GameInfo game)
        {
            uint playersCount = game.CurrentPlayersCount + game.SlotsCount - game.PlayerSlotsCount;

            byte[] packet = new byte[GamePlayersChangedPacketLength];
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
            byte[] gameTypeBytes = hostInfo.GameType switch
            {
                WarcraftType.TheFrozenThrone => TheFrozenThroneId,
                WarcraftType.ReignOfChaos => ReignOfChaosId,
                _ => throw new ArgumentOutOfRangeException(nameof(hostInfo))
            };

            byte[] packet = new byte[QueryForLanGamePacketLength];
            packet[0] = FirstHeaderByte;
            packet[1] = QueryForLanGameOPCode;
            BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(2), QueryForLanGamePacketLength);
            gameTypeBytes.CopyTo(packet.AsSpan(4));
            BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(8), (uint)hostInfo.Version.Id());
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

            WarcraftType? gameType = ParseGameType(replyPacket[4..8]);
            if (gameType == null)
                return null;

            uint gameId = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[12..]);
            uint slotsCount = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[^22..]);
            uint currentPlayersCount = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[^14..]);
            uint playerSlotsCount = BinaryPrimitives.ReadUInt32LittleEndian(replyPacket[^10..]);
            ushort port = BinaryPrimitives.ReadUInt16LittleEndian(replyPacket[^2..]);

            string name = GetStringSegment(replyPacket[20..]);

            int encodedSegmentStartIndex = 22 + Encoding.UTF8.GetByteCount(name);
            if (encodedSegmentStartIndex >= replyPacket.Length)
                return null;

            byte[] decrypted = DecodeStringPart(replyPacket[encodedSegmentStartIndex..]);

            // Decoded data must contain at least: 5 bytes prefix + 2 mapWidth + 2 mapHeight + 4 bytes + mapName null
            if (decrypted.Length < 14)
                return null;

            ReadOnlySpan<byte> decryptedSpan = decrypted;
            ushort mapWidth = BinaryPrimitives.ReadUInt16LittleEndian(decryptedSpan[5..]);
            ushort mapHeight = BinaryPrimitives.ReadUInt16LittleEndian(decryptedSpan[7..]);

            string mapName = GetStringSegment(decryptedSpan[13..]);
            string lastMapNameSegment = mapName.Split('\\').LastOrDefault();
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
            int firstZeroIndex = data.IndexOf((byte)0);
            if (firstZeroIndex == -1)
                return "";

            return Encoding.UTF8.GetString(data[..firstZeroIndex]);
        }

        // Decode encoded string part. This algorithm is described in 3b part of GamePacketSpecs doc.
        // Calculates exact output size upfront to avoid a second allocation from slicing.
        private static byte[] DecodeStringPart(ReadOnlySpan<byte> data)
        {
            int firstZeroIndex = data.IndexOf((byte)0);
            if (firstZeroIndex == -1)
                return [];

            ReadOnlySpan<byte> dataCut = data[..firstZeroIndex];

            // Every 8th byte (starting at 0) is a mask byte, rest is data
            int maskCount = (dataCut.Length + 7) / 8;
            int decodedSize = dataCut.Length - maskCount;
            byte[] result = new byte[decodedSize];
            int writeIndex = 0;
            byte mask = 0;

            for (int i = 0; i < dataCut.Length; i++)
            {
                byte b = dataCut[i];

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
}
