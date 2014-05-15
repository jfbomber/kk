using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KK.Models;
using System.Web.Routing;

namespace kenya_keys.Controllers
{
    public class BlogController : Controller
    {

		protected override void Initialize(RequestContext context) {
			// context.RouteData.Values["Admin"] 
			base.Initialize (context);
			ViewData["IsAdmin"] = context.HttpContext.Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
		}

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public ActionResult Edit(int? blogId = null)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                Response.Redirect("~/Home/Index");
            }
            Blog blog = new Blog();
            if (blogId != null) { blog = Blog.Get(Convert.ToInt32(blogId)); }
			return View(blog);
        }

		/// <summary>
		/// Deletes a blog
		/// </summary>
		/// <param name="blogId">Blog identifier.</param>
		public JsonResult Delete(int blogId) {
			Blog blog = Blog.Delete (blogId);
			return Json (blog);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="blogId"></param>
		/// <returns></returns>
		public ActionResult View(int? blogId = null)
		{

			Blog blog = new Blog();
			if (blogId != null) {
				blog = Blog.Get (Convert.ToInt32 (blogId));
			} else {
				RedirectToAction ("Home", "Blog");
			}
			return View(blog); // PartialView to return Partial
		}


		public ActionResult List(int offset = 0, int limit = 5, int? total = null)
		{
			ViewData ["blogs"] = Blog.GetAll (offset, limit);
			if (total == null) {
				total = Blog.GetTotalCount ();
			}
			ViewData ["total"] = total;
			ViewData ["limit"] = limit;
			ViewData ["offset"] = offset;

			return PartialView();
		}

        [HttpPost]
		public JsonResult Save(string blog_description, string blog_title, string blog_hidden, string blog_html, string blog_start_date, string blog_end_date, int? blog_id = null) {
			Blog blog = new Blog();
			if (blog_id != null) {
				blog = Blog.Get (Convert.ToInt32 (blog_id));
				blog.BlogPostDate = DateTime.Now;
			} else {
				blog.BlogAuthor = User.Identity.Name;
			}
            
            blog.BlogDescription = blog_description;
            blog.BlogHidden = blog_hidden == "yes" ? true : false;
            blog.BlogTitle = blog_title;
            blog.BlogHtml = blog_html;
            if (blog_start_date.Length > 0) {
                blog.BlogStartDate = Convert.ToDateTime(blog_start_date);
            }
            if (blog_end_date.Length > 0) {
                blog.BlogEndDate = Convert.ToDateTime(blog_end_date);
            }

            blog.Save();
			return Json (blog);
        }
    }
}
