using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using fifa.Data;
using fifa.Models;
using fifa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fifa.Controllers
{
    public class MainController : Controller
    {
        private readonly ClubsContext _dbContext;
        private MainService _mainService;
        public MainController(ClubsContext dbContext, MainService mainService)
        {
            _dbContext = dbContext;
            _mainService = mainService;
        }

        public IActionResult ValidateLogo(string Logo)
        {
            bool result = !(Logo == "https://images-na.ssl-images-amazon.com/images/I/61NWgO5keQL._SY445_.jpg" ||
                            Logo == "https://is1-ssl.mzstatic.com/image/thumb/Video113/v4/74/30/fd/7430fd41-956d-349f-fa2e-3e627623b6dd/pr_source.png/268x0w.jpg");
            if (!result)
            {
                return null;
            }
            return Json(result);
        }

        public int Zero()
        {
            return 0;
        }

        public Boolean ValidateLogoBool(string Logo)
        {
            return !(Logo == "https://images-na.ssl-images-amazon.com/images/I/61NWgO5keQL._SY445_.jpg" ||
                     Logo == "https://is1-ssl.mzstatic.com/image/thumb/Video113/v4/74/30/fd/7430fd41-956d-349f-fa2e-3e627623b6dd/pr_source.png/268x0w.jpg");
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddClub()
        {
            return View();
        }

        public async Task<IActionResult> AddClubAction(IFormCollection data)
        {
            int count = _dbContext.Clubs.OrderBy(c => -c.Id).Take(1).ToList()[0].Id;
            Console.WriteLine(count);
            var club = new Club()
            {
                Id = count + 1,
                Name = data["Name"],
                Logo = data["Logo"],
                LeagueId = Int32.Parse(data["LeagueId"])
            };
            _dbContext.Add(club);
            _dbContext.SaveChanges();

            for (int i = 1; i <= 11; i++)
            {
                var player = new Player()
                {
                    Name = data["PlayerName_" + i],
                    Age = 22,
                    Photo = "",
                    Nation = "USSR",
                    Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a9/Flag_of_the_Soviet_Union.svg/800px-Flag_of_the_Soviet_Union.svg.png",
                    Score = Int32.Parse(data["PlayerScore_" + i]),
                    Position = data["PlayerPosition_" + i],
                    Number = Int32.Parse(data["PlayerNumber_" + i]),
                    ClubId = club.Id
                };
                _dbContext.Entry(player).State = EntityState.Added;
                _dbContext.SaveChanges();
            }
            
            return View();
        }

        public IActionResult DeleteClub(int ClubId)
        {
            var club = _dbContext.Clubs.FirstOrDefault(cc => cc.Id == ClubId);
            _dbContext.Clubs.Attach(club);
            _dbContext.Clubs.Remove(club);
            _dbContext.SaveChanges();
            
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
        
        public Game Play(Club a, Club b, string Season="Training", string League="Training", bool notFinal=true)
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
                if (rnd.Next(1, 51 - Math.Abs(aAvg - bAvg)) == 1)
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

            return game;
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
                    ClubId = club.Id,
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

        public IActionResult UefaChampionsPlay(string season)
        {
            Random rnd = new Random();
            var leagues = _dbContext.Leagues.ToList();
            var clubs = new List<Club>();
            foreach (League league in leagues)
            {
                var moveClubs = GetForUEFA(league.Id, season, league.Name);
                foreach (var club in moveClubs)
                {
                    clubs.Add(club);
                }
            }

            string[] alphabet = new string[8]{"A", "B", "C", "D", "E", "F", "G", "H"};
            var groups = new List<Group>();
            foreach (var letter in alphabet)
            {
                var groupClubs = new List<Club>();
                for (int i = 0; i < 4; i++)
                {
                    int numberRnd = rnd.Next(0, clubs.Count);
                    groupClubs.Add(clubs[numberRnd]);
                    clubs.RemoveAt(numberRnd);
                }
                var results = new List<Results>();
                foreach (var aClub in groupClubs)
                {
                    foreach (var bClub in groupClubs)
                    {
                        if (aClub.Name != bClub.Name)
                        {
                            Play(aClub, bClub, season, season + letter);
                        }
                    }
                }

                foreach (var club in groupClubs)
                {
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
                    var games = _dbContext.Games.Where(g => g.Season == season && g.League == season + letter && (g.HomeClub == club.Name || g.GuestClub == club.Name)).ToList();
                    foreach (var game in games)
                    {
                        result.Games++;
                        if (game.Winner == club.Name)
                        {
                            result.Wins++;
                            result.Score += 3;
                        } else if (game.Winner == "Draw")
                        {
                            result.Draws++;
                            result.Score += 1;
                        }
                        else
                        {
                            result.Loses++;
                        }

                        if (game.HomeClub == club.Name)
                        {
                            result.ScoredGoals += game.HomeScore;
                            result.MissedGoals += game.GuestScore;
                        } else if (game.GuestClub == club.Name)
                        {
                            result.ScoredGoals += game.GuestScore;
                            result.MissedGoals += game.HomeScore;
                        }
                    }

                    result.GoalDifference = result.ScoredGoals + result.MissedGoals;
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
                
                groups.Add(new Group()
                {
                    Name = letter,
                    ResultsList = results
                });
            }
            
            List<Club> sixteenClubs = new List<Club>();
            foreach (var group in groups)
            {
                sixteenClubs.Add(_dbContext.Clubs.FirstOrDefault(c => c.Name == group.ResultsList[0].Club));
                sixteenClubs.Add(_dbContext.Clubs.FirstOrDefault(c => c.Name == group.ResultsList[1].Club));
            }
            
            List<Game> sixteen = new List<Game>();
            List<Club> eightClubs = new List<Club>();
            for (int i = 4; i <= 16; i += 4)
            {
                var playoffs = sixteenClubs;
                var aGame = Play(playoffs[i - 1], playoffs[i - 3], season, season + "sixteen");
                var bGame = Play(playoffs[i - 3], playoffs[i - 1], season, season + "sixteen");

                if (aGame.HomeScore + bGame.GuestScore > aGame.GuestScore + bGame.HomeScore)
                {
                    eightClubs.Add(playoffs[i - 1]);
                } else if (aGame.HomeScore + bGame.GuestScore < aGame.GuestScore + bGame.HomeScore)
                {
                    eightClubs.Add(playoffs[i - 3]);
                }
                else
                {
                    if (aGame.GuestScore < bGame.GuestScore)
                    {
                        eightClubs.Add(playoffs[i - 1]);    
                    } else if (aGame.GuestScore > bGame.GuestScore)
                    {
                        eightClubs.Add(playoffs[i - 3]);
                    }
                    else
                    {
                        int rndPenalty = rnd.Next(0, 2);
                        if (rndPenalty == 0)
                        {
                            eightClubs.Add(playoffs[i - 1]);
                        }
                        else
                        {
                            eightClubs.Add(playoffs[i - 3]);
                        }
                    }
                }
                
                var cGame = Play(playoffs[i - 2], playoffs[i - 4], season, season + "sixteen");
                var dGame = Play(playoffs[i - 4], playoffs[i - 2], season, season + "sixteen");
                
                if (cGame.HomeScore + dGame.GuestScore > cGame.GuestScore + dGame.HomeScore)
                {
                    eightClubs.Add(playoffs[i - 2]);
                } else if (cGame.HomeScore + dGame.GuestScore < cGame.GuestScore + dGame.HomeScore)
                {
                    eightClubs.Add(playoffs[i - 4]);
                }
                else
                {
                    if (cGame.GuestScore < dGame.GuestScore)
                    {
                        eightClubs.Add(playoffs[i - 2]);    
                    } else if (cGame.GuestScore > dGame.GuestScore)
                    {
                        eightClubs.Add(playoffs[i - 4]);
                    }
                    else
                    {
                        int rndPenalty = rnd.Next(0, 2);
                        if (rndPenalty == 0)
                        {
                            eightClubs.Add(playoffs[i - 2]);
                        }
                        else
                        {
                            eightClubs.Add(playoffs[i - 4]);
                        }
                    }
                }
                
                aGame.HomeLogo = playoffs[i - 1].Logo;
                bGame.HomeLogo = playoffs[i - 3].Logo;
                aGame.GuestLogo = playoffs[i - 3].Logo;
                bGame.GuestLogo = playoffs[i - 1].Logo;
                
                cGame.HomeLogo = playoffs[i - 2].Logo;
                dGame.HomeLogo = playoffs[i - 4].Logo;
                cGame.GuestLogo = playoffs[i - 4].Logo;
                dGame.GuestLogo = playoffs[i - 2].Logo;

                sixteen.Add(aGame);
                sixteen.Add(bGame);
                sixteen.Add(cGame);
                sixteen.Add(dGame);
            }
            
            List<Game> eight = new List<Game>();
            List<Club> fourClubs = new List<Club>();
            for (int i = 4; i <= 8; i += 4)
            {
                var playoffs = eightClubs;
                var aGame = Play(playoffs[i - 1], playoffs[i - 3], season, season + "eight");
                var bGame = Play(playoffs[i - 3], playoffs[i - 1], season, season + "eight");

                if (aGame.HomeScore + bGame.GuestScore > aGame.GuestScore + bGame.HomeScore)
                {
                    fourClubs.Add(playoffs[i - 1]);
                } else if (aGame.HomeScore + bGame.GuestScore < aGame.GuestScore + bGame.HomeScore)
                {
                    fourClubs.Add(playoffs[i - 3]);
                }
                else
                {
                    if (aGame.GuestScore < bGame.GuestScore)
                    {
                        fourClubs.Add(playoffs[i - 1]);    
                    } else if (aGame.GuestScore > bGame.GuestScore)
                    {
                        fourClubs.Add(playoffs[i - 3]);
                    }
                    else
                    {
                        int rndPenalty = rnd.Next(0, 2);
                        if (rndPenalty == 0)
                        {
                            fourClubs.Add(playoffs[i - 1]);
                        }
                        else
                        {
                            fourClubs.Add(playoffs[i - 3]);
                        }
                    }
                }
                
                var cGame = Play(playoffs[i - 2], playoffs[i - 4], season, season + "eight");
                var dGame = Play(playoffs[i - 4], playoffs[i - 2], season, season + "eight");
                
                if (cGame.HomeScore + dGame.GuestScore > cGame.GuestScore + dGame.HomeScore)
                {
                    fourClubs.Add(playoffs[i - 2]);
                } else if (cGame.HomeScore + dGame.GuestScore < cGame.GuestScore + dGame.HomeScore)
                {
                    fourClubs.Add(playoffs[i - 4]);
                }
                else
                {
                    if (cGame.GuestScore < dGame.GuestScore)
                    {
                        fourClubs.Add(playoffs[i - 2]);    
                    } else if (cGame.GuestScore > dGame.GuestScore)
                    {
                        fourClubs.Add(playoffs[i - 4]);
                    }
                    else
                    {
                        int rndPenalty = rnd.Next(0, 2);
                        if (rndPenalty == 0)
                        {
                            fourClubs.Add(playoffs[i - 2]);
                        }
                        else
                        {
                            fourClubs.Add(playoffs[i - 4]);
                        }
                    }
                }
                
                aGame.HomeLogo = playoffs[i - 1].Logo;
                bGame.HomeLogo = playoffs[i - 3].Logo;
                aGame.GuestLogo = playoffs[i - 3].Logo;
                bGame.GuestLogo = playoffs[i - 1].Logo;
                
                cGame.HomeLogo = playoffs[i - 2].Logo;
                dGame.HomeLogo = playoffs[i - 4].Logo;
                cGame.GuestLogo = playoffs[i - 4].Logo;
                dGame.GuestLogo = playoffs[i - 2].Logo;
                
                eight.Add(aGame);
                eight.Add(bGame);
                eight.Add(cGame);
                eight.Add(dGame);
            }
            
            List<Game> four = new List<Game>();
            List<Club> twoClubs = new List<Club>();
            for (int i = 4; i <= 4; i += 4)
            {
                var playoffs = fourClubs;
                var aGame = Play(playoffs[i - 1], playoffs[i - 3], season, season + "four");
                var bGame = Play(playoffs[i - 3], playoffs[i - 1], season, season + "four");

                if (aGame.HomeScore + bGame.GuestScore > aGame.GuestScore + bGame.HomeScore)
                {
                    twoClubs.Add(playoffs[i - 1]);
                } else if (aGame.HomeScore + bGame.GuestScore < aGame.GuestScore + bGame.HomeScore)
                {
                    twoClubs.Add(playoffs[i - 3]);
                }
                else
                {
                    if (aGame.GuestScore < bGame.GuestScore)
                    {
                        twoClubs.Add(playoffs[i - 1]);    
                    } else if (aGame.GuestScore > bGame.GuestScore)
                    {
                        twoClubs.Add(playoffs[i - 3]);
                    }
                    else
                    {
                        int rndPenalty = rnd.Next(0, 2);
                        if (rndPenalty == 0)
                        {
                            twoClubs.Add(playoffs[i - 1]);
                        }
                        else
                        {
                            twoClubs.Add(playoffs[i - 3]);
                        }
                    }
                }
                
                var cGame = Play(playoffs[i - 2], playoffs[i - 4], season, season + "four");
                var dGame = Play(playoffs[i - 4], playoffs[i - 2], season, season + "four");
                
                if (cGame.HomeScore + dGame.GuestScore > cGame.GuestScore + dGame.HomeScore)
                {
                    twoClubs.Add(playoffs[i - 2]);
                } else if (cGame.HomeScore + dGame.GuestScore < cGame.GuestScore + dGame.HomeScore)
                {
                    twoClubs.Add(playoffs[i - 4]);
                }
                else
                {
                    if (cGame.GuestScore < dGame.GuestScore)
                    {
                        twoClubs.Add(playoffs[i - 2]);    
                    } else if (cGame.GuestScore > dGame.GuestScore)
                    {
                        twoClubs.Add(playoffs[i - 4]);
                    }
                    else
                    {
                        int rndPenalty = rnd.Next(0, 2);
                        if (rndPenalty == 0)
                        {
                            twoClubs.Add(playoffs[i - 2]);
                        }
                        else
                        {
                            twoClubs.Add(playoffs[i - 4]);
                        }
                    }
                }
                
                aGame.HomeLogo = playoffs[i - 1].Logo;
                bGame.HomeLogo = playoffs[i - 3].Logo;
                aGame.GuestLogo = playoffs[i - 3].Logo;
                bGame.GuestLogo = playoffs[i - 1].Logo;
                
                cGame.HomeLogo = playoffs[i - 2].Logo;
                dGame.HomeLogo = playoffs[i - 4].Logo;
                cGame.GuestLogo = playoffs[i - 4].Logo;
                dGame.GuestLogo = playoffs[i - 2].Logo;
                
                four.Add(aGame);
                four.Add(bGame);
                four.Add(cGame);
                four.Add(dGame);
            }
            var final = Play(twoClubs[0], twoClubs[1], season, season + "final", false);
            final.HomeLogo = twoClubs[0].Logo;
            final.GuestLogo = twoClubs[1].Logo;
            
            int rndFinalPenalty = rnd.Next(0, 2);
            if (rndFinalPenalty == 0)
            {
                final.Winner = twoClubs[0].Name;
            }
            else
            {
                final.Winner = twoClubs[1].Name;
            }

            UefaChampions uefaChampions = new UefaChampions()
            {
                Groups = groups,
                Sixteen = sixteen,
                Eight = eight,
                Four = four,
                Final = final
            };

            return View(uefaChampions);
        }

        public async Task<IActionResult> Seasons()
        {
            var seasons = _dbContext.Seasons.ToList();
            return View(seasons);
        }

        public async Task<IActionResult> Leagues(string season)
        {
            List<SeasonLeague> leagues = new List<SeasonLeague>();
            var ligs = _dbContext.Leagues.ToList();
            foreach (var lig in ligs)
            {
                leagues.Add(new SeasonLeague()
                {
                    Id = lig.Id,
                    Name = lig.Name,
                    Logo = lig.Logo,
                    Place = lig.Place,
                    Season = season
                });
            }

            return View(leagues);
        }

        public async Task<IActionResult> Games(string league, string season)
        {
            List<GameView> gameViews = new List<GameView>();
            var games = _dbContext.Games.Where(g => g.Season == season && g.League == league).ToList();
            foreach (var game in games)
            {
                var aLogo = _dbContext.Clubs.FirstOrDefault(c => c.Name == game.HomeClub).Logo;
                var bLogo = _dbContext.Clubs.FirstOrDefault(c => c.Name == game.GuestClub).Logo;
                var gameView = new GameView()
                {
                    Id = game.Id,
                    HomeLogo = aLogo,
                    GuestLogo = bLogo,
                    HomeClub = game.HomeClub,
                    GuestClub = game.GuestClub,
                    League = game.League,
                    Season = game.Season,
                    HomeGoals = game.HomeGoals,
                    GuestGoals = game.GuestGoals,
                    HomeScore = game.HomeScore,
                    GuestScore = game.GuestScore,
                    Winner = game.Winner
                };
                gameViews.Add(gameView);
            }

            return View(gameViews);
        }

        public List<Club> GetForUEFA(int leagueId, string season, string league)
        {
            List<Results> results = new List<Results>();
            var clubs = _dbContext.Clubs.Where(c => c.LeagueId == leagueId).ToList();
            var modelLeague = _dbContext.Leagues.FirstOrDefault(ml => ml.Id == leagueId);
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
            
            var moveClubs = new List<Club>();

            for (int i = 0; i < modelLeague.Place; i++)
            {
                var club = _dbContext.Clubs.FirstOrDefault(c => c.Name == results[i].Club);
                moveClubs.Add(club);
            }

            return moveClubs;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Django()
        {
            return View();
        }
    }
}