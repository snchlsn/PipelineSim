#region Using Directives
using System;
using MIPSEmulator.Assembly;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// Represents a MIPS processor's program counter, as well as the combinational logic feeding into it.
		/// </summary>
		#endregion
		public sealed class ProgramCounter: ClockedComponent
		{
			#region Fields
			private uint address, nextAddress;
			#endregion
			
			#region Properties
			#region XML Header
			/// <summary>
			/// Gets the instruction address currently latched in the program counter.
			/// </summary>
			/// <value>The latched address.</value>
			#endregion
			public uint Address
			{
				get { return address; }
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			#region XML Header
			/// <summary>
			/// Updates outputs based on latched values.
			/// </summary>
			/// <param name="sender">The <c>object</c> that raised the event.</param>
			/// <param name="e">A <c>System.EventArgs</c>.</param>
			#endregion
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				if (address != nextAddress)
				{
					address = nextAddress;
					OnStateChanged(EventArgs.Empty);
				}
				return;
			}
			
			#region XML Header
			/// <summary>
			/// Latches the next instruction address.
			/// </summary>
			/// <param name="sender">The <c>object</c> that raised the event.</param>
			/// <param name="e">A <c>System.EventArgs</c>.</param>
			#endregion
			protected override void ChangeState(object sender, EventArgs e)
			{
				if (!Owner.Control.StallPC)
					switch (Owner.Control.PCSource)
					{
						case PCSource.Branch:
						nextAddress = unchecked(Owner.DecodeRegister.FollowingInstructionAddress + (BitConverter.ToUInt32(BitConverter.GetBytes((int)Owner.DecodeRegister.Instruction.Immediate) , 0) << 2));
							break;
						
						case PCSource.Jump:
							nextAddress = ((Instruction)Owner.FetchRegister.Instruction).Target;
							break;
						
						default:
							nextAddress = address + 4;
							break;
					}
				
				return;
			}
			#endregion
			
			#region XML Header
			/// <summary>
			/// Resets the instruction address to <c>0</c>.
			/// </summary>
			#endregion
			internal override void Reset()
			{
				address = 0;
				base.Reset();
				return;
			}
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>ProgramCounter</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>ProgramCounter</c> is a component.
			/// </param>
			#endregion
			internal ProgramCounter(Processor owner): base(owner, false) {}
			#endregion
			#endregion
		}
	}
}
