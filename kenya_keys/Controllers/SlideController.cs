using KK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kenya_keys.Controllers
{
    public class SlideController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }
        

        public ActionResult List()
        {
            ViewData["slides"] = KK.Models.Slide.GetAll();
            return PartialView();
        }


        public ActionResult Edit()
        {
            ViewData["slides"] = KK.Models.Slide.GetAll();
            return View();
        }

		public JsonResult Test(string value) {
			return Json(value);
		}


        [HttpPost]
		public JsonResult Save(string slide_text, int slide_index, int photo_id, int? slide_id = null)
        {
            Slide slide = new Slide();
			slide.SlideID = slide_id;
            slide.SlideText = slide_text;
            slide.SlideIndex = slide_index;
            slide.PhotoID = photo_id;
            slide.Save();
            return Json(slide);
        }
        


    }
}
