using System.Collections.Generic;
using CacheStack.InternalExtensions;
using ServiceStack.CacheAccess;

namespace CacheStack
{
	/// <summary>
	/// Controls the caching logic for the item being cached.  You can set the duration and when to invalidate when removing the cache key isn't enough
	/// </summary>
	/// <example>
	/// context.InvalidateOn(TriggerFrom.Any&lt;User&gt;())
	/// context.InvalidateOn(TriggerFrom.Id&lt;User&gt;(id))
	/// context.InvalidateOn(TriggerFrom.Name("myKey"))
	/// context.UseCacheProfile(CacheProfile.HighPriority)
	/// </example>
	public interface ICacheContext
	{
		/// <summary>
		/// Invalidates the cache for this item, when the specified watchers are triggered
		/// </summary>
		/// <param name="watchers">Objects to watch for cache triggers. <remarks>Use the <c>TriggerFrom</c> helper</remarks></param>
		ICacheContext InvalidateOn(params ICacheTriggerWatcher[] watchers);
		/// <summary>
		/// Used to specify the cache duration based on a profile
		/// </summary>
		/// <param name="profile">Cache profile</param>
		ICacheContext UseCacheProfile(object profile);
	}

	/// <summary>
	/// Controls the caching logic for the item being cached.  You can set the duration and when to invalidate when removing the cache key isn't enough
	/// </summary>
	/// <example>
	/// context.InvalidateOn(TriggerFrom.Any&lt;User&gt;())
	/// context.InvalidateOn(TriggerFrom.Id&lt;User&gt;(id))
	/// context.InvalidateOn(TriggerFrom.Name("myKey"))
	/// context.UseCacheProfile(CacheProfile.HighPriority)
	/// </example>
	public class CacheContext : ICacheContext
	{
		internal const string AnySuffix = "__Any";
		internal const string IdSuffix = "__Id_";

		public IList<ICacheTriggerWatcher> TriggerWatchers { get; private set; }
		public object CacheProfile { get; private set; }
		protected ICacheClient Cache { get; private set; }

		public CacheContext(ICacheClient cache)
		{
			TriggerWatchers = new List<ICacheTriggerWatcher>();
			Cache = cache;
		}

		public ICacheContext InvalidateOn(params ICacheTriggerWatcher[] watchers)
		{
			if (watchers == null)
				return this;

			TriggerWatchers.AddRange(watchers);
			return this;
		}

		public ICacheContext UseCacheProfile(object profile)
		{
			CacheProfile = profile;
			return this;
		}
	}
}
