namespace API.DTOs.Input
{
    public class LeagueInput
    {
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
