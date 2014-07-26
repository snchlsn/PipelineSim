#region Using Directives
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;

using ProgramCounter = MIPSEmulator.Processor.ProgramCounter;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class ProgramCounterView: ClockedComponentView
			{
				#region Fields
				private ProgramCounter programCounter;
				
				private ToolTip toolTip;
				#endregion
				
				#region Properties
				protected override Size DefaultSize
				{
					get { return new Size(60, 35); }
				}
				
				public ProgramCounter ProgramCounter
				{
					get { return programCounter; }
					set
					{
						if (programCounter != value)
						{
							if (programCounter != null)
								programCounter.StateChanged -= SetToolTip;
							
							programCounter = value;
							
							if (programCounter != null)
							{
								programCounter.StateChanged += SetToolTip;
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
					StringBuilder toolTipText = new StringBuilder(31);
					
					toolTipText.Append("Instruction address: ").Append(Globals.HexPrefix).Append(programCounter.Address.ToString(Globals.UintFormat));
					toolTip.SetToolTip(this, toolTipText.ToString());
					
					#if DEBUG
					if (toolTipText.Capacity > 31)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>ProgramCounterView</c> instance.
				/// </summary>
				#endregion
				public ProgramCounterView(): base(false)
				{
					Label pcLabel;
					
					pcLabel = new Label();
					pcLabel.AutoSize = true;
					pcLabel.Top = 2;
					pcLabel.Left = 20;
					pcLabel.Text = "PC";
					Controls.Add(pcLabel);
					
					toolTip = new ToolTip();
					toolTip.ToolTipTitle = "Program Counter";
					toolTip.InitialDelay = toolTipInitialDelay;
					toolTip.ReshowDelay = toolTipReshowDelay;
				}
				#endregion
				#endregion
			}
		}
	}
}
