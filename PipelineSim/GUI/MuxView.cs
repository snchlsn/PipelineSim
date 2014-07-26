#region Using Directives
using System;
using System.Drawing;
using System.Windows.Forms;
using FormsExtensions;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			#region XML Header
			/// <summary>
			/// Displays a multiplexer in a block diagram.
			/// </summary>
			#endregion
			protected class MuxView: Control
			{
				#region Fields
				private OrientedTextLabel muxLabel;
				
				private byte inputCount = 2;
				private Orientation orientation = Orientation.Horizontal;
				#endregion
				
				#region Properties
				protected override Size DefaultSize
				{
					get { return new Size(50, 25); }
				}
				
				#region XML Header
				/// <summary>
				/// Gets or sets the number of input lines that are connected to the mux.
				/// </summary>
				/// <value>The number of input lines.</value>
				#endregion
				public byte InputCount
				{
					get { return inputCount; }
					set { inputCount = value; }
				}
				
				public Orientation Orientation
				{
					get { return orientation; }
					set
					{
						if (orientation != value)
						{
							orientation = value;
							
							Size = new Size(Height, Width);
							muxLabel.Location = new Point(muxLabel.Top, muxLabel.Left);
							
							if (orientation == Orientation.Vertical)
							{
								muxLabel.RotationAngle = 270;
								muxLabel.AutoSize = false;
								muxLabel.Size = Size.Empty;
								muxLabel.SizeChanged += SetLabelSize;
								muxLabel.AutoSize = true;
							}
							else
							{
								muxLabel.RotationAngle = 0;
								muxLabel.AutoSize = true;
							}
						}
					}
				}
				
				private void SetLabelSize(object sender, EventArgs e)
				{
					Label label = (Label)sender;
					
					label.AutoSize = false;
					label.SizeChanged -= SetLabelSize;
					label.Size = new Size(label.Height, label.Width);
					return;
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				#region XML Header
				/// <summary>
				/// Paints the mux's border.
				/// </summary>
				/// <param name="sender">The <c>object</c> that raised the event.</param>
				/// <param name="e">A <c>System.Windows.Forms.PaintEventArgs</c>.</param>
				#endregion
				private void PaintBorder(object sender, PaintEventArgs e)
				{
					Pen pen = new Pen(ForeColor);
					int radius;
					int diameter;
					
					e.Graphics.Clear(BackColor);
					
					if (orientation == Orientation.Vertical)
					{
						radius = Height >> 2;
						diameter = radius << 1;
						e.Graphics.DrawArc(pen, 0, 0, Width - 1, diameter, 0, -180);
						e.Graphics.DrawLine(pen, 0, radius, 0, Height - radius);
						e.Graphics.DrawLine(pen, Width - 1, radius, Width - 1, Height - radius);
						e.Graphics.DrawArc(pen, 0, Height - diameter - 1, Width - 1, diameter, 0, 180);
					}
					else
					{
						radius = Width >> 2;
						diameter = radius << 1;
						e.Graphics.DrawArc(pen, 0, 0, diameter, Height - 1, -90, -180);
						e.Graphics.DrawLine(pen, radius, 0, Width - radius, 0);
						e.Graphics.DrawLine(pen, radius, Height - 1, Width - radius, Height - 1);
						e.Graphics.DrawArc(pen, Width - diameter - 1, 0, diameter, Height - 1, -90, 180);
					}
					
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>MuxView</c> instance.
				/// </summary>
				#endregion
				public MuxView()
				{
					muxLabel = new OrientedTextLabel();
					muxLabel.AutoSize = true;
					muxLabel.Text = "Mux";
					muxLabel.Left = 10;
					muxLabel.Top = 4;
					Controls.Add(muxLabel);
					
					Paint += PaintBorder;
				}
				#endregion
				#endregion
			}
		}
	}
}
