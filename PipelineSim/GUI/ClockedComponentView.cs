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
			#region XML Header
			/// <summary>
			/// Base class for a <c>Control</c> representing a synchronous block of logic in a block
			/// diagram.
			/// </summary>
			#endregion
			protected abstract class ClockedComponentView: Control
			{
				#region Fields
				private const byte radius = 5;
				
				private readonly bool positiveEdge;
				private Point clockSymbolPosition;
				private AnchorStyles clockSymbolEdge = AnchorStyles.Bottom; //TODO: Replace with new enumeration?
				#endregion
				
				#region Properties
				protected AnchorStyles ClockSymbolEdge
				{
					get { return clockSymbolEdge; }
					set { clockSymbolEdge = value; }
				}
				
				protected Point ClockSymbolPosition
				{
					get { return clockSymbolPosition; }
					set { clockSymbolPosition = value; }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				#region XML Header
				/// <summary>
				/// Paints the component's border, as well as the clock symbol.
				/// </summary>
				/// <param name="sender">The <c>object</c> that raised the event.</param>
				/// <param name="e">A <c>System.Windows.Forms.PaintEventArgs</c>.</param>
				#endregion
				private void PaintBorder(object sender, PaintEventArgs e)
				{
					Rectangle border = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
					Pen pen = new Pen(ForeColor);
					Point[] vertices = new Point[3];
					byte diameter = radius << 1;
					byte triangleHeight = (byte)(diameter * 4 / 5);
					
					e.Graphics.Clear(BackColor);
					
					if (!positiveEdge)
					{
						switch (clockSymbolEdge)
						{
							//TODO: Add offsets.
							case AnchorStyles.Bottom:
								border.Height -= diameter;
								e.Graphics.DrawEllipse(pen, 0, border.Height, diameter, diameter);
								break;
							
							case AnchorStyles.Left:
								border.X = diameter;
								border.Width -= diameter;
								e.Graphics.DrawEllipse(pen, 0, 0, diameter, diameter);
								break;
							
							case AnchorStyles.Right:
								border.Width -= diameter;
								e.Graphics.DrawEllipse(pen, border.Width, 0, diameter, diameter);
								break;
							
							case AnchorStyles.Top:
								border.Y = diameter;
								border.Height -= diameter;
								e.Graphics.DrawEllipse(pen, 0, 0, diameter, diameter);
								break;
							
							default:
								throw new InvalidOperationException();
						}
					}
					
					switch (clockSymbolEdge)
					{
						case AnchorStyles.Bottom:
							vertices[0] = new Point(0, border.Height);
							vertices[2] = new Point(diameter, border.Height);
							vertices[1] = new Point(radius, border.Height - triangleHeight);
							break;
						
						case AnchorStyles.Left:
							vertices[0] = new Point(0, 0);
							vertices[2] = new Point(0, diameter);
							vertices[1] = new Point(triangleHeight, radius);
							break;
						
						case AnchorStyles.Right:
							vertices[0] = new Point(border.Width, 0);
							vertices[2] = new Point(border.Width, diameter);
							vertices[1] = new Point(border.Width - triangleHeight, radius);
							break;
						
						case AnchorStyles.Top:
							vertices[0] = new Point(0, 0);
							vertices[2] = new Point(diameter, 0);
							vertices[1] = new Point(radius, triangleHeight);
							break;
						
						default:
							throw new InvalidOperationException();
					}
					
					e.Graphics.DrawLines(pen, vertices);
					e.Graphics.DrawRectangle(pen, border);
					
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>ClockedComponentView</c> instance.
				/// </summary>
				/// <param name="positiveEdge">
				/// <c>true</c> if the component is clocked on the positive edge;
				/// <c>false</c> if it is clocked on the negative edge.
				/// </param>
				#endregion
				public ClockedComponentView(bool positiveEdge)
				{
					this.positiveEdge = positiveEdge;
					Paint += PaintBorder;
				}
				#endregion
				#endregion
			}
		}
	}
}
