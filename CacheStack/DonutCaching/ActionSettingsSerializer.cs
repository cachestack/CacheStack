using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;

namespace CacheStack.DonutCaching
{
	public interface IActionSettingsSerializer
	{
		string Serialize(ActionSettings actionSettings);
		ActionSettings Deserialize(string serializedActionSettings);
	}

	public class ActionSettingsSerializer : IActionSettingsSerializer
	{
		private readonly TypeSerializer<ActionSettingsSerializingShim> _serializer;

		public ActionSettingsSerializer()
		{
			_serializer = new TypeSerializer<ActionSettingsSerializingShim>();
		}

		public string Serialize(ActionSettings actionSettings)
		{
			var shim = new ActionSettingsSerializingShim
				{
					C = actionSettings.ControllerName,
					A = actionSettings.ActionName
				};
			if (actionSettings.RouteValues != null)
			{
				shim.V = new Dictionary<string, object>(actionSettings.RouteValues);
			}

			return _serializer.SerializeToString(shim);
		}

		public ActionSettings Deserialize(string serializedActionSettings)
		{
			var shim = _serializer.DeserializeFromString(serializedActionSettings);

			var result = new ActionSettings
				{
					ControllerName = shim.C,
					ActionName = shim.A
				};
			if (shim.V != null && shim.V.Any())
			{
				result.RouteValues = new System.Web.Routing.RouteValueDictionary(shim.V);	
			}
			return result;
		}
	}

	// Added this because RouteValueDictionary itself doesn't serialize properly. As a bonus, this class allows us to have very small property names which helps minimize serialized string size.
	public class ActionSettingsSerializingShim
	{
		public string C { get; set; }
		public string A { get; set; }
		public IDictionary<string, object> V { get; set; }
	}
}