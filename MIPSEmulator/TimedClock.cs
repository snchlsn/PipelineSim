#region Using Directives
using System;
using System.Timers;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// A <c>SystemClock</c> implementation that oscillates with a configurable period.
	/// </summary>
	#endregion
	public class TimedClock: SystemClock, IDisposable
	{
		#region Fields
		private Timer clockTimer;
		private static int interval = 1000;
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets or sets a value indicating whether the <c>TimedClock</c> is running, and should raise
		/// <c>FallingEdge</c> and <c>RisingEdge</c> events.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the <c>TimedClock</c> is running; <c>false</c> otherwise.
		/// The default is <c>false</c>.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">
		/// A value was assigned after the <c>TimedClock</c> was replaced as <c>SystemClock.Provider</c>.
		/// </exception>
		#endregion
		public bool Enabled
		{
			get
			{
				return Provider == this && clockTimer.Enabled;
			}
			set
			{
				if (Provider == this)
					clockTimer.Enabled = value;
				else
					throw new InvalidOperationException("TimedClock.Enable may only be set while TimedClock is SystemClock.Provider.");
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the interval between <c>Level</c> changes, in milliseconds.  This is equal to half
		/// the period.  The default is 1000.
		/// </summary>
		/// <returns>The interval between <c>Level</c> changes, in milliseconds.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The assigned value is less than <c>1</c>.
		/// </exception>
		#endregion
		public static int Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", value, "The assigned value must be positive.");
				
				interval = value;
				if (Provider is TimedClock)
					((TimedClock)Provider).clockTimer.Interval = value;
			}
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Inverts the value of <c>SystemClock.Level</c>.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InvertLevel(object sender, EventArgs e)
		{
			SetLevel(!Level);
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>TimedClock</c> instance, and assigns it to
		/// <c>SystemClock.Provider</c>.
		/// </summary>
		#endregion
		public static void SetAsProvider()
		{
			SystemClock.Provider = new TimedClock();
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Releases all resources used by the <c>TimedClock</c>.
		/// </summary>
		#endregion
		public void Dispose()
		{
			Dispose(true);
			return;
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && clockTimer != null)
			{
				clockTimer.Dispose();
				clockTimer = null;
			}
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>TimedClock</c> instance.
		/// </summary>
		#endregion
		private TimedClock()
		{
			clockTimer = new Timer(interval);
			clockTimer.Elapsed += InvertLevel;
		}
		#endregion
		#endregion
	}
}
