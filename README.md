CacheStack
===========

Cache extensions for [Service Stack ICacheClient](https://github.com/ServiceStack/ServiceStack/wiki/Caching). Includes mvc donut caching and the notion of a cache context.  This is a .net 4.5 library designed to work with ASP.NET MVC 4+. This library can be installed via [nuget](https://nuget.org/packages/CacheStack).

Additional Information
----------------------
Donut caching is a customized version of [MvcDonutCaching](http://mvcdonutcaching.codeplex.com) persisting content via ICacheClient and leveraging cache context for cache invalidation.

The cache context allows you to setup cache profiles and triggers. Cache profiles are not constrained to the web.config and can be defined in a variety of ways. Cache triggers allow objects to be invalidated from the cache without worrying about specific cache keys.

Conventions/Best Practices
-----------

* Relation triggers will need a ReferencesAttribute to work properly. [Spruce](https://github.com/jgeurts/spruce) and [OrmLite](https://github.com/servicestack/servicestack.ormlite) both have implementations that will suffice.  You will also see the best results if your object relations have an Id field specified.
* If you use attribute routing, you'll be further ahead to set `CacheStackSettings.UseRouteNameForCacheKey = true;`


Example Usage
------------------------------

### Configuring cache durations using cache profiles
```csharp
// Somewhere in your application configuration

// Get the cache client from your DI framework of choice or some other way
CacheStackSettings.CacheClient = ObjectFactory.GetInstance<ICacheClient>();
// All of our routes are unique and not shared, so we can use the route name instead of reflection to get a unique cache key
CacheStackSettings.UseRouteNameforCacheKey = true;
CacheStackSettings.CacheProfileDurations = profile => {
	// Can get these values from a db, web.config, or anywhere else
	switch ((CacheProfile)profile) {
		case CacheProfile.Profile1:
			return TimeSpan.FromSeconds(1);
		case CacheProfile.Profile2:
			return TimeSpan.FromMinutes(60);
		default:
			return TimeSpan.FromMinutes(15);
	}
};

// Somewhere else in your solution
public enum CacheProfile {
	Profile1,
	Profile2,
}
```


### Using cache context with donut caching and mvc controller actions
```cscharp
	public class MyController : Controller, IWithCacheContext {
		public ICacheContext CacheContext { get; private set; }
		
		public MyController(ICacheClient cache) {
			CacheContext = new CacheContext(cache);
		}
		
		[DonutOutputCache]
		public ActionResult Index() {
			// Set the cache profile to use when caching this action
			CacheContext.UseCacheProfile(CacheProfile.Profile2);
			// Setup a trigger to invalidate the cache for this action when any MyObject item is updated
			CacheContext.InvalidateOn(TriggerFrom.Any<MyObject>());
		
			return View();
		}
	}
```


### Invalidating cache objects
```csharp
// Somewhere in your code - for this example, we'll invalidate from a controller action
public ActionResult MyAction(id) {
	var myObject = Db.GetByIdOrDefault<MyObject>(id);
	myObject.UpdatedProperty = DateTime.UtcNow;
	Db.Save(myObject);
	
	// This will invalidate all objects watching for changes to MyObject types with this object's id
	Cache.Trigger(TriggerFor.Id<MyObject>(myObject.Id));
}
```
