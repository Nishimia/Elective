using Elective.Models.SubjectModels;
using ElectiveBLL.Models;
using ElectiveDAL.Managers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Elective.Controllers
{
	[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
	public class SubjectController : Controller
	{
		private SubjectManager _subjectManager;

		public SubjectController()
		{

		}

		public SubjectController(SubjectManager subjectManager)
		{
			SubjectManager = subjectManager;
		}

		public SubjectManager SubjectManager
		{
			get
			{
				return _subjectManager ?? HttpContext.GetOwinContext().Get<SubjectManager>();
			}
			private set
			{
				_subjectManager = value;
			}
		}

		[HttpGet]
		public ActionResult Index()
		{
			var subjects = SubjectManager.Subjects.ToList();
			var models = new List<SubjectViewModel>();
			foreach (var subject in subjects)
			{
				var model = new SubjectViewModel() { SubjectId = subject.Id, SubjectName = subject.SubjectName };
				models.Add(model);
			}
			return View(models);
		}

		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Create(SubjectCreateModel model)
		{
			if (ModelState.IsValid)
			{
				var subject = new Subject() { SubjectName = model.SubjectName };
				var result = await SubjectManager.CreateAsync(subject);
				if (!result.Succeeded)
				{
					AddErrors(result);
					return View();
				}
			}
			else
			{
				return View();
			}
			return RedirectToAction("Index", "Subject");
		}

		[HttpGet]
		public async Task<ActionResult> Delete(string id)
		{
			var subjectToDelete = await SubjectManager.FindByIdAsync(id);
			var result = await SubjectManager.DeleteAsync(subjectToDelete);
			if (!result.Succeeded)
			{
				AddErrors(result);
				return View();
			}
			return RedirectToAction("Index", "Subject");
		}

		[HttpGet]
		public async Task<ActionResult> Edit(string id)
		{
			var subject = await SubjectManager.FindByIdAsync(id);
			if(subject != null)
			{
				var model = new SubjectEditModel() { SubjectId = subject.Id, SubjectName = subject.SubjectName };
				return View(model);
			}
			return RedirectToAction("Index", "Subject");
		}

		[HttpPost]
		public async Task<ActionResult> Edit(SubjectEditModel model)
		{
			if (ModelState.IsValid)
			{
				using (var subjectManager = SubjectManager)
				{
					var subject = await subjectManager.FindByIdAsync(model.SubjectId);
					if (subject != null)
					{
						subject.SubjectName = model.SubjectName;

						var result = await subjectManager.UpdateAsync(subject);
						if (!result.Succeeded)
						{
							AddErrors(result);
							return View(model);
						}
					}
				}
			}
			else
			{
				return View(model);
			}
			return RedirectToAction("Index", "Subject");
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}
	}
}