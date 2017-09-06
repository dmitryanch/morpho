using Core.Interfaces;

namespace Core.Lang.RU
{
	public unsafe class RuMorphoSigns : IMorphoSigns
	{
		private readonly byte[] _morpho = new byte[12];

		public bool Equals(IMorphoSigns other)
		{
			return GetHashCode() == other.GetHashCode();
		}

		public bool Contains(byte sign)
		{
			unchecked
			{
				fixed (byte* bp = _morpho)
				{
					if (bp[0] == 0)
					{
						return false;
					}
					for (byte i = 0; i < 12; i++)
					{
						if (bp[i] == sign)
						{
							return true;
						}
					}
					return false;
				}
			}
		}

		public void Add(byte sign)
		{
			unchecked
			{
				fixed (byte* bp = _morpho)
				{
					for (byte i = 0; i < 12; i++)
					{
						if (bp[i] > 0)
						{
							continue;
						}
						bp[i] = sign;
						return;
					}
				}
			}
		}

		public byte Length
		{
			get
			{
				unchecked
				{
					fixed (byte* bp = _morpho)
					{
						for (byte i = 0; i < 12; i++)
						{
							if (bp[i] > 0)
							{
								continue;
							}
							return i;
						}
						return 12;
					}
				}
			}
		}

		public byte[] List
		{
			get
			{
				unchecked
				{
					fixed (byte* bp = _morpho)
					{
						var length = Length;
						var arr = new byte[length];
						for (byte i = 0; i < length; i++)
						{
							arr[i] = bp[i];
						}
						return arr;
					}
				}
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				fixed (byte* bp = _morpho)
				{
					var hash1 = 1;
					var hash2 = 0;
					var i = 0;
					while (i < 12 && bp[i] != 0)
					{
						//hash1 ^= bp[i++];

						//hash1 *= bp[i++];
						//hash2 += bp[i++];

						hash1 += bp[i++] + (bp[i++] << 8) + (bp[i++] << 16) + (bp[i++] << 24);
					}

					return hash1 + hash2;
				}
			}
		}

		public override bool Equals(object obj)
		{
			var signs = (RuMorphoSigns)obj;

			return !signs.Equals(default(RuMorphoSigns)) && Equals(signs);
		}

		public override string ToString()
		{
			return string.Join(", ", List);
		}
	}
}
