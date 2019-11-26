using System.Linq;
using fifa.Data;
using fifa.Models;
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

        [HttpGet("{id}")]
        public Club GetById(int id)
        {
            Club club = _dbContext.Clubs.FirstOrDefault(c => c.Id == id);

            return club;
        }
    }
}