using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CacheStack.DonutCaching
{
	public static class HtmlHelperExtensions
	{
		private static readonly IActionSettingsSerializer Serializer = new EncryptingActionSettingsSerializer(new ActionSettingsSerializer(), new Encryptor());

		/// <summary>
		/// Invokes the specified child action method and returns the result as an HTML string.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the action method to invoke.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		/// <returns>The child action result as an HTML string.</returns>
		public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, bool excludeFromParentCache)
		{
			return htmlHelper.Action(actionName, null, null, excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and returns the result as an HTML string.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the action method to invoke.</param>
		/// <param name="routeValues">An object that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		/// <returns>The child action result as an HTML string.</returns>
		public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, object routeValues, bool excludeFromParentCache)
		{
			return htmlHelper.Action(actionName, null, new RouteValueDictionary(routeValues), excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and returns the result as an HTML string.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the action method to invoke.</param>
		/// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		/// <returns>The child action result as an HTML string.</returns>
		public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, RouteValueDictionary routeValues, bool excludeFromParentCache)
		{
			return htmlHelper.Action(actionName, null, routeValues, excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the action method to invoke.</param>
		/// <param name="controllerName">The name of the controller that contains the action method.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		/// <returns>The child action result as an HTML string.</returns>
		public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, string controllerName, bool excludeFromParentCache)
		{
			return htmlHelper.Action(actionName, controllerName, null, excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the action method to invoke.</param>
		/// <param name="controllerName">The name of the controller that contains the action method.</param>
		/// <param name="routeValues">An object that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		/// <returns>The child action result as an HTML string.</returns>
		public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, bool excludeFromParentCache)
		{
			return htmlHelper.Action(actionName, controllerName, new RouteValueDictionary(routeValues), excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the child action method to invoke.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>

		public static void RenderAction(this HtmlHelper htmlHelper, string actionName, bool excludeFromParentCache)
		{
			RenderAction(htmlHelper, actionName, null, null, excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the child action method to invoke.</param>
		/// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>

		public static void RenderAction(this HtmlHelper htmlHelper, string actionName, object routeValues, bool excludeFromParentCache)
		{
			RenderAction(htmlHelper, actionName, null, new RouteValueDictionary(routeValues), excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the child action method to invoke.</param>
		/// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>        
		public static void RenderAction(this HtmlHelper htmlHelper, string actionName, RouteValueDictionary routeValues, bool excludeFromParentCache)
		{
			RenderAction(htmlHelper, actionName, null, routeValues, excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the child action method to invoke.</param>
		/// <param name="controllerName">The name of the controller that contains the action method.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>        
		public static void RenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName, bool excludeFromParentCache)
		{
			RenderAction(htmlHelper, actionName, controllerName, null, excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the child action method to invoke.</param>
		/// <param name="controllerName">The name of the controller that contains the action method.</param>
		/// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		public static void RenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, bool excludeFromParentCache)
		{
			RenderAction(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), excludeFromParentCache);
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and renders the result inline in the parent view.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the child action method to invoke.</param>
		/// <param name="controllerName">The name of the controller that contains the action method.</param>
		/// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		public static void RenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, bool excludeFromParentCache)
		{
			string serializedActionSettings = null;
			if (excludeFromParentCache)
			{
				serializedActionSettings = GetSerializedActionSettings(actionName, controllerName, routeValues);

				htmlHelper.ViewContext.Writer.Write("<!--DC#{0}#-->", serializedActionSettings);
			}

			htmlHelper.RenderAction(actionName, controllerName, routeValues);

			if (excludeFromParentCache)
			{
				serializedActionSettings = serializedActionSettings ?? GetSerializedActionSettings(actionName, controllerName, routeValues);
				htmlHelper.ViewContext.Writer.Write("<!--/DC#{0}#-->", serializedActionSettings);
			}
		}

		/// <summary>
		/// Invokes the specified child action method using the specified parameters and controller name and returns the result as an HTML string.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="actionName">The name of the action method to invoke.</param>
		/// <param name="controllerName">The name of the controller that contains the action method.</param>
		/// <param name="routeValues">A dictionary that contains the parameters for a route. You can use routeValues to provide the parameters that are bound to the action method parameters. The routeValues parameter is merged with the original route values and overrides them.</param>
		/// <param name="excludeFromParentCache">A flag that determines whether the action should be excluded from any parent cache.</param>
		/// <returns>The child action result as an HTML string.</returns>
		public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, bool excludeFromParentCache)
		{
			if (excludeFromParentCache)
			{
				var serializedActionSettings = GetSerializedActionSettings(actionName, controllerName, routeValues);

				return new MvcHtmlString(string.Format("<!--DC#{0}#-->{1}<!--/DC#{0}#-->", serializedActionSettings, htmlHelper.Action(actionName, controllerName, routeValues)));
			}

			return htmlHelper.Action(actionName, controllerName, routeValues);
		}

		private static string GetSerializedActionSettings(string actionName, string controllerName, RouteValueDictionary routeValues)
		{
			var actionSettings = new ActionSettings
			{
				ActionName = actionName,
				ControllerName = controllerName,
				RouteValues = routeValues
			};

			return Serializer.Serialize(actionSettings);
		}
	}
}