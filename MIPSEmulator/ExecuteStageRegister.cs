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
		/// A register separating the exexute stage of a MIPS processor from the memory stage.
		/// </summary>
		#endregion
		public sealed class ExecuteStageRegister: StageRegister
		{
			#region Fields
			private uint aluResult, nextALUResult;
			private uint branchTarget, nextBranchTarget;
			private Register destinationRegister, nextDestinationRegister;
			private uint memoryWriteValue, nextMemoryWriteValue;
			#endregion
			
			#region Properties
			public uint ALUResult
			{
				get { return aluResult; }
			}
			
			public uint BranchTarget
			{
				get { return branchTarget; }
			}
			
			public Register DestinationRegister
			{
				get { return destinationRegister; }
			}
			
			public uint MemoryWriteValue
			{
				get { return memoryWriteValue; }
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				aluResult = nextALUResult;
				branchTarget = nextBranchTarget;
				destinationRegister = nextDestinationRegister;
				memoryWriteValue = nextMemoryWriteValue;
				
				base.ChangeOutputs(sender, e);
				return;
			}
			
			protected override void ChangeState(object sender, EventArgs e)
			{
				DecodeStageRegister decodeRegister = Owner.DecodeRegister;
				
				nextSignals = decodeRegister.Signals;
				nextInstructionAddress = decodeRegister.InstructionAddress;
				nextALUResult = Owner.ALU.Result;
				unchecked
				{
					nextBranchTarget = (uint)(decodeRegister.FollowingInstructionAddress + ((int)decodeRegister.Instruction.Immediate << 2));
				}
				nextDestinationRegister = (decodeRegister.Signals.HasFlag(ControlSignals.RegDst) ? decodeRegister.Instruction.RD : decodeRegister.Instruction.RT);
				
				if (Owner.Mode != HazardMode.StallAndForward)
					nextMemoryWriteValue = decodeRegister.RegisterReadValue2;
				else
					switch (Owner.Control.ForwardB)
					{
						case ForwardSource.Decode:
							nextMemoryWriteValue = decodeRegister.RegisterReadValue2;
							break;
						
						case ForwardSource.Execute:
							nextMemoryWriteValue = Owner.ExecuteRegister.ALUResult;
							break;
						
						case ForwardSource.Memory:
							MemoryStageRegister memoryRegister = Owner.MemoryRegister;
							
							nextMemoryWriteValue = (memoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? memoryRegister.ReadValue : memoryRegister.ALUResult);
							break;
						
						default:
							throw new InvalidOperationException();
					}
				
				return;
			}
			#endregion
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>ExecuteStageRegister</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>ExecuteStageRegister</c> is a component.
			/// </param>
			#endregion
			internal ExecuteStageRegister(Processor owner): base(owner) {}
			#endregion
			#endregion
		}
	}
}
