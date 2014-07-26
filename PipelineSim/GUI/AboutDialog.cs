#region Using Directives
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
#endregion

namespace PipelineSim.GUI
{
	#region XML Header
	/// <summary>
	/// Displays information about the application to the user.
	/// </summary>
	#endregion
	internal sealed class AboutDialog: Form
	{
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Creates and initializes all child <c>Control</c>s.
		/// </summary>
		/// <param name="sender">The current <c>AboutDialog</c>.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InitializeControls(object sender, EventArgs e)
		{
			#region Local Variables
			Label [] labels = new Label[2];
			#endregion
			
			#region Version Label
			labels[0] = new Label();
			labels[0].Text = ProductName + " v" + ProductVersion + " beta (" + Enum.GetName(typeof(ProcessorArchitecture), Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture).ToLower() + ")";
			labels[0].Left = 5;
			labels[0].Top = 5;
			labels[0].AutoSize = true;
			Controls.Add(labels[0]);
			#endregion
			
			#region Developer Label
			labels[1] = new Label();
			labels[1].Text = "Developed by Seth Nicholson for OIT CSET Software Senior Project 2012-2013.";
			labels[1].Left = labels[0].Left;
			labels[1].Top = labels[0].Bottom + 10;
			labels[1].AutoSize = true;
			Controls.Add(labels[1]);
			#endregion
			
			Load -= InitializeControls;
			return;
		}
		
		private void OpenLink(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Process.Start((string)e.Link.LinkData);
				e.Link.Visited = true;
			}
			return;
		}
		#endregion
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>AboutDialog</c> instance.
		/// </summary>
		#endregion
		public AboutDialog()
		{
			#region Property Assignments
			Text = "About " + ProductName;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			BackColor = Color.White;
			ShowInTaskbar = false;
			ShowIcon = true;
			MinimizeBox = false;
			MaximizeBox = false;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			#endregion
			
			#region Event Handler Assignments
			Load += InitializeControls;
			#endregion
		}
		#endregion
	}
}
