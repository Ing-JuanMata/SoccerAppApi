namespace API.DTOs.Input
{
    public class TeamInput
    {
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required int PlayersQuantity { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
