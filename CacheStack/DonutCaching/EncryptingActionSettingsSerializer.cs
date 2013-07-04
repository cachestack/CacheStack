namespace CacheStack.DonutCaching
{
	public class EncryptingActionSettingsSerializer : IActionSettingsSerializer
	{
		private readonly IActionSettingsSerializer _serializer;
		private readonly IEncryptor _encryptor;

		public EncryptingActionSettingsSerializer(IActionSettingsSerializer serializer, IEncryptor encryptor)
		{
			_serializer = serializer;
			_encryptor = encryptor;
		}

		public string Serialize(ActionSettings actionSettings)
		{
			var serializedActionSettings = _serializer.Serialize(actionSettings);

			return _encryptor.Encrypt(serializedActionSettings);
		}

		public ActionSettings Deserialize(string serializedActionSettings)
		{
			var decryptedSerializedActionSettings = _encryptor.Decrypt(serializedActionSettings);

			return _serializer.Deserialize(decryptedSerializedActionSettings);
		}
	}
}