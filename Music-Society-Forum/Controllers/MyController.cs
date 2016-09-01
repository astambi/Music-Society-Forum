using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using Music_Society_Forum.Models;
using Music_Society_Forum.Extensions;

namespace Music_Society_Forum.Controllers
{
    [Authorize]
    public class MyController : BaseController
    {
        // GET: My
        public ActionResult Index()
        {
            ViewBag.PostsCount = db.Posts
                        .Where(p => p.Author != null && p.Author.UserName == User.Identity.Name)
                        .Count();
            ViewBag.CommentsCount = db.Comments
                        .Include(c => c.Post)
                        .Where(c => c.Author != null && c.Author.UserName == User.Identity.Name)
                        .Count();
            return View();
        }
        
        // GET: My/Posts
        public ActionResult Posts(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var posts = db.Posts
                        .Include(p => p.Author)
                        .Include(p => p.Comments)
                        .Where(p => p.Author != null && p.Author.UserName == User.Identity.Name);

            ViewBag.IsAdmin = isAdmin();
            ViewBag.CurrentSort = sortOrder;    // keep the sort order the same while paging

            // Sorting Functionality with Column Sort Links, default -> by Date DESC
            ViewBag.TitleSortParam = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewBag.DateSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.AuthorSortParam = sortOrder == "author_asc" ? "author_desc" : "author_asc";
            ViewBag.CommentsSortParam = sortOrder == "comments_asc" ? "comments_desc" : "comments_asc";

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
            posts = GetSortedPosts(sortOrder, posts);
            //return View(posts.ToList());

            // Paging Links
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        // GET: My/Comments
        public ActionResult Comments(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var comments = db.Comments
                            .Include(c => c.Author)
                            .Include(c => c.Post)
                            .Where(c => c.Author != null && c.Author.UserName == User.Identity.Name);

            ViewBag.IsAdmin = isAdmin();
            ViewBag.CurrentSort = sortOrder;    // keep the sort order the same while paging

            // Sorting Functionality with Column Sort Links, default -> by Date DESC
            ViewBag.TitleSortParam = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewBag.DateSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.AuthorSortParam = sortOrder == "author_asc" ? "author_desc" : "author_asc";

            // Paging Links
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString; // keep the search string the same & provide paging 

            // Search Functionality: in Post Title / Comment Text / Author
            if (!String.IsNullOrEmpty(searchString))
            {
                comments = comments
                    .Where(c => c.Post.Title.Contains(searchString) ||
                                c.Text.Contains(searchString) ||
                                c.Author.FullName.Contains(searchString));
            }
            // Sorting
            comments = GetSortedComments(sortOrder, comments);
            //return View(comments.ToList());

            // Paging Links
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(comments.ToPagedList(pageNumber, pageSize));
        }
                
    }
}
