using System;

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// A <c>SystemClock</c> implementation that allows client code to have full control over level changes.
	/// </summary>
	#endregion
	public class ManualClock: SystemClock
	{
		#region Methods
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>ManualClock</c> instance, and assigns it to
		/// <c>SystemClock.Provider</c>.
		/// </summary>
		#endregion
		public static void SetAsProvider()
		{
			SystemClock.Provider = new ManualClock();
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Inverts the level of the system clock.  That is, if it is a logical low, it will be set to logical
		/// high, and vice versa.
		/// </summary>
		#endregion
		public void InvertLevel()
		{
			if (this != Provider)
				throw new InvalidOperationException("The InvertLevel method may not be called on a ManualClock that has been replaced as SystemClock.Provider.");
			
			SetLevel(!Level);
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>ManualClock</c> instance.
		/// </summary>
		#endregion
		private ManualClock() {}
		#endregion
		#endregion
	}
}
