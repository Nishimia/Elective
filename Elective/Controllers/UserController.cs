using Elective.Models.CourseModels;
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
	public class UserController : Controller
	{
		private ElectiveUserManager _userManager;

		public UserController()
		{

		}

		public UserController(ElectiveUserManager userManager)
		{
			_userManager = userManager;
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

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public ActionResult Index()
		{
			var userViewModels = new List<UserViewModel>();

			using (var userManager = UserManager)
			{
				var users = UserManager.Users.ToList();
				userViewModels = users.Select(user => new UserViewModel()
				{
					UserId = user.Id,
					UserName = user.UserName,
					CourseAmount = user.EnrolledInCourses.Count(),
					IsBanned = user.Banned,
					IsTutor = userManager.IsInRoleAsync(user.Id, ElectiveBLL.Constants.TUTOR_ROLE_NAME).Result,
					FirstName = user.FirstName,
					SecondName = user.SecondName
				}).ToList();
			}

			return View(userViewModels);
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public async Task<ActionResult> Edit(string id)
		{
			var userEditModel = new UserEditModel();
			using (var userManager = UserManager)
			{
				var user = await userManager.FindByIdAsync(id);
				if (user == null)
				{
					return RedirectToAction("Index", "User");
				}
				userEditModel.UserId = user.Id;
				userEditModel.IsBanned = user.Banned;
				userEditModel.FirstName = user.FirstName;
				userEditModel.IsTutor = userManager.IsInRoleAsync(user.Id, ElectiveBLL.Constants.TUTOR_ROLE_NAME).Result;
				userEditModel.SecondName = user.SecondName;
				userEditModel.UserName = user.UserName;
			}
			return View(userEditModel);
		}

		[HttpPost]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public async Task<ActionResult> Edit(UserEditModel model)
		{
			if (ModelState.IsValid)
			{
				using (var userManager = UserManager)
				{
					var saveChanges = false;
					var user = await userManager.FindByIdAsync(model.UserId);
					if (user != null)
					{
						if (user.Banned != model.IsBanned)
						{
							user.Banned = model.IsBanned;
							saveChanges = true;
						}
						if (model.IsTutor && !await userManager.IsInRoleAsync(user.Id, ElectiveBLL.Constants.TUTOR_ROLE_NAME))
						{
							await userManager.AddToRoleAsync(model.UserId, ElectiveBLL.Constants.TUTOR_ROLE_NAME);
							saveChanges = true;
						}
						if (!model.IsTutor && await userManager.IsInRoleAsync(user.Id, ElectiveBLL.Constants.TUTOR_ROLE_NAME))
						{
							await userManager.RemoveFromRoleAsync(model.UserId, ElectiveBLL.Constants.TUTOR_ROLE_NAME);
							saveChanges = true;
						}
					}

					if (saveChanges)
					{
						await userManager.UpdateAsync(user);
					}
				}
			}

			return RedirectToAction("Index", "User");
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.STUDENT_ROLE_NAME)]
		public async Task<ActionResult> PassedCourses()
		{
			var userName = User.Identity.Name;
			var user = await UserManager.FindByNameAsync(userName);
			if(user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var courses = await UserManager.GetUserCoursesAsync(user.Id, false);
			if(courses is null)
			{
				throw new ArgumentNullException(nameof(courses));
			}
			var courseViewModels = courses.Where(course => course.EndDate < DateTime.Today).Select(course => new UserCourseViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				Mark = course.EnrolledInCourseStudents.FirstOrDefault(uc => uc.UserId == user.Id).StudentMark
			});
			return View("UserCourses", courseViewModels);
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.STUDENT_ROLE_NAME)]
		public async Task<ActionResult> ActiveCourses()
		{
			var userName = User.Identity.Name;
			var user = await UserManager.FindByNameAsync(userName);
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var courses = await UserManager.GetUserCoursesAsync(user.Id, false);
			if (courses is null)
			{
				throw new ArgumentNullException(nameof(courses));
			}
			var courseViewModels = courses.Where(course => course.StartDate < DateTime.Today && DateTime.Today < course.EndDate).Select(course => new UserCourseViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				Mark = course.EnrolledInCourseStudents.FirstOrDefault(uc => uc.UserId == user.Id).StudentMark
			});
			return View("UserCourses", courseViewModels);
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.STUDENT_ROLE_NAME)]
		public async Task<ActionResult> FutureCourses()
		{
			var userName = User.Identity.Name;
			var user = await UserManager.FindByNameAsync(userName);
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var courses = await UserManager.GetUserCoursesAsync(user.Id, false);
			if (courses is null)
			{
				throw new ArgumentNullException(nameof(courses));
			}
			var courseViewModels = courses.Where(course => DateTime.Today < course.StartDate).Select(course => new UserCourseViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				Mark = course.EnrolledInCourseStudents.FirstOrDefault(uc => uc.UserId == user.Id).StudentMark
			});
			return View("UserCourses", courseViewModels);
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.STUDENT_ROLE_NAME)]
		public async Task<ActionResult> Delete(string courseId)
		{
			var user = await UserManager.FindByNameAsync(User.Identity.Name);
			if(user is null)
			{
				return RedirectToAction("Index", "Home");
			}
			await UserManager.RemoveFromCourseAsync(user.Id, courseId);
			return Redirect(Request.UrlReferrer.ToString());
		}

	}
}