using Music_Society_Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Music_Society_Forum.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var posts = db.Posts
                        .Include(p => p.Author)
                        .OrderByDescending(p => p.Date);
            ViewBag.SidebarPosts = posts
                        .Take(5)
                        .ToList();
            return View(posts.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }        
    }
}