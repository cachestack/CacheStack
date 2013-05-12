using System;

namespace CacheStack
{
	public class CacheHitEventArgs : EventArgs
	{
		public string CacheKey { get; set; }
		public Type Type { get; set; }
	}
}
