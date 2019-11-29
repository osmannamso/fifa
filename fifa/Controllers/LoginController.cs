using System;
using System.Linq;
using fifa.Data;
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
            Console.WriteLine("asdasd");
            Console.WriteLine(userF);
            var login = userF.Login;
            var password = userF.Password;
            var user = _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return "No";
            }
            return user.Token;
        }
        [HttpGet]
        [CheckHeaderFilter]
        public string GetLogin(string token)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Token == token);
            return user.Login;
        }
    }
}