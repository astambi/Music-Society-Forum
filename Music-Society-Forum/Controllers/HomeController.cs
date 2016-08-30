using Music_Society_Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;

namespace Music_Society_Forum.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var posts = db.Posts
                        .Include(p => p.Author);            
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
                        .Where(p => p.Author.UserName.Contains("editor@"))
                        // TODO users in role "Editor"
                        .OrderByDescending(p => p.Date)
                        .Take(5)
                        .ToList();
            ViewBag.Recommended = db.Posts
                        .Where(p => p.IsRecommended)
                        .OrderByDescending(p => p.Date)
                        .Take(5).ToList();

            ViewBag.CurrentSort = sortOrder;    // keep the sort order the same while paging

            // Paging Links
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString; // keep the search string the same & provide paging 

            // Search Functionality: in Post Title / Post Body / Author
            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString) ||
                                         p.Body.Contains(searchString) ||
                                         p.Author.FullName.Contains(searchString));
            }
            // Sorting
            posts = posts.OrderByDescending(p => p.Date);
            //return View(posts.ToList());

            // Paging Links
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Project Requirements";
            return View();
        }        
    }
}