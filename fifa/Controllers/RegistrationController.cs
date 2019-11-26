using System;
using fifa.Data;
using fifa.Models;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Controllers
{
    [ApiController]
    [Route("api/registration")]
    public class RegistrationController
    {
        private readonly ClubsContext _dbContext;

        public RegistrationController(ClubsContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public string Registration()
        {
            return "Yeah";
        }

        [HttpPost]
        public string Register([FromBody]User userF)
        {
            var login = userF.Login;
            var password = userF.Password;
            Console.WriteLine("asdasd");
            Console.WriteLine(login);
            var token = SecurePasswordHasher.Hash(login);
            User user = new User()
            {
                Login = login,
                Password = password,
                Token = token
            };
            _dbContext.Add(user);
            _dbContext.SaveChanges();
            return "Done";
        }
    }
}