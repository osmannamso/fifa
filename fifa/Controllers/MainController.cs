using System;
using System.Collections.Generic;
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

        public IActionResult PlaySeason(string season)
        {
            var s = new Season()
            {
                Name = season
            };
            _dbContext.Entry(s).State = EntityState.Added;
            _dbContext.SaveChanges();
            SeasonPlay(s);
            
            return View();
        }

        public IActionResult Bombardiers(string season, string league)
        {
            var scorers = _dbContext.TopScorers.Where(t => t.League == league && t.Season == season).OrderBy(t => -t.Count).Take(20).ToList();
            List<TopScorersView> topscorers = new List<TopScorersView>();
            foreach (var scorer in scorers)
            {
                var player = _dbContext.Players.FirstOrDefault(p => p.Id == scorer.PlayerId);
                var club = _dbContext.Clubs.FirstOrDefault(c => c.Id == player.ClubId);
                TopScorersView topview = new TopScorersView()
                {
                    Name = player.Name,
                    Photo = player.Photo,
                    Club = club.Name,
                    Logo = club.Logo,
                    Count = scorer.Count
                };
                topscorers.Add(topview);
            }

            return View(topscorers);
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

        public async Task<IActionResult> Players(int id)
        {
            var players = _dbContext.Players.Where(c => c.ClubId == id).ToList();
            return View(players);
        }
        
        public void Play(Club a, Club b, string Season="Training", string League="Training", bool notFinal=true)
        {
            int homePlus = 0;
            if (notFinal)
            {
                homePlus += 3;
            }
            Random rnd = new Random();
            var aPlayers = _dbContext.Players.Where(p => p.ClubId == a.Id).OrderBy(p => -p.Score).Take(11).ToList();
            var bPlayers = _dbContext.Players.Where(p => p.ClubId == b.Id).OrderBy(p => -p.Score).Take(11).ToList();
            int aAvg, bAvg, sumA = 0, sumB = 0;
            string []defenders = new string[7]{"RB", "RWB", "CB", "LB", "LWB", "LCB", "RCB"};
            string[] midlefields = new string[13] {"CDM", "CM", "CAM", "RM", "RW", "LM", "LW", "LCM", "RCM", "LDM", "RAM", "LAM", "RDM"};
            string[] attackers = new string[6] {"CF", "RF", "LF", "ST", "LS", "RS"};
            // A Team
            var aAttackers = new List<Player>();
            var aMidleFields = new List<Player>();;
            var aDefenders = new List<Player>();;
            foreach (var aPlayer in aPlayers)
            {
                sumA += aPlayer.Score;
                if (attackers.Contains(aPlayer.Position))
                {
                    aAttackers.Add(aPlayer);
                } else if (midlefields.Contains(aPlayer.Position))
                {
                    aMidleFields.Add(aPlayer);
                } else if (defenders.Contains(aPlayer.Position))
                {
                    aDefenders.Add(aPlayer);
                }
            }
            var aPlayingClub = aAttackers.Concat(aMidleFields).ToList();
            aPlayingClub = aPlayingClub.Concat(aDefenders).ToList();
            aAvg = sumA / 11;
            // B Team
            var bAttackers = new List<Player>();
            var bMidleFields = new List<Player>();
            var bDefenders = new List<Player>();
            foreach (var bPlayer in bPlayers)
            {
                sumB += bPlayer.Score;
                if (attackers.Contains(bPlayer.Position))
                {
                    bAttackers.Add(bPlayer);
                }else if (midlefields.Contains(bPlayer.Position))
                {
                    bMidleFields.Add(bPlayer);
                }else if (defenders.Contains(bPlayer.Position))
                {
                    bDefenders.Add(bPlayer);
                }
            }
            var bPlayingClub = bAttackers.Concat(bMidleFields).ToList();
            bPlayingClub = bPlayingClub.Concat(bDefenders).ToList();
            bAvg = sumB / 11;

            string aGoals = "";
            string bGoals = "";
            int aScore = 0;
            int bScore = 0;

            for (int i = 0; i < 90; i++)
            {
                if (rnd.Next(1, 41 - Math.Abs(aAvg - bAvg)) == 1)
                {
                    if (rnd.Next(aAvg, 101 + homePlus) > rnd.Next(bAvg, 101))
                    {
                        foreach (var aPlayer in aPlayingClub)
                        {
                            if (rnd.Next(0, 2) == 1)
                            {
                                aGoals += aPlayer.Name + " " + i + ", ";
                                aScore++;
                                if (_dbContext.TopScorers.FirstOrDefault(c => c.PlayerId == aPlayer.Id && c.League == League && c.Season == Season) != null)
                                {
                                    var topScorer = _dbContext.TopScorers.FirstOrDefault(c =>
                                        c.PlayerId == aPlayer.Id && c.League == League && c.Season == Season);
                                    topScorer.Count++;
                                    _dbContext.Update(topScorer);
                                    _dbContext.SaveChanges();
                                }
                                else
                                {
                                    var topScorer = new TopScorers()
                                    {
                                        PlayerId = aPlayer.Id,
                                        Count = 1,
                                        League = League,
                                        Season = Season
                                    };
                                    _dbContext.Entry(topScorer).State = EntityState.Added;
                                    _dbContext.SaveChanges();
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var bPlayer in bPlayingClub)
                        {
                            if (rnd.Next(0, 2) == 1)
                            {
                                bGoals += bPlayer.Name + " " + i + ", ";
                                bScore++;
                                if (_dbContext.TopScorers.FirstOrDefault(c => c.PlayerId == bPlayer.Id && c.League == League && c.Season == Season) != null)
                                {
                                    var topScorer = _dbContext.TopScorers.FirstOrDefault(c =>
                                        c.PlayerId == bPlayer.Id && c.League == League && c.Season == Season);
                                    topScorer.Count++;
                                    _dbContext.Update(topScorer);
                                    _dbContext.SaveChanges();
                                }
                                else
                                {
                                    var topScorer = new TopScorers()
                                    {
                                        PlayerId = bPlayer.Id,
                                        Count = 1,
                                        League = League,
                                        Season = Season
                                    };
                                    _dbContext.Entry(topScorer).State = EntityState.Added;
                                    _dbContext.SaveChanges();
                                }
                                break;
                            }
                        }
                    }
                }
            }
            string winner;
            if (aScore > bScore)
            {
                winner = a.Name;
            }else if (bScore > aScore)
            {
                winner = b.Name;
            }
            else
            {
                winner = "Draw";
            }
            var game = new Game()
            {
                HomeClub = a.Name,
                GuestClub = b.Name,
                League = League,
                Season = Season,
                HomeGoals = aGoals,
                GuestGoals = bGoals,
                HomeScore = aScore,
                GuestScore = bScore,
                Winner = winner
            };
            _dbContext.Entry(game).State = EntityState.Added;
            _dbContext.SaveChanges();
        }

        public void LeaguePlay(League league, Season season)
        {
            var clubs = _dbContext.Clubs.Where(c => c.LeagueId == league.Id).ToList();
            foreach (var aClub in clubs)
            {
                foreach (var bClub in clubs)
                {
                    if (aClub.Id != bClub.Id)
                    {
                        Play(aClub, bClub, season.Name, league.Name);
                    }
                }
            }
        }

        public void SeasonPlay(Season season)
        {
            var leagues = _dbContext.Leagues.ToList();
            foreach (var league in leagues)
            {
                LeaguePlay(league, season);
            }
        }
        
        public async Task<IActionResult> Results(string league, string season, int leagueId)
        {
            List<Results> results = new List<Results>();
            var clubs = _dbContext.Clubs.Where(c => c.LeagueId == leagueId).ToList();
            foreach (var club in clubs)
            {
                var games = _dbContext.Games.Where(g =>
                    g.Season == season && g.League == league && (g.HomeClub == club.Name || g.GuestClub == club.Name));
                var result = new Results()
                {
                    Games = 0,
                    Wins = 0,
                    Draws = 0,
                    Loses = 0,
                    ScoredGoals = 0,
                    MissedGoals = 0,
                    GoalDifference = 0,
                    Score = 0,
                    Club = club.Name,
                    Logo = club.Logo
                };
                foreach (var game in games)
                {
                    result.Games++;
                    if (game.Winner == club.Name)
                    {
                        result.Wins++;
                        result.Score += 3;
                    }else if (game.Winner == "Draw")
                    {
                        result.Draws++;
                        result.Score++;
                    }
                    else
                    {
                        result.Loses++;
                    }

                    if (game.HomeClub == club.Name)
                    {
                        result.ScoredGoals += game.HomeScore;
                        result.MissedGoals += game.GuestScore;
                    }
                    else if (game.GuestClub == club.Name)
                    {
                        result.ScoredGoals += game.GuestScore;
                        result.MissedGoals += game.HomeScore;
                    }
                }

                result.GoalDifference = result.ScoredGoals - result.MissedGoals;
                results.Add(result);
            }

            for (int i = 0; i < results.Count; i++)
            {
                for (int j = i; j < results.Count; j++)
                {
                    if (results[i].Score < results[j].Score)
                    {
                        Results r = results[i];
                        results[i] = results[j];
                        results[j] = r;
                    }else if (results[i].Score == results[j].Score)
                    {
                        if (results[i].ScoredGoals < results[j].ScoredGoals)
                        {
                            Results r = results[i];
                            results[i] = results[j];
                            results[j] = r;
                        }
                    }
                }
            }
            return View(results);
        }
    }
}