#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace MIPSEmulator
{
	#region XML Header
	/// <summary>
	/// Represents external data RAM, for use by a <c>MIPSEmulator.Processor</c>.
	/// </summary>
	#endregion
	public sealed class DataMemory: IEnumerable<KeyValuePair<uint, uint>>
	{
		#region Fields
		private SortedDictionary<uint, uint> data;
		private Processor owner;
		#endregion
		
		#region Properties
		public Processor Owner
		{
			get { return owner; }
			internal set { owner = value; }
		}
		#endregion
		
		#region Events
		#region XML Header
		/// <summary>
		/// Occcurs when all values in memory are reset to their default of <c>0</c>.
		/// </summary>
		#endregion
		public event EventHandler Cleared;
		
		#region XML Header
		/// <summary>
		/// Occurs when a word is written to memory, regardless of whether its value has changed.
		/// </summary>
		#endregion
		public event MemoryAccessEventHandler WordStored;
		#endregion
		
		#region Methods
		#region Event Raisers
		#region XML Header
		/// <summary>
		/// Raises the <c>Cleared</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnCleared(EventArgs e)
		{
			if (Cleared != null)
				Cleared(this, e);
			return;
		}
		
		private void OnWordStored(MemoryAccessEventArgs e)
		{
			if (WordStored != null)
				WordStored(this, e);
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Resets all values in memory to their default of <c>0</c>.
		/// </summary>
		#endregion
		internal void Clear()
		{
			data.Clear();
			OnCleared(EventArgs.Empty);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Is not, and never will be implemented.  It's just here to appease the compiler.  Don't call it.
		/// </summary>
		/// <returns>Nothing.</returns>
		/// <exception cref="System.NotImplementedException">
		/// You called it.  Didn't I tell you not to do that?
		/// </exception>
		#endregion
		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		public IEnumerator<KeyValuePair<uint, uint>> GetEnumerator()
		{
			return data.GetEnumerator();
		}
		
		#region XML Header
		/// <summary>
		/// Gets the value of a word in memory.
		/// </summary>
		/// <param name="address">The address of the word to get.</param>
		/// <returns>
		/// The word at the specified address, as a <c>uint</c>.  If no word has been stored at
		/// that address, the default value of <c>0</c> is returned.
		/// </returns>
		#endregion
		public uint LoadWord(uint address)
		{
			if (data.ContainsKey(address))
				return data[address];
			else
				return 0;
		}
		
		internal void StoreWord(uint address, uint word)
		{
			data[address] = word;
			OnWordStored(new MemoryAccessEventArgs(address));
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>DataMemory</c> instance.
		/// </summary>
		/// <param name="owner">
		/// The <c>MIPSEmulator.Processor</c> that owns the new <c>DataMemory</c>.
		/// </param>
		#endregion
		internal DataMemory(Processor owner)
		{
			data = new SortedDictionary<uint, uint>();
			this.owner = owner;
		}
		#endregion
		#endregion
	}
}
