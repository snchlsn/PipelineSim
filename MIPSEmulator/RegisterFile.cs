#region Using Directives
using System;
using System.ComponentModel;
using MIPSEmulator.Assembly;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// Represents a register file in a MIPS processor.
		/// </summary>
		#endregion
		public sealed class RegisterFile: ProcessorComponent
		{
			#region Fields
			private uint[] registers = new uint[31];
			#endregion
			
			#region Properties and Indexers
			#region XML Header
			/// <summary>
			/// Gets the value of a register.
			/// </summary>
			/// <param name="register">
			/// A <c>MIPSEmulator.Assembly.Register</c> value specifying the register to read.
			/// </param>
			/// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
			/// <paramref name="register"/> is not one of the defined
			/// <c>MIPSEmulator.Assembly.Register</c> values.
			/// </exception>
			#endregion
			public uint this[Register register]
			{
				get
				{
					if (!Enum.IsDefined(typeof(Register), register))
						throw new InvalidEnumArgumentException("register", (int)register, typeof(Register));
					
					if (register != Register.Zero)
						return registers[(int)register - 1];
					
					return 0;
				}
				private set
				{
					if (register != Register.Zero)
						registers[(int)register - 1] = value;
				}
			}
			#endregion
			
			#region Events
			#region XML Header
			/// <summary>
			/// Occurs when a value is written to a register, regardless of whether that value differs from
			/// the old.
			/// </summary>
			#endregion
			public event RegisterAccessEventHandler RegisterWritten;
			#endregion
			
			#region Methods
			#region Event Raisers
			#region XML Header
			/// <summary>
			/// Raises the <c>RegisterWritten</c> event.
			/// </summary>
			/// <param name="e">
			/// A <c>MIPSEmulator.RegisterAccessEventArgs</c> providing information about the event.
			/// </param>
			#endregion
			private void OnRegisterWritten(RegisterAccessEventArgs e)
			{
				if (RegisterWritten != null)
					RegisterWritten(this, e);
				return;
			}
			#endregion
			
			#region Event Handlers
			private void Write(object sender, EventArgs e)
			{
				MemoryStageRegister memoryRegister = Owner.MemoryRegister;
				
				if (memoryRegister.Signals.HasFlag(ControlSignals.RegWrite))
				{
					Register destinationRegister = memoryRegister.DestinationRegister;
					
					this[destinationRegister] = (memoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? memoryRegister.ReadValue : memoryRegister.ALUResult);
					OnRegisterWritten(new RegisterAccessEventArgs(destinationRegister));
				}
				return;
			}
			#endregion
			
			#region XML Header
			/// <summary>
			/// Resets all register values to <c>0</c>.
			/// </summary>
			#endregion
			internal void Reset()
			{
				registers.Initialize();
				return;
			}
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>RegisterFile</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>RegisterFile</c> is a component.
			/// </param>
			#endregion
			internal RegisterFile(Processor owner): base(owner)
			{
				owner.ExecutionStarted += (sender, e) => { SystemClock.EdgeRising += Write; };
				owner.ExecutionStopped += (sender, e) => { SystemClock.EdgeRising -= Write; };
			}
			#endregion
			#endregion
		}
	}
}
