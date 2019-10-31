using ElectiveBLL.Interfaces.ModelInterfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Validators
{
	public class CourseValidator<TCourse> : CourseValidator<TCourse, string>
		where TCourse : class, ICourse<string>
	{
	}

	public class CourseValidator<TCourse, TKey> : IIdentityValidator<TCourse>
		where TCourse : class, ICourse<TKey>
		where TKey : IEquatable<TKey>
	{		
		public virtual Task<IdentityResult> ValidateAsync(TCourse course)
		{
			if(course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			var errors = new List<string>();
			ValidateDate(course, errors);
			ValidateName(course, errors);
			if(errors.Count() > 0)
			{
				return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
			}
			return Task.FromResult(IdentityResult.Success);
		}

		private void ValidateName(TCourse course, List<string> errors)
		{
			if (string.IsNullOrWhiteSpace(course.CourseName) || course.CourseName.Length < ElectiveBLL.Constants.TITLE_MIN_LENGTH)
			{
				errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.PropertyTooShort, nameof(course.CourseName)));
			}
			else if(course.CourseName.Length > ElectiveBLL.Constants.TITLE_MAX_LENGTH)
			{
				errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.PropertyIsTooLong, nameof(course.CourseName)));
			}
		}

		private void ValidateDate(TCourse course, List<string> errors)
		{
			if(course.StartDate >= course.EndDate)
			{
				errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.StartDateIsLaterThanEndDate,
					nameof(course.StartDate), nameof(course.EndDate)));
			}
		}
	}
}
