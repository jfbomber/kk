using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kenya_keys.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Edit(string id = null) 
        {
            if (id != null)
            {
                KK.User user = KK.User.GetByUserName(id);
                ViewData.Add("user", user);
            }

            List<KK.Role> roles = KK.Role.GetAll();
            ViewData.Add("roles", roles);

            return View();
        }

        [HttpGet]
        public ContentResult Update(string userName, string password, string firstName, string lastName, string email, string userId = null, string isActive = "on") {
            KK.User user = new KK.User();
            if (userId != "null") {
                user.UserID = Convert.ToInt32(userId);
                user.IsActive = isActive == "on" ? true : false;
            } else {
                user.CreatedByID = KK.User.GetByUserName(User.Identity.Name).UserID;
            }

            user.UserName = userName;
            user.Password = password;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            
            user.Save();
            return Content(user.UserID.ToString()); 
        }

        [HttpGet]
        public ContentResult UpdatePassword(string userName, string oldPassword, string newPassword)
        {
            string result = "failure";
            string message = null;

            try
            {
                KK.User.UpdatePassword(userName, oldPassword, newPassword);
                result = "success";
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return new ContentResult { Content = string.Format("{{\"result\":{0},\"data\":\"{1}\"}}", result, message) };
        }
    
    }
}
