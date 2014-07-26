#region Using Directives
using System;
using MIPSEmulator.Assembly;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// A register separating the memory stage of a MIPS processor from the writeback stage.
		/// </summary>
		#endregion
		public sealed class MemoryStageRegister: StageRegister
		{
			#region Fields
			private uint aluResult, nextALUResult;
			private Register destinationRegister, nextDestinationRegister;
			private uint readValue, nextReadValue;
			#endregion
			
			#region Properties
			public uint ALUResult
			{
				get { return aluResult; }
			}
			
			public Register DestinationRegister
			{
				get { return destinationRegister; }
			}
			
			#region XML Header
			/// <summary>
			/// Gets the latched value read from memory that is being passed to the writeback stage.
			/// </summary>
			/// <value>The latched value read from memory.</value>
			#endregion
			public uint ReadValue
			{
				get { return readValue; }
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			protected override void ChangeOutputs(object sender, EventArgs e)
			{
				readValue = nextReadValue;
				destinationRegister = nextDestinationRegister;
				aluResult = nextALUResult;
				
				base.ChangeOutputs(sender, e);
				return;
			}
			
			protected override void ChangeState(object sender, EventArgs e)
			{
				nextSignals = Owner.ExecuteRegister.Signals;
				nextInstructionAddress = Owner.ExecuteRegister.InstructionAddress;
				nextReadValue = Owner.DataSynchronizer.ReadValue;
				nextDestinationRegister = Owner.ExecuteRegister.DestinationRegister;
				nextALUResult = Owner.ExecuteRegister.ALUResult;
				return;
			}
			#endregion
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>MemoryStageRegister</c> instance belonging to the
			/// specfied <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>MemoryStageRegister</c> is a component.
			/// </param>
			#endregion
			internal MemoryStageRegister(Processor owner): base(owner) {}
			#endregion
			#endregion
		}
	}
}
