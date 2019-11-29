using SimpleHttpServer.REST;

namespace Common.Security
{
	public interface IKeyDecryptable<TKey, TVal> : IDecryptable
	{
		TVal GetValue(TKey key, Passphrase passphrase);
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
		T GetValue(Passphrase passphrase);
	}

	/// <summary>
	/// Represents an object that can be decrypted with a passphrase to expose an underlying value
	/// and have that value updated
	/// </summary>
	public interface IDecryptable
	{
		bool TryGetValue(object requester, Passphrase passphrase, out object result);
		void SetValue(object innerObject, EPermissionLevel view, object requester, Passphrase passphrase);
		bool IsAuthorised(object requester, Passphrase passphrase);
	}
}