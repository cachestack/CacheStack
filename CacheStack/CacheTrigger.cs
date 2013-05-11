namespace CacheStack
{
	public interface ICacheTrigger
	{
		/// <summary>
		/// Key to store cache keys for items watching updates to any item
		/// </summary>
		string CacheKeyForAnyItem { get; }
		/// <summary>
		/// Key to store cache keys for an individual item
		/// </summary>
		string CacheKeyForIndividualItem { get; }
	}

	public class CacheTrigger : ICacheTrigger
	{
		public string CacheKeyForAnyItem { get; set; }
		public string CacheKeyForIndividualItem { get; set; }
	}
}