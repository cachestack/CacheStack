using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using ServiceStack.Text;

namespace CacheStack.DonutCaching
{
	public interface IDonutHoleFiller
	{
		string RemoveDonutHoleWrappers(string content, ControllerContext filterContext);
		string ReplaceDonutHoleContent(string content, ControllerContext filterContext);
	}

	public class DonutHoleFiller : IDonutHoleFiller
	{
		private static readonly Regex DonutHoles = new Regex("<!--DC#(.*?)#-->(.*?)<!--/DC#\\1#-->", RegexOptions.Compiled | RegexOptions.Singleline);

		private readonly IActionSettingsSerializer _actionSettingsSerializer;

		public DonutHoleFiller(IActionSettingsSerializer actionSettingsSerializer)
		{
			if (actionSettingsSerializer == null)
			{
				throw new ArgumentNullException("actionSettingsSerializer");
			}

			_actionSettingsSerializer = actionSettingsSerializer;
		}

		public string RemoveDonutHoleWrappers(string content, ControllerContext filterContext)
		{
			return DonutHoles.Replace(content, match => match.Groups[2].Value);
		}

		public string ReplaceDonutHoleContent(string content, ControllerContext filterContext)
		{
			return DonutHoles.Replace(content, match =>
			{
				var actionSettings = _actionSettingsSerializer.Deserialize(match.Groups[1].Value);

				return InvokeAction(filterContext.Controller, actionSettings.ActionName, actionSettings.ControllerName, actionSettings.RouteValues);
			});
		}

		private static string InvokeAction(ControllerBase controller, string actionName, string controllerName, RouteValueDictionary routeValues)
		{
			var viewContext = new ViewContext(controller.ControllerContext, new WebFormView(controller.ControllerContext, "tmp"),
											  controller.ViewData, controller.TempData, TextWriter.Null);

			var htmlHelper = new HtmlHelper(viewContext, new ViewPage());

			try
			{
				return htmlHelper.Action(actionName, controllerName, routeValues).ToString();
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message == "No route in the route table matches the supplied values.")
				{
					var values = routeValues == null ? string.Empty : routeValues.Keys.Aggregate(string.Empty, (current, key) => current + ("\n" + key + ": " + routeValues[key]));
					throw new Exception("Unable to find route. Controller: {0}, Action: {1}, Route Values: {2}".Fmt(controllerName, actionName, values), ex);
				}
				throw;
			}
		}
	}
}