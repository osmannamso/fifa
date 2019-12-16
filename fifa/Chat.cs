using System;
using System.Threading.Tasks;
using fifa.Controllers;
using fifa.Data;
using fifa.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace fifa
{
    public class Chat : Hub
    {
        public async Task Send(string message)
        {
            string[] stringArray = {"Osman bad", "Barca bad", "Messi bad"};
            if (Array.IndexOf(stringArray, message) > -1)
            {
                await Clients.All.SendAsync("Send", "please don't write such things");
            }
            else
            {
                await Clients.All.SendAsync("Send", message);
            }
        }

        public async Task PlaySeason(string season)
        {
            var s = new Season()
            {
                Name = season
            };
            ClubsContext dbContext = new ClubsContext();
            dbContext.Entry(s).State = EntityState.Added;
            dbContext.SaveChanges();
            MainController mainController = new MainController(dbContext);
            mainController.SeasonPlay(s);

            await Clients.All.SendAsync("PlaySeason", "Season is played look at stats");
        }

        public async Task Test(string text)
        {
            System.Threading.Thread.Sleep(5000);
            await Clients.All.SendAsync("Test", "Test Done: " + text);
        }
    }
}