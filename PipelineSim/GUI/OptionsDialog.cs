#region Using Directives
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PipelineSim.Config;
using MIPSEmulator;
#endregion

namespace PipelineSim.GUI
{
	#region XML Header
	/// <summary>
	/// A <c>System.Windows.Forms.Form</c> that exposes application settings to the user.
	/// </summary>
	#endregion
	internal sealed class OptionsDialog: Form
	{
		#region Fields
		private TextBox editorCommandTextBox;
		private NumericUpDown periodSpinBox;
		private CheckBox mixTrackingCheckBox;
		
		private GeneralSettings generalSettings;
		#endregion
		
		#region Properties
		protected override Padding DefaultPadding
		{
			get { return new Padding(10); }
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Updates settings from user input, and saves changes.
		/// </summary>
		/// <param name="sender">The <c>object</c>that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void CommitChanges(object sender, EventArgs e)
		{
			generalSettings.EditorCommand = editorCommandTextBox.Text;
			generalSettings.ClockPeriod = int.Parse(periodSpinBox.Text.Contains(".") ? periodSpinBox.Text.Remove(periodSpinBox.Text.IndexOf('.'), 1) : periodSpinBox.Text);
			generalSettings.MixTracking = mixTrackingCheckBox.Checked;
			generalSettings.Save();
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes all child <c>Control</c>s.
		/// </summary>
		/// <param name="sender">The current <c>OptionsDialog</c>.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InitializeControls(object sender, EventArgs e)
		{
			#region Local Variables
			RadioButton[] hazardModeRadioButtons;
			Button[] buttons = new Button[2];
			GroupBox groupBox;
			Panel[] panels = new Panel[3];
			Label label;
			#endregion
			
			#region Control Initialization
			//TODO: Add tool tips.
			
			#region Editor Command Text Box
			panels[0] = new Panel();
			panels[0].Dock = DockStyle.Top;
			panels[0].Padding = new Padding(0, 10, 0, 10);
			
			editorCommandTextBox = new TextBox();
			editorCommandTextBox.Text = generalSettings.EditorCommand;
			editorCommandTextBox.SelectionLength = 0;
			editorCommandTextBox.Dock = DockStyle.Fill;
			panels[0].Height = editorCommandTextBox.Height + panels[0].Padding.Vertical;
			panels[0].Controls.Add(editorCommandTextBox);
			
			buttons[0] = new Button();
			buttons[0].Text = "...";
			buttons[0].AutoSize = true;
			buttons[0].AutoSizeMode = AutoSizeMode.GrowAndShrink;
			buttons[0].Dock = DockStyle.Right;
			buttons[0].Click += ShowSelectEditorDialog;
			panels[0].Controls.Add(buttons[0]);
			
			label = new Label();
			label.Text = "Text editor:";
			label.Dock = DockStyle.Left;
			label.AutoSize = true;
			panels[0].Controls.Add(label);
			
			Controls.Add(panels[0]);
			#endregion
			
			#region Top Panel
			panels[1] = new Panel();
			panels[1].Dock = DockStyle.Top;
			
			#region Right Panel
			panels[2] = new Panel();
			panels[2].Dock = DockStyle.Left;
			
			#region Period Spin Box
			label = new Label();
			label.Text = "Clock period:";
			label.Top = 5;
			label.Left = 5;
			label.AutoSize = true;
			panels[2].Controls.Add(label);
			
			periodSpinBox = new NumericUpDown();
			periodSpinBox.Top = label.Top;
			periodSpinBox.Left = label.Right;
			periodSpinBox.Width = 45;
			periodSpinBox.DecimalPlaces = 1;
			periodSpinBox.Increment = .1M;
			periodSpinBox.Maximum = (decimal)GeneralSettings.MaxPeriod;
			periodSpinBox.Minimum = (decimal)GeneralSettings.MinPeriod;
			periodSpinBox.Value = ((decimal)generalSettings.ClockPeriod) / 10;
			panels[2].Controls.Add(periodSpinBox);
			
			label = new Label();
			label.Text = "s";
			label.Top = periodSpinBox.Top;
			label.Left = periodSpinBox.Right;
			panels[2].Controls.Add(label);
			#endregion
			
			#region Mix Tracking Check Box
			mixTrackingCheckBox = new CheckBox();
			mixTrackingCheckBox.Text = "Mix tracking";
			mixTrackingCheckBox.Top = label.Bottom + 10;
			mixTrackingCheckBox.Left = 5;
			mixTrackingCheckBox.AutoSize = true;
			mixTrackingCheckBox.Checked = generalSettings.MixTracking;
			panels[2].Controls.Add(mixTrackingCheckBox);
			#endregion
			
			panels[1].Controls.Add(panels[2]);
			#endregion
			
			#region Hazard Mode Radio Buttons
			groupBox = new GroupBox();
			groupBox.Text = "Hazard mode";
			groupBox.Width = 140;
			groupBox.Dock = DockStyle.Left;
			groupBox.Padding = new Padding(5);
			groupBox.Margin = new Padding(5);
			
			hazardModeRadioButtons = new RadioButton[3];
			for (byte mode = 0; mode < 3; ++mode)
			{
				hazardModeRadioButtons[mode] = new RadioButton();
				hazardModeRadioButtons[mode].AutoSize = true;
				hazardModeRadioButtons[mode].Tag = mode;
				hazardModeRadioButtons[mode].Checked = (HazardMode)mode == generalSettings.HazardMode;
				hazardModeRadioButtons[mode].CheckedChanged += (sender1, e1) => { if (((RadioButton)sender1).Checked) generalSettings.HazardMode = (HazardMode)((RadioButton)sender1).Tag; };
				hazardModeRadioButtons[mode].Dock = DockStyle.Top;
			}
			hazardModeRadioButtons[(int)HazardMode.Fail].Text = "Fail";
			hazardModeRadioButtons[(int)HazardMode.Stall].Text = "Stall";
			hazardModeRadioButtons[(int)HazardMode.StallAndForward].Text = "Stall and forward";
			groupBox.Controls.AddRange(hazardModeRadioButtons);
			
			panels[1].Controls.Add(groupBox);
			#endregion
			
			Controls.Add(panels[1]);
			#endregion
			
			#region Dialog Result Buttons
			buttons[0] = new Button();
			CancelButton = buttons[0];
			buttons[0].Text = "Cancel";
			buttons[0].Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			buttons[0].Top = ClientSize.Height - buttons[0].Height - Padding.Bottom;
			buttons[0].Left = ClientSize.Width - buttons[0].Width - Padding.Right;
			Controls.Add(buttons[0]);
			
			buttons[1] = new Button();
			AcceptButton = buttons[1];
			buttons[1].Text = "OK";
			buttons[1].Anchor = buttons[0].Anchor;
			buttons[1].Top = buttons[0].Top;
			buttons[1].Left = buttons[0].Left - buttons[1].Width - Padding.Right;
			buttons[1].Click += CommitChanges;
			buttons[1].Click += (sender1, e1) => { Close(); };
			Controls.Add(buttons[1]);
			#endregion
			#endregion
			
			ClientSize = new Size(ClientSize.Width, panels[0].Bottom + buttons[1].Height + Padding.Vertical);
			Load -= InitializeControls;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Shows the user a dialog prompting them to select a program to be used to edit script files.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void ShowSelectEditorDialog(object sender, EventArgs e)
		{
			using (OpenFileDialog selectEditorDialog = new OpenFileDialog())
			{
				selectEditorDialog.Filter = "Executables (*.exe)|*.exe";
				selectEditorDialog.Title = "Select Text Editor";
				selectEditorDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
				if (selectEditorDialog.ShowDialog() == DialogResult.OK)
					editorCommandTextBox.Text = selectEditorDialog.FileName;
			}
			return;
		}
		#endregion
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>OptionsDialog</c> instance.
		/// </summary>
		#endregion
		public OptionsDialog()
		{
			Text = ProductName + " Options";
			MaximizeBox = false;
			MinimizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			ShowIcon = false;
			ShowInTaskbar = false;
			
			generalSettings = GeneralSettings.GetSection(ConfigurationUserLevel.None);
			
			Load += InitializeControls;
		}
		#endregion
		#endregion
	}
}
