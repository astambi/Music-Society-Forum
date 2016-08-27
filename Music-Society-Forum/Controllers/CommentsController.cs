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

namespace Music_Society_Forum.Controllers
{
    [ValidateInput(false)]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool isAdmin()
        {
            bool isAdmin = User.Identity.IsAuthenticated &&
                           User.IsInRole("Administrators");
            return isAdmin;
        }

        public bool isCommentOwner(Comment comment)
        {
            if (!User.Identity.IsAuthenticated || comment.Author_Id == null)
                return false;
            var currentUser = db.Users
                            .Where(u => u.UserName == User.Identity.Name)
                            .FirstOrDefault();
            bool isCommentOwner = comment.Author_Id == currentUser.Id;
            return isCommentOwner;
        }

        // GET: Comments
        public ActionResult Index()
        {
            var commentsWithAuthorsPosts = db.Comments
                                .Include(c => c.Author)
                                .Include(c => c.Post)
                                .OrderByDescending(c => c.Date)
                                .ToList();
            ViewBag.IsAdmin = isAdmin();
            return View(commentsWithAuthorsPosts);
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select a comment", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested comment does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            ViewBag.CommentAuthor = db.Comments
                                .Include(c => c.Author)
                                .Where(c => c.Id == id)
                                .Select(c => c.Author)
                                .FirstOrDefault();
            var commentedPost = db.Comments
                                .Include(c => c.Post)
                                .Where(c => c.Id == id)
                                .Select(c => c.Post)
                                .FirstOrDefault();
            ViewBag.CommentedPost = commentedPost;
            ViewBag.PostAuthor = db.Posts
                                .Include(p => p.Author)
                                .Where(p => p.Id == commentedPost.Id)
                                .Select(p => p.Author)
                                .FirstOrDefault();
            ViewBag.IsAdmin = isAdmin();
            ViewBag.IsOwner = isCommentOwner(comment);
            return View(comment);
        }

        // GET: Comments/Create/5
        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select the article you would like to comment", NotificationType.INFO);
                return RedirectToAction("Index", "Posts");
            }
            Post post = db.Posts.Find(id);            
            if (post == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested article does not exist", NotificationType.INFO);
                return RedirectToAction("Index", "Posts");
            }

            // TODO Create comment !!!

            Comment comment = new Comment();
            comment.Post_Id = post.Id;
            return View(comment);
        }

        // POST: Comments/Create/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Text, Author_Id, Post_Id")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                // TODO Create comment !!!

                comment.Author = db.Users
                                .Where(u => u.UserName == User.Identity.Name)
                                .FirstOrDefault();
                db.Comments.Add(comment);
                db.SaveChanges();
                this.AddNotification("Created a new comment", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            else
            {               
                this.AddNotification("Unable to create comment", NotificationType.ERROR);
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select a comment", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested comment does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            if (!isAdmin() && !isCommentOwner(comment))
            {
                this.AddNotification("The comment was created by another user", NotificationType.INFO);
                return RedirectToAction("My", "Posts");
            }
            var authors = db.Users
                        .OrderBy(u => u.FullName)
                        .ThenBy(u => u.UserName)
                        .ToList();
            var posts = db.Posts
                        .OrderByDescending(p => p.Date)
                        .ToList();
            ViewBag.Authors = authors;
            ViewBag.Posts = posts;
            ViewBag.CommentAuthor = db.Comments
                        .Where(c => c.Id == id)
                        .Select(u => u.Author)
                        .FirstOrDefault();
            ViewBag.IsAdmin = isAdmin();
            ViewBag.IsOwner = isCommentOwner(comment);

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,Date, Author_Id, Post_Id")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Modified comment", NotificationType.SUCCESS);
                if (isCommentOwner(comment))
                    return RedirectToAction("My", "Posts");
                return RedirectToAction("Index");
            }
            else
            {
                this.AddNotification("Unable to modify comment", NotificationType.ERROR);
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select a comment", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested comment does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            if (!isAdmin() && !isCommentOwner(comment))
            {
                this.AddNotification("The comment was created by another user", NotificationType.INFO);
                return RedirectToAction("My", "Posts");
            }
            ViewBag.CommentAuthor = db.Comments
                                    .Where(c => c.Id == id)
                                    .Select(c => c.Author)
                                    .FirstOrDefault();
            ViewBag.CommentedPost = db.Comments
                                    .Include(c => c.Post)
                                    .Where(c => c.Id == id)
                                    .Select(c => c.Post)
                                    .FirstOrDefault();
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            this.AddNotification("Deleted comment", NotificationType.SUCCESS);
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
