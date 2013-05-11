namespace CacheStack
{
	public interface IWithCacheContext
	{
		ICacheContext CacheContext { get; }
	}
}
