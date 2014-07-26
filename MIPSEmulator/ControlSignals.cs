#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Enumerates the signals that the control unit sends down the pipeline, with the exception of ALUOp,
	/// which is encoded separately.
	/// </summary>
	#endregion
	//TODO: Document individual signals.
	[Flags]
	public enum ControlSignals: byte
	{
		None = 0x00,
		ALUSrc = 0x01,
		RegDst = 0x02,
		MemWrite = 0x04,
		
		#region XML Header
		/// <summary>
		/// Set for load instructions.  Doubles as the MemtoReg signal.
		/// </summary>
		#endregion
		MemRead = 0x08,
		Branch = 0x10,
		RegWrite = 0x20,
	}
}
