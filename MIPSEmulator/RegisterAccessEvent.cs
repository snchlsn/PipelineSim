#region Using Directives
using System;
using MIPSEmulator.Assembly;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Represents a method that will handle events related to register accesses.
	/// </summary>
	#endregion
	public delegate void RegisterAccessEventHandler(object sender, RegisterAccessEventArgs e);
	
	#region XML Header
	/// <summary>
	/// Provides information for events relating to register accesses.
	/// </summary>
	#endregion
	public class RegisterAccessEventArgs: EventArgs
	{
		#region XML Header
		/// <summary>
		/// The register that was accessed.
		/// </summary>
		#endregion
		public readonly Register Register;
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>RegisterAccessEventArgs</c> instance.
		/// </summary>
		/// <param name="register">
		/// A <c>MIPSEmulator.Assembly.Register</c> specifying the register that was accessed.
		/// </param>
		#endregion
		public RegisterAccessEventArgs(Register register)
		{
			Register = register;
		}
	}
}
