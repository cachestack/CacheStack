using ServiceStack.Text;

namespace CacheStack.DonutCaching
{
	public interface IActionSettingsSerialiser
	{
		string Serialise(ActionSettings actionSettings);
		ActionSettings Deserialise(string serialisedActionSettings);
	}

	public class ActionSettingsSerialiser : IActionSettingsSerialiser
	{
		private readonly TypeSerializer<ActionSettings> _serialiser;

		public ActionSettingsSerialiser()
		{
			_serialiser = new TypeSerializer<ActionSettings>();
		}

		public string Serialise(ActionSettings actionSettings)
		{
			return _serialiser.SerializeToString(actionSettings);
		}

		public ActionSettings Deserialise(string serialisedActionSettings)
		{
			return _serialiser.DeserializeFromString(serialisedActionSettings);
		}
	}
}