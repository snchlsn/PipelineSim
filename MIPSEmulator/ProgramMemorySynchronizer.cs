#region XML Header
using System;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		public sealed class ProgramMemorySynchronizer: ClockedComponent
		{
			#region Fields
			private ProgramMemory program;
			private uint instruction, nextInstruction;
			#endregion
			
			#region Properties
			#region XML Header
			/// <summary>
			/// Gets the latched instruction.
			/// </summary>
			/// <value>A <c>uint</c> containing the latched instruction.</value>
			#endregion
			public uint Instruction
			{
				get { return instruction; }
			}
			
			public ProgramMemory Program
			{
				get { return program; }
				internal set { program = value; }
			}
			#endregion
			
			#region Events
			#region XML Header
			/// <summary>
			/// Occurs when an attempt is made to read an instruction from an address that does not exist.
			/// </summary>
			/// <remarks>
			/// When an out-of-bounds access is made, this event is raised, and the latched instruction is
			/// subsequently set to a bubble.
			/// </remarks>
			#endregion
			public event MemoryAccessEventHandler OutOfBoundsAccess;
			#endregion
			
			#region Methods
			#region Event Raisers
			#region XML Header
			/// <summary>
			/// Raises the <c>OutOfBoundsAccess</c> event.
			/// </summary>
			/// <param name="e">
			/// A <c>MIPSEmulator.MemoryAccessEventArgs</c> specifying the address that was accessed.
			/// </param>
			#endregion
			private void OnOutOfBoundsAccess(MemoryAccessEventArgs e)
			{
				if (OutOfBoundsAccess != null)
					OutOfBoundsAccess(this, e);
				return;
			}
			#endregion
			
			#region Event Handlers
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				if (instruction != nextInstruction)
				{
					instruction = nextInstruction;
					OnStateChanged(EventArgs.Empty);
				}
				return;
			}
			
			protected override void ChangeState(object sender, EventArgs e)
			{
				if (program != null)
				{
					uint address = Owner.PC.Address;
					
					try
					{
						nextInstruction = (uint)program.GetInstruction(address);
					}
					catch (ArgumentOutOfRangeException)
					{
						nextInstruction = 0;
						OnOutOfBoundsAccess(new MemoryAccessEventArgs(address));
					}
				}
				
				return;
			}
			#endregion
			
			#region XML Header
			/// <summary>
			/// Resets the latched instruction to a bubble.
			/// </summary>
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
			/// Creates and initializes a new <c>ProgramMemorySynchronizer</c> instance belonging to the
			/// specified <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>ProgramMemorySynchronizer</c> is
			/// a component.
			/// </param>
			#endregion
			internal ProgramMemorySynchronizer(Processor owner): base(owner, true) {}
			#endregion
			#endregion
		}
	}
}
