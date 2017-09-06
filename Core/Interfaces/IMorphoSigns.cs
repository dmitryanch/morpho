using System;

namespace Core.Interfaces
{
	public interface IMorphoSigns : IEquatable<IMorphoSigns>
	{
		bool Contains(byte sign);
		void Add(byte sign);
		byte Length { get; }
		byte[] List { get; }
	}
}
