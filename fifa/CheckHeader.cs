using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using fifa.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace fifa
{
    public class CheckHeaderFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string token = context.HttpContext.Request.Headers["token"];
            if (token == null)
            {
                throw new Exception("Necessary HTTP headers not present!");
            }

            ClubsContext dbContext = new ClubsContext();
            var user = dbContext.Users.FirstOrDefault(u => u.Token == token);
            if (user == null)
            {
                throw new Exception("Incorrect token or expired!");
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }
    }
}