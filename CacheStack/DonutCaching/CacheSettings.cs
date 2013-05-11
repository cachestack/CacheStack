﻿using System.Web.UI;

namespace CacheStack.DonutCaching
{
	public class CacheSettings
	{
		public bool IsCachingEnabled { get; set; }
		public int Duration { get; set; }
		public string VaryByParam { get; set; }
		public string VaryByCustom { get; set; }
		public OutputCacheLocation Location { get; set; }
		public bool NoStore { get; set; }

		public bool IsServerCachingEnabled
		{
			get
			{
				return IsCachingEnabled && (Location == OutputCacheLocation.Any ||
											Location == OutputCacheLocation.Server ||
											Location == OutputCacheLocation.ServerAndClient);
			}
		}
	}
}
