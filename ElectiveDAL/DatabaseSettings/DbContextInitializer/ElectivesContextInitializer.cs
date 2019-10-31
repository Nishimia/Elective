using ElectiveBLL;
using ElectiveDAL.DatabaseSettings.DbContext;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ElectiveDAL.DatabaseSettings.DbContextInitializer
{
	/// <summary>
	/// Initialiser for TicketBoxContext
	/// </summary>
	public class ElectivesContextInitializer : DropCreateDatabaseIfModelChanges<ElectivesContext>
	{
		/// <summary>
		/// Set initial values in database
		/// </summary>
		/// <param name="context"></param>
		protected override void Seed(ElectivesContext context)
		{
			var tutorRole = new IdentityRole(Constants.TUTOR_ROLE_NAME);
			var studentRole = new IdentityRole(Constants.STUDENT_ROLE_NAME);
			var adminRole = new IdentityRole(Constants.ADMIN_ROLE_NAME);

			context.Roles.Add(tutorRole);
			context.Roles.Add(studentRole);
			context.Roles.Add(adminRole);
		}
	}
}
