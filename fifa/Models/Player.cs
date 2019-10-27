using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace fifa.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Photo { get; set; }
        public string Nation { get; set; }
        public string Flag { get; set; }
        public int Score { get; set; }
        public string Position { get; set; }
        public int Number { get; set; }
        public int? ClubId { get; set; }
    }
}