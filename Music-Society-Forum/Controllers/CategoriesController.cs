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
    public class CategoriesController : BaseController
    {
        [Authorize(Roles = "Administrators")]
        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select an category", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested category does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            ViewBag.Posts = db.Posts
                        .Include(p => p.Author)
                        .Include(p => p.Category)
                        .Where(p => p.Category.Id == id)
                        .OrderByDescending(p => p.Date)
                        .ToList();
            return View(category);
        }

        [Authorize(Roles = "Administrators")]
        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrators")]
        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                this.AddNotification("Created a new category", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [Authorize(Roles = "Administrators")]
        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select an category", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested category does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Authorize(Roles = "Administrators")]
        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Modified category", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Authorize(Roles = "Administrators")]
        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                this.AddNotification("Please select an category", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                //return HttpNotFound();
                this.AddNotification("The requested category does not exist", NotificationType.INFO);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Authorize(Roles = "Administrators")]
        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            this.AddNotification("Deleted category", NotificationType.SUCCESS);
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
