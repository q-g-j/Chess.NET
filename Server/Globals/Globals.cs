using Server.Models;

namespace Server
{
    public static class Globals
    {
        public static PlayerDBContext? PlayerDBContext;

        public static Dictionary<int, List<int>> InvitationsDict = new();
    }
}
