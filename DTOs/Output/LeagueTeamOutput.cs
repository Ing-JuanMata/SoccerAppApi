namespace API.DTOs.Output
{
    public class LeagueTeamOutput
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<string> Teams { get; set; } = [];
    }
}
