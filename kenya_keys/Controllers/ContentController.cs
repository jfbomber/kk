using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KK;
using KK.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace KK.Controllers
{
    public class ContentController : Controller
    {
        /// <summary>
        /// Returns a view to edit a Content object
        /// </summary>
        /// <param name="contentID"></param>
        /// <returns></returns>
        public ActionResult Edit(int? contentID = null)
        {
            KK.Models.Content content = new Content();
            if (contentID != null)
            {
                if (!Request.IsAuthenticated || !User.IsInRole("admin")) {
                    return RedirectToAction("Index", "Home");
                }
                content = KK.Models.Content.Get(Convert.ToInt32(contentID));
            }
            return PartialView(content);
        }
        /// <summary>
        /// Returns a view to edit a ContentItem object
        /// </summary>
        /// <param name="contentID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ActionResult ItemEdit(int? contentID = null, int? itemID = null)
        {
            KK.Models.ContentItem item = new ContentItem();
            if (contentID != null)
            {
                item.ContentID = Convert.ToInt32(contentID);
                item.Content = KK.Models.Content.Get(Convert.ToInt32(contentID));
            }
            

            if (itemID != null)
            {
                if (!Request.IsAuthenticated || !User.IsInRole("admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                item = KK.Models.ContentItem.Get(Convert.ToInt32(itemID));
                item.Content = KK.Models.Content.Get(item.ContentID);
            }
            return PartialView(item);
        }

        /// <summary>
        /// Gets the Display of a content with the ContentID
        /// </summary>
        /// <param name="contentID"></param>
        /// <returns></returns>
        public ActionResult DisplayByID(int contentID)
        {
            KK.Models.Content content = KK.Models.Content.Get(contentID);
            RedirectToAction("Display", "Content", content) ;
            return PartialView("Display", content);
        }

        /// <summary>
        /// Gets the Display of a Content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult Display(KK.Models.Content content)
        {
            content.IsAdmin = Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
           
            // KK.Models.Content content = KK.Models.Content.Get(contentController, contentAction, contentTitle);
            return PartialView(content);
        }
        /// <summary>
        /// Returns an item as a JSON object
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public JsonResult GetContentItem(int itemID)
        {

            KK.Models.ContentItem item = KK.Models.ContentItem.Get(itemID);
            item.IsAdmin = Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
            return Json(item, JsonRequestBehavior.AllowGet);  
        }
        /// <summary>
        /// Displays the item, not used yet
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ActionResult ItemDisplay(KK.Models.ContentItem item)// ) //KK.Models.Content content)
        {
            item.IsAdmin = Request.IsAuthenticated && User.IsInRole("admin") ? true : false;
            // KK.Models.Content content = KK.Models.Content.Get(contentController, contentAction, contentTitle);
            return PartialView(item);
        }


        /// <summary>
        /// Saves a Content object
        /// </summary>
        /// <param name="contentController"></param>
        /// <param name="contentAction"></param>
        /// <param name="contentTitle"></param>
        /// <param name="contentHtml"></param>
        /// <param name="contentID"></param>
        /// <param name="photoID"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult Save(string contentController, string contentAction, string contentTitle, string contentHtml, int? contentID, int? photoID) // 
        {
            
            KK.Models.Content content = new Models.Content();
            if (contentID != null)
            {
                content = KK.Models.Content.Get(Convert.ToInt32(contentID));
            }
            else
            {
                content.ContentAdded = DateTime.Now;
                content.ContentAddedBy = User.Identity.Name;
            }
            content.ContentPhotoID = photoID;
            content.ContentUpdated = DateTime.Now;
            content.ContentUpdatedBy = User.Identity.Name;
            content.ContentAction = contentAction;
            content.ContentController = contentController;
            content.ContentTitle = contentTitle;
            content.ContentHtml = contentHtml;
            content.Save();
            
            string result = string.Format("{{ \"Result\" : \"success\", \"Data\" : {{ \"ContentID\" :{0} }} }}", content.ContentID);
            return new ContentResult { Content = result }; 
        }

        /// <summary>
        /// Saves a ContentItem object
        /// </summary>
        /// <param name="itemTitle"></param>
        /// <param name="itemHtml"></param>
        /// <param name="contentID"></param>
        /// <param name="itemID"></param>
        /// <param name="photoID"></param>
        /// <param name="itemHidden"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult ItemSave(string itemTitle, string itemHtml, int contentID, int? itemID, int? photoID, bool itemHidden = false) // 
        {

            KK.Models.ContentItem item = new Models.ContentItem();
            if (itemID != null)
            {
                item = KK.Models.ContentItem.Get(Convert.ToInt32(itemID));
            }
            else
            {
                item.ContentID = contentID;
                item.ItemAdded = DateTime.Now;
                item.ItemAddedBy = User.Identity.Name;
                
            }
            item.ItemPhotoID = photoID;
            item.ItemLastUpdated = DateTime.Now;
            item.ItemLastUpdatedBy = User.Identity.Name;
            item.ItemTitle = itemTitle;
            item.ItemHtml = itemHtml;
            item.Save();

            string result = string.Format("{{ \"Result\" : \"success\", \"Data\" : {{ \"ItemID\" :{0}, \"ContentID\":{1} }} }}", item.ItemID, item.ContentID);
            return new ContentResult { Content = result };
        }

        /// <summary>
        /// Delets a ContentItem object
        /// </summary>
        /// <param name="itemID">Primary Key</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ItemDelete(int itemID) // 
        {
            KK.Models.ContentItem item = KK.Models.ContentItem.Get(itemID);
            KK.Models.ContentItem.Delete(itemID);
            return Json(item);
        }

    }
}
