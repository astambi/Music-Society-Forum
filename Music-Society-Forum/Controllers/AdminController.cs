using Music_Society_Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Music_Society_Forum.Controllers
{
    [ValidateInput(false)]
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            var postsWithAuthors = db.Posts
                                .Include(p => p.Author)
                                .OrderByDescending(p => p.Date)
                                .Where(p => p.Date.Month >= DateTime.Now.Month)
                                .ToList();
            var commentsWithAuthorsPosts = db.Comments
                                .Include(c => c.Author)
                                .Include(c => c.Post)
                                .OrderByDescending(c => c.Date)
                                .Where(c => c.Date.Month >= DateTime.Now.Month)
                                .ToList();
            ViewBag.Comments = commentsWithAuthorsPosts;
            return View(postsWithAuthors);
        }

        public ActionResult Posts()
        {
            var postsWithAuthors = db.Posts
                                .Include(p => p.Author)
                                .OrderByDescending(p => p.Date)
                                .ToList();
            return View(postsWithAuthors);
        }

        public ActionResult Comments()
        {
            var commentsWithAuthorsPosts = db.Comments
                                .Include(c => c.Author)
                                .Include(c => c.Post)
                                .OrderByDescending(c => c.Date)
                                .ToList();
            return View(commentsWithAuthorsPosts);
        }        
    }
}
