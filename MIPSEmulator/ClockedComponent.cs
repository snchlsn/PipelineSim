
using System;

namespace MIPSEmulator
{
	public partial class Processor
	{
		public abstract class ClockedComponent: ProcessorComponent
		{
			#region Fields
			private readonly bool positiveEdge;
			#endregion
			
			#region Events
			#region XML Header
			/// <summary>
			/// Occurs when any of the component's output properties change.
			/// </summary>
			#endregion
			public event EventHandler StateChanged;
			#endregion
			
			#region Methods
			#region Event Raisers
			#region XML Header
			/// <summary>
			/// Raises the <c>StateChanged</c> event.
			/// </summary>
			/// <param name="e">A <c>System.EventArgs</c>.</param>
			/// <remarks>
			/// Should be called by derived classes in the <c>ChangeOutputs</c> method.
			/// </remarks>
			#endregion
			protected void OnStateChanged(EventArgs e)
			{
				if (StateChanged != null)
					StateChanged(this, e);
				return;
			}
			#endregion
			
			#region Event Handlers
			#region XML Header
			/// <summary>
			/// When overridden in a derived class, changes output properties to match current state.
			/// </summary>
			/// <param name="sender">The <c>object</c> that raised the event.</param>
			/// <param name="e">A <c>System.EventArgs</c></param>
			/// <remarks>
			/// Implementations of this method should call <c>OnStateChanged</c> if current state does not
			/// match previous state.
			/// </remarks>
			#endregion
			protected abstract void ChangeOutputs(object sender, EventArgs e);
			
			#region XML Header
			/// <summary>
			/// When overridden in a derived class, sets internal state based on current input.
			/// </summary>
			/// <param name="sender">The <c>object</c> that raised the event.</param>
			/// <param name="e">A <c>System.EventArgs</c>.</param>
			/// <remarks>
			/// Note that the <c>StateChanged</c> event should only be raised in the <c>ChangeOutputs</c>
			/// method.  It should not be raised in this method, as this method should not make
			/// any externally observable changes.
			/// </remarks>
			#endregion
			protected abstract void ChangeState(object sender, EventArgs e);
			
			private void Lock(object sender, EventArgs e)
			{
				if (positiveEdge)
				{
					SystemClock.EdgeRising -= ChangeState;
					SystemClock.EdgeRisen -= ChangeOutputs;
				}
				else
				{
					SystemClock.EdgeFalling -= ChangeState;
					SystemClock.EdgeFallen -= ChangeOutputs;
				}
				return;
			}
			
			private void Unlock(object sender, EventArgs e)
			{
				if (positiveEdge)
				{
					SystemClock.EdgeRising += ChangeState;
					SystemClock.EdgeRisen += ChangeOutputs;
				}
				else
				{
					SystemClock.EdgeFalling += ChangeState;
					SystemClock.EdgeFallen += ChangeOutputs;
				}
				return;
			}
			#endregion
			
			internal virtual void Reset()
			{
				OnStateChanged(EventArgs.Empty);
				return;
			}
			
			#region Constructors
			internal ClockedComponent(Processor owner, bool positiveEdge): base(owner)
			{
				this.positiveEdge = positiveEdge;
				
				owner.ExecutionStopped += Lock;
				owner.ExecutionStarted += Unlock;
			}
			#endregion
			#endregion
		}
	}
}
