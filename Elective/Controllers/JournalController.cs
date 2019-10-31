using Elective.Models.JournalModels;
using Elective.Models.JournalViewModels;
using Elective.Models.UserModels;
using ElectiveDAL.Managers;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Elective.Controllers
{
	[Authorize(Roles = ElectiveBLL.Constants.TUTOR_ROLE_NAME)]
	public class JournalController : Controller
	{
		private CourseManager _courseManager;
		private ElectiveUserManager _userManager;

		public JournalController()
		{

		}

		public JournalController(CourseManager courseManager, ElectiveUserManager userManager)
		{
			_courseManager = courseManager;
			_userManager = userManager;
		}

		public CourseManager CourseManager
		{
			get
			{
				return _courseManager ?? HttpContext.GetOwinContext().Get<CourseManager>();
			}
			set
			{
				_courseManager = value;
			}
		}

		public ElectiveUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().Get<ElectiveUserManager>();
			}
			set
			{
				_userManager = value;
			}
		}

		public async Task<ActionResult> Index(string login)
        {
			var user = await UserManager.FindByNameAsync(login);
			if(user == null)


			{
				return RedirectToAction("Index", "Home");
			}
			var userId = user.Id;
			var tutoredCourses = (await UserManager.GetUserCoursesAsync(userId, true)).ToList();
			var coursesJournalViewModels = tutoredCourses.Select(course => new CourseJournalViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				UserAmount = course.EnrolledInCourseStudents.Count(),
				Days = (course.EndDate - course.StartDate).Days,
				EndDate = course.EndDate,
				StartDate = course.StartDate
			});
            return View(coursesJournalViewModels);
        }

		public async Task<ActionResult> Edit(string id)
		{
			var course = await CourseManager.FindByIdAsync(id);
			if(course == null)
			{
				RedirectToAction("Index", "Journal", new { login = User.Identity.Name });
			}
			var students = (await CourseManager.GetStudentsByCourseAsync(course.Id)).ToList();

			var userJournalModels = new List<UserJournalModel>();
			foreach(var student in students)
			{
				var userJournalModel = new UserJournalModel();
				var user = await UserManager.FindByIdAsync(student.Id);
				userJournalModel.Mark = await UserManager.GetMarkAsync(student.Id, course.Id);
				userJournalModel.UserName = string.Format("{0} {1}", student.FirstName, student.SecondName);
				userJournalModel.UserId = student.Id;
				userJournalModels.Add(userJournalModel);
			}

			//var userJournalModels = students.Select(student => new UserJournalModel()
			//{
			//	UserId = student.Id,
			//	UserName = string.Format("{0} {1}", student.FirstName, student.SecondName),
			//	Mark = UserManager.GetMark(student.Id, course.Id).Result
			//}).ToList();

			var journalEditModel = new JournalEditModel() { CourseId = course.Id, UserJournalModels = userJournalModels};

			return View(journalEditModel);
		}

		[HttpPost]
		public async Task<ActionResult> Edit(JournalEditModel model)
		{
			foreach (var user in model.UserJournalModels)
			{
				await UserManager.SetMarkAsync(user.UserId, model.CourseId, user.Mark);
			}
			return RedirectToAction("Index", new { login = User.Identity.Name });
		}

	}
}