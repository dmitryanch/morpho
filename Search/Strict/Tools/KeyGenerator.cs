using System.Text;
using Utils.MPHF;

namespace Strict.Tools
{
	internal sealed class KeyGenerator : IKeySource
	{
		readonly uint _nbKeys;
		uint _currentKey;
		string[] _keys;

		public KeyGenerator(string[] keys)
		{
			_nbKeys = (uint)keys.Length;
			_keys = keys;
		}

		public uint NbKeys { get { return _nbKeys; } }

		public byte[] Read()
		{
			return Encoding.UTF8.GetBytes(string.Format("{0}", _keys[_currentKey++]));
		}

		public void Rewind()
		{
			_currentKey = 0;
		}
	}
}
