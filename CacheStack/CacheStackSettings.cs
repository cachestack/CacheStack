using System;
using System.Collections.Generic;
using ServiceStack.CacheAccess;

namespace CacheStack
{
	public static class CacheStackSettings
	{
		/// <summary>
		/// Cache client to use with output caching
		/// </summary>
		public static ICacheClient CacheClient { get; set; }

		/// <summary>
		/// When generating cache keys, use the route name if available
		/// </summary>
		public static bool UseRouteNameForCacheKey { get; set; }

		/// <summary>
		/// Defines the types of objects to search when TriggerFor.ObjectAndRelations is called. These types should have one or more properties with ReferencesAttribute
		/// </summary>
		public static IList<Type> TypesForObjectRelations = new List<Type>();

		/// <summary>
		/// Defines the method for getting cache durations by profile
		/// </summary>
		public static Func<object, TimeSpan> CacheProfileDurations = profile => TimeSpan.FromMinutes(15);

		/// <summary>
		/// Provides a mechanism to get all cache keys for a particular object
		/// </summary>
		public static IDictionary<Type, Func<object, IEnumerable<string>>> CacheKeysForObject = new Dictionary<Type, Func<object, IEnumerable<string>>>();
	}
}
