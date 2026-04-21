using DonerApp.Models;

namespace DonerApp
{
    public static class Session
    {
        public static Employee? CurrentEmployee { get; set; }

        public static bool IsLoggedIn => CurrentEmployee != null;

        public static bool IsManager => CurrentEmployee?.Role == "manager";

        public static void Logout()
        {
            CurrentEmployee = null;
        }
    }
}