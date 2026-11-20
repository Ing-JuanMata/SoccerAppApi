namespace API.Models
{
    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required int PlayersQuantity { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastUpdated { get; set; }
        public ICollection<TeamLeague> TeamLeagues { get; set; } = new List<TeamLeague>();
    }
}
