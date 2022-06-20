namespace WC3LanGame.Warcraft3.Types
{
    internal record GameInfo(uint GameId, string Name, ushort Port, WarcraftType GameType, uint SlotsCount,
        uint CurrentPlayersCount, uint PlayerSlotsCount, string MapName, ushort MapWidth, ushort MapHeight)
    {
        public string MapSizeCategory => (MapWidth * MapHeight) switch
            {
                <= 13456 => "Small",
                < 26640 => "Medium",
                _ => "Huge"
            };
        
    }
}
