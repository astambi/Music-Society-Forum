using Music_Society_Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Music_Society_Forum.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public bool isAdmin()
        {
            bool isAdmin = User.Identity.IsAuthenticated &&
                           User.IsInRole("Administrators");
            return isAdmin;
        }
    }
}