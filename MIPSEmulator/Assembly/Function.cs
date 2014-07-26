namespace MIPSEmulator.Assembly
{
	#region XML Header
	/// <summary>
	/// Specifies a MIPS R-type function code.
	/// </summary>
	#endregion
	public enum Function: byte
	{
		#region XML Header
		/// <summary>
		/// Indicates a bubble in the pipeline, caused by a stall or processor reset.
		/// </summary>
		/// <remarks>
		/// This is a valid MIPS function code, but the function it represents is not implemented by this
		/// simplified emulator.
		/// </remarks>
		#endregion
		Bubble = 0x00,
		
		#region XML Header
		/// <summary>
		/// Add without overflow detection.
		/// </summary>
		#endregion
		Addu = 0x21,
		
		#region XML Header
		/// <summary>
		/// Bitwise and operation.
		/// </summary>
		#endregion
		And = 0x24,
		
		#region XML Header
		/// <summary>
		/// Bitwise nor operation.
		/// </summary>
		#endregion
		Nor = 0x27,
		
		#region XML Header
		/// <summary>
		/// Bitwise or operation.
		/// </summary>
		#endregion
		Or = 0x25,
		
		#region XML Header
		/// <summary>
		/// Shift left with variable shift amount.
		/// </summary>
		#endregion
		Sllv = 0x04,
		
		#region XML Header
		/// <summary>
		/// Shift right with two's complement sign preservation and variable shift amount.
		/// </summary>
		#endregion
		Srav = 0x07,
		
		#region XML Header
		/// <summary>
		/// Shift right with variable shift amount.
		/// </summary>
		#endregion
		Srlv = 0x06,
		
		#region XML Header
		/// <summary>
		/// Subtract without underflow detection.
		/// </summary>
		#endregion
		Subu = 0x23,
		
		#region XML Header
		/// <summary>
		/// Bitwise exclusive or operation.
		/// </summary>
		#endregion
		Xor = 0x26
	}
}
