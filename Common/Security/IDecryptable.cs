namespace Common
{
	public interface IKeyDecryptable<TKey, TVal> : IDecryptable
	{
		TVal GetValue(TKey key, string password);
	}

	public interface IKeyDecryptable : IDecryptable
	{
	}

	public interface IDecryptable<T> : IDecryptable
	{
		T GetValue(string password);
	}

	public interface IDecryptable 
	{
		bool TryGetValue(object requester, string passphrase, out object result);
		void SetValue(object innerObject, object requester, string passphrase);
	}
}