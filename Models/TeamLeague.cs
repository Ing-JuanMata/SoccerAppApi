namespace API.Models
{
    public class TeamLeague
    {
        public int IdTeam { get; set; }
        public int IdLeague { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastUpdated { get; set; }
        public required Team Team { get; set; }
        public required League League { get; set; }
    }
}
