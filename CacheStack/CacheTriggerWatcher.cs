namespace CacheStack
{
	public interface ICacheTriggerWatcher
	{
		/// <summary>
		/// Name of the trigger to watch for
		/// </summary>
		string Name { get; }
	}

	public class CacheTriggerWatcher : ICacheTriggerWatcher
	{
		public string Name { get; set; }
	}
}