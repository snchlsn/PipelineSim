#region Using Directives
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;

using RegisterFile = MIPSEmulator.Processor.RegisterFile;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class RegisterFileView: ClockedComponentView
			{
				#region Fields
				private RegisterFile registerFile;
				
				private ToolTip toolTip;
				#endregion
				
				#region Properties
				public RegisterFile RegisterFile
				{
					get { return registerFile; }
					set
					{
						if (registerFile != value)
						{
							if (registerFile != null)
								registerFile.Owner.FetchRegister.StateChanged -= SetToolTip;
							
							registerFile = value;
							
							if (registerFile != null)
							{
								registerFile.Owner.FetchRegister.StateChanged += SetToolTip;
								SetToolTip();
							}
						}
					}
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void SetToolTip(object sender = null, EventArgs e = null)
				{
					StringBuilder toolTipText = new StringBuilder(68);
					Instruction instruction = (Instruction)registerFile.Owner.FetchRegister.Instruction;
					uint rsValue = registerFile[instruction.RS];
					uint rtValue = registerFile[instruction.RT];
					
					toolTipText.Append("Read value 1: ").Append(Globals.HexPrefix).Append(rsValue.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(rsValue), 0)).AppendLine(")");
					toolTipText.Append("Read value 2: ").Append(Globals.HexPrefix).Append(rtValue.ToString(Globals.UintFormat)).Append(" (").Append(BitConverter.ToInt32(BitConverter.GetBytes(rtValue), 0)).Append(')');
					
					#if DEBUG
					if (toolTipText.Capacity > 68)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					toolTip.SetToolTip(this, toolTipText.ToString());
					foreach (Control control in Controls)
						toolTip.SetToolTip(control, toolTipText.ToString());
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>RegisterFileView</c> instance.
				/// </summary>
				#endregion
				public RegisterFileView(): base(true)
				{
					Label[] labels = new Label[2];
					
					Width = 90;
					Height = 120;
					
					labels[0] = new Label();
					labels[0].AutoSize = true;
					labels[0].Text = "Read R1";
					labels[0].Left = 2;
					labels[0].Top = 2;
					Controls.Add(labels[0]);
					
					labels[1] = new Label();
					labels[1].Width = Width - 4;
					labels[1].TextAlign = ContentAlignment.TopRight;
					labels[1].Text = "Registers";
					labels[1].Left = labels[0].Left;
					labels[1].Top = labels[0].Bottom + 5;
					Controls.Add(labels[1]);
					
					labels[0] = new Label();
					labels[0].AutoSize = true;
					labels[0].Text = "Read R2";
					labels[0].Left = labels[1].Left;
					labels[0].Top = labels[1].Bottom;
					Controls.Add(labels[0]);
					
					labels[1] = new Label();
					labels[1].AutoSize = true;
					labels[1].Text = "Write Data";
					labels[1].Left = labels[0].Left;
					labels[1].Top = labels[0].Bottom + 20;
					Controls.Add(labels[1]);
					
					labels[0] = new Label();
					labels[0].AutoSize = true;
					labels[0].Text = "Write Reg";
					labels[0].Left = labels[1].Left;
					labels[0].Top = labels[1].Bottom + 10;
					Controls.Add(labels[0]);
					
					toolTip = new ToolTip();
					toolTip.ToolTipTitle = "Register File";
					toolTip.InitialDelay = toolTipInitialDelay;
					toolTip.ReshowDelay = toolTipReshowDelay;
				}
				#endregion
				#endregion
			}
		}
	}
}
