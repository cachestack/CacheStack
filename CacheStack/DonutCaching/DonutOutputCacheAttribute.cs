using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using ServiceStack.CacheAccess;

namespace CacheStack.DonutCaching
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class DonutOutputCacheAttribute : ActionFilterAttribute, IExceptionFilter
	{
		private readonly IKeyGenerator _keyGenerator;
		private readonly IDonutHoleFiller _donutHoleFiller;
		private readonly ICacheClient _cacheClient;
		private readonly ICacheSettingsManager _cacheSettingsManager;
		private readonly ICacheHeadersHelper _cacheHeadersHelper;
		private CacheSettings _cacheSettings;

		/// <summary>
		/// A semicolon-separated list of strings that correspond to query-string values for the GET method, or to parameter values for the POST method.
		/// </summary>
		public string VaryByParam { get; set; }
		/// <summary>
		/// Represents text that is used for custom output caching requirements
		/// </summary>
		public string VaryByCustom { get; set; }
		/// <summary>
		/// Where to store the cached output. Keep in mind that anything other than Server cannot be cleared
		/// </summary>
		public OutputCacheLocation Location { get; set; }
		/// <summary>
		/// Gets or sets a value that indicates whether to store the cache. This is mainly used by cache proxies and dictates whether the proxy should store the file locally or not
		/// </summary>
		public bool NoStore { get; set; }

		public DonutOutputCacheAttribute()
		{
			var keyBuilder = new KeyBuilder();

			_keyGenerator = new KeyGenerator(keyBuilder);
			_donutHoleFiller = new DonutHoleFiller(new EncryptingActionSettingsSerialiser(new ActionSettingsSerialiser(), new Encryptor()));
			_cacheSettingsManager = new CacheSettingsManager();
			_cacheHeadersHelper = new CacheHeadersHelper();
			_cacheClient = CacheStackSettings.CacheClient;

			Location = (OutputCacheLocation) (-1);
			// Set the order relatively high so that we can have other attributes run before this one
			Order = 100;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			_cacheSettings = new CacheSettings
				{
					IsCachingEnabled = _cacheSettingsManager.IsCachingEnabledGlobally,
					VaryByCustom = VaryByCustom,
					VaryByParam = VaryByParam,
					Location = (int) Location == -1 ? OutputCacheLocation.Server : Location,
					NoStore = NoStore,
					Duration = 10
				};

			var cacheKey = _keyGenerator.GenerateKey(filterContext, _cacheSettings);

			if (_cacheSettings.IsServerCachingEnabled)
			{
				if (_cacheClient == null)
					throw new NullReferenceException("CacheStackSettings.CacheClient has not been configured. Please initialize the ICacheClient for CacheStack.");

				var cachedItem = _cacheClient.Get<CacheItem>(cacheKey);

				if (cachedItem != null)
				{
					filterContext.Result = new ContentResult
					{
						Content = _donutHoleFiller.ReplaceDonutHoleContent(cachedItem.Content, filterContext),
						ContentType = cachedItem.ContentType
					};
				}
			}

			if (filterContext.Result == null)
			{
				var cachingWriter = new StringWriter(CultureInfo.InvariantCulture);

				var originalWriter = filterContext.HttpContext.Response.Output;

				filterContext.HttpContext.Response.Output = cachingWriter;

				filterContext.HttpContext.Items[cacheKey] = new Action<bool, ICacheContext>((hasErrors, context) =>
				{
					filterContext.HttpContext.Items.Remove(cacheKey);
					filterContext.HttpContext.Response.Output = originalWriter;

					if (hasErrors)
						return;

					var cacheItem = new CacheItem
						{
							Content = cachingWriter.ToString(),
							ContentType = filterContext.HttpContext.Response.ContentType
						};

					filterContext.HttpContext.Response.Write(_donutHoleFiller.RemoveDonutHoleWrappers(cacheItem.Content, filterContext));

					if (_cacheSettings.IsServerCachingEnabled && filterContext.HttpContext.Response.StatusCode == 200)
					{
						_cacheClient.CacheAndSetTriggers(context as CacheContext, cacheKey, cacheItem);
					}
				});
			}
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			if (_cacheSettings == null)
				return;

			ExecuteCallback(filterContext, filterContext.Exception != null);

			if (!filterContext.IsChildAction)
			{
				_cacheHeadersHelper.SetCacheHeaders(filterContext.HttpContext.Response, _cacheSettings);
			}
		}

		public void OnException(ExceptionContext filterContext)
		{
			if (_cacheSettings == null)
				return;

			ExecuteCallback(filterContext, true);
		}

		private void ExecuteCallback(ControllerContext controllerContext, bool hasErrors)
		{
			var cacheKey = _keyGenerator.GenerateKey(controllerContext, _cacheSettings);

			var callback = controllerContext.HttpContext.Items[cacheKey] as Action<bool, ICacheContext>;

			if (callback != null)
			{
				var controller = controllerContext.Controller as IWithCacheContext;
				callback.Invoke(hasErrors, controller != null ? controller.CacheContext : null);
			}
		}
	}
}