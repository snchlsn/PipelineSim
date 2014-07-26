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
		/// Represents the control unit of a MIPS processor.  Incorporates hazard detection and forwarding
		/// logic.
		/// </summary>
		#endregion
		public sealed class ControlUnit: ProcessorComponent
		{
			#region Properties
			public ALUOp ALUOp
			{
				get
				{
					if (Owner.FetchRegister.HasBubble)
						return ALUOp.Or;
					
					Instruction instruction = ((Instruction)Owner.FetchRegister.Instruction);
					
					switch (instruction.Opcode)
					{
						case Opcode.Addiu:
							return ALUOp.Add;
						
						case Opcode.Andi:
							return ALUOp.And;
						
						case Opcode.Beq:
							return ALUOp.Subtract;
						
						case Opcode.J:
							return ALUOp.Or;
						
						case Opcode.Lw:
							return ALUOp.Add;
						
						case Opcode.Ori:
							return ALUOp.Or;
						
						case Opcode.RType:
							switch (instruction.Function)
							{
								case Function.Addu:
									return ALUOp.Add;
								
								case Function.And:
									return ALUOp.And;
								
								case Function.Nor:
									return ALUOp.Nor;
								
								case Function.Or:
									return ALUOp.Or;
								
								case Function.Sllv:
									return ALUOp.ShiftLeftLogical;
								
								case Function.Srav:
									return ALUOp.ShiftRightArithmetic;
								
								case Function.Srlv:
									return ALUOp.ShiftRightLogical;
								
								case Function.Subu:
									return ALUOp.Subtract;
								
								case Function.Xor:
									return ALUOp.Xor;
								
								default:
									throw new InvalidOperationException();
							}
						
						case Opcode.Sw:
							return ALUOp.Add;
						
						case Opcode.Xori:
							return ALUOp.Xor;
						
						default:
							throw new InvalidOperationException();
					}
				}
			}
			
			#region XML Header
			/// <summary>
			/// Gets a value indicating whether the fetch stage register should be flushed on the next edge.
			/// </summary>
			/// <value>
			/// <c>true</c> if the fetch stage register should be flushed; <c>false</c> otherwise.
			/// </value>
			/// <remarks>
			/// Will return <c>true</c> if and only if a branch is to be taken.
			/// </remarks>
			#endregion
			public bool FlushFetch
			{
				get
				{
					return PCSource == PCSource.Branch;
				}
			}
			
			public ForwardSource ForwardA
			{
				get
				{
					DecodeStageRegister decodeRegister = Owner.decodeRegister;
					ExecuteStageRegister executeRegister = Owner.ExecuteRegister;
					MemoryStageRegister memoryRegister = Owner.MemoryRegister;
					
					if (executeRegister.Signals.HasFlag(ControlSignals.RegWrite) && executeRegister.DestinationRegister == decodeRegister.Instruction.RS)
						return ForwardSource.Execute;
					
					if (memoryRegister.Signals.HasFlag(ControlSignals.RegWrite) && memoryRegister.DestinationRegister == decodeRegister.Instruction.RS)
						return ForwardSource.Memory;
					
					return ForwardSource.Decode;
				}
			}
			
			public ForwardSource ForwardB
			{
				get
				{
					DecodeStageRegister decodeRegister = Owner.decodeRegister;
					ExecuteStageRegister executeRegister = Owner.ExecuteRegister;
					MemoryStageRegister memoryRegister = Owner.MemoryRegister;
					
					if (executeRegister.Signals.HasFlag(ControlSignals.RegWrite) && executeRegister.DestinationRegister == decodeRegister.Instruction.RT)
						return ForwardSource.Execute;
					
					if (memoryRegister.Signals.HasFlag(ControlSignals.RegWrite) && memoryRegister.DestinationRegister == decodeRegister.Instruction.RT)
						return ForwardSource.Memory;
					
					return ForwardSource.Decode;
				}
			}
			
			#region XML Header
			/// <summary>
			/// Gets a value indicating which signal should next be loaded into the PC.
			/// </summary>
			/// <value>One of the <c>MIPSEmulator.PCSource</c> values.</value>
			/// <remarks>
			/// If a jump instruction is in a branch's delay slot, the jump and the branch will
			/// resolve in the same clock cycle.  This conflict will be resolved in favor of the
			/// branch.
			/// </remarks>
			#endregion
			public PCSource PCSource
			{
				get
				{
					if (Owner.DecodeRegister.Signals.HasFlag(ControlSignals.Branch) && Owner.ALU.Result == 0)
						return PCSource.Branch;
					else if (((Instruction)Owner.FetchRegister.Instruction).Opcode == Opcode.J)
						return PCSource.Jump;
					else
						return PCSource.Next;
				}
			}
			
			public ControlSignals Signals
			{
				get
				{
					ControlSignals signals = GetPreliminarySignals();
					
					if (signals == ControlSignals.None || (CheckComputeUseHazard(signals) | CheckLoadUseHazard(signals)) != HazardTypes.None)
						return ControlSignals.None;
					
					return signals;
				}
			}
			
			#region XML Header
			/// <summary>
			/// Gets a value indicating whether the fetch stage register should be stalled on the next cycle.
			/// </summary>
			/// <value>
			/// <c>true</c> if the fetch stage register should be stalled; <c>false</c> otherwise.
			/// </value>
			/// <remarks>
			/// Will return <c>true</c> if and only if a hazard is detected.
			/// </remarks>
			#endregion
			public bool StallFetch
			{
				get
				{
					return CheckHazard() != HazardTypes.None;
				}
			}
			
			#region XML Header
			/// <summary>
			/// Gets a value indicating whether the PC should be stalled on the next edge.
			/// </summary>
			/// <value><c>true</c> if the PC should be stalled; <c>false</c> otherwise.</value>
			/// <remarks>
			/// Will return <c>true</c> if and only if there is a hazard other than a branch.
			/// </remarks>
			#endregion
			public bool StallPC
			{
				get
				{
					ControlSignals signals = GetPreliminarySignals();
					
					return (CheckComputeUseHazard(signals) | CheckLoadUseHazard(signals)) != HazardTypes.None;
				}
			}
			#endregion
			
			#region Methods
			#region XML Header
			/// <summary>
			/// Checks for hazards in the pipeline.
			/// </summary>
			/// <returns>
			/// A <c>ControlSignals</c> value indicting which hazards were detected.
			/// </returns>
			#endregion
			internal HazardTypes CheckHazard()
			{
				ControlSignals signals = GetPreliminarySignals();
				
				return CheckComputeUseHazard(signals) | CheckLoadUseHazard(signals) | (PCSource == PCSource.Branch ? HazardTypes.Branch : HazardTypes.None);
			}
			
			#region XML Header
			/// <summary>
			/// Checks the pipeline for compute/use hazards.
			/// </summary>
			/// <param name="signals">The result of calling <c>GetPreliminarySignals</c>.</param>
			/// <returns>A <c>HazardTypes</c> value indicating which hazards were detected.</returns>
			/// <remarks>
			/// If forwarding is enabled, then compute/use hazards are not an issue, and this method will always
			/// return <c>HazardTypes.None</c>.
			/// </remarks>
			#endregion
			private HazardTypes CheckComputeUseHazard(ControlSignals signals)
			{
				HazardTypes hazard = HazardTypes.None;
				
				//If forwarding is not enabled
				if (Owner.Mode != HazardMode.StallAndForward)
				{
					Instruction fetchedInstruction = (Instruction)Owner.FetchRegister.Instruction;
					ExecuteStageRegister executeRegister = Owner.ExecuteRegister;
					DecodeStageRegister decodeRegister = Owner.DecodeRegister;
					
					//If instruction in memory stage is arithmetic
					if (executeRegister.Signals.HasFlag(ControlSignals.RegWrite) && !executeRegister.Signals.HasFlag(ControlSignals.MemRead))
					{
						//If RT in decode is a source register and destination matches
						if ((signals.HasFlag(ControlSignals.RegWrite | ControlSignals.RegDst) || signals.HasFlag(ControlSignals.Branch | ControlSignals.MemWrite)) && executeRegister.DestinationRegister == fetchedInstruction.RT)
					    	hazard = HazardTypes.ComputeUseMemory;
						
						//If RS in decode is a source register and destination matches
						else if (signals.HasFlag(ControlSignals.RegWrite) && !signals.HasFlag(ControlSignals.RegDst) && executeRegister.DestinationRegister == fetchedInstruction.RS)
							hazard = HazardTypes.ComputeUseMemory;
					}
					
					//If instruction in execute stage is arithmetic
					if (decodeRegister.Signals.HasFlag(ControlSignals.RegWrite) && !decodeRegister.Signals.HasFlag(ControlSignals.MemRead))
					{
						Register destinationRegister = (decodeRegister.Signals.HasFlag(ControlSignals.RegDst) ? decodeRegister.Instruction.RD : decodeRegister.Instruction.RT);
						
						//If RT in decode is a source register and destination matches
						if ((signals.HasFlag(ControlSignals.RegWrite | ControlSignals.RegDst) || signals.HasFlag(ControlSignals.Branch | ControlSignals.MemWrite)) && destinationRegister == fetchedInstruction.RT)
							hazard |= HazardTypes.ComputeUseExecute;
						
						//If RS in decode is a source register and destination matches
						else if (signals.HasFlag(ControlSignals.RegWrite) && !signals.HasFlag(ControlSignals.RegDst) && !decodeRegister.Signals.HasFlag(ControlSignals.RegDst) && destinationRegister == fetchedInstruction.RS)
							hazard |= HazardTypes.ComputeUseExecute;
					}
				}
				
				return hazard;
			}
			
			#region XML Header
			/// <summary>
			/// Checks the pipeline for load/use hazards.
			/// </summary>
			/// <param name="signals">The result of calling <c>GetPreliminarySignals</c>.</param>
			/// <returns>A <c>HazardTypes</c> value indicating which hazards were detected.</returns>
			/// <remarks>
			/// If forwarding is enabled, then load/use hazards between the decode and memory stages
			/// are not an issue, and this method will not check for them.
			/// </remarks>
			#endregion
			private HazardTypes CheckLoadUseHazard(ControlSignals signals)
			{
				HazardTypes hazard = HazardTypes.None;
				ExecuteStageRegister executeRegister = Owner.ExecuteRegister;
				DecodeStageRegister decodeRegister = Owner.DecodeRegister;
				Instruction fetchedInstruction = (Instruction)Owner.FetchRegister.Instruction;
				
				//If forwarding is not enabled and the instruction in the memory stage is a load
				if (Owner.Mode != HazardMode.StallAndForward && executeRegister.Signals.HasFlag(ControlSignals.MemRead))
				{
					//If RT in decode is a source register and destination matches
					if ((signals.HasFlag(ControlSignals.RegWrite | ControlSignals.RegDst) || signals.HasFlag(ControlSignals.Branch | ControlSignals.MemWrite)) && executeRegister.DestinationRegister == fetchedInstruction.RT)
				    	hazard = HazardTypes.LoadUseMemory;
					
					//If RS in decode is a source register and destination matches
					else if (signals.HasFlag(ControlSignals.RegWrite) && !signals.HasFlag(ControlSignals.RegDst) && executeRegister.DestinationRegister == fetchedInstruction.RS)
						hazard = HazardTypes.LoadUseMemory;
				}
				
				//If the instruction in the execute stage is a load
				if (decodeRegister.Signals.HasFlag(ControlSignals.MemRead))
				{
					Register destinationRegister = (decodeRegister.Signals.HasFlag(ControlSignals.RegDst) ? decodeRegister.Instruction.RD : decodeRegister.Instruction.RT);
						
					//If RT in decode is a source register and destination matches
					if ((signals.HasFlag(ControlSignals.RegWrite | ControlSignals.RegDst) || signals.HasFlag(ControlSignals.Branch | ControlSignals.MemWrite)) && destinationRegister == fetchedInstruction.RT)
						hazard |= HazardTypes.LoadUseExecute;
					
					//If RS in decode is a source register and destination matches
					else if (signals.HasFlag(ControlSignals.RegWrite) && !signals.HasFlag(ControlSignals.RegDst) && !decodeRegister.Signals.HasFlag(ControlSignals.RegDst) && destinationRegister == fetchedInstruction.RS)
						hazard |= HazardTypes.LoadUseExecute;
				}
				
				return hazard;
			}
			
			#region XML Header
			/// <summary>
			/// Internally used to generate a <c>ControlSignals</c> value that does not account for hazards.
			/// </summary>
			/// <returns>
			/// The preliminary <c>ControlSignals</c> value.
			/// </returns>
			#endregion
			private ControlSignals GetPreliminarySignals()
			{
				Instruction instruction = (Instruction)Owner.FetchRegister.Instruction;
					
				switch (instruction.Opcode)
				{
					case Opcode.Addiu:
						return (instruction.RT == Register.Zero ? ControlSignals.ALUSrc : ControlSignals.RegWrite | ControlSignals.ALUSrc);
					
					case Opcode.Andi:
						return (instruction.RT == Register.Zero ? ControlSignals.ALUSrc : ControlSignals.RegWrite | ControlSignals.ALUSrc);
					
					case Opcode.Beq:
						return ControlSignals.Branch | ControlSignals.RegDst;
					
					case Opcode.J:
						return ControlSignals.None;
					
					case Opcode.Lw:
						return (instruction.RT == Register.Zero ? ControlSignals.MemRead : ControlSignals.MemRead | ControlSignals.RegWrite);
					
					case Opcode.Ori:
						return (instruction.RT == Register.Zero ? ControlSignals.ALUSrc : ControlSignals.RegWrite | ControlSignals.ALUSrc);
					
					case Opcode.RType:
						return (instruction.RD == Register.Zero ? ControlSignals.RegDst : ControlSignals.RegWrite | ControlSignals.RegDst);
					
					case Opcode.Sw:
						return ControlSignals.MemWrite | ControlSignals.ALUSrc | ControlSignals.RegDst;
					
					case Opcode.Xori:
						return (instruction.RT == Register.Zero ? ControlSignals.ALUSrc : ControlSignals.RegWrite | ControlSignals.ALUSrc);
					
					default:
						throw new InvalidOperationException();
				}
			}
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>ControlUnit</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>ControlUnit</c> is a component.
			/// </param>
			#endregion
			internal ControlUnit(Processor owner): base(owner) {}
			#endregion
			#endregion
		}
	}
}
