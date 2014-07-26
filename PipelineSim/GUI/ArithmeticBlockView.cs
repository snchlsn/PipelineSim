#region Using Directives
using System;
using System.ComponentModel;
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
			protected class ArithmeticBlockView: UserControl
			{
				#region Fields
				private Point[] vertices = new Point[7];
				private Orientation orientation = Orientation.Horizontal;
				
				private OrientedTextLabel blockLabel;
				#endregion
				
				#region Properties
				protected override Size DefaultSize
				{
					get { return new Size(80, 30); }
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
							
							if (orientation == Orientation.Vertical)
							{
								blockLabel.RotationAngle = 270;
								blockLabel.AutoSize = false;
								blockLabel.Size = Size.Empty;
								blockLabel.SizeChanged += SetLabelSize;
								blockLabel.AutoSize = true;
							}
							else
							{
								blockLabel.RotationAngle = 0;
								blockLabel.AutoSize = true;
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
				
				public override string Text
				{
					get { return blockLabel.Text; }
					set { blockLabel.Text = value; }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void PaintBorder(object sender, PaintEventArgs e)
				{
					e.Graphics.DrawPolygon(new Pen(Color.Black), vertices);
					return;
				}
				
				private void SetVertices(object sender, EventArgs e)
				{
					vertices[0] = Point.Empty;
					
					if (orientation == Orientation.Vertical)
					{
						int fifthOfHeight = ClientSize.Height / 5;
						
						vertices[1] = new Point(ClientSize.Width - 1, fifthOfHeight);
						vertices[2] = new Point(ClientSize.Width - 1, ClientSize.Height - fifthOfHeight);
						vertices[3] = new Point(0, ClientSize.Height - 1);
						vertices[4] = new Point(0, ClientSize.Height - (fifthOfHeight << 1));
						vertices[5] = new Point(ClientSize.Width / 3, ClientSize.Height >> 1);
						vertices[6] = new Point(0, fifthOfHeight << 1);
					}
					else
					{
						int fifthOfWidth = ClientSize.Width / 5;
						
						vertices[1] = new Point(fifthOfWidth, ClientSize.Height - 1);
						vertices[2] = new Point(ClientSize.Width - fifthOfWidth, ClientSize.Height - 1);
						vertices[3] = new Point(ClientSize.Width - 1, 0);
						vertices[4] = new Point(ClientSize.Width - (fifthOfWidth << 1), 0);
						vertices[5] = new Point(ClientSize.Width >> 1, ClientSize.Height / 3);
						vertices[6] = new Point(fifthOfWidth << 1, 0);
					}
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>ArithmeticBlockView</c> instance.
				/// </summary>
				#endregion
				public ArithmeticBlockView()
				{
					blockLabel = new OrientedTextLabel();
					blockLabel.AutoSize = true;
					blockLabel.Text = "Add";
					blockLabel.SizeChanged += (sender, e) => { ((Label)sender).Location = (Orientation == Orientation.Horizontal ? new Point((Width - ((Label)sender).Width) >> 1, Height - ((Label)sender).Height - 5) : new Point(Width - ((Label)sender).Width - 5, (Height - ((Label)sender).Height) >> 1)); };
					Controls.Add(blockLabel);
					
					Load += SetVertices;
					ClientSizeChanged += SetVertices;
					Paint += PaintBorder;
				}
				#endregion
				#endregion
			}
		}
	}
}
