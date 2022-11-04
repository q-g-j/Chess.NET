namespace Server.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int InactiveCounter { get; set; } = 0;
        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
