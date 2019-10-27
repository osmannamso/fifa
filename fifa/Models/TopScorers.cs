namespace fifa.Models
{
    public class TopScorers
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Count { get; set; }
        public string League { get; set; }
        public string Season { get; set; }
    }
}