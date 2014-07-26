namespace MIPSEmulator.Assembly
{
	#region XML Header
	/// <summary>
	/// Specifies a MIPS instruction opcode.
	/// </summary>
	#endregion
	public enum Opcode: byte
	{
		#region XML Header
		/// <summary>
		/// Add immediate without overflow detection.
		/// </summary>
		#endregion
		Addiu = 0x09,
		
		#region XML Header
		/// <summary>
		/// Bitwise and immediate.
		/// </summary>
		#endregion
		Andi = 0x0C,
		
		#region XML Header
		/// <summary>
		/// Compare and branch if equal.
		/// </summary>
		#endregion
		Beq = 0x04,
		
		#region XML Header
		/// <summary>
		/// Jump to target.
		/// </summary>
		#endregion
		J = 0x02,
		
		#region XML Header
		/// <summary>
		/// Load word.
		/// </summary>
		#endregion
		Lw = 0x23,
		
		#region XML Header
		/// <summary>
		/// Bitwise or immediate.
		/// </summary>
		#endregion
		Ori = 0x0D,
		
		#region XML Header
		/// <summary>
		/// An R-type instruction.  The exact instruction is specified by the function field.
		/// </summary>
		#endregion
		RType = 0x00,
		
		#region XML Header
		/// <summary>
		/// Store word.
		/// </summary>
		#endregion
		Sw = 0x2B,
		
		#region XML Header
		/// <summary>
		/// Bitwise exclusive or immediate.
		/// </summary>
		#endregion
		Xori = 0x0E
	}
}
