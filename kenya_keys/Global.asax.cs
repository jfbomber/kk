using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace KK
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

   		/// <summary>
   		/// This is call set on the Login
   		/// </summary>
   		/// <param name="sender">Sender.</param>
   		/// <param name="e">E.</param>
		protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			// Retrieve the auth cookie
			// this is called too often it would be nice to check if it needs to be called.
		    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
		    if (authCookie != null)
		    {
		        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				if (authTicket != null) {


					if (!string.IsNullOrEmpty (authTicket.UserData)) {
						string[] parts = authTicket.UserData.Split ('=');
						if (parts.Length == 2 && parts [0] == "roles") {
							FormsIdentity id = new FormsIdentity (authTicket);
							GenericPrincipal principal = new GenericPrincipal (id, parts [1].Split (','));
							Context.User = principal;
						}
					}
				}
		   }  
		}
    }
}