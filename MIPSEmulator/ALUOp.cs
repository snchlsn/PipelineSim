namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Specifies one of the operations supported by the arithmetic logic unit.
	/// </summary>
	#endregion
	public enum ALUOp: byte
	{
		#region XML Header
		/// <summary>
		/// Adds the two operands.
		/// </summary>
		#endregion
		Add,
		
		#region XML Header
		/// <summary>
		/// Subtracts the second operand from the first.
		/// </summary>
		#endregion
		Subtract,
		
		#region XML Header
		/// <summary>
		/// Shifts the first operand left by the second operand.
		/// </summary>
		#endregion
		ShiftLeftLogical,
		
		#region XML Header
		/// <summary>
		/// Shifts the first operand right by the second operand and sign extends.
		/// </summary>
		#endregion
		ShiftRightArithmetic,
		
		#region XML Header
		/// <summary>
		/// Shifts the first operand right by the second operand.
		/// </summary>
		#endregion
		ShiftRightLogical,
		
		#region XML Header
		/// <summary>
		/// Performs a bitwise and on the two operands.
		/// </summary>
		#endregion
		And,
		
		#region XML Header
		/// <summary>
		/// Performs a bitwise or on the two operands.
		/// </summary>
		#endregion
		Or,
		
		#region XML Header
		/// <summary>
		/// Performs a bitwise nor on the two operands.
		/// </summary>
		#endregion
		Nor,
		
		#region XML Header
		/// <summary>
		/// Performs a bitwise exclusive or on the two operands.
		/// </summary>
		#endregion
		Xor
	}
}
