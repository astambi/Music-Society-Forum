using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Music_Society_Forum.Models;
using Music_Society_Forum.Extensions;
using PagedList;

namespace Music_Society_Forum.Controllers
{
    [ValidateInput(false)]
    public class PostsController : BaseController
    {       
        public bool isPostOwner(Post post)
        {
            if (!User.Identity.IsAuthenticated || post.Author_Id == null)
                return false; 
            var currentUser = db.Users
                            .Where(u => u.UserName == User.Identity.Name)
                            .FirstOrDefault();
            bool isPostOwner = post.Author_Id == currentUser.Id;
            return isPostOwner;
        }

        // GET: Posts
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {            
            var posts = db.Posts
                        .Include(p => p.Author)
                        .Include(p => p.Category)
                        .Include(p => p.Comments);

            ViewBag.IsAdmin = isAdmin();
            ViewBag.CurrentSort = sortOrder;    // keep the sort order the same while paging
                                             
            // Sorting Functionality with Column Sort Links, default -> by Date DESC
            ViewBag.TitleSortParam = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewBag.DateSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.AuthorSortParam = sortOrder == "author_asc" ? "author_desc" : "author_asc";
            ViewBag.CommentsSortParam = sortOrder == "comments_asc" ? "comments_desc" : "comments_asc";
            ViewBag.CategorySortParam = sortOrder == "category_asc" ? "category_desc" : "category_asc";

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
                                         p.Author.FullName.Contains(searchString) ||
                                         p.Category.Name.Contains(searchString));
            }

            // Sorting 
            posts = GetSortedPosts(sortOrder, posts);
            //return View(posts.ToList());

            // Paging Links
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select an article", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested article does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            ViewBag.PostAuthor = db.Posts
                            .Where(p => p.Id == id)
                            .Select(u => u.Author)
                            .FirstOrDefault();
            ViewBag.IsAdmin = isAdmin();
            ViewBag.IsOwner = isPostOwner(post);
            ViewBag.Comments = db.Comments
                            .Include(c => c.Author)
                            .Where(c => c.Post.Id == id)
                            .OrderByDescending(c => c.Date)
                            .ToList();
            ViewBag.Category = db.Posts
                            .Include(p => p.Category)
                            .Where(p => p.Id == id)
                            .Select(p => p.Category)
                            .FirstOrDefault();
            ViewBag.Next = db.Posts
                            .Where(p => p.Date > post.Date)
                            .OrderBy(p => p.Date)
                            .FirstOrDefault();
            ViewBag.Previous = db.Posts
                            .Where(p => p.Date < post.Date)
                            .OrderByDescending(p => p.Date)
                            .FirstOrDefault();
            return View(post);
        }
             

        // GET: Posts/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body, Author_Id, Category_Id")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Author = db.Users
                             .Where(u => u.UserName == User.Identity.Name)
                             .FirstOrDefault();
                db.Posts.Add(post);
                db.SaveChanges();
                this.AddNotification("Created a new article", NotificationType.SUCCESS);
                return RedirectToAction("Posts", "My");
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select an article", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested article does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            } 
            if (!isAdmin() && !isPostOwner(post))
            {
                this.AddNotification("The article was created by another user", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            var authors = new List<ApplicationUser>() { post.Author };
            if (isAdmin())
            {
                authors = db.Users
                        .OrderBy(u => u.FullName)
                        .ThenBy(u => u.UserName)
                        .ToList();
            }  
            ViewBag.Authors = authors;
            ViewBag.Owner = post.Author;
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.PostCategory = post.Category;
            ViewBag.IsAdmin = isAdmin();
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Date, Author_Id, Category_Id")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Modified article", NotificationType.SUCCESS);
                if (isPostOwner(post))
                    return RedirectToAction("Posts", "My");                
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select an article", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested article does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            if (!isAdmin() && !isPostOwner(post))
            {
                this.AddNotification("The article was created by another user", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            if (!isAdmin() && isPostOwner(post) && post.Comments.Count > 0)
            {
                this.AddNotification("You are not allowed to delete an article with comments", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            ViewBag.PostAuthor = db.Posts
                            .Where(p => p.Id == id)
                            .Select(u => u.Author)
                            .FirstOrDefault();
            ViewBag.Comments = db.Comments
                            .Include(c => c.Author)
                            .Where(c => c.Post.Id == id)
                            .OrderByDescending(c => c.Date)
                            .ToList();
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            this.AddNotification("Deleted article", NotificationType.SUCCESS);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
