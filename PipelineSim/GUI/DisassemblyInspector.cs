#region Using Directives
using System;
using System.Drawing;
using System.Windows.Forms;
using MIPSEmulator;
using MIPSEmulator.Assembly;
#endregion

namespace PipelineSim.GUI
{
	internal sealed class DisassemblyInspector: Form
	{
		#region Fields
		private ListView disassemblyList;
		
		private Processor processor;
		#endregion
		
		#region Properties
		public Processor Processor
		{
			get { return processor; }
			set
			{
				if (processor != null)
					processor.ProgramLoaded -= CreateDisassemblyListing;
				
				processor = value;
				if (processor != null)
				{
					processor.ProgramLoaded += CreateDisassemblyListing;
					if (disassemblyList != null)
						CreateDisassemblyListing();
				}
				else if (disassemblyList != null)
					disassemblyList.Items.Clear();
			}
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Clears all <c>System.Windows.Forms.ListViewItem</c>s from the <c>disassemblyList</c>,
		/// and then replaces them with a disassembly listing for all
		/// <c>MIPSEmulator.Assembly.Instruction</c>s in <c>processor.Program</c>.
		/// </summary>
		/// <param name="sender">
		/// The <c>object</c> that raised the event.  The default is <c>null</c>.
		/// </param>
		/// <param name="e">
		/// A <c>System.EventArgs</c>.  The default is <c>null</c>.
		/// </param>
		#endregion
		private void CreateDisassemblyListing(object sender = null, EventArgs e = null)
		{
			if (processor.Program != null)
			{
				ListViewItem item;
				uint address = 0;
				
				disassemblyList.SuspendLayout();
				disassemblyList.Items.Clear();
				
				foreach (Instruction instruction in processor.Program)
				{
					item = new ListViewItem(Globals.HexPrefix + address.ToString(Globals.UintFormat));
					item.SubItems.Add(Globals.HexPrefix + ((uint)instruction).ToString(Globals.UintFormat));
					item.SubItems.Add(instruction.ToString());
					disassemblyList.Items.Add(item);
					
					address += 4;
				}
				
				disassemblyList.ResumeLayout();
			}
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes all child <c>Control</c>s.
		/// </summary>
		/// <param name="sender">The current <c>DisassemblyInspector</c>.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InitializeControls(object sender, EventArgs e)
		{
			disassemblyList = new ListView();
			disassemblyList.Dock = DockStyle.Fill;
			disassemblyList.View = View.Details;
			disassemblyList.Columns.Add("Address", 75);
			disassemblyList.Columns.Add("Machine Code", 85);
			disassemblyList.Columns.Add("Assembly Instruction", 125);
			Controls.Add(disassemblyList);
			
			Load -= InitializeControls;
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Disposes of the resources (other than memory) used by the <c>DisassemblyInspector</c>.
		/// </summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release onky unmanaged resources.
		/// </param>
		#endregion
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Processor = null;
			
			base.Dispose(disposing);
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>DisassemblyInspector</c> instance.
		/// </summary>
		/// <param name="processor">
		/// The <c>MIPSEmulator.Processor</c> whose loaded <c>MIPSEmulator.ProgramMemory</c> will be
		/// disassembled and displayed.
		/// </param>
		#endregion
		public DisassemblyInspector(Processor processor)
		{
			Text = "Disassembly Inspector";
			Processor = processor;
			
			Load += InitializeControls;
			if (processor != null)
				Load += CreateDisassemblyListing;
		}
		#endregion
		#endregion
	}
}
