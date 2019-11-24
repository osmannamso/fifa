using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fifa.Data;
using fifa.Models;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly ClubsContext _dbContext;

        public ClubController(ClubsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ClubController()
        {
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            Club club = _dbContext.Clubs.FirstOrDefault(c => c.Id == 451);
            Console.WriteLine(club.Name);
            
            return new string[] {"Barca", "Real"};
        }

        [HttpGet("{id}")]
        public Club GetById(int id)
        {
            Club club = _dbContext.Clubs.FirstOrDefault(c => c.Id == id);

            return club;
        }
    }
}