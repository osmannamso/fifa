using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using fifa.Data;
using fifa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fifa.Controllers
{
    public class MainController : Controller
    {
        private readonly ClubsContext _dbContext;
        public MainController(ClubsContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Clubs(int page)
        {
            int jump = 12;
            var clubs = await _dbContext.Clubs.Skip(jump * (page - 1)).Take(jump).ToListAsync();
            return View(clubs);
        }

        public ActionResult Club(int id)
        {
            var club = _dbContext.Clubs.FirstOrDefault(c => c.Id == id);
            return View(club);
        }
    }
}