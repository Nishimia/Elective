using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElectiveDAL.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectiveBLL.Models;
using Microsoft.AspNet.Identity;

namespace ElectiveDAL.Validators.Tests
{
	[TestClass()]
	public class UserCourseValidatorTests
	{
		[TestMethod()]
		public async Task ValidateAsync_UserWithBlankFirstNameAndSecondName_ShouldReturnIdentityResultFailed()
		{
			//	Arrange
			var user = new ElectiveUser();
			var validator = new UserCourseValidator<ElectiveUser>();

			//	Act
			var result = await validator.ValidateAsync(user);

			//	Assert
			Assert.IsFalse(result.Succeeded);
		}

		[TestMethod()]
		public async Task ValidateAsync_UserNameIsLongerThanAllowed_ShouldReturnIdentityResultFailed()
		{
			//	Arrange
			var user = new ElectiveUser() { FirstName = new string('a', 400), SecondName = new string('s', 400)};
			var validator = new UserCourseValidator<ElectiveUser>();

			//	Act
			var result = await validator.ValidateAsync(user);

			//	Assert
			Assert.IsFalse(result.Succeeded);
		}

		[TestMethod]
		public async Task ValidateAsync_NamesWithNotAllowedSymbols_ShouldReturnIdentityResultFailed()
		{
			//	Arrange
			var user = new ElectiveUser() { FirstName = "0202ixpsdm", SecondName = "dspmod/**omodno"};
			var validator = new UserCourseValidator<ElectiveUser>();
			
			// Act
			var result = await validator.ValidateAsync(user);

			//Assert
			Assert.IsFalse(result.Succeeded);
		}

		[TestMethod]
		public async Task ValidateAsync_AppropriateName_ShouldReturnIdentityResultSuccess()
		{
			//	Arrange
			var user = new ElectiveUser() { FirstName = "Nate", SecondName = "Black"};
			var validator = new UserCourseValidator<ElectiveUser>();

			// Act
			var result = await validator.ValidateAsync(user);

			//Assert
			Assert.IsTrue(result.Succeeded);
		}
	}
}