namespace API.Models
{
    public class League
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public bool Enabled { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public ICollection<TeamLeague> TeamLeagues { get; set; } = new List<TeamLeague>();
    }
}
