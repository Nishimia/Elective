using ElectiveBLL.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectiveDAL.Managers
{
	public class ElectiveSignInManager : SignInManager<ElectiveUser, string>
	{
		public ElectiveSignInManager(ElectiveUserManager userManager, IAuthenticationManager authenticationManager)
		   : base(userManager, authenticationManager)
		{

		}

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ElectiveUser user)
		{
			return user.GenerateUserIdentityAsync((ElectiveUserManager)UserManager);
		}

		public static ElectiveSignInManager Create(IdentityFactoryOptions<ElectiveSignInManager> options, IOwinContext context)
		{
			return new ElectiveSignInManager(context.GetUserManager<ElectiveUserManager>(), context.Authentication);
		}
	}
}
