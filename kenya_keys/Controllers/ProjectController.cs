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
    public class ProjectController : Controller
    {

		/// <summary>
		/// Index the specified project_id.
		/// </summary>
		/// <param name="project_id">Project_id.</param>
		public ActionResult Index(int? project_id = null)
        {
			if (project_id == null) {
				this.ViewData ["Projects"] = Project.GetAll ();
			} else {
				int selectedId = Convert.ToInt32(project_id);
				Project project = Project.GetById (Convert.ToInt32(project_id));
				if (project.ParentID != null) {
					project = Project.GetById (Convert.ToInt32 (project.ParentID));
				}

				this.ViewData ["Parent"] = project;
				List<Project> projects = project.GetChildren ();
				int index = 0;
				if (selectedId > 0) 
				{
					foreach (Project proj in projects) {
						index++;
						if (proj.ProjectID == project_id) 
						{
							proj.Selected = true;
							break;
						}
					}
				}


				this.ViewData ["Projects"] = projects;
			}
            return PartialView("Index");
        }

		/// <summary>
		/// Show the specified projectId.
		/// </summary>
		/// <param name="projectId">Project identifier.</param>
		public ActionResult Show(int projectId)
		{
			Project project = Project.GetById(projectId);
			Project parent = null;
			this.ViewData ["Project"] = project;

			if (project.ParentID != null) {
				parent = Project.GetById (Convert.ToInt32 (project.ParentID));
			}

			this.ViewData ["Parent"] = parent;


			return PartialView("Show");
		}

		/// <summary>
		/// Edit the specified project_id and parent_id.
		/// </summary>
		/// <param name="project_id">Project_id.</param>
		/// <param name="parent_id">Parent_id.</param>
		public ActionResult Edit(int? project_id = null, int? parent_id = null)
		{
			if (project_id != null)
			{
				KK.Models.Project project = Project.GetById(Convert.ToInt32(project_id));
				this.ViewData["Project"] = project;
			}

			if (parent_id != null) {
				this.ViewData ["Parent"] = Project.GetById (Convert.ToInt32 (parent_id));
			}
			return PartialView();
		}

		/// <summary>
		/// List this instance.
		/// </summary>
        public ActionResult List()
        {
            this.ViewData["Projects"] = Project.GetAll();
            return PartialView("List");
        }

		/// <summary>
		/// Save the specified project_id, proj_name, proj_display, photo_id, proj_html and parent_id.
		/// </summary>
		/// <param name="project_id">Project_id.</param>
		/// <param name="proj_name">Proj_name.</param>
		/// <param name="proj_display">Proj_display.</param>
		/// <param name="photo_id">Photo_id.</param>
		/// <param name="proj_html">Proj_html.</param>
		/// <param name="parent_id">Parent_id.</param>
		[HttpPost, ValidateInput(false)]
		public ContentResult Save(int? project_id, string proj_name, bool? proj_display, int? photo_id, string proj_html, int? parent_id)
        {
            Session.Remove("temp_image_data");
            Project project = new Project();
            if (project_id != null)
            {
                project = Project.GetById(Convert.ToInt32(project_id));
            }
			project.ParentID = parent_id;
            project.ProjectName = proj_name;
            project.ProjectHtml = proj_html;
            project.ProjectHide = proj_display == null ? false : Convert.ToBoolean(proj_display);
            if (photo_id != null) {
                project.ProjectImageID = Convert.ToInt32(photo_id);
            }
            
            project.Save();


            string result = string.Format("{{ result : 'success', data : {{ project_id :{0} }}", project.ProjectID);
            return new ContentResult { Content = result };  
        }

		/// <summary>
		/// Delete the specified projectID.
		/// </summary>
		/// <param name="projectID">Project I.</param>
        [HttpGet]
        public ContentResult Delete(int projectID)
        {
            Project.Delete(projectID);
			string result = string.Format("{{ \"result\" : \"success\", \"data\" : {{ \"project_id\" :{0} }} }}", projectID);
            return new ContentResult { Content = result }; 
        }

		/// <summary>
		/// Shows the page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="pageId">Page identifier.</param>
		public ActionResult ShowPage(int pageId)
		{
			this.ViewData["Page"] = ProjectPage.GetById(pageId);
			return PartialView();
		}

		/// <summary>
		/// Pages the edit.
		/// </summary>
		/// <returns>The edit.</returns>
		/// <param name="project_id">Project_id.</param>
		/// <param name="page_id">Page_id.</param>
		public ActionResult PageEdit(int project_id, int? page_id = null)
		{
			this.ViewData ["Project"] = Project.GetById (project_id);
			// get project pages
			List<KK.Models.ProjectPage> pages = ProjectPage.GetByAllByProjectId (project_id);
			this.ViewData ["PageIndex"] = pages.Count;

			if (page_id != null) {
				KK.Models.ProjectPage page = ProjectPage.GetById (Convert.ToInt32 (page_id));
				this.ViewData ["Page"] = page;
			}
			return PartialView("PageEdit");
		}


		[HttpPost, ValidateInput(false)]
		public JsonResult SavePage(int project_id, int page_index, string page_html, int? page_id = null)
		{

			ProjectPage page = new ProjectPage ();
			if (page_id != null) {
				page = ProjectPage.GetById (Convert.ToInt32 (page_id));
			}

			page.ProjectID = project_id;
			page.PageIndex = page_index;
			page.PageHtml = page_html;
			page.Save ();

			Response.ContentType = "application/json";
			return Json (page);
		}


		[HttpGet]
		public ContentResult DeletePage(int pageId)
		{

			ProjectPage Page = ProjectPage.GetById (pageId);
			ProjectPage.Delete (pageId);
			string result = string.Format("{{ \"result\" : \"success\", \"data\" : {{ \"project_id\" :{0} }} }}", Page.ProjectID);
			return new ContentResult { Content = result }; 
		}


       
    }
}
