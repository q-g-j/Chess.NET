namespace Server.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int InvitingPlayerId { get; set; }
    }
}
