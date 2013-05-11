using System.Configuration;
using System.Security;
using System.Web.Configuration;
using ServiceStack.Logging;

namespace CacheStack.DonutCaching
{
	public interface ICacheSettingsManager
	{
		bool IsCachingEnabledGlobally { get; }
	}

	public class CacheSettingsManager : ICacheSettingsManager
	{
		private const string AspnetInternalProviderName = "AspNetInternalProvider";
		private readonly OutputCacheSection _outputCacheSection;

		public CacheSettingsManager()
		{
			try
			{
				_outputCacheSection = (OutputCacheSection) ConfigurationManager.GetSection("system.web/caching/outputCache");
			}
			catch (SecurityException)
			{
				var log = LogManager.GetLogger(GetType());
				log.Warn("DonutCaching does not have permission to read web.config section 'OutputCacheSection'.");
				_outputCacheSection = new OutputCacheSection { DefaultProviderName = AspnetInternalProviderName, EnableOutputCache = true };
			}

		}

		public bool IsCachingEnabledGlobally
		{
			get { return _outputCacheSection.EnableOutputCache; }
		}
	}
}
