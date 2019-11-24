using System.Linq;
using fifa.Data;
using fifa.Migrations;
using fifa.Models;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController
    {
        private readonly ClubsContext _dbContext;

        public LoginController(ClubsContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public string Login([FromBody]User userF)
        {
            var login = userF.Login;
            var password = userF.Password;
            var user = _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return "Password or Login incorrect";
            }
            return user.Token;
        }
    }
}