using KK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace kenya_keys.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
		protected override void Initialize(RequestContext context) {
			base.Initialize (context);
			bool IsAdmin = context.HttpContext.Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
			if (!IsAdmin) {
				// return RedirectToAction("Index", "Home");// Response.Redirect ("~/Home/Index");
			}
			base.Initialize (context);
		}

        public ActionResult Index()
        {
            ViewBag.Title = "Admin";

            List<KK.User> users = KK.User.GetAll();
            
            ViewData.Add("users", users);
            ViewData["photos"] = KK.Models.Photo.GetAll();
            
            return View();
        }
        
        public ActionResult Slides() {
            ViewData["slides"] = KK.Models.Slide.GetAll();
            return View();// RedirectToAction("Edit", "Slide", null);
        }

        public ActionResult View(string contentController, string contentAction,  string elementId)
        {
            ViewData["content"] = KK.Models.Content.Get(contentController, contentAction, elementId);
            return PartialView("View");
        }

        public ActionResult Edit(string contentController, string contentAction, string elementId)
        {
            ViewData["content"] = KK.Models.Content.Get(contentController, contentAction, elementId);
            return PartialView("Edit");
        }

        [HttpPost]
        public ContentResult Save(int contentId, string elementId, string elementHtml, string contentController, string contentAction)
        {
          
            string result = string.Format("{{ result : 'success' }}");
            return new ContentResult { Content = result }; ;
        }
    }
}
