#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// Base class for a register that separates two stages of the pipeline.
		/// </summary>
		#endregion
		public abstract class StageRegister: ClockedComponent
		{
			#region Fields
			private ControlSignals signals;
			private uint instructionAddress = 1;
			
			#region XML Header
			/// <summary>
			/// Used to latch the next value of the <c>Signals</c> property.
			/// </summary>
			/// <remarks>
			/// Should be set by derived classes in the <c>ChangeState</c> method.
			/// </remarks>
			#endregion
			protected ControlSignals nextSignals;
			
			#region XML Header
			/// <summary>
			/// Used to latch the next value of the <c>InstructionAddress</c> property.
			/// </summary>
			/// <remarks>
			/// Should be set by derived classes in the <c>ChangeState</c> method.
			/// </remarks>
			#endregion
			protected uint nextInstructionAddress = 1;
			#endregion
			
			#region Properties
			#region XML Header
			/// <summary>
			/// Gets the address from which the instruction latched in the <c>StageRegister</c> was
			/// loaded.
			/// </summary>
			/// <value>
			/// The address of the latched instruction -or- <c>1</c> if the <c>StageRegister</c>
			/// is uninitialized or has latched a bubble.
			/// </value>
			/// <remarks>
			/// This property is not used internally by the emulator; it is maintained for diagnostic
			/// purposes.
			/// </remarks>
			#endregion
			public uint InstructionAddress
			{
				get { return instructionAddress; }
			}
			
			#region XML Header
			/// <summary>
			/// Gets a value indicating whether the instruction latched in the <c>StageRegister</c>
			/// is a bubble.
			/// </summary>
			/// <value><c>true</c> if the instruction is a bubble; <c>false</c> otherwise.</value>
			#endregion
			public bool HasBubble
			{
				get { return (instructionAddress & 3) != 0; }
			}
			
			public ControlSignals Signals
			{
				get { return signals; }
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			#region XML Header
			/// <summary>
			/// Changes output properties to match current state.
			/// </summary>
			/// <param name="sender">The <c>object</c> that raised the event.</param>
			/// <param name="e">A <c>System.EventArgs</c>.</param>
			#endregion
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				signals = nextSignals;
				instructionAddress = nextInstructionAddress;
				OnStateChanged(EventArgs.Empty);
				return;
			}
			#endregion
			
			#region XML Header
			/// <summary>
			/// Sets all output properties to their default values.
			/// </summary>
			#endregion
			internal override void Reset()
			{
				signals = ControlSignals.None;
				instructionAddress = 1;
				base.Reset();
				return;
			}
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>StageRegister</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>Processor</c> whereof the new <c>StageRegister</c> is a component.
			/// </param>
			#endregion
			internal StageRegister(Processor owner): base(owner, false) {}
			#endregion
			#endregion
		}
	}
}
