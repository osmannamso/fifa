using Microsoft.AspNetCore.Identity;

namespace fifa.Models { 
    public class User {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    } 
}