using System.Web.Routing;

namespace CacheStack.DonutCaching
{
	public class ActionSettings
	{
		public string ActionName { get; set; }
		public string ControllerName { get; set; }
		public RouteValueDictionary RouteValues { get; set; }
	}
}