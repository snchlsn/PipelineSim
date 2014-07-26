#region Using Directives
using System;
#endregion

namespace PipelineSim
{
	#region XML Header
	/// <summary>
	/// Contains global constants for the PipelineSim program.
	/// </summary>
	#endregion
	internal sealed class Globals
	{
		#region Numeric Format Strings
		#region XML Header
		/// <summary>
		/// The numeric format <c>string</c> used to display <c>uint</c> values.
		/// </summary>
		#endregion
		public const string UintFormat = "X8";
		
		#region XML Header
		/// <summary>
		/// The numeric format <c>string</c> used to display <c>ushort</c> values.
		/// </summary>
		#endregion
		public const string UshortFormat = "X4";
		
		#region XML Header
		/// <summary>
		/// The numeric format <c>string</c> used to display <c>byte</c> values.
		/// </summary>
		#endregion
		public const string ByteFormat = "X2";
		
		#region XML Header
		/// <summary>
		/// The numeric format <c>string</c> used to display nibble values.
		/// </summary>
		#endregion
		public const string NibbleFormat = "X";
		
		#region XML Header
		/// <summary>
		/// The numeric format <c>string</c> used to display jump target addresses.
		/// </summary>
		#endregion
		public const string TargetFormat = "X7";
		
		#region XML Header
		/// <summary>
		/// The prefix uses to indicate to the user that a number is given in hexadecimal.
		/// </summary>
		#endregion
		public const string HexPrefix = "0x";
		#endregion
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>Globals</c> instance.  Should never be used, as all members
		/// are static.
		/// </summary>
		#endregion
		private Globals() {}
	}
}
