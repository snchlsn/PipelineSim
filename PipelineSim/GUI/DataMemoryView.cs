#region Using Directives
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;

using DataMemorySynchronizer = MIPSEmulator.Processor.DataMemorySynchronizer;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class DataMemoryView: ClockedComponentView
			{
				#region Fields
				private DataMemorySynchronizer dataMemorySynchronizer;
				
				private ToolTip toolTip;
				#endregion
				
				#region Properties
				public DataMemorySynchronizer DataMemorySynchronizer
				{
					get { return dataMemorySynchronizer; }
					set
					{
						if (dataMemorySynchronizer != value)
						{
							if (dataMemorySynchronizer != null)
								dataMemorySynchronizer.StateChanged -= SetToolTip;
							
							dataMemorySynchronizer = value;
							
							if (dataMemorySynchronizer != null)
							{
								dataMemorySynchronizer.StateChanged += SetToolTip;
								SetToolTip();
							}
						}
					}
				}
				
				protected override Size DefaultSize
				{
					get { return new Size(90, 120); }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void SetToolTip(object sender = null, EventArgs e = null)
				{
					StringBuilder toolTipText = new StringBuilder(22);
					uint readValue = dataMemorySynchronizer.ReadValue;
					
					toolTipText.Append("Read value: ").Append(Globals.HexPrefix).Append(readValue.ToString(Globals.UintFormat));
					toolTip.SetToolTip(this, toolTipText.ToString());
					
					#if DEBUG
					if (toolTipText.Capacity > 22)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>DataMemoryView</c> instance.
				/// </summary>
				#endregion
				public DataMemoryView(): base(true)
				{
					Label label;
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Data Memory";
					label.Top = 40;
					label.Left = 15;
					Controls.Add(label);
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Address";
					label.Top = 15;
					label.Left = 2;
					Controls.Add(label);
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Write Data";
					label.Top = Bottom - 30;
					label.Left = 2;
					Controls.Add(label);
					
					label = new Label();
					label.AutoSize = true;
					label.Text = "Read Data";
					label.Top = 70;
					label.Left = 30;
					Controls.Add(label);
					
					toolTip = new ToolTip();
					toolTip.ToolTipTitle = "Data Memory";
					toolTip.InitialDelay = toolTipInitialDelay;
					toolTip.ReshowDelay = toolTipReshowDelay;
				}
				#endregion
				#endregion
			}
		}
	}
}
