namespace API.DTOs.Input
{
    public class TeamInput
    {
        public required string Name { get; set; }
        public required string Country { get; set; }
        public int PlayersQuantity { get; set; }
    }
}
