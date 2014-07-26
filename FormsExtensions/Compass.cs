#region Using Directives
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace FormsExtensions
{
	#region XML Header
	/// <summary>
	/// A Panel that draws a compass with a rotatable needle.
	/// </summary>
	#endregion
	public class Compass: UserControl
	{
		#region Fields
		private Label[] cardinalLabels;
		
		private GraphicsPath needlePath;
		private Pen neeedlePen;
		
		private double bearing = 0, oldBearing = 1;
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets or sets the angle of the needle in radians relative to due east.
		/// </summary>
		#endregion
		public double Bearing
		{
			get
			{
				return bearing;
			}
			set
			{
				double twoPi = 2 * Math.PI;
				
				if (value >= 0f && value <= twoPi)
					bearing = value;
				else if (value < 0f)
					bearing = value - twoPi * (int)(value / twoPi - 1);
				else
					bearing = value - twoPi * (int)(value / twoPi);
			}
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Draws the compass needle if Direction has changed since it was last drawn.
		/// </summary>
		/// <param name="sender">The current Compass.</param>
		/// <param name="e">Provides data about the Compass being painted.</param>
		#endregion
		private void DrawNeedle(object sender, PaintEventArgs e)
		{
			if (oldBearing != Bearing)
			{
				Graphics graphics = CreateGraphics();
				
				graphics.DrawPath(new Pen(BackColor, neeedlePen.Width), needlePath);
				needlePath.Reset();
				needlePath.AddLine(Width / 2, Height / 2, (int)((Width / 2d) * (1 + Math.Cos(Bearing)) - cardinalLabels[2].Width * Math.Cos(Bearing)), (int)((Height / 2) * (1 - Math.Sin(Bearing)) + cardinalLabels[2].Width * Math.Sin(Bearing)));
				graphics.DrawPath(neeedlePen, needlePath);
				
				oldBearing = Bearing;
				
				Bearing += .1;
			}
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new Compass instance.
		/// </summary>
		#endregion
		public Compass()
		{
			#region Control Initialization
			cardinalLabels = new Label[4];
			for (int i = 0; i < cardinalLabels.Length; ++i)
			{
				cardinalLabels[i] = new Label();
				cardinalLabels[i].TextAlign = ContentAlignment.MiddleCenter;
				cardinalLabels[i].Width = cardinalLabels[i].Height;
				cardinalLabels[i].BackColor = Color.White;
			}
			cardinalLabels[0].Dock = DockStyle.Top;
			cardinalLabels[0].Text = "N";
			cardinalLabels[1].Dock = DockStyle.Bottom;
			cardinalLabels[1].Text = "S";
			cardinalLabels[2].Dock = DockStyle.Right;
			cardinalLabels[2].Text = "E";
			cardinalLabels[3].Dock = DockStyle.Left;
			cardinalLabels[3].Text = "W";
			Controls.AddRange(cardinalLabels);
			#endregion
			
			#region Field Initialization
			needlePath = new GraphicsPath();
			neeedlePen = new Pen(Color.Black, 2f);
			#endregion
			
			#region Event Handler Assignments
			Paint += DrawNeedle;
			#endregion
		}
		#endregion
	}
}
