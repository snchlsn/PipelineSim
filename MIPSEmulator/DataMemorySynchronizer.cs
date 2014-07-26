#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		public sealed class DataMemorySynchronizer: ClockedComponent
		{
			#region Fields
			private DataMemory data;
			private uint readValue, nextReadValue;
			#endregion
			
			#region Properties
			public DataMemory Data
			{
				get { return data; }
				internal set { data = value; }
			}
			
			public uint ReadValue
			{
				get { return readValue; }
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				if (readValue != nextReadValue)
				{
					readValue = nextReadValue;
					OnStateChanged(EventArgs.Empty);
				}
				return;
			}
			
			protected override void ChangeState(object sender, EventArgs e)
			{
				if (data != null)
				{
					if (Owner.ExecuteRegister.Signals.HasFlag(ControlSignals.MemWrite))
						data.StoreWord(Owner.ExecuteRegister.ALUResult, Owner.ExecuteRegister.MemoryWriteValue);
					else if (Owner.ExecuteRegister.Signals.HasFlag(ControlSignals.MemRead))
						nextReadValue = data.LoadWord(Owner.ExecuteRegister.ALUResult);
				}
				return;
			}
			#endregion
			
			internal override void Reset()
			{
				//Nothing needs to be reset here.
				return;
			}

			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>DataMemorySynchronizer</c> instance belonging to the
			/// specified <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>DataMemorySynchronizer</c> is
			/// a component.
			/// </param>
			#endregion
			internal DataMemorySynchronizer(Processor owner): base(owner, true) {}
			#endregion
			#endregion
		}
	}
}
