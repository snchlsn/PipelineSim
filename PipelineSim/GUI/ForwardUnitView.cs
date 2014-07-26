#region Using Directives
using System;
using System.Drawing;
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
			protected class ForwardUnitView: CombinationalLogicBlockView
			{
				#region Fields
				private ControlUnit control;
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
					get { return new Size(100, 50); }
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
				/// Creates and initializes a new <c>ForwardUnitView</c> instance.
				/// </summary>
				#endregion
				public ForwardUnitView()
				{
					Text = "Forwarding\nUnit";
				}
				#endregion
				#endregion
			}
		}
	}
}
