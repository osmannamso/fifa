using System.Text;

namespace fifa.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public int? Place { get; set; }
    }
}