#region Using Directives
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;

using ControlUnit = MIPSEmulator.Processor.ControlUnit;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected class ControlUnitView: CombinationalLogicBlockView
			{
				#region Fields
				private ControlUnit control;
				
				private ToolTip toolTip;
				#endregion
				
				#region Properties
				public ControlUnit Control
				{
					get { return control; }
					set
					{
						if (control != value)
						{
							if (control != null)
								control.Owner.ControlPathUpdated -= SetToolTip;
							
							control = value;
							
							if (control != null)
							{
								control.Owner.ControlPathUpdated += SetToolTip;
								SetToolTip();
							}
						}
					}
				}
				
				protected override Size DefaultSize
				{
					get { return new Size(80, 80); }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void SetToolTip(object sender = null, EventArgs e = null)
				{
					//TODO: Implement.
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>ControlUnitView</c> instance.
				/// </summary>
				#endregion
				public ControlUnitView()
				{
					Text = "Control";
					
					toolTip = new ToolTip();
					toolTip.ToolTipTitle = "Control & Hazard Unit";
				}
				#endregion
				#endregion
			}
		}
	}
}
