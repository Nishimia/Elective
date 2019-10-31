using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elective.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
			Response.StatusCode = 404;
            return View();
        }

		public ActionResult Forbidden()
		{
			Response.StatusCode = 403;
			return View();
		}

		public ActionResult Banned()
		{
			return View();
		}

		public ActionResult InternalError()
		{
			Response.StatusCode = 500;
			return View();
		}
    }
}