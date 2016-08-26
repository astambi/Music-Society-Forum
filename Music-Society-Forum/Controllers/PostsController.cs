﻿using System;
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
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool isAdmin()
        {
            bool isAdmin = User.Identity.IsAuthenticated && 
                           User.IsInRole("Administrators");
            return isAdmin;
        }

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
        public ActionResult Index()
        {
            var postsWithAuthors = db.Posts
                                .Include(p => p.Author)
                                .OrderByDescending(p => p.Date)
                                .ToList();
            ViewBag.IsAdmin = isAdmin();
            return View(postsWithAuthors);
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
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
            return View(post);
        }

        // GET: Posts/My
        [Authorize]
        public ActionResult My()
        {
            var posts = db.Posts
                        .Where(p => p.Author != null && p.Author.UserName == User.Identity.Name)
                        .OrderByDescending(p => p.Date)
                        .ToList();
            ViewBag.IsAdmin = isAdmin();
            ViewBag.Comments = db.Comments
                        .Include(c => c.Post)
                        .Where(c => c.Author != null && c.Author.UserName == User.Identity.Name)
                        .OrderByDescending(c => c.Date)
                        .ToList(); 
            return View(posts);
        }

        // GET: Posts/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body, Author_Id")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Author = db.Users
                             .Where(u => u.UserName == User.Identity.Name)
                             .FirstOrDefault();
                db.Posts.Add(post);
                db.SaveChanges();
                this.AddNotification("Review created", NotificationType.SUCCESS);
                return RedirectToAction("My");
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }            
            if (!isAdmin() && !isPostOwner(post))
            {
                this.AddNotification("Review created by another user", NotificationType.INFO);
                return RedirectToAction("My");
            }
            var authors = db.Users
                        .OrderBy(u => u.FullName)
                        .ThenBy(u => u.UserName)
                        .ToList();
            ViewBag.Authors = authors;
            ViewBag.PostAuthor = db.Posts
                        .Where(p => p.Id == id)
                        .Select(u => u.Author)
                        .FirstOrDefault();
            ViewBag.IsAdmin = isAdmin();
            ViewBag.IsOwner = isPostOwner(post);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Date, Author_Id")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Review modified", NotificationType.SUCCESS);
                if (isAdmin())
                    return RedirectToAction("Index");
                return RedirectToAction("My");
            }            
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }            
            ViewBag.PostAuthor = db.Posts
                                .Where(p => p.Id == id)
                                .Select(u => u.Author)
                                .FirstOrDefault();
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            this.AddNotification("Review deleted", NotificationType.SUCCESS);
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
