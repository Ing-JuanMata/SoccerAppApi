namespace API.DTOs.Output
{
    public class TeamOutput
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required int PlayersQuantity { get; set; }
        public required bool Enabled { get; set; }
        public required DateTime LastUpdated { get; set; }
    }
}
