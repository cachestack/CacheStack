using System;

namespace CacheStack.DonutCaching
{
	[Serializable]
	public class CacheItem
	{
		public string Content { get; set; }
		public string ContentType { get; set; }
	}
}
