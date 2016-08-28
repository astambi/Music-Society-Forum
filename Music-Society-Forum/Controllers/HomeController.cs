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
                        .OrderByDescending(p => p.Date)
                        .Take(10);
            ViewBag.RecentPosts = posts
                        .Take(5)
                        .ToList();
            ViewBag.RecentlyCommentedPosts = db.Comments
                        .Include(c => c.Post)
                        .OrderByDescending(p => p.Date)
                        .Select(c => c.Post)
                        .Distinct()
                        .Take(5)
                        .ToList();
            ViewBag.MostCommentedPosts = db.Posts
                        .Include(p => p.Comments)
                        .OrderByDescending(p => p.Comments.Count)
                        .Take(5)
                        .ToList();         
            ViewBag.EditorPosts = db.Posts
                        .Include(p => p.Author)
                        .Where(p => p.Author.UserName == "gould@gmail.com")
                        // TODO users in role "Editor"
                        .OrderByDescending(p => p.Date)
                        .Take(5)
                        .ToList();
            return View(posts.ToList());
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";
            return View();
        }        
    }
}