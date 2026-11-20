namespace API.DTOs.Output
{
    public class LeagueOutput
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required bool Enabled { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required DateTime LastUpdated { get; set; }
    }
}
