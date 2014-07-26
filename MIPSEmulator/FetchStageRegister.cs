#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// A register separating the fetch stage of a MIPS processor from the decode stage.
		/// </summary>
		#endregion
		public sealed class FetchStageRegister: StageRegister
		{
			#region Fields
			private uint instruction, nextInstruction;
			private uint followingInstructionAddress, nextFollowingInstructionAddress;
			#endregion
			
			#region Properties
			public uint FollowingInstructionAddress
			{
				get { return followingInstructionAddress; }
			}
			
			#region XML Header
			/// <summary>
			/// Gets the latched instruction being passed to the processor's decode stage.
			/// </summary>
			/// <value>The latched instruction.</value>
			#endregion
			public uint Instruction
			{
				get { return instruction; }
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				instruction = nextInstruction;
				followingInstructionAddress = nextFollowingInstructionAddress;
				
				base.ChangeOutputs(sender, e);
				return;
			}
			
			protected override void ChangeState(object sender, EventArgs e)
			{
				if (Owner.Control.FlushFetch || Owner.ProgramSynchronizer.Instruction == 0)
				{
					nextInstruction = 0;
					nextInstructionAddress = 1;
				}
				else if (!Owner.Control.StallFetch)
				{
					nextInstructionAddress = Owner.PC.Address;
					nextInstruction = Owner.ProgramSynchronizer.Instruction;
					nextFollowingInstructionAddress = Owner.PC.Address + 4;
				}
				return;
			}
			#endregion
			
			internal override void Reset()
			{
				instruction = 0;
				base.Reset();
				return;
			}
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>FetchStageRegister</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>Processor</c> whereof the new <c>FetchStageRegister</c> is a component.
			/// </param>
			#endregion
			internal FetchStageRegister(Processor owner): base(owner) {}
			#endregion
			#endregion
		}
	}
}
