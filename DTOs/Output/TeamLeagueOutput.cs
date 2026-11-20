namespace API.DTOs.Output
{
    public class TeamLeagueOutput
    {
        public required int IdTeam {  get; set; }
        public required string Name { get; set; }
        public ICollection<string> Leagues { get; set; } = [];
    }
}
