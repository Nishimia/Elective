using ElectiveBLL.Interfaces.ModelInterfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElectiveDAL.Validators
{
	public class UserCourseValidator<TUser> : UserCourseValidator<TUser, string>
		where TUser : class, IElectiveUser<string>
	{

	}


	public class UserCourseValidator<TUser, TKey> : IIdentityValidator<TUser>
		where TUser : class, IElectiveUser<TKey>
	{
		public virtual Task<IdentityResult> ValidateAsync(TUser user)
		{
			if(user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var errors = new List<string>();
			ValidateName(user.FirstName, errors);
			ValidateName(user.SecondName, errors);
			if (errors.Count > 0)
			{
				return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
			}
			return Task.FromResult(IdentityResult.Success);
		}

		private void ValidateName(string name, List<string> errors)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.ValueCannotBeNullOrEmpty, nameof(name)));
			}
			else
			{
				if (name.Length > ElectiveBLL.Constants.USER_NAME_MAX_LENGTH)
				{
					errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.PropertyIsTooLong, nameof(name)));
				}
				if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
				{
					errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.NameShouldContainOnlyLetters, nameof(name)));
				}
			}
		}
	}
}
