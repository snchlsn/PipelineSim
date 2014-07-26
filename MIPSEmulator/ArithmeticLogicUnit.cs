#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// Represents the arithmetic logic unit in a MIPS processor.
		/// </summary>
		#endregion
		public sealed class ArithmeticLogicUnit: ProcessorComponent
		{
			#region Properties
			#region XML Header
			/// <summary>
			/// Gets the result of applying the set operation to the two operands.
			/// </summary>
			/// <exception cref="System.InvalidOperationException">
			/// <c>Owner.ExecuteRegister.ALUOp</c> is not set to a recognized value of
			/// <c>MIPSEmulator.ALUOp</c>.  It should not be possible for client code to cause
			/// this exception to be thrown.
			/// </exception>
			#endregion
			public uint Result
			{
				get
				{
					uint result;
					uint leftOperand, rightOperand;
					DecodeStageRegister decodeRegister = Owner.DecodeRegister;
					ExecuteStageRegister executeRegister = Owner.ExecuteRegister;
					MemoryStageRegister memoryRegister = Owner.MemoryRegister;
					ALUOp operation = decodeRegister.ALUOp;
					ControlUnit control = Owner.Control;
					HazardMode mode = Owner.Mode;
					
					if (mode == HazardMode.StallAndForward)
						switch (control.ForwardA)
						{
							case ForwardSource.Decode:
								leftOperand = decodeRegister.RegisterReadValue1;
								break;
							
							case ForwardSource.Execute:
								leftOperand = executeRegister.ALUResult;
								break;
							
							case ForwardSource.Memory:
								leftOperand = (memoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? memoryRegister.ReadValue : memoryRegister.ALUResult);
								break;
							
							default:
								throw new InvalidOperationException();
						}
					else
						leftOperand = decodeRegister.RegisterReadValue1;
					
					if (decodeRegister.Signals.HasFlag(ControlSignals.ALUSrc))
						rightOperand = BitConverter.ToUInt32(BitConverter.GetBytes((int)decodeRegister.Instruction.Immediate), 0);
					else if (mode == HazardMode.StallAndForward)
						switch (control.ForwardB)
						{
							case ForwardSource.Decode:
								rightOperand = decodeRegister.RegisterReadValue2;
								break;
							
							case ForwardSource.Execute:
								rightOperand = executeRegister.ALUResult;
								break;
							
							case ForwardSource.Memory:
								rightOperand = (memoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? memoryRegister.ReadValue : memoryRegister.ALUResult);
								break;
							
							default:
								throw new InvalidOperationException();
						}
					else
						rightOperand = decodeRegister.RegisterReadValue2;
					
					switch (operation)
					{
						case ALUOp.Add:
							result = unchecked(leftOperand + rightOperand);
							break;
							
						case ALUOp.And:
							result = leftOperand & rightOperand;
							break;
							
						case ALUOp.Nor:
							result = ~(leftOperand | rightOperand);
							break;
							
						case ALUOp.Or:
							result = leftOperand | rightOperand;
							break;
							
						case ALUOp.ShiftLeftLogical:
							result = leftOperand << (int)rightOperand;
							break;
							
						case ALUOp.ShiftRightArithmetic:
							if ((leftOperand & 0x80000000) != 0)
								result = (leftOperand >> (int)rightOperand) | (0xFFFFFFFF << (int)(32 - rightOperand));
							else
								result = leftOperand >> (int)rightOperand;
							
							break;
							
						case ALUOp.ShiftRightLogical:
							result = leftOperand >> (int)rightOperand;
							break;
							
						case ALUOp.Subtract:
							result = unchecked(leftOperand - rightOperand);
							break;
							
						case ALUOp.Xor:
							result = leftOperand ^ rightOperand;
							break;
							
						default:
							throw new InvalidOperationException("Owner.ExecuteRegister.ALUOp is set to an unrecognized value of ALUOp: " + ((byte)operation).ToString());
					}
					
					return result;
				}
			}
			#endregion
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>ArithmeticLogicUnit</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>ArithmeticLogicUnit</c> is a component.
			/// </param>
			#endregion
			public ArithmeticLogicUnit(Processor owner): base(owner) {}
			#endregion
		}
	}
}
