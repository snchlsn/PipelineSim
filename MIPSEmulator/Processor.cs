#region Using Directives
using System;
using System.ComponentModel;
using MIPSEmulator.Assembly;
#endregion

namespace MIPSEmulator
{
	//TODO: Refactor combinational logic properties as methods?
	
	#region XML Header
	/// <summary>
	/// A simplified virtual pipelined MIPS processor.
	/// </summary>
	/// <remarks>
	/// Data is clocked on the positive edge; control is clocked on the negative edge.
	/// </remarks>
	#endregion
	public partial class Processor
	{
		#region Fields
		#region Hardware Components
		private ArithmeticLogicUnit alu;
		private ControlUnit control;
		private DataMemorySynchronizer dataSynchronizer;
		private DecodeStageRegister decodeRegister;
		private ExecuteStageRegister executeRegister;
		private FetchStageRegister fetchRegister;
		private MemoryStageRegister memoryRegister;
		private ProgramCounter pc;
		private ProgramMemorySynchronizer programSynchronizer;
		private RegisterFile registers;
		#endregion
		
		private HazardMode mode;
		private bool failed = false;
		private bool checkingForEnd = false;
		private bool programHasFinished = false;
		#endregion
		
		#region Properties
		#region Hardware Components
		#region XML Header
		/// <summary>
		/// Gets the processor's arithmetic logic unit.
		/// </summary>
		/// <value>The <c>Processor.ArithmeticLogicUnit</c> used by the <c>Processor</c>.</value>
		#endregion
		public ArithmeticLogicUnit ALU
		{
			get { return alu; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the processor's control unit.
		/// </summary>
		/// <value>The <c>Processor.ControlUnit</c> used by the <c>Processor</c>.</value>
		#endregion
		public ControlUnit Control
		{
			get { return control; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the processor's synchronous interface to its data memory.
		/// </summary>
		/// <value>The <c>Processor.DataMemorySynchronizer</c> used by the <c>Processor</c>.</value>
		#endregion
		public DataMemorySynchronizer DataSynchronizer
		{
			get { return dataSynchronizer; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the register separating the processor's decode stage from its execute stage.
		/// </summary>
		/// <value>The <c>Processor.DecodeStageRegister</c> used by the <c>Processor</c>.</value>
		#endregion
		public DecodeStageRegister DecodeRegister
		{
			get { return decodeRegister; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the register separating the processor's execute stage from its memory stage.
		/// </summary>
		/// <value>The <c>Processor.ExecuteStageRegister</c> used by the <c>Processor</c>.</value>
		#endregion
		public ExecuteStageRegister ExecuteRegister
		{
			get { return executeRegister; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the register separating the processor's fetch stage from its decode stage.
		/// </summary>
		/// <value>The <c>Processor.FetchStageRegister</c> used by the <c>Processor</c>.</value>
		#endregion
		public FetchStageRegister FetchRegister
		{
			get { return fetchRegister; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the register separating the processor's memory stage from its writeback stage.
		/// </summary>
		/// <value>The <c>Processor.MemoryStageRegister</c> used by the <c>Processor</c>.</value>
		#endregion
		public MemoryStageRegister MemoryRegister
		{
			get { return memoryRegister; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the processor's program counter.
		/// </summary>
		/// <value>The <c>Processor.ProgramCounter</c> used by the <c>Processor</c>.</value>
		#endregion
		public ProgramCounter PC
		{
			get { return pc; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the processor's synchronous interface to its program memory.
		/// </summary>
		/// <value>The <c>Processor.ProgramMemorySynchronizer</c> used by the <c>Processor</c>.</value>
		#endregion
		public ProgramMemorySynchronizer ProgramSynchronizer
		{
			get { return programSynchronizer; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the processor's register file.
		/// </summary>
		/// <value>The <c>Processor.RegisterFile</c> used by the <c>Processor</c>.</value>
		#endregion
		public RegisterFile Registers
		{
			get { return registers; }
		}
		#endregion
		
		public DataMemory Data
		{
			get { return dataSynchronizer.Data; }
		}
		
		public ProgramMemory Program
		{
			get { return programSynchronizer.Program; }
			set
			{
				bool programWasNull = (Program == null);
				
				programSynchronizer.Program = value;
				OnProgramLoaded(EventArgs.Empty);
				
				if (programWasNull)
				{
					if (Program != null)
						OnExecutionStarted(EventArgs.Empty);
				}
				else
					Reset();
			}
		}
		
		private bool CheckingForEnd
		{
			get { return checkingForEnd; }
			set
			{
				if (checkingForEnd != value)
				{
					if (value)
					{
						ControlPathUpdated += CheckForEndOfProgram;
						programSynchronizer.OutOfBoundsAccess -= StartCheckingForEnd;
					}
					else
					{
						ControlPathUpdated -= CheckForEndOfProgram;
						programSynchronizer.OutOfBoundsAccess += StartCheckingForEnd;
					}
					checkingForEnd = value;
				}
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets a value indicating whether the <c>Processor</c> has encountered an error condition.
		/// </summary>
		/// <value>
		/// <c>true</c> if an error has occurred; <c>false</c> otherwise.
		/// </value>
		#endregion
		public bool Failed
		{
			get { return failed; }
			private set
			{
				if (failed != value)
				{
					failed = value;
					if (Mode == HazardMode.Fail)
					{
						if (failed)
							ControlPathUpdated -= CheckForFailure;
						else
							ControlPathUpdated += CheckForFailure;
					}
				}
			}
		}
		
		public bool IsRunning
		{
			get { return !(Program == null || failed || programHasFinished); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets a value determining the <c>Processor</c>'s capabilities in dealing with hazards.
		/// </summary>
		/// <value>
		/// One of the <c>MIPSEmulator.HazardMode</c> values.
		/// </value>
		/// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
		/// The assigned value is not one of the defined <c>MIPSEmulator.HazardMode</c> values.
		/// </exception>
		#endregion
		public HazardMode Mode
		{
			get { return mode; }
			set
			{
				if (!Enum.IsDefined(typeof(HazardMode), value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(HazardMode));
				
				if (mode != value)
				{
					if (mode == HazardMode.Fail)
						ControlPathUpdated -= CheckForFailure;
					
					mode = value;
					
					if (mode == HazardMode.Fail)
						ControlPathUpdated += CheckForFailure;
					
					OnModeChanged(EventArgs.Empty);
				}
			}
		}
		
		public bool ProgramHasFinished
		{
			get { return programHasFinished; }
			private set
			{
				if (programHasFinished != value)
				{
					programHasFinished = value;
					if (programHasFinished)
					{
						OnExecutionStopped(EventArgs.Empty);
						OnProgramFinished(EventArgs.Empty);
					}
				}
			}
		}
		#endregion
		
		#region Events
		#region XML Header
		/// <summary>
		/// Occurs on the negative edge of the clock, after all state changes have been processed.
		/// </summary>
		#endregion
		public event EventHandler ControlPathUpdated;
		
		#region XML Header
		/// <summary>
		/// Occurs on the positive edge of the clock, after all state changes have been processed.
		/// </summary>
		#endregion
		public event EventHandler DataPathUpdated;
		
		#region XML Header
		/// <summary>
		/// Occurs when the <c>Processor</c> begins executing a program.
		/// </summary>
		#endregion
		public event EventHandler ExecutionStarted;
		
		#region XML Header
		/// <summary>
		/// Occurs when the <c>Processor</c> stops executing a program, whether the end was reached,
		/// the <c>Processor</c> was reset while running, or a failure condition was encountered.
		/// </summary>
		#endregion
		public event EventHandler ExecutionStopped;
		
		#region XML Header
		/// <summary>
		/// Occurs when a failure condition is encountered.
		/// </summary>
		#endregion
		public event HazardEventHandler Failure;
		
		#region XML Header
		/// <summary>
		/// Occurs when the <c>Mode</c> property is changed.
		/// </summary>
		#endregion
		public event EventHandler ModeChanged;
		
		#region XML Header
		/// <summary>
		/// Occurs when the last instruction in the running program finishes executing.
		/// </summary>
		#endregion
		public event EventHandler ProgramFinished;
		
		#region XML Header
		/// <summary>
		/// Occurs when the <c>Program</c> property is assigned, whether the value has changed or not.
		/// </summary>
		#endregion
		public event EventHandler ProgramLoaded;
		#endregion
		
		#region Methods
		#region Event Raisers
		#region XML Header
		/// <summary>
		/// Raises the <c>ControlPathUpdated</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnControlPathUpdated(EventArgs e)
		{
			if (ControlPathUpdated != null)
				ControlPathUpdated(this, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>DataPathUpdated</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnDataPathUpdated(EventArgs e)
		{
			if (DataPathUpdated != null)
				DataPathUpdated(this, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>ExecutionStarted</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnExecutionStarted(EventArgs e)
		{
			if (ExecutionStarted != null)
				ExecutionStarted(this, e);
			
			SystemClock.EdgeFallen += RaiseStateChangeEvent;
			SystemClock.EdgeRisen += RaiseStateChangeEvent;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>ExecutionStopped</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnExecutionStopped(EventArgs e)
		{
			SystemClock.EdgeFallen -= RaiseStateChangeEvent;
			SystemClock.EdgeRisen -= RaiseStateChangeEvent;
			
			if (ExecutionStopped != null)
				ExecutionStopped(this, e);
			
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>Failure</c> event.
		/// </summary>
		/// <param name="e">
		/// A <c>MIPSEmulator.HazardEventArgs</c> that provides information about the event.
		/// </param>
		#endregion
		private void OnFailure(HazardEventArgs e)
		{
			if (Failure != null)
				Failure(this, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>ModeChanged</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnModeChanged(EventArgs e)
		{
			if (ModeChanged != null)
				ModeChanged(this, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>ProgramFinished</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnProgramFinished(EventArgs e)
		{
			if (ProgramFinished != null)
				ProgramFinished(this, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>ProgramLoaded</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnProgramLoaded(EventArgs e)
		{
			if (ProgramLoaded != null)
				ProgramLoaded(this, e);
			return;
		}
		#endregion
		
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Checks whether the end of the program has been reached, and raises the <c>ProgramFinished</c>
		/// event if it has.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void CheckForEndOfProgram(object sender, EventArgs e)
		{
			if (memoryRegister.HasBubble && executeRegister.HasBubble && decodeRegister.HasBubble && fetchRegister.HasBubble)
			{
				CheckingForEnd = false;
				ProgramHasFinished = true;
			}
			
			return;
		}
		
		private void CheckForFailure(object sender, EventArgs e)
		{
			HazardTypes hazardType = control.CheckHazard();
			
			if (hazardType != HazardTypes.None)
			{
				Failed = true;
				OnExecutionStopped(EventArgs.Empty);
				OnFailure(new HazardEventArgs(hazardType));
			}
			return;
		}
		
		private void RaiseStateChangeEvent(object sender, EventArgs e)
		{
			if (SystemClock.Level)
				OnDataPathUpdated(EventArgs.Empty);
			else
				OnControlPathUpdated(EventArgs.Empty);
			
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Causes the <c>Processor</c> to start watching for the end of the executing program.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void StartCheckingForEnd(object sender, EventArgs e)
		{
			CheckingForEnd = true;
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Resets all clocked components, sets <c>Failed</c> and <c>ProgramHasFinished</c> to false,
		/// clears data memory, and sets the system clock level to low.
		/// </summary>
		#endregion
		public void Reset()
		{
			if (IsRunning)
				OnExecutionStopped(EventArgs.Empty);
			
			registers.Reset();
			pc.Reset();
			programSynchronizer.Reset();
			fetchRegister.Reset();
			decodeRegister.Reset();
			executeRegister.Reset();
			memoryRegister.Reset();
			Data.Clear();
			Failed = false;
			CheckingForEnd = false;
			ProgramHasFinished = false;
			
			SystemClock.Reset();
			
			if (Program != null)
				OnExecutionStarted(EventArgs.Empty);
			
			return;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>Processor</c> instance.
		/// </summary>
		#endregion
		public Processor()
		{
			alu = new ArithmeticLogicUnit(this);
			control = new ControlUnit(this);
			dataSynchronizer = new DataMemorySynchronizer(this);
			dataSynchronizer.Data = new DataMemory(this);
			decodeRegister = new DecodeStageRegister(this);
			executeRegister = new ExecuteStageRegister(this);
			fetchRegister = new FetchStageRegister(this);
			memoryRegister = new MemoryStageRegister(this);
			pc = new ProgramCounter(this);
			programSynchronizer = new ProgramMemorySynchronizer(this);
			registers = new RegisterFile(this);
			
			mode = HazardMode.StallAndForward;
			
			programSynchronizer.OutOfBoundsAccess += StartCheckingForEnd;
		}
		#endregion
		#endregion
	}
}
