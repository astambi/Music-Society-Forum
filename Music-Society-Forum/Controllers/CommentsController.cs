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
            return View(commentsWithAuthorsPosts.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
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

        // GET: Comments/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,Date")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,Date")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
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
        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            this.AddNotification("Comment deleted", NotificationType.SUCCESS);
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
