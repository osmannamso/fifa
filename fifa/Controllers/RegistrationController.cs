using System.Collections.Generic;
using System.Security.Cryptography;
using fifa.Data;
using fifa.Models;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Controllers
{
    public class RegistrationController
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
            public string Register(string login, string password)
            {
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
}