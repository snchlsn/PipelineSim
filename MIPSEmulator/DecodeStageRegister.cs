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
		/// A register separating the decode stage of a MIPS processor from the execute stage.
		/// </summary>
		#endregion
		public sealed class DecodeStageRegister: StageRegister
		{
			#region Fields
			private Instruction instruction, nextInstruction;
			private uint followingInstructionAddress, nextFollowingInstructionAddress;
			private ALUOp aluOp, nextALUOp;
			private uint[] registerReadValues = new uint[2], nextRegisterReadValues = new uint[2];
			#endregion
			
			#region Properties
			public ALUOp ALUOp
			{
				get { return aluOp; }
			}
			
			public uint FollowingInstructionAddress
			{
				get { return followingInstructionAddress; }
			}
			
			#region XML Header
			/// <summary>
			/// Gets the latched instruction being passed to the execute stage.
			/// </summary>
			/// <value>
			/// A <c>MIPSEmulator.Assembly.Instruction</c> containing the latched instruction.
			/// </value>
			/// <remarks>
			/// Used instead of separately storing rd, rs, rt, and the immediate.
			/// </remarks>
			#endregion
			public Instruction Instruction
			{
				get { return instruction; }
			}
			
			#region XML Header
			/// <summary>
			/// Gets the latched value read from register rs.
			/// </summary>
			/// <value>The latched value, as a <c>uint</c>.</value>
			#endregion
			public uint RegisterReadValue1
			{
				get { return registerReadValues[0]; }
			}
			
			#region XML Header
			/// <summary>
			/// Gets the latched value read from register rt.
			/// </summary>
			/// <value>The latched value, as a <c>uint</c>.</value>
			#endregion
			public uint RegisterReadValue2
			{
				get { return registerReadValues[1]; }
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
				followingInstructionAddress = nextFollowingInstructionAddress;
				instruction = nextInstruction;
				aluOp = nextALUOp;
				Array.Copy(nextRegisterReadValues, registerReadValues, 2);
				
				base.ChangeOutputs(sender, e);
				return;
			}
			
			#region XML Header
			/// <summary>
			/// Sets internal state based on current input.
			/// </summary>
			/// <param name="sender">The <c>object</c> that raised the event.</param>
			/// <param name="e">A <c>System.EventArgs</c>.</param>
			#endregion
			protected override void ChangeState(object sender, EventArgs e)
			{
				nextSignals = Owner.Control.Signals;
				nextFollowingInstructionAddress = Owner.FetchRegister.FollowingInstructionAddress;
				nextInstruction = (Instruction)Owner.FetchRegister.Instruction;
				nextInstructionAddress = (Owner.Control.Signals == ControlSignals.None && nextInstruction.Opcode != Opcode.J ? 1 : Owner.FetchRegister.InstructionAddress);
				nextALUOp = Owner.Control.ALUOp;
				nextRegisterReadValues[0] = Owner.Registers[nextInstruction.RS];
				nextRegisterReadValues[1] = Owner.Registers[nextInstruction.RT];
				return;
			}
			#endregion
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>DecodeStageRegister</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>DecodeStageRegister</c> is a component.
			/// </param>
			#endregion
			internal DecodeStageRegister(Processor owner): base(owner) {}
			#endregion
			#endregion
		}
	}
}
