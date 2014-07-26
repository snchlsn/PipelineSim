#region Using Directives
using System;
using System.Drawing;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;

using RegisterFile = MIPSEmulator.Processor.RegisterFile;
#endregion

namespace PipelineSim.GUI
{
	internal sealed class RegisterInspector: Form
	{
		#region Fields
		private ListView registerList;
		
		private RegisterFile registerFile;
		#endregion
		
		#region Properties
		public RegisterFile RegisterFile
		{
			get { return registerFile; }
			set
			{
				if (registerFile != null)
					registerFile.RegisterWritten -= UpdateRegister;
				
				registerFile = value;
				
				if (registerFile != null)
					registerFile.RegisterWritten += UpdateRegister;
				
				if (registerList != null)
					UpdateAllRegisters();
			}
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Creates and initializes all child <c>Control</c>s.
		/// </summary>
		/// <param name="sender">The current <c>RegisterInspector</c>.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InitializeControls(object sender, EventArgs e)
		{
			ListViewItem item;
			
			registerList = new ListView();
			registerList.Dock = DockStyle.Fill;
			registerList.View = View.Details;
			registerList.Columns.Add("Register");
			registerList.Columns.Add("Value (hex)");
			registerList.Columns.Add("Value (dec)");
			
			for (byte register = 0; register < 32; ++register)
			{
				item = new ListViewItem(Enum.GetName(typeof(Register), register).ToLower());
				item.SubItems.Add(string.Empty);
				item.SubItems.Add(string.Empty);
				registerList.Items.Add(item);
			}
			
			Controls.Add(registerList);
			
			Load -= InitializeControls;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Replaces the <c>Text</c> of each <c>System.Windows.Forms.ListViewSubItem</c> in the
		/// Value column of <c>registerList</c> with a <c>string</c> giving the current value of
		/// the corresponding <c>uint</c> in <c>registerFile</c>.
		/// </summary>
		/// <param name="sender">
		/// The <c>object</c> that raised the event.  The default is <c>null</c>.
		/// </param>
		/// <param name="e">
		/// A <c>System.EventArgs</c>.  The default is <c>null</c>.
		/// </param>
		#endregion
		private void UpdateAllRegisters(object sender = null, EventArgs e = null)
		{
			if (registerFile != null)
				for (byte register = 0; register < 32; ++register)
					UpdateRegister((Register)register);
			
			return;
		}
		
		private void UpdateRegister(object sender, RegisterAccessEventArgs e)
		{
			UpdateRegister(e.Register);
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Disposes of the resources (other than memory) used by the <c>RegisterInspector</c>.
		/// </summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release onky unmanaged resources.
		/// </param>
		#endregion
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				RegisterFile = null;
			
			base.Dispose(disposing);
			return;
		}
		
		private void UpdateRegister(Register register)
		{
			registerList.Items[(int)register].SubItems[1].Text = Globals.HexPrefix + registerFile[register].ToString(Globals.UintFormat);
			registerList.Items[(int)register].SubItems[2].Text = BitConverter.ToInt32(BitConverter.GetBytes(registerFile[register]), 0).ToString();
			return;
		}
		
		//TODO: Remove remaining Processor references, and fix other inspectors.
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>RegisterInspector</c> instance.
		/// </summary>
		/// <param name="processor">
		/// The <c>MIPSEmulator.Processor</c> whose <c>Registers</c> will be monitored by the
		/// new <c>RegisterInspector</c>.
		/// </param>
		#endregion
		public RegisterInspector(Processor processor)
		{
			Text = "Register Inspector";
			RegisterFile = (processor == null ? null : processor.Registers);
			
			Load += InitializeControls;
			if (registerFile != null)
				Load += UpdateAllRegisters;
		}
		#endregion
		#endregion
	}
}
