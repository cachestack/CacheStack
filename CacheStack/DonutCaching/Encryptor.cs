using System;
using System.Text;
using System.Web.Security;

namespace CacheStack.DonutCaching
{
	public interface IEncryptor
	{
		string Encrypt(string plainText);
		string Decrypt(string encryptedText);
	}

	public class Encryptor : IEncryptor
	{
		const string EncryptPurpose = "DonutCaching";

		public string Encrypt(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var bytes = Encoding.UTF8.GetBytes(text);
			return Convert.ToBase64String(MachineKey.Protect(bytes, EncryptPurpose));
		}

		public string Decrypt(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var textAsBytes = Convert.FromBase64String(text);
			var bytes = MachineKey.Unprotect(textAsBytes, EncryptPurpose);
			if (bytes == null)
				return string.Empty;
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
