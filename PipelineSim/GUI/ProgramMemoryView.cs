#region Using Directives
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;

using ProgramMemorySynchronizer = MIPSEmulator.Processor.ProgramMemorySynchronizer;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class ProgramMemoryView: ClockedComponentView
			{
				#region Fields
				private ProgramMemorySynchronizer programMemorySynchronizer;
				
				private ToolTip toolTip;
				#endregion
				
				#region Properties
				protected override Size DefaultSize
				{
					get { return new Size(90, 120); }
				}
				
				public ProgramMemorySynchronizer ProgramMemorySynchronizer
				{
					get { return programMemorySynchronizer; }
					set
					{
						if (programMemorySynchronizer != value)
						{
							if (programMemorySynchronizer != null)
								programMemorySynchronizer.StateChanged -= SetToolTip;
							
							programMemorySynchronizer = value;
							
							if (programMemorySynchronizer != null)
							{
								programMemorySynchronizer.StateChanged += SetToolTip;
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
					StringBuilder toolTipText = new StringBuilder(23);
					
					toolTipText.Append("Instruction: ").Append(Globals.HexPrefix).Append(programMemorySynchronizer.Instruction.ToString(Globals.UintFormat));
					
					#if DEBUG
					if (toolTipText.Capacity > 23)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					toolTip.SetToolTip(this, toolTipText.ToString());
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>ProgramMemoryView</c> instance.
				/// </summary>
				#endregion
				public ProgramMemoryView(): base(true)
				{
					Label label;
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Address";
					label.Top = 2;
					label.Left = 2;
					Controls.Add(label);
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Program\nMemory";
					label.TextAlign = ContentAlignment.MiddleCenter;
					label.Top = 30;
					label.Left = 10;
					Controls.Add(label);
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Instruction";
					label.Top = Bottom - 30;
					label.Left = Right - 60;
					Controls.Add(label);
					
					toolTip = new ToolTip();
					toolTip.ToolTipTitle = "Program Memory";
					toolTip.InitialDelay = toolTipInitialDelay;
					toolTip.ReshowDelay = toolTipReshowDelay;
				}
				#endregion
				#endregion
			}
		}
	}
}
