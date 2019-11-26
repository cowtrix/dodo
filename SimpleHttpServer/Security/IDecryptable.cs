using SimpleHttpServer.REST;

namespace Common.Security
{
	public interface IKeyDecryptable<TKey, TVal> : IDecryptable
	{
		TVal GetValue(TKey key, string password);
	}

	/// <summary>
	/// Represents an object that can be decrypted by a valid combination
	/// of both a key value and a passphrase
	/// </summary>
	public interface IKeyDecryptable : IDecryptable
	{
	}

	public interface IDecryptable<T> : IDecryptable
	{
		T GetValue(string password);
	}

	/// <summary>
	/// Represents an object that can be decrypted with a passphrase to expose an underlying value
	/// and have that value updated
	/// </summary>
	public interface IDecryptable
	{
		bool TryGetValue(object requester, string passphrase, out object result);
		void SetValue(object innerObject, EUserPriviligeLevel view, object requester, string passphrase);
	}
}