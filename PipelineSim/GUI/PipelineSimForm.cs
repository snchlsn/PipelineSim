#region Using Directives
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;
using PipelineSim.Config;
#endregion

namespace PipelineSim.GUI
{
	#region XML Header
	/// <summary>
	/// The main <c>Form</c> for the MIPS Pipeline Simulator.
	/// </summary>
	#endregion
	public sealed partial class PipelineSimForm: Form
	{
		#region Fields
		private DataMemoryInspector dataMemoryInspector;
		private DisassemblyInspector disassemblyInspector;
		private RegisterInspector registerInspector;
		
		private ToolStripMenuItem recentFilesMenu, stepMenuItem, halfStepMenuItem, runMenuItem, stopMenuItem, reloadMenuItem;
		
		private Processor processor;
		private FileInfo openFile;
		private MixTracker mixTracker;
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets the <c>PipelineSimForm</c>'s background color.
		/// </summary>
		#endregion
		public override Color BackColor
		{
			get { return Color.White; }
			set { throw new NotImplementedException(); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the default size of a <c>PipelineSimForm</c>.
		/// </summary>
		#endregion
		protected override Size DefaultSize
		{
			get { return new Size(895, 560); }
		}
		
		public string OpenFileName
		{
			get { return openFile.Name; }
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		#region Menu Event Handlers
		#region XML Header
		/// <summary>
		/// Clears the list of recently opened files.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void ClearRecentFiles(object sender, EventArgs e)
		{
			RecentFilesSettings recentFilesSettings = RecentFilesSettings.GetSection(ConfigurationUserLevel.None);
			
			recentFilesSettings.RecentFiles.Clear();
			recentFilesSettings.Save();
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Lauches the text editor specified by <c>GeneralSetting.EditorCommand</c>, passing it
		/// the path to the open script, if one is open.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void LaunchTextEditor(object sender, EventArgs e)
		{
			string editorCommand = GeneralSettings.GetSection(ConfigurationUserLevel.None).EditorCommand;
			
			if (openFile == null)
				Process.Start(editorCommand);
			else
				Process.Start(editorCommand, openFile.FullName);
			
			return;
		}
		
		private void OpenRecentFile(object sender, EventArgs e)
		{
			OpenScript(new FileInfo(((ToolStripMenuItem)sender).Text));
			return;
		}
		
		private void ReloadScript(object sender, EventArgs e)
		{
			OpenScript(openFile);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Stops the clock if it's running, and resets the emulator.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void ResetSimulation(object sender, EventArgs e)
		{
			TimedClock timedClock = SystemClock.Provider as TimedClock;
			
			if (timedClock != null)
				timedClock.Enabled = false;
			
			processor.Reset();
			SetSimulationMenuEnables(false);
			return;
		}
		
		private void RunSimulation(object sender, EventArgs e)
		{
			TimedClock timedClock = SystemClock.Provider as TimedClock;
			
			if (timedClock == null)
			{
				TimedClock.SetAsProvider();
				timedClock = (TimedClock)SystemClock.Provider;
			}
			
			timedClock.Enabled = true;
			SetSimulationMenuEnables(true);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Displays a dialog giving information about the program.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void ShowAboutDialog(object sender, EventArgs e)
		{
			using (AboutDialog aboutDialog = new AboutDialog())
			{
				aboutDialog.ShowDialog();
			}
			return;
		}
		
		
		private void ShowOpenDialog(object sender, EventArgs e)
		{
			using (OpenFileDialog openDialog = new OpenFileDialog())
			{
				openDialog.Title = "Open Script";
				openDialog.Filter = "MIPS scripts (*.ms)|*.ms|All files|*";
				if (openDialog.ShowDialog() == DialogResult.OK)
					OpenScript(new FileInfo(openDialog.FileName));
			}
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Displays the options dialog to the user.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void ShowOptionsDialog(object sender, EventArgs e)
		{
			using (OptionsDialog optionsDialog = new OptionsDialog())
			{
				optionsDialog.ShowDialog();
			}
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Calls <c>Process.Start</c>, passing the <paramref name="sender"/>'s <c>Tag</c> as the
		/// <c>fileName</c> argument.
		/// </summary>
		/// <param name="sender">The <c>ToolStripMenuItem</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void StartProcess(object sender, EventArgs e)
		{
			Process.Start((string)((ToolStripMenuItem)sender).Tag);
			return;
		}
		
		private void StepSimulation(object sender, EventArgs e)
		{
			ManualClock manualClock = SystemClock.Provider as ManualClock;
			
			if (manualClock == null)
			{
				ManualClock.SetAsProvider();
				manualClock = (ManualClock)SystemClock.Provider;
			}
			
			manualClock.InvertLevel();
			if ((bool)((ToolStripMenuItem)sender).Tag)
				manualClock.InvertLevel();
			
			return;
		}
		
		private void StopSimulation(object sender, EventArgs e)
		{
			((TimedClock)SystemClock.Provider).Enabled = false;
			SetSimulationMenuEnables(false);
			return;
		}
		
		private void ToggleDataMemoryInspector(object sender, EventArgs e)
		{
			if (dataMemoryInspector == null)
			{
				dataMemoryInspector = new DataMemoryInspector(processor);
				dataMemoryInspector.Closed += (sender1, e1) => { ((ToolStripMenuItem)sender).Checked = false; dataMemoryInspector.Dispose(); dataMemoryInspector = null; };
				dataMemoryInspector.Show();
			}
			else
				dataMemoryInspector.Close();
			
			return;
		}
		
		private void ToggleDisassemblyInspector(object sender, EventArgs e)
		{
			if (disassemblyInspector == null)
			{
				disassemblyInspector = new DisassemblyInspector(processor);
				disassemblyInspector.Closed += (sender1, e1) => { ((ToolStripMenuItem)sender).Checked = false; disassemblyInspector.Dispose(); disassemblyInspector = null; };
				disassemblyInspector.Show();
			}
			else
				disassemblyInspector.Close();
			
			return;
		}
		
		private void ToggleRegisterInspector(object sender, EventArgs e)
		{
			if (registerInspector == null)
			{
				registerInspector = new RegisterInspector(processor);
				registerInspector.FormClosed += (sender1, e1) => { ((ToolStripMenuItem)sender).Checked = false; registerInspector.Dispose(); registerInspector = null; };
				registerInspector.Show();
			}
			else
				registerInspector.Close();
			
			return;
		}
		#endregion
		
		private void EnableReload(object sender, EventArgs e)
		{
			reloadMenuItem.Enabled = true;
			processor.ProgramLoaded -= EnableReload;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes all child <c>Control</c>s.
		/// </summary>
		/// <param name="sender">The <c>PipelineSimForm</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InitializeControls(object sender, EventArgs e)
		{
			#region Local Variables
			ToolStripMenuItem menuItem, subMenuItem, subSubMenuItem;
			ProcessorView processorView;
			#endregion
			
			#region ProcessorView
			processorView = new ProcessorView(processor);
			processorView.Dock = DockStyle.Fill;
			Controls.Add(processorView);
			#endregion
			
			#region Main Menu
			MainMenuStrip = new MenuStrip();
			MainMenuStrip.Dock = DockStyle.Top;
			
			#region File
			menuItem = new ToolStripMenuItem("&File");
			menuItem.DropDownItems.Add(new ToolStripMenuItem("&Open script...", null, ShowOpenDialog, Keys.Control | Keys.O));
			recentFilesMenu = new ToolStripMenuItem("&Recent files");
			menuItem.DropDownItems.Add(recentFilesMenu);
			reloadMenuItem = new ToolStripMenuItem("Re&load", null, ReloadScript);
			reloadMenuItem.Enabled = false;
			menuItem.DropDownItems.Add(reloadMenuItem);
			menuItem.DropDownItems.Add(new ToolStripSeparator());
			menuItem.DropDownItems.Add("E&xit", null, (sender1, e1) => { Close(); });
			MainMenuStrip.Items.Add(menuItem);
			#endregion
			
			#region View
			menuItem = new ToolStripMenuItem("&View");
			subMenuItem = new ToolStripMenuItem("Data &memory inspector", null, ToggleDataMemoryInspector);
			subMenuItem.CheckOnClick = true;
			menuItem.DropDownItems.Add(subMenuItem);
			subMenuItem = new ToolStripMenuItem("&Disassembly inspector", null, ToggleDisassemblyInspector);
			subMenuItem.CheckOnClick = true;
			menuItem.DropDownItems.Add(subMenuItem);
			subMenuItem = new ToolStripMenuItem("&Register inspector", null, ToggleRegisterInspector);
			subMenuItem.CheckOnClick = true;
			menuItem.DropDownItems.Add(subMenuItem);
			MainMenuStrip.Items.Add(menuItem);
			#endregion
			
			#region Simulation
			menuItem = new ToolStripMenuItem("&Simulation");
			runMenuItem = new ToolStripMenuItem("&Run", null, RunSimulation, Keys.F7);
			menuItem.DropDownItems.Add(runMenuItem);
			stopMenuItem = new ToolStripMenuItem("&Stop", null, StopSimulation, Keys.F8);
			menuItem.DropDownItems.Add(stopMenuItem);
			menuItem.DropDownItems.Add(new ToolStripSeparator());
			halfStepMenuItem = new ToolStripMenuItem("&Half step", null, StepSimulation, Keys.F9);
			halfStepMenuItem.Tag = false;
			menuItem.DropDownItems.Add(halfStepMenuItem);
			stepMenuItem = new ToolStripMenuItem("&Full step", null, StepSimulation, Keys.F10);
			stepMenuItem.Tag = true;
			menuItem.DropDownItems.Add(stepMenuItem);
			menuItem.DropDownItems.Add(new ToolStripSeparator());
			menuItem.DropDownItems.Add(new ToolStripMenuItem("R&eset", null, ResetSimulation, Keys.F5));
			MainMenuStrip.Items.Add(menuItem);
			
			SetSimulationMenuEnables(false);
			#endregion
			
			#region Tools
			menuItem = new ToolStripMenuItem("&Tools");
			menuItem.DropDownItems.Add("Text &editor", null, LaunchTextEditor);
			menuItem.DropDownItems.Add(new ToolStripSeparator());
			menuItem.DropDownItems.Add("&Options...", null, ShowOptionsDialog);
			MainMenuStrip.Items.Add(menuItem);
			#endregion
			
			#region Help
			menuItem = new ToolStripMenuItem("&Help");
			subMenuItem = new ToolStripMenuItem("&Web");
			subSubMenuItem = new ToolStripMenuItem("MIPS &Quick Reference", null, StartProcess);
			subSubMenuItem.Tag = "http://www.mips.com/media/files/MD00565-2B-MIPS32-QRC-01.01.pdf";
			subMenuItem.DropDownItems.Add(subSubMenuItem);
			subSubMenuItem = new ToolStripMenuItem("MIPS &Encoding Reference", null, StartProcess);
			subSubMenuItem.Tag = "https://www.student.cs.uwaterloo.ca/~isg/res/mips/opcodes";
			subMenuItem.DropDownItems.Add(subSubMenuItem);
			menuItem.DropDownItems.Add(subMenuItem);
			menuItem.DropDownItems.Add(new ToolStripSeparator());
			menuItem.DropDownItems.Add("About...", null, ShowAboutDialog);
			MainMenuStrip.Items.Add(menuItem);
			#endregion
			
			Controls.Add(MainMenuStrip);
			#endregion
			
			Load -= InitializeControls;
			return;
		}
		
		private void PopulateRecentFilesMenu(object sender, EventArgs e)
		{
			RecentFilesCollection recentFiles = RecentFilesSettings.GetSection(ConfigurationUserLevel.None).RecentFiles;
			
			
			recentFilesMenu.DropDownItems.Clear();
			
			if (recentFiles.Count == 0)
				recentFilesMenu.DropDownItems.Add("(None)");
			else
			{
				foreach (RecentFileElement recentFile in recentFiles)
					recentFilesMenu.DropDownItems.Add(recentFile.FilePath, null, OpenRecentFile);
				
				recentFilesMenu.DropDownItems.Add(new ToolStripSeparator());
				recentFilesMenu.DropDownItems.Add("&Clear list", null, ClearRecentFiles);
			}
			
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Displays an error message when a hazard is encountered in fail mode.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">
		/// A <c>MIPSEmulator.HazardEventArgs</c> that provides data for the event.
		/// </param>
		#endregion
		private void ShowHazardError(object sender, HazardEventArgs e)
		{
			StringBuilder message = new StringBuilder("The emulator has encounterd a hazard in fail mode, and is now in an error state.\n\nHazard(s) encountered:", 256);
			
			if (e.HazardTypes.HasFlag(HazardTypes.Branch))
				message.Append("\nBranch");
			
			if (e.HazardTypes.HasFlag(HazardTypes.ComputeUseExecute))
				message.Append("\nCompute/use between decode and execute");
			else if (e.HazardTypes.HasFlag(HazardTypes.LoadUseExecute))
				message.Append("\nLoad/use between decode and execute");
			
			if (e.HazardTypes.HasFlag(HazardTypes.ComputeUseMemory))
				message.Append("\nCompute/use between decode and memory");
			else if (e.HazardTypes.HasFlag(HazardTypes.LoadUseMemory))
				message.Append("\nLoad/use between decode and memory");
			
			MessageBox.Show(message.ToString(), "Hazard Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Sets the <c>processor.HazardMode</c> and <c>TimedClock.Interval</c> according to the
		/// values of their corresponding configuration properties.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void UpdateSettings(object sender, EventArgs e)
		{
			GeneralSettings generalSettings = GeneralSettings.GetSection(ConfigurationUserLevel.None);
			
			processor.Mode = generalSettings.HazardMode;
			TimedClock.Interval = generalSettings.ClockPeriod * 50;
			return;
		}
		#endregion
		
		private void OpenScript(FileInfo script)
		{
			if (!script.Exists)
			{
				MessageBox.Show("File Not Found", script.FullName + " does not exist.", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			TimedClock timedClock = SystemClock.Provider as TimedClock;
			bool retry = false;
			
			if (timedClock != null)
				timedClock.Enabled = false;
			
			do
			{
				try
				{
					RecentFilesSettings recentFileSettings;
					
					retry = false;
					processor.Program = new ProgramMemory(Assembler.AssembleScript(script.FullName));
					openFile = script;
					SetTitle();
					
					recentFileSettings = RecentFilesSettings.GetSection(ConfigurationUserLevel.None);
					recentFileSettings.RecentFiles.Add(script.FullName);
					recentFileSettings.Save();
				}
				catch (AssemblyException e)
				{
					retry = MessageBox.Show(e.Message, "Error(s) in Script", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
				}
				catch (DllNotFoundException)
				{
					MessageBox.Show("Could not find mips_assembler.dll.\nScripts cannot be opened without it.", "Missing DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (Exception e)
				{
					retry = MessageBox.Show("An unexpected error occurred while attempting to assemble the script.\n\n" + e.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
				}
			} while (retry);
			
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Sets the <c>Enabled</c> properties of the items in the simulation menu.
		/// </summary>
		/// <param name="running">
		/// <c>true</c> if the emulator is running; <c>false</c> otherwise.
		/// </param>
		#endregion
		private void SetSimulationMenuEnables(bool running)
		{
			runMenuItem.Enabled = stepMenuItem.Enabled = halfStepMenuItem.Enabled = !running;
			stopMenuItem.Enabled = running;
			return;
		}
		
		private void SetTitle()
		{
			Text = (openFile == null ? ProductName : openFile.Name + " - " + ProductName);
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>PipelineSimForm</c> instance.
		/// </summary>
		#endregion
		public PipelineSimForm()
		{
			processor = new Processor();
			mixTracker = new MixTracker(this, processor);
			
			#region Event Handler Assignments
			processor.Failure += ShowHazardError;
			processor.ProgramLoaded += EnableReload;
			
			GeneralSettings.ChangesSaved += UpdateSettings;
			RecentFilesSettings.ChangesSaved += PopulateRecentFilesMenu;
			
			Load += InitializeControls;
			Load += (sender, e) => { SetTitle(); };
			Load += PopulateRecentFilesMenu;
			Load += UpdateSettings;
			#endregion
		}
		#endregion
		#endregion
	}
}
