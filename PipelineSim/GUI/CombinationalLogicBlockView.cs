#region Using Directives
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected class CombinationalLogicBlockView: Label
			{
				#region Fields
				private const byte arcRadius = 10;
				#endregion
				
				#region Properties
				protected override Size DefaultSize
				{
					get { return new Size(35, 35); }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void PaintBorder(object sender, PaintEventArgs e)
				{
					Pen pen = new Pen(Color.Black);
					byte arcDiameter = arcRadius << 1;
					
					e.Graphics.DrawLine(pen, arcRadius, 0, Width - 1 - arcRadius, 0);
					e.Graphics.DrawArc(pen, Width - 1 - arcDiameter, 0, arcDiameter, arcDiameter, 270, 90);
					e.Graphics.DrawLine(pen, Width - 1, arcRadius, Width - 1, Height - 1 - arcRadius);
					e.Graphics.DrawArc(pen, Width - 1 - arcDiameter, Height - 1 - arcDiameter, arcDiameter, arcDiameter, 0, 90);
					e.Graphics.DrawLine(pen, arcRadius, Height - 1, Width - 1 - arcRadius, Height - 1);
					e.Graphics.DrawArc(pen, 0, Height - 1 - arcDiameter, arcDiameter, arcDiameter, 90, 90);
					e.Graphics.DrawLine(pen, 0, arcRadius, 0, Height - 1 - arcRadius);
					e.Graphics.DrawArc(pen, 0, 0, arcDiameter, arcDiameter, 180, 90);
					
					return;
				}
				#endregion
				
				#region Constructors
				public CombinationalLogicBlockView()
				{
					TextAlign = ContentAlignment.MiddleCenter;
					
					Paint += PaintBorder;
				}
				#endregion
				#endregion
			}
		}
	}
}
