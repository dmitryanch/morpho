using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
	public interface IMap<TKey, TValue>
	{
		TValue Get(TKey key);
		KeyValuePair<TKey, TValue>[] Get(TKey[] keys);
		void Add(TKey key, TValue value);
		void Add(KeyValuePair<TKey, TValue>[] keyValuePairs);
		void Remove(TKey key);
		void Remove(TKey[] keys);
		void Update(TKey key, TValue value);
		void Update(KeyValuePair<TKey, TValue>[] keyValuePairs);
		bool Has(TKey key);
	}
}
