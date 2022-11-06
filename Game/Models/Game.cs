namespace ChessDotNET.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int WhiteId { get; set; }
        public int BlackId { get; set; }
        public string LastMoveStartWhite { get; set; }
        public string LastMoveEndWhite { get; set; }
        public string LastMoveStartBlack { get; set; }
        public string LastMoveEndBlack { get; set; }
        public string MoveInfo { get; set; }
        public bool HasPlayerQuit { get; set; } = false;
    }
}
