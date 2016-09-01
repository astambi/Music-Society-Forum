using Music_Society_Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Music_Society_Forum.Extensions;

namespace Music_Society_Forum.Controllers
{
    [ValidateInput(false)]
    [Authorize(Roles = "Administrators")]
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            var newPosts = db.Posts
                            .Include(p => p.Author)
                            .OrderByDescending(p => p.Date)
                            .Take(5)
                            .ToList();
            ViewBag.RecommendedPosts = db.Posts
                            .Include(c => c.Author)
                            .Where(p => p.IsRecommended)
                            .OrderByDescending(c => c.Date)
                            .ToList();
            ViewBag.PostsCount = db.Posts.Count();
            ViewBag.CommentsCount = db.Comments.Count();           
            
            return View(newPosts);
        }        

        // GET: Admin/Edit/5
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
            var authors = new List<ApplicationUser>();
            if (post.Author != null)                
                authors.Add(post.Author);
            if (isAdmin())
            {
                var allUsers = db.Users
                            .OrderBy(u => u.FullName)
                            .ThenBy(u => u.UserName);
                authors.AddRange(allUsers);
            }            
            ViewBag.Authors = authors;
            ViewBag.Owner = post.Author;
            return View(post);
        }

        // POST: Posts/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Title, Body, Date, Author_Id, IsRecommended")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Modified article", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            this.AddNotification("Could not modify article", NotificationType.ERROR);
            return View(post);
        }
        
    }
}
