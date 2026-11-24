namespace API.DTOs.Output
{
    public class TeamLeagueOutput
    {
        public required int Id {  get; set; }
        public required string Name { get; set; }
        public ICollection<string> Leagues { get; set; } = [];
    }
}
