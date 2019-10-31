namespace fifa.Models
{
    public class GameView
    {
        public int Id { get; set; }
        public string HomeLogo { get; set; }
        public string GuestLogo { get; set; }
        public string HomeClub { get; set; }
        public string GuestClub { get; set; }
        public string League { get; set; }
        public string Season { get; set; }
        public string HomeGoals { get; set; }
        public string GuestGoals { get; set; }
        public int HomeScore { get; set; }
        public int GuestScore { get; set; }
        public string Winner { get; set; }
    }
}