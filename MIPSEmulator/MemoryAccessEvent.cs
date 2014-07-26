#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Represents the method that will handle events related to emulator memory accesses.
	/// </summary>
	#endregion
	public delegate void MemoryAccessEventHandler(object sender, MemoryAccessEventArgs e);
	
	#region XML Header
	/// <summary>
	/// Provides information about events related to emulator memory accesses.
	/// </summary>
	#endregion
	public class MemoryAccessEventArgs: EventArgs
	{
		#region XML Header
		/// <summary>
		/// The memory address that was accessed.
		/// </summary>
		#endregion
		public readonly uint Address;
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>MemoryAccessEventArgs</c> instance.
		/// </summary>
		/// <param name="address">The memory address that was accessed.</param>
		#endregion
		public MemoryAccessEventArgs(uint address)
		{
			Address = address;
		}
	}
}
