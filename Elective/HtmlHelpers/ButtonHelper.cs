using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elective.HtmlHelpers
{
	public static class ButtonHelper
	{
		public static MvcHtmlString Button(this HtmlHelper helper, string content, string reference, params string[] classes)
		{
			var builder = new TagBuilder("a");

			builder.AddCssClass("btn");
			builder.AddCssClass("btn-primary");

			foreach (var c in classes)
			{
				builder.AddCssClass(c);
			}

			builder.InnerHtml = content;
			builder.Attributes.Add("href", reference);

			return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
		}
	}
}