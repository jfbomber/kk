using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace KK.Controllers
{
    public class HomeController : Controller
    {
		public KK.User user;
		public bool IsAdmin;
		protected override void Initialize(RequestContext context) {
			// context.RouteData.Values["Admin"] 
			base.Initialize (context);
			ViewData["IsAdmin"] = context.HttpContext.Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
		}

        
		/// <summary>
		/// Default/Index Page
		/// </summary>
        public ActionResult Index()
        {
			return View();
        }

        public ActionResult Project()
        {
            return View();
        }

		/// <summary>
		/// About Page
		/// </summary>
        public ActionResult About()
        {
            KK.Models.Content content = KK.Models.Content.Get("Home", "About", "About Us");
			content.IsAdmin = IsAdmin;
            return View(content);
            
        }

        /// <summary>
        /// Blog Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Blog()
        {
            ViewBag.Message = "Kenya Keys Blogs";

            return View();
        }
		
		/// <summary>
		/// Contact Page
		/// </summary>
		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}



        /// <summary>
        /// Contact Page
        /// </summary>
        public ActionResult Sponsorship()
        {

            ViewBag.Title = "Sponsorship";
            KK.Models.Content content = KK.Models.Content.Get("Home", "Sponsorship", "Sponsorships");
            content.IsAdmin = Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
            return View(content);
        }

        /// <summary>
        /// In The News Page
        /// </summary>
        public ActionResult InTheNews()
        {
            ViewBag.Title = "In The News";
            KK.Models.Content content = KK.Models.Content.Get("Home", "InTheNews", "In The News");
            content.IsAdmin = Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
            return View(content);
        }

        /// <summary>
        /// Donate Page
        /// </summary>
        public ActionResult Donate()
        {
            ViewBag.Title = "Donate & Get Involved";
            KK.Models.Content content = KK.Models.Content.Get("Home", "Donate", "Donate & Get Involved");
            return View(content);
        }

        /// <summary>
        /// Unauthorized Page
        /// </summary>
        public ActionResult Unauthorized()
        {
            return View();
        }

		/// <summary>
		/// Called for the User Login
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
        [HttpPost]
        public ActionResult Login(string username, string password)
        {   
            // Check if the login is valid
            string error = string.Empty;
            KK.User user = KK.User.Login(username, password, ref error);
            
            if (error == string.Empty && user.UserName != null)
            {
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, 
				                                                                     username, 
				                                                                     DateTime.Now, 
				                                                                     DateTime.Now.Add(new TimeSpan(1,0,0,0)), 
				                                                                     true, 
				                                                                     "roles="+String.Join(",", user.Roles));
                string encAuthTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encAuthTicket);
                if (Response.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    Response.Cookies.Set(authCookie);
                }
                else
                {
                    Response.Cookies.Add(authCookie);
                }
            }
            else
            {
                ModelState.AddModelError("", error);
            }
            return RedirectToAction("Index", "Home");
        }

        public ContentResult Logout()
        {
            string username = User.Identity.Name;

            string result = string.Format("{{ result : 'success', data : {{ username :'{0}' }}", username);

            FormsAuthentication.SignOut();
            Session.Clear();

            return new ContentResult { Content = result }; ;
        }

    }
}
