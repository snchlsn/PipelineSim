#region Using Directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MIPSEmulator;

#endregion

namespace PipelineSim.GUI
{
	internal sealed class DataMemoryInspector: Form
	{
		#region Fields
		private ListView dataListView;
		
		private DataMemory data;
		#endregion
		
		#region Properties
		public Processor Processor
		{
			get { return (data == null ? null : data.Owner); }
			set
			{
				if (data != null)
					data.WordStored -= UpdateWord;
				
				if (value == null)
				{
					data = null;
					if (dataListView != null)
						dataListView.Items.Clear();
				}
				else
				{
					data = value.Data;
					if (dataListView != null)
						CreateDataListing();
					
					data.WordStored += UpdateWord;
				}
			}
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		private void CreateDataListing(object sender = null, EventArgs e = null)
		{
			if (data != null)
			{
				ListViewItem item;
				
				dataListView.SuspendLayout();
				dataListView.Items.Clear();
				
				foreach (KeyValuePair<uint, uint> word in data)
				{
					item = new ListViewItem(Globals.HexPrefix + word.Key.ToString(Globals.UintFormat));
					item.Name = word.Key.ToString("X");
					item.Tag = word.Key;
					item.SubItems.Add(Globals.HexPrefix + word.Value.ToString(Globals.UintFormat));
					item.SubItems.Add(word.Value.ToString());
					dataListView.Items.Add(item);
				}
				
				dataListView.ResumeLayout(true);
			}
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes all child <c>Control</c>s.
		/// </summary>
		/// <param name="sender">The current <c>DataMemoryInspector</c>.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void InitializeControls(object sender, EventArgs e)
		{
			dataListView = new ListView();
			dataListView.Dock = DockStyle.Fill;
			dataListView.View = View.Details;
			dataListView.Columns.Add("Address");
			dataListView.Columns.Add("Value (hex)");
			dataListView.Columns.Add("Value (dec)");
			Controls.Add(dataListView);
			
			Load -= InitializeControls;
			return;
		}
		
		private void UpdateWord(object sender, MemoryAccessEventArgs e)
		{
			int index = dataListView.Items.IndexOfKey(e.Address.ToString("X"));
			uint word = data.LoadWord(e.Address);
			
			if (index >= 0)
			{
				dataListView.Items[index].SubItems[1].Text = Globals.HexPrefix + word.ToString(Globals.UintFormat);
				dataListView.Items[index].SubItems[2].Text = BitConverter.ToInt32(BitConverter.GetBytes(word), 0).ToString();
			}
			else
			{
				int start = 0;
				int end = dataListView.Items.Count - 1;
				ListViewItem item;
				
				item = new ListViewItem(Globals.HexPrefix + e.Address.ToString(Globals.UintFormat));
				item.Name = e.Address.ToString("X");
				item.Tag = e.Address;
				item.SubItems.Add(Globals.HexPrefix + word.ToString(Globals.UintFormat));
				item.SubItems.Add(BitConverter.ToInt32(BitConverter.GetBytes(word), 0).ToString());
				
				if (dataListView.Items.Count > 0)
				{
					index = end >> 1;
					while (end - start > 1)
					{
						if ((uint)dataListView.Items[index].Tag < e.Address)
							start = index + 1;
						else
							end = index;
						
						index = start + ((end - start) >> 1);
					}
					
					if ((uint)dataListView.Items[start].Tag > e.Address)
						dataListView.Items.Insert(start, item);
					else if ((uint)dataListView.Items[end].Tag < e.Address)
						dataListView.Items.Insert(end + 1, item);
					else
						dataListView.Items.Insert(end, item);
				}
				else
					dataListView.Items.Add(item);
			}
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Disposes of the resources (other than memory) used by the <c>DataMemoryInspector</c>.
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
		/// Creates and initializes a new <c>DataMemoryInspector</c> instance.
		/// </summary>
		/// <param name="processor">
		/// The <c>MIPSEmulator.Processor</c> whose <c>Data</c> the new <c>DataMemoryInspector</c>
		/// will monitor.
		/// </param>
		#endregion
		public DataMemoryInspector(Processor processor)
		{
			Text = "Data Memory Inspector";
			Processor = processor;
			
			Load += InitializeControls;
			if (data != null)
				Load += CreateDataListing;
		}
		#endregion
		#endregion
	}
}
