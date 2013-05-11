using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.CacheAccess;

namespace CacheStack
{
	public static class CacheExtensions
	{
		private static readonly ConcurrentDictionary<string, List<string>> Triggers = new ConcurrentDictionary<string, List<string>>();
		
		/// <summary>
		/// Trigger a notification to clear the cache for items that are watching the specified <c>ICacheTrigger</c>
		/// </summary>
		/// <param name="cache">Cache to work with</param>
		/// <param name="triggers">Triggers to clear cache listeners <remarks>Use the <c>TriggerFor</c> helper</remarks></param>
		public static void Trigger(this ICacheClient cache, params ICacheTrigger[] triggers)
		{
			var keys = new List<string>();
			foreach (var trigger in triggers)
			{
				AddUniqueKey(keys, trigger.CacheKeyForAnyItem);
				AddUniqueKey(keys, trigger.CacheKeyForIndividualItem);
			}
			foreach (var key in keys)
			{
				// Handle the case where users expect an actual cache key with this name to be cleared
				cache.Remove(key);
				ClearTrigger(cache, key);
			}
		}

		private static void AddUniqueKey(ICollection<string> keys, string key)
		{
			if (string.IsNullOrEmpty(key))
				return;
			if (keys.Any(x => x == key))
				return;
			keys.Add(key);
		}

		private static void ClearTrigger(ICacheClient cache, string key)
		{
			if (string.IsNullOrEmpty(key))
				return;

			List<string> keys;
			// Do not need the trigger keys any more since they will be re-added as needed
			Triggers.TryRemove(key, out keys);
			if (keys == null)
				return;

			foreach (var cacheKey in keys)
			{
				cache.Remove(cacheKey);
			}
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="key">Cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <returns></returns>
		public static T GetOrCache<T>(this ICacheClient cache, string key, Func<ICacheContext, T> cacheAction) where T : class
		{
			var item = cache.Get<T>(key);
			if (item == null)
			{
				var context = new CacheContext(cache);

				item = cacheAction(context);

				// No need to cache null values
				if (item != null)
					cache.CacheAndSetTriggers(context, key, item);
			}
			return item;
		}

		/// <summary>
		/// Caches the specified item using the context information
		/// </summary>
		/// <typeparam name="T">Type of object to cache</typeparam>
		/// <param name="cache">Cache to store the object</param>
		/// <param name="context">Context information for how to cache the object</param>
		/// <param name="key">Cache key</param>
		/// <param name="item">Item to cache</param>
		public static void CacheAndSetTriggers<T>(this ICacheClient cache, CacheContext context, string key, T item)
		{
			// Don't cache if there is no context
			if (context == null)
				return;

			// Don't cache if there are no profile durations configured
			if (CacheStackSettings.CacheProfileDurations == null)
				return;

			var expiration = CacheStackSettings.CacheProfileDurations(context.CacheProfile);

			cache.Set(key, item, expiration);

			foreach (var watch in context.TriggerWatchers)
			{
				var keys = Triggers.GetOrAdd(watch.Name, new List<string>());
				keys.Add(key);
			}
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="key">Cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <returns></returns>
		public static T GetOrCacheStruct<T>(this ICacheClient cache, string key, Func<ICacheContext, T> cacheAction) where T : struct
		{
			// Wrap the value type as nullable and check the cache
			var item = cache.Get<T?>(key);
			if (item == null)
			{
				var context = new CacheContext(cache);

				item = cacheAction(context);
				cache.CacheAndSetTriggers(context, key, item);
			}
			return item.Value;
		}
	}
}