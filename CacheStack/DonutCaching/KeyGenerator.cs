using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CacheStack.DonutCaching
{
	public interface IKeyGenerator
	{
		string GenerateKey(ControllerContext context, CacheSettings cacheSettings);
	}

	public class KeyGenerator : IKeyGenerator
	{
		private readonly IKeyBuilder _keyBuilder;

		public KeyGenerator(IKeyBuilder keyBuilder)
		{
			if (keyBuilder == null)
			{
				throw new ArgumentNullException("keyBuilder");
			}

			_keyBuilder = keyBuilder;
		}

		public string GenerateKey(ControllerContext context, CacheSettings cacheSettings)
		{
			string actionName;
			string controllerName = null;

			var prefix = context.RouteData.DataTokens["routeName"];
			if (!CacheStackSettings.UseRouteNameForCacheKey || prefix == null)
			{
				actionName = context.RouteData.Values["action"].ToString();
				controllerName = context.RouteData.Values["controller"].ToString();
				var area = context.RouteData.DataTokens["area"];
				if (area != null)
					controllerName = area + "." + controllerName;
			}
			else
			{
				actionName = prefix.ToString();
			}

			// remove controller, action and DictionaryValueProvider which is added by the framework for child actions
			var filteredRouteData = context.RouteData.Values.Where(x => !x.Key.Equals("controller", StringComparison.OrdinalIgnoreCase) &&
																   !x.Key.Equals("action", StringComparison.OrdinalIgnoreCase) &&
																   !(x.Value is DictionaryValueProvider<object>));

			var routeValues = new RouteValueDictionary(filteredRouteData.ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value));

			if (!context.IsChildAction)
			{
				// note that route values take priority over form values and form values take priority over query string values

				foreach (var formKey in context.HttpContext.Request.Form.AllKeys)
				{
					if (!routeValues.ContainsKey(formKey.ToLowerInvariant()))
					{
						routeValues.Add(formKey.ToLowerInvariant(),
										context.HttpContext.Request.Form[formKey].ToLowerInvariant());
					}
				}

				foreach (var queryStringKey in context.HttpContext.Request.QueryString.AllKeys)
				{
					// queryStringKey is null if url has qs name without value. e.g. test.com?q
					if (queryStringKey != null && !routeValues.ContainsKey(queryStringKey.ToLowerInvariant()))
					{
						routeValues.Add(queryStringKey.ToLowerInvariant(),
										context.HttpContext.Request.QueryString[queryStringKey].ToLowerInvariant());
					}
				}
			}

			if (!string.IsNullOrEmpty(cacheSettings.VaryByParam))
			{
				if (cacheSettings.VaryByParam.ToLowerInvariant() == "none")
				{
					routeValues.Clear();
				}
				else if (cacheSettings.VaryByParam != "*")
				{
					var parameters = cacheSettings.VaryByParam.ToLowerInvariant().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					routeValues = new RouteValueDictionary(routeValues.Where(x => parameters.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value));
				}
			}

			if (!string.IsNullOrEmpty(cacheSettings.VaryByCustom))
			{
				routeValues.Add(cacheSettings.VaryByCustom.ToLowerInvariant(), context.HttpContext.ApplicationInstance.GetVaryByCustomString(HttpContext.Current, cacheSettings.VaryByCustom));
			}

			var key = _keyBuilder.BuildKey(controllerName, actionName, routeValues);

			return key;
		}
	}
}