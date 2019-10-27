using System.Collections.Generic;

namespace fifa.Models
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int LeagueId { get; set; }
    }
}