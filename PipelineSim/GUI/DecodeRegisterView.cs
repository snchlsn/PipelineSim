#region Using Directives
using System;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;

using StageRegister = MIPSEmulator.Processor.StageRegister;
using DecodeRegister = MIPSEmulator.Processor.DecodeStageRegister;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class DecodeRegisterView: StageRegisterView
			{
				#region Fields
				private const string stageName = "ID/EX";
				#endregion
				
				#region Properties
				protected override string StageName
				{
					get	{ return stageName; }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				protected override void SetToolTip(object sender, EventArgs e)
				{
					StringBuilder toolTipText = new StringBuilder(267);
					ControlSignals signals = StageRegister.Signals;
					DecodeRegister decodeRegister = (DecodeRegister)StageRegister;
					Instruction instruction = decodeRegister.Instruction;
					
					toolTipText.Append("RegDst: ").AppendLine(signals.HasFlag(ControlSignals.RegDst) ? "1" : "0");
					toolTipText.Append("ALUSrc: ").AppendLine(signals.HasFlag(ControlSignals.ALUSrc) ? "1" : "0");
					toolTipText.Append("ALUOp: ").Append(Globals.HexPrefix).Append(((byte)decodeRegister.ALUOp).ToString(Globals.NibbleFormat)).Append(" (").Append(Enum.GetName(typeof(ALUOp), decodeRegister.ALUOp)).AppendLine(")");
					toolTipText.Append("Branch: ").AppendLine(signals.HasFlag(ControlSignals.Branch) ? "1" : "0");
					toolTipText.Append("MemRead: ").AppendLine(signals.HasFlag(ControlSignals.MemRead) ? "1" : "0");
					toolTipText.Append("MemWrite: ").AppendLine(signals.HasFlag(ControlSignals.MemWrite) ? "1" : "0");
					toolTipText.Append("RegWrite: ").AppendLine(signals.HasFlag(ControlSignals.RegWrite) ? "1" : "0");
					toolTipText.Append("MemToReg: ").AppendLine(signals.HasFlag(ControlSignals.MemRead) ? "1" : "0");
					toolTipText.Append("Reg read value 1: ").Append(Globals.HexPrefix).Append(decodeRegister.RegisterReadValue1.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(decodeRegister.RegisterReadValue1), 0).ToString()).AppendLine(")");
					toolTipText.Append("Reg read value 2: ").Append(Globals.HexPrefix).Append(decodeRegister.RegisterReadValue2.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(decodeRegister.RegisterReadValue2), 0).ToString()).AppendLine(")");
					toolTipText.Append("Immediate value: ").Append(Globals.HexPrefix).Append(((int)instruction.Immediate).ToString(Globals.UintFormat)).Append(" (").Append(instruction.Immediate.ToString()).AppendLine(")");
					toolTipText.Append("Rt: ").Append(Globals.HexPrefix).Append(((byte)instruction.RT).ToString(Globals.ByteFormat)).Append(" (").Append(Enum.GetName(typeof(Register), instruction.RT)).AppendLine(")");
					toolTipText.Append("Rd: ").Append(Globals.HexPrefix).Append(((byte)instruction.RD).ToString(Globals.ByteFormat)).Append(" (").Append(Enum.GetName(typeof(Register), instruction.RD)).AppendLine(")");
					toolTipText.Append("Rs: ").Append(Globals.HexPrefix).Append(((byte)instruction.RS).ToString(Globals.ByteFormat)).Append(" (").Append(Enum.GetName(typeof(Register), instruction.RS)).AppendLine(")");
					toolTip.SetToolTip(this, toolTipText.ToString());
					
					#if DEBUG
					if (toolTipText.Capacity > 267)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					base.SetToolTip(sender, e);
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>DecodeRegisterView</c> instance.
				/// </summary>
				#endregion
				public DecodeRegisterView() {}
				#endregion
				#endregion
			}
		}
	}
}
