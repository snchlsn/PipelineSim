#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Base class for a virtual global hardware clock.
	/// </summary>
	/// <remarks>
	/// This class is implemented as a singleton.  Client code is responsible for selecting a derived
	/// class to serve as the single instance.
	/// The <c>EdgeFalling</c> and <c>EdgeRising</c> events should be used to latch values in synchronous
	/// components.  The <c>EdgeFallen</c> and <c>EdgeRisen</c> events should be used to propagate signals
	/// between components, or to observe changes to the state of the system.
	/// </remarks>
	#endregion
	public abstract class SystemClock
	{
		#region Fields
		private static bool level = false, rising = false, falling = false;
		private static SystemClock provider = null;
		
		#if DEBUG
		private static int fallingEdges = 0;
		private static int fallenEdges = 0;
		private static int risingEdges = 0;
		private static int risenEdges = 0;
		#endif
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets a value indicating whether the clock is transitioning from logical high to logical low
		/// (i.e., a <c>FallingEdge</c> event is in the process of being handled).
		/// </summary>
		/// <value>
		/// <c>true</c> if the clock is transitioning from high to low; <c>false</c> otherwise.
		/// </value>
		#endregion
		public static bool IsFalling
		{
			get { return falling; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets a value indicating whether the clock is transitioning from logical low to logical high
		/// (i.e., a <c>RisingEdge</c> event is in the process of being handled).
		/// </summary>
		/// <value>
		/// <c>true</c> if the clock is transitioning from low to high; <c>false</c> otherwise.
		/// </value>
		#endregion
		public static bool IsRising
		{
			get { return rising; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets a value indicating whether the clock is currently at a logical high or a logical low.
		/// </summary>
		/// <value><c>true</c> if logical high; <c>false</c> if logical low.</value>
		#endregion
		public static bool Level
		{
			get { return level; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the derived singleton that provides methods for changing the value of the <c>Level</c>
		/// property.
		/// </summary>
		/// <value>The derived signleton.</value>
		/// <remarks>
		/// Each derived class must provide a static method for creating an instance of itself and
		/// assigning that instance to this property.
		/// </remarks>
		#endregion
		public static SystemClock Provider
		{
			get { return provider; }
			protected set
			{
				#if DEBUG
				if (provider == null)
				{
					EdgeFallen += (sender, e) => { ++fallenEdges; };
					EdgeFalling += (sender, e) => { ++fallingEdges; };
					EdgeRisen += (sender, e) => { ++risenEdges; };
					EdgeRising += (sender, e) => { ++risingEdges; };
				}
				#endif
				
				if (provider is IDisposable)
					((IDisposable)provider).Dispose();
				
				provider = value;
			}
		}
		#endregion
		
		#region Events
		#region XML Header
		/// <summary>
		/// Occurs before <c>Level</c> is changed from <c>true</c> to <c>false</c>.
		/// </summary>
		/// <remarks>Should be used for latching.</remarks>
		#endregion
		public static event EventHandler EdgeFalling;
		
		#region XML Header
		/// <summary>
		/// Occurs after <c>Level</c> is changed from <c>true</c> to <c>false</c>.
		/// </summary>
		/// <remarks>Should be used for signal propagation or observation.</remarks>
		#endregion
		public static event EventHandler EdgeFallen;
		
		#region XML Header
		/// <summary>
		/// Occurs before <c>Level</c> is changed from <c>false</c> to <c>true</c>.
		/// </summary>
		/// <remarks>Should be used for latching.</remarks>
		#endregion
		public static event EventHandler EdgeRising;
		
		#region XML Header
		/// <summary>
		/// Occurs after <c>Level</c> is changed from <c>false</c> to <c>true</c>.
		/// </summary>
		/// <remarks>Should be used for signal propagation or observation.</remarks>
		#endregion
		public static event EventHandler EdgeRisen;
		#endregion
		
		#region Methods
		#region Event Raisers
		#region XML Header
		/// <summary>
		/// Raises the <c>EdgeFalling</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private static void OnEdgeFalling(EventArgs e)
		{
			if (EdgeFalling != null)
			{
				falling = true;
				EdgeFalling(provider, e);
				falling = false;
			}
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>EdgeFallen</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private static void OnEdgeFallen(EventArgs e)
		{
			if (EdgeFallen != null)
				EdgeFallen(provider, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>EdgeRising</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private static void OnEdgeRising(EventArgs e)
		{
			if (EdgeRising != null)
			{
				rising = true;
				EdgeRising(provider, e);
				rising = false;
			}
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>EdgeRisen</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private static void OnEdgeRisen(EventArgs e)
		{
			if (EdgeRisen != null)
				EdgeRisen(provider, e);
			return;
		}
		#endregion
		
		internal static void Reset()
		{
			level = false;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// May be called by a derived class to set the value of <c>level</c>.
		/// </summary>
		/// <param name="val">
		/// <c>true</c> to set a logical high; <c>false</c> to set a logical low.
		/// </param>
		/// <remarks>
		/// Has no effect if the calling instance is not assigned to the <c>Provider</c> property.
		/// </remarks>
		#endregion
		protected void SetLevel(bool val)
		{
			if (val != level && this == provider)
			{
				if (val)
				{
					OnEdgeRising(EventArgs.Empty);
					level = val;
					OnEdgeRisen(EventArgs.Empty);
				}
				else
				{
					OnEdgeFalling(EventArgs.Empty);
					level = val;
					OnEdgeFallen(EventArgs.Empty);
				}
			}
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>SystemClock</c> instance.
		/// </summary>
		#endregion
		protected SystemClock() {}
		#endregion
		#endregion
	}
}
