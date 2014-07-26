#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	[Flags]
	public enum HazardTypes: byte
	{
		None = 0x00,
		Branch = 0x01,
		ComputeUseMemory = 0x02,
		ComputeUseExecute = 0x04,
		LoadUseMemory = 0x08,
		LoadUseExecute = 0x10
	}
}
