namespace MIPSEmulator.Assembly
{
	#region XML Header
	/// <summary>
	/// Specifies one of the 32 registers defined by the MIPS ISA.
	/// </summary>
	#endregion
	public enum Register: byte
	{
		#region XML Header
		/// <summary>
		/// Holds a constant value of zero.
		/// </summary>
		#endregion
		Zero,
		
		#region XML Header
		/// <summary>
		/// Temporary register for use by the assembler.
		/// </summary>
		#endregion
		AT,
		
		#region XML Header
		/// <summary>
		/// Return value register 0.
		/// </summary>
		#endregion
		V0,
		
		#region XML Header
		/// <summary>
		/// Return value register 1.
		/// </summary>
		#endregion
		V1,
		
		#region XML Header
		/// <summary>
		/// Argument register 0.
		/// </summary>
		#endregion
		A0,
		
		#region XML Header
		/// <summary>
		/// Argument register 1.
		/// </summary>
		#endregion
		A1,
		
		#region XML Header
		/// <summary>
		/// Argument register 2.
		/// </summary>
		#endregion
		A2,
		
		#region XML Header
		/// <summary>
		/// Argument register 3.
		/// </summary>
		#endregion
		A3,
		
		#region XML Header
		/// <summary>
		/// Temporary register 0.
		/// </summary>
		#endregion
		T0,
		
		#region XML Header
		/// <summary>
		/// Temporary register 1.
		/// </summary>
		#endregion
		T1,
		
		#region XML Header
		/// <summary>
		/// Temporary register 2.
		/// </summary>
		#endregion
		T2,
		
		#region XML Header
		/// <summary>
		/// Temporary register 3.
		/// </summary>
		#endregion
		T3,
		
		#region XML Header
		/// <summary>
		/// Temporary register 4.
		/// </summary>
		#endregion
		T4,
		
		#region XML Header
		/// <summary>
		/// Temporary register 5.
		/// </summary>
		#endregion
		T5,
		
		#region XML Header
		/// <summary>
		/// Temporary register 6.
		/// </summary>
		#endregion
		T6,
		
		#region XML Header
		/// <summary>
		/// Temporary register 7.
		/// </summary>
		#endregion
		T7,
		
		#region XML Header
		/// <summary>
		/// Saved register 0.
		/// </summary>
		#endregion
		S0,
		
		#region XML Header
		/// <summary>
		/// Saved register 1.
		/// </summary>
		#endregion
		S1,
		
		#region XML Header
		/// <summary>
		/// Saved register 2.
		/// </summary>
		#endregion
		S2,
		
		#region XML Header
		/// <summary>
		/// Saved register 3.
		/// </summary>
		#endregion
		S3,
		
		#region XML Header
		/// <summary>
		/// Saved register 4.
		/// </summary>
		#endregion
		S4,
		
		#region XML Header
		/// <summary>
		/// Saved register 5.
		/// </summary>
		#endregion
		S5,
		
		#region XML Header
		/// <summary>
		/// Saved register 6.
		/// </summary>
		#endregion
		S6,
		
		#region XML Header
		/// <summary>
		/// Saved register 7.
		/// </summary>
		#endregion
		S7,
		
		#region XML Header
		/// <summary>
		/// Temporary register 8.
		/// </summary>
		#endregion
		T8,
		
		#region XML Header
		/// <summary>
		/// Temporary register 9.
		/// </summary>
		#endregion
		T9,
		
		#region XML Header
		/// <summary>
		/// Kernel register 0.
		/// </summary>
		#endregion
		K0,
		
		#region XML Header
		/// <summary>
		/// Kernel register 1.
		/// </summary>
		#endregion
		K1,
		
		#region XML Header
		/// <summary>
		/// Global pointer.
		/// </summary>
		#endregion
		GP,
		
		#region XML Header
		/// <summary>
		/// Stack pointer.
		/// </summary>
		#endregion
		SP,
		
		#region XML Header
		/// <summary>
		/// Saved register 8.  May be used as the frame pointer.
		/// </summary>
		#endregion
		S8,
		
		#region XML Header
		/// <summary>
		/// Return address register.
		/// </summary>
		#endregion
		RA
	}
}
