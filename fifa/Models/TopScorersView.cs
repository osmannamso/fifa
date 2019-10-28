using System.Security.Cryptography;

namespace fifa.Models
{
    public class TopScorersView
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Club { get; set; }
        public string Logo { get; set; }
        public int Count { get; set; }
    }
}