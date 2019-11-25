using System;
using System.Linq;
using fifa.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace fifa
{
    public class CheckHeader : Attribute
    {
        public CheckHeader()
        {
            
        }
        protected bool AuthorizeCore(HttpContext httpContext)
        {
            ClubsContext dbContext = new ClubsContext();
            // Get the headers
            var headers = httpContext.Request.Headers;
            // Do some checks (not sure what your wanting to do)
            var user = dbContext.Users.FirstOrDefault(u => u.Token == headers["token"]);
            if (user != null)
            {
                return true;
            }
            return false;
        }
    }
}