using ElectiveBLL.Interfaces.ManagerInterfaces;
using ElectiveBLL.Interfaces.ModelInterfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Validators
{
	public class SubjectValidator<TSubject> : SubjectValidator<TSubject, string>
		where TSubject : class, ISubject<string>
	{
		public SubjectValidator(ISubjectManager<TSubject, string> manager)
			: base(manager)
		{

		}
	}

	public class SubjectValidator<TSubject, TKey> : IIdentityValidator<TSubject>
		where TSubject : class, ISubject<TKey>
		where TKey : IEquatable<TKey>
	{
		public SubjectValidator(ISubjectManager<TSubject, TKey> manager)
		{
			Manager = manager ?? throw new ArgumentNullException(nameof(manager));
		}

		private ISubjectManager<TSubject, TKey> Manager { get; set; }

		public virtual async Task<IdentityResult> ValidateAsync(TSubject subject)
		{
			if(subject is null)
			{
				throw new ArgumentNullException(nameof(subject));
			}
			var errors = new List<string>();
			await ValidateSubjectNameAsync(subject, errors);
			if(errors.Count() > 0)
			{
				return IdentityResult.Failed(errors.ToArray());
			}
			return IdentityResult.Success;
		}

		private async Task ValidateSubjectNameAsync(TSubject subject, List<string> errors)
		{
			if (string.IsNullOrWhiteSpace(subject.SubjectName))
			{
				errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.PropertyTooShort, nameof(subject.SubjectName)));
			}
			else if(subject.SubjectName.Length > ElectiveBLL.Constants.TITLE_MAX_LENGTH)
			{
				errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.PropertyIsTooLong, nameof(subject.SubjectName)));
			}
			else
			{
				var owner = await Manager.FindByNameAsync(subject.SubjectName).WithCurrentCulture();
				if (owner != null && !EqualityComparer<TKey>.Default.Equals(owner.Id, subject.Id))
				{
					errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.DuplicateSubjectName, subject.SubjectName));
				}
			}
		}
	}
}
