#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Specifies a mode of operation with respect to how the processor handles hazards.
	/// </summary>
	#endregion
	public enum HazardMode: byte
	{
		#region XML Header
		/// <summary>
		/// In this mode, the processor cannot handle hazards, and will throw an exception if one is detected.
		/// </summary>
		#endregion
		Fail = 2,
		
		#region XML Header
		/// <summary>
		/// In this mode, the processor will stall the pipeline when a hazard is detected until the hazard
		/// is no longer a problem.
		/// </summary>
		#endregion
		Stall = 1,
		
		#region XML Header
		/// <summary>
		/// In this mode, the processor will solve hazards by forwarding data from one stage to another
		/// when it is possible to do so, and stall the pipeline when forwarding is not possible.
		/// </summary>
		#endregion
		StallAndForward = 0
	}
}
