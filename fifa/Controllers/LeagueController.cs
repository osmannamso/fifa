using fifa.Models;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Controllers
{
    public class LeagueController : Controller
    {
        private IRepository repository;

        public LeagueController(IRepository r)
        {
            repository = r;
        }
        // GET
        public IActionResult Index()
        {
            return View(repository.GetAll());
        }
    }
}