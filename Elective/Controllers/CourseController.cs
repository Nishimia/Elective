using Elective.Logging;
using Elective.Models.CourseModels;
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
	public class CourseController : Controller
	{
		private CourseManager _courseManager;
		private SubjectManager _subjectManager;
		private ElectiveUserManager _userManager;
		private static readonly log4net.ILog log = LogHelper.GetLogger();

		public CourseController()
		{
		}

		public CourseController(CourseManager courseManager, SubjectManager subjectManager, ElectiveUserManager userManager)
		{
			_courseManager = courseManager;
			_subjectManager = subjectManager;
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

		public SubjectManager SubjectManager
		{
			get
			{
				return _subjectManager ?? HttpContext.GetOwinContext().Get<SubjectManager>();
			}
			set
			{
				_subjectManager = value;
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


		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public ActionResult Index()
		{
			var courseViewModels = new List<CourseViewModel>();
			var courses = CourseManager.Courses.ToList();
			courseViewModels = courses.Select(course => new CourseViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				EndDate = course.EndDate,
				StartDate = course.StartDate
			}).ToList();
			return View(courseViewModels);
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public ActionResult Create()
		{
			var courseCreateModel = new CourseCreateModel();
			var users = UserManager.Users.ToList();
			var tutors = users.Where(user => UserManager.IsInRoleAsync(user.Id, ElectiveBLL.Constants.TUTOR_ROLE_NAME).Result);
			courseCreateModel.TutorSelectList = tutors.Select(tutor => new SelectListItem()
			{
				Value = tutor.Id,
				Text = string.Format("{0} {1} {2}", tutor.FirstName, tutor.SecondName, tutor.UserName)
			});

			var subjects = SubjectManager.Subjects.ToList();
			courseCreateModel.SubjectSelectList = subjects.Select(subject => new SelectListItem()
			{
				Value = subject.SubjectName,
				Text = subject.SubjectName
			});
			return View(courseCreateModel);
		}

		[HttpPost]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public async Task<ActionResult> Create(CourseCreateModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var course = new Course() { CourseName = model.CourseName, EndDate = model.EndDate, StartDate = model.StartDate };
					var result = await CourseManager.CreateAsync(course);
					if (!result.Succeeded)
					{
						log.Info("Cannot create course");
						AddErrors(result);
						return View("Create", model);
					}

					if (model.Subjects != null)
					{
						result = await CourseManager.AddToSubjectsAsync(course.Id, model.Subjects.ToArray());
						if (!result.Succeeded)
						{
							log.Info("Cannot add course to subjects");
							AddErrors(result);
							return View("Create", model);
						}
					}

					if (!string.IsNullOrWhiteSpace(model.TutorId))
					{
						result = await UserManager.AddToCourseAsync(model.TutorId, course.Id, true);
						if (!result.Succeeded)
						{
							log.Info("Cannot add tutor to course");
							AddErrors(result);
							return View("Create", model);
						}
					}
				}
				catch (Exception ex)
				{
					log.Error("Failed to create course", ex);
					return RedirectToAction("InternalError", "Error");
				}
				return RedirectToAction("Index", "Course");
			}

			try
			{
				var users = UserManager.Users.ToList();
				var tutors = users.Where(user => UserManager.IsInRoleAsync(user.Id, ElectiveBLL.Constants.TUTOR_ROLE_NAME).Result);
				model.TutorSelectList = tutors.Select(tutor => new SelectListItem()
				{
					Value = tutor.Id,
					Text = string.Format("{0} {1} {2}", tutor.FirstName, tutor.SecondName, tutor.UserName)
				});

				var subjects = SubjectManager.Subjects.ToList();
				model.SubjectSelectList = subjects.Select(subject => new SelectListItem()
				{
					Value = subject.SubjectName,
					Text = subject.SubjectName
				});
			}
			catch (Exception ex)
			{
				log.Error("Failed to turn users and courses into courseCreateModel", ex);
				return RedirectToAction("InternalError", "Error");
			}
			return View("Create", model);

		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public async Task<ActionResult> Edit(string id)
		{
			var course = await CourseManager.FindByIdAsync(id);
			var courseEditModel = new CourseEditModel();
			if (course is null)
			{
				log.Info("Failed to edit course");
				return RedirectToAction("Index", "Course");
			}
			courseEditModel.CourseId = course.Id;
			courseEditModel.CourseName = course.CourseName;
			courseEditModel.EndDate = course.EndDate;
			courseEditModel.StartDate = course.StartDate;
			courseEditModel.TutorId = (await CourseManager.GetTutorAsync(course.Id))?.Id;
			courseEditModel.TutorSelectList = UserManager.GetUsersByRole(ElectiveBLL.Constants.TUTOR_ROLE_NAME).Select(tutor => new SelectListItem
			{
				Text = string.Format("{0}, {1}, {2}", tutor.FirstName, tutor.SecondName, tutor.UserName),
				Value = tutor.Id
			}).ToList();
			courseEditModel.Subjects = (await CourseManager.GetAllSubjectsAsync(course.Id)).ToList();
			courseEditModel.SubjectSelectList = SubjectManager.Subjects.Select(subject => new SelectListItem()
			{
				Text = subject.SubjectName,
				Value = subject.SubjectName,
			}).ToList();
			return View(courseEditModel);
		}

		[HttpPost]
		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public async Task<ActionResult> Edit(CourseEditModel model)
		{
			if (ModelState.IsValid)
			{
				var course = await CourseManager.FindByIdAsync(model.CourseId);
				if (course is null)
				{
					log.Info("Failed to find course");
					return RedirectToAction("Index", "Course");
				}
				course.CourseName = model.CourseName;
				course.EndDate = model.EndDate;
				course.StartDate = model.StartDate;

				var oldSubjectNameList = (await CourseManager.GetAllSubjectsAsync(course.Id)).ToList();

				if (model.Subjects != null)
				{
					var newSubjectNameList = model.Subjects;

					var subjectNameToRemoveList = oldSubjectNameList.Except(newSubjectNameList);
					await CourseManager.RemoveFromSubjectsAsync(course.Id, subjectNameToRemoveList.ToArray());

					var subjectNameToAddList = newSubjectNameList.Except(oldSubjectNameList);
					await CourseManager.AddToSubjectsAsync(course.Id, subjectNameToAddList.ToArray());
				}
				else
				{
					await CourseManager.RemoveFromSubjectsAsync(course.Id, oldSubjectNameList.ToArray());
				}

				var oldTutor = await CourseManager.GetTutorAsync(course.Id);
				if (oldTutor != null)
				{
					await UserManager.RemoveFromCourseAsync(oldTutor.Id, course.Id);
				}

				var newTutor = await UserManager.FindByIdAsync(model.TutorId);
				if (newTutor != null)
				{
					await UserManager.AddToCourseAsync(newTutor.Id, course.Id, true);
				}
			}
			else
			{
				model.SubjectSelectList = SubjectManager.Subjects.ToList().Select(subject => new SelectListItem()
				{
					Text = subject.SubjectName,
					Value = subject.SubjectName,
				}).ToList();
				model.TutorSelectList = UserManager.GetUsersByRole(ElectiveBLL.Constants.TUTOR_ROLE_NAME).Select(tutor => new SelectListItem
				{
					Value = tutor.Id,
					Text = string.Format("{0}, {1}, {2}", tutor.FirstName, tutor.SecondName, tutor.UserName)
				}).ToList();
				return View(model);
			}
			return RedirectToAction("Index", "Course");
		}

		[Authorize(Roles = ElectiveBLL.Constants.ADMIN_ROLE_NAME)]
		public async Task<ActionResult> Delete(string id)
		{
			var courseToDelete = await CourseManager.FindByIdAsync(id);
			var result = await CourseManager.DeleteAsync(courseToDelete);
			if (!result.Succeeded)
			{
				AddErrors(result);
				return View();
			}
			return RedirectToAction("Index", "Course");
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult AvailableCourses()
		{
			ViewBag.Subjects = SubjectManager.Subjects.Select(subject => new SelectListItem()
			{
				Text = subject.SubjectName,
				Value = subject.SubjectName
			});

			var courses = CourseManager.Courses.Where(course => course.StartDate > DateTime.Today).ToList();
			var courseViewModels = courses.Select(course => new CourseViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				Days = (course.EndDate - course.StartDate).Days,
				EndDate = course.EndDate,
				StartDate = course.StartDate,
				Subjects = string.Join(", ", CourseManager.GetAllSubjectsAsync(course.Id).Result.ToArray()),
				UserAmount = course.EnrolledInCourseStudents.Count()
			});
			return View(courseViewModels);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Details(string id)
		{
			var course = await CourseManager.FindByIdAsync(id);
			if (course is null)
			{
				log.Info("Course was null when we were trying to check it's details");
				return RedirectToAction("NotFound", "Error");
			}

			var courseViewModel = new CourseViewModel()
			{
				CourseId = course.Id,
				CourseName = course.CourseName,
				Days = (course.EndDate - course.StartDate).Days,
				EndDate = course.EndDate,
				StartDate = course.StartDate,
				Subjects = string.Join(", ", CourseManager.GetAllSubjectsAsync(course.Id).Result),
				UserAmount = course.EnrolledInCourseStudents.Count()
			};
			return View(courseViewModel);
		}

		[HttpGet]
		[Authorize(Roles = ElectiveBLL.Constants.STUDENT_ROLE_NAME)]
		public async Task<ActionResult> Subscribe(string courseId)
		{
			var user = await UserManager.FindByNameAsync(User.Identity.Name);
			var result = await UserManager.AddToCourseAsync(user.Id, courseId, false);
			if (!result.Succeeded)
			{
				log.Info("Failed to register on course");
			}
			return RedirectToAction("Details", "Course", new{id = courseId});
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