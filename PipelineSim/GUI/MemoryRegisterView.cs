#region Using Directives
using System;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;

using StageRegister = MIPSEmulator.Processor.StageRegister;
using MemoryRegister = MIPSEmulator.Processor.MemoryStageRegister;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class MemoryRegisterView: StageRegisterView
			{
				#region Fields
				private const string stageName = "MEM/WB";
				#endregion
				
				#region Properties
				protected override string StageName
				{
					get { return stageName; }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				protected override void SetToolTip(object sender, EventArgs e)
				{
					StringBuilder toolTipText = new StringBuilder(94);
					MemoryRegister memoryRegister = (MemoryRegister)StageRegister;
					ControlSignals signals = memoryRegister.Signals;
					uint readValue = memoryRegister.ReadValue;
					uint aluResult = memoryRegister.ALUResult;
					
					toolTipText.Append("RegWrite: ").AppendLine(signals.HasFlag(ControlSignals.RegWrite) ? "1" : "0");
					toolTipText.Append("MemToReg: ").AppendLine(signals.HasFlag(ControlSignals.MemRead) ? "1" : "0");
					toolTipText.Append("Mem read value: ").Append(Globals.HexPrefix).Append(readValue.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(readValue), 0)).AppendLine(")");
					toolTipText.Append("ALU result: ").Append(Globals.HexPrefix).Append(aluResult.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(aluResult), 0)).Append(')');
					toolTip.SetToolTip(this, toolTipText.ToString());
					
					#if DEBUG
					if (toolTipText.Capacity > 94)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					base.SetToolTip(sender, e);
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>MemoryRegisterView</c> instance.
				/// </summary>
				#endregion
				public MemoryRegisterView() {}
				#endregion
				#endregion
			}
		}
	}
}
