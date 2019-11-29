using System;
using System.Collections.Generic;
using System.Linq;
using fifa.Data;
using fifa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Controllers
{
    [ApiController]
    [Route("api/club")]
    public class ClubController : ControllerBase
    {
        private readonly ClubsContext _dbContext;

        public ClubController(ClubsContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{name}")]
        public IEnumerable<Club> Search(string name)
        {
            var clubs = _dbContext.Clubs.Where(c => c.Name.StartsWith(name)).Take(10);

            return clubs;
        }
        
        [HttpGet("set/{club}")]
        public String SetClub(string club)
        {
            HttpContext.Session.SetString("Club", club);

            return club;
        }

        [CheckHeaderFilter]
        [HttpGet("get/")]
        public String GetClub()
        {
            Console.WriteLine("ASDASD");
            Console.WriteLine(HttpContext.Session.GetString("Club"));
            return HttpContext.Session.GetString("Club");
        }
    }
}