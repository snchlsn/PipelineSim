#region Using Directives
using System;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;

using StageRegister = MIPSEmulator.Processor.StageRegister;
using ExecuteRegister = MIPSEmulator.Processor.ExecuteStageRegister;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class ExecuteRegisterView: StageRegisterView
			{
				#region Fields
				private const string stageName = "EX/MEM";
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
					StringBuilder toolTipText = new StringBuilder(177);
					ExecuteRegister executeRegister = (ExecuteRegister)StageRegister;
					ControlSignals signals = executeRegister.Signals;
					uint memoryWriteValue = executeRegister.MemoryWriteValue;
					uint aluResult = executeRegister.ALUResult;
					Register destinationRegister = executeRegister.DestinationRegister;
					
					toolTipText.Append("MemRead: ").AppendLine(signals.HasFlag(ControlSignals.MemRead) ? "1" : "0");
					toolTipText.Append("MemWrite: ").AppendLine(signals.HasFlag(ControlSignals.MemWrite) ? "1" : "0");
					toolTipText.Append("RegWrite: ").AppendLine(signals.HasFlag(ControlSignals.RegWrite) ? "1" : "0");
					toolTipText.Append("MemToReg: ").AppendLine(signals.HasFlag(ControlSignals.MemRead) ? "1" : "0");
					toolTipText.Append("Branch target: ").Append(Globals.HexPrefix).AppendLine(executeRegister.BranchTarget.ToString(Globals.UintFormat));
					toolTipText.Append("ALU result: ").Append(Globals.HexPrefix).Append(aluResult.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(aluResult), 0)).AppendLine(")");
					toolTipText.Append("Mem write value: ").Append(Globals.HexPrefix).Append(memoryWriteValue.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(memoryWriteValue), 0)).AppendLine(")");
					toolTipText.Append("Destination reg: ").Append(Globals.HexPrefix).Append(((byte)destinationRegister).ToString(Globals.ByteFormat)).Append(" (").Append(Enum.GetName(typeof(Register), destinationRegister)).Append(')');
					toolTip.SetToolTip(this, toolTipText.ToString());
					
					#if DEBUG
					if (toolTipText.Capacity > 177)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					base.SetToolTip(sender, e);
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>ExecuteRegisterView</c> instance.
				/// </summary>
				#endregion
				public ExecuteRegisterView() {}
				#endregion
				#endregion
			}
		}
	}
}
