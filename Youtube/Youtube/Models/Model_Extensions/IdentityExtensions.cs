using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Youtube.Models.Model_Extensions
{
    public static class IdentityExtensions
    {
        public static string GetApplicationUserUsername(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ApplicationUserUsername");
            if(claim != null)
            {
                return claim.Value;
            } else
            {
                return "";
            }

        }
    }
}