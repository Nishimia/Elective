using Elective.Logging;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Elective
{
	public class MvcApplication : System.Web.HttpApplication
    {
		private static readonly log4net.ILog log = LogHelper.GetLogger();
		protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			//DependencyResolver.SetResolver(new AutofacDependencyResolver())
        }

		protected void Application_Error()
		{
			var ex = Server.GetLastError();
			log.Error("",ex);
		}
	}
}
