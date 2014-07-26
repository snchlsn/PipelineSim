#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Represents the method that will handle events related to hazards in a pipeline.
	/// </summary>
	#endregion
	public delegate void HazardEventHandler(object sender, HazardEventArgs e);
	
	#region XML Header
	/// <summary>
	/// Provides information about events relating to hazards in a pipeline.
	/// </summary>
	#endregion
	public class HazardEventArgs: EventArgs
	{
		#region XML Header
		/// <summary>
		/// The type(s) of hazard that caused the event.
		/// </summary>
		#endregion
		public readonly HazardTypes HazardTypes;
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>HazardEventArgs</c> instance.
		/// </summary>
		/// <param name="hazardTypes">
		/// A <c>HazardTypes</c> value specifying the type(s) of hazard that caused the event.
		/// </param>
		#endregion
		public HazardEventArgs(HazardTypes hazardTypes)
		{
			HazardTypes = hazardTypes;
		}
	}
}
