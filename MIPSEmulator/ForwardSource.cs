namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Specifies a select value for a forwarding mux.
	/// </summary>
	#endregion
	public enum ForwardSource: byte
	{
		#region XML Header
		/// <summary>
		/// Selects input from the decode register (no forwarding).
		/// </summary>
		#endregion
		Decode,
		
		#region XML Header
		/// <summary>
		/// Selects input from the execute register.
		/// </summary>
		#endregion
		Execute,
		
		#region XML Header
		/// <summary>
		/// Selects input from the memory register.
		/// </summary>
		#endregion
		Memory
	}
}
