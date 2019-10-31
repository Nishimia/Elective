using Microsoft.Owin;
using Owin;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
[assembly: OwinStartupAttribute(typeof(Elective.Startup))]
namespace Elective
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
