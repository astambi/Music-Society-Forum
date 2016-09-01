using Music_Society_Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Music_Society_Forum.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public bool isAdmin()
        {
            bool isAdmin = User.Identity.IsAuthenticated &&
                           User.IsInRole("Administrators");
            return isAdmin;
        }

        public static IQueryable<Post> GetSortedPosts(string sortOrder, IQueryable<Post> posts)
        {
            switch (sortOrder)
            {
                case "category_desc":
                    posts = posts.OrderByDescending(p => p.Category.Name);
                    break;
                case "category_asc":
                    posts = posts.OrderBy(p => p.Category.Name);
                    break;
                case "title_desc":
                    posts = posts.OrderByDescending(p => p.Title);
                    break;
                case "title_asc":
                    posts = posts.OrderBy(p => p.Title);
                    break;
                case "date_asc":
                    posts = posts.OrderBy(p => p.Date);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(p => p.Date);
                    break;
                case "author_desc":
                    posts = posts.OrderByDescending(p => p.Author.FullName);
                    break;
                case "author_asc":
                    posts = posts.OrderBy(p => p.Author.FullName);
                    break;
                case "comments_desc":
                    posts = posts.OrderByDescending(p => p.Comments.Count());
                    break;
                case "comments_asc":
                    posts = posts.OrderBy(p => p.Comments.Count());
                    break;
                default:
                    posts = posts.OrderByDescending(p => p.Date);
                    break;
            }

            return posts;
        }

        public static IQueryable<Comment> GetSortedComments(string sortOrder, IQueryable<Comment> comments)
        {
            switch (sortOrder)
            {
                case "title_desc":
                    comments = comments.OrderByDescending(c => c.Post.Title);
                    break;
                case "title_asc":
                    comments = comments.OrderBy(c => c.Post.Title);
                    break;
                case "date_asc":
                    comments = comments.OrderBy(c => c.Date);
                    break;
                case "date_desc":
                    comments = comments.OrderByDescending(c => c.Date);
                    break;
                case "author_desc":
                    comments = comments.OrderByDescending(c => c.Author.FullName);
                    break;
                case "author_asc":
                    comments = comments.OrderBy(c => c.Author.FullName);
                    break;
                default:
                    comments = comments.OrderByDescending(c => c.Date);
                    break;
            }

            return comments;
        }
    }
}