using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KK.Models;
using System.Windows;
using System.Windows.Input;
namespace KK.Controllers
{
    public class PhotoController : Controller
    {
        //
        // GET: /Photo/


		/// <summary>
		/// Returns the last images that was stored in the session
		/// </summary>
        [HttpGet]
        public ContentResult Last()
        {
            if (Session["temp_image"] != null)
            {
                Photo photo = (Photo)Session["temp_image"];
                Session.Remove("temp_image");
                return new ContentResult { Content = string.Format("{{\"photo_id\":{0},\"photo_name\":\"{1}\"}}", photo.PhotoID, photo.PhotoName) }; 
            }
            return null;
        }

		/// <summary>
		/// Post for photo upload
		/// </summary>
		/// <param name="upload">Upload.</param>
		/// <param name="storeRaw">If set to <c>true</c> store the photo with no sizing</param>
		/// <param name="redirect">Redirect.</param>
        [HttpPost]
        public JsonResult Add(HttpPostedFileBase upload, bool storeRaw = false, string redirect = "")
        { 
            Photo photo = new Photo(upload, storeRaw);
            photo.Save();

            Session["temp_image"] = photo;

            if (redirect.Length > 0)
            {
                Response.Redirect("~/Admin/Index");
            }
            return Json(photo); 
        }

		/// <summary>
		/// Delete the specified photoID.
		/// </summary>
		/// <param name="photoID">Photo ID.</param>
        public JsonResult Delete(int photoID)
        {
            Photo photo = Photo.Get(photoID);
            Photo.Delete(photoID);
            return Json(photo);
        }

		/// <summary>
		/// Gets the photo.
		/// </summary>
		/// <returns>The photo.</returns>
		/// <param name="photoId">Photo identifier.</param>
		/// <param name="size">Size.</param>
        public ActionResult GetPhoto(int photoId, string size = "t")
        {
           byte[] imgdata = Photo.GetPhotoData(photoId, size);
            if (imgdata == null) {
                System.Drawing.Image img = System.Drawing.Image.FromFile(Server.MapPath("~/images/graphics/image-not-found.jpg"));
                imgdata = Photo.ConvertImageToByteArray(img);
            }
            return new FileContentResult(imgdata, "image/jpeg");
        }

		/// <summary>
		/// Returns a list of photos in a gallery type view
		/// </summary>
        public ActionResult List()
        {
            ViewData["photos"] = Photo.GetAll();
            return PartialView("List");
        }
			
		/// <summary>
		/// Customized Drop Down Menu for Photos
		/// </summary>
		/// <returns>The down.</returns>
		/// <param name="onchange">Onchange.</param>
        public ActionResult DropDown(string onchange = "alert(this.options[this.selectedIndex].value)")
        {
			// object for the partial view
            PhotoDropDown photos = new PhotoDropDown();
			// the js onchange event
            photos.OnChange = onchange;
            return PartialView(photos);
        }
    }
}
