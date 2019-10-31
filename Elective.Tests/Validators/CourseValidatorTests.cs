using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using ElectiveBLL.Models;

namespace ElectiveDAL.Validators.Tests
{
	[TestClass()]
	public class CourseValidatorTests
	{
		[TestMethod()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidateAsync_CourseIsNull_ThrowArgumentNullException()
		{
			//	Arrange
			Course course = null;
			var validator = new CourseValidator<Course>();

			//	Act
			validator.ValidateAsync(course);
		}

		[TestMethod()]
		public async Task ValidateAsync_CourseNameIsNull_ShouldReturnIdentityResultFails()
		{
			//	Arrange
			Course course = new Course() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1)};
			var validator = new CourseValidator<Course>();

			//	Act
			var result = await validator.ValidateAsync(course);

			//	Assert
			Assert.IsFalse(result.Succeeded);
		}

		[TestMethod()]
		public async Task ValidateAsync_CourseNameIsLongerThanAllowed_ShouldReturnIdentityResultFails()
		{
			//	Arrange
			Course course = new Course() { CourseName = new string('a', 400), StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) };
			var validator = new CourseValidator<Course>();

			//	Act
			var result = await validator.ValidateAsync(course);

			//	Assert
			Assert.IsFalse(result.Succeeded);
		}

		[TestMethod()]
		public async Task ValidateAsync_StartDateIsLaterThanEndDate_ShouldReturnIdentityResultFails()
		{
			//	Arrange
			Course course = new Course() { CourseName = "lnnlmll", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(-1) };
			var validator = new CourseValidator<Course>();

			//	Act
			var result = await validator.ValidateAsync(course);

			//	Assert
			Assert.IsFalse(result.Succeeded);
		}

		[TestMethod()]
		public async Task ValidateAsync_CourseIsCorrect_ShouldReturnIdentityResultSuccess()
		{
			//	Arrange
			Course course = new Course() { CourseName = "lnnlmll", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(+1) };
			var validator = new CourseValidator<Course>();

			//	Act
			var result = await validator.ValidateAsync(course);

			//	Assert
			Assert.IsTrue(result.Succeeded);
		}
	}
}