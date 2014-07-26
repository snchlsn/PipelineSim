#region Using Directives
using System;
using System.Drawing;
using System.Windows.Forms;
using MIPSEmulator;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		#region XML Header
		/// <summary>
		/// Monitors the state of a <c>MIPSEmulator.Processor</c> and displays a block diagram
		/// representing it.
		/// </summary>
		#endregion
		private partial class ProcessorView: UserControl
		{
			#region Fields
			#region Colors
			private static readonly Color focusedControlColor = Color.Red;
			private static readonly Color unfocusedControlColor = Color.FromArgb(120, focusedControlColor);
			private static readonly Color focusedActiveDataColor = Color.DarkGreen;
			private static readonly Color unfocusedActiveDataColor = Color.FromArgb(140, focusedActiveDataColor);
			private static readonly Color focusedInactiveDataColor = Color.Black;
			private static readonly Color unfocusedInactiveDataColor = Color.FromArgb(128, focusedInactiveDataColor);
			#endregion
			
			#region Tool Tip Delays
			private const int toolTipInitialDelay = 10;
			private const int toolTipReshowDelay = 10;
			#endregion
			
			private Processor processor;
			
			#region Component Views
			private SignalsView signalsView;
			private DecodeRegisterView decodeRegisterView;
			private ExecuteRegisterView executeRegisterView;
			private FetchRegisterView fetchRegisterView;
			private MemoryRegisterView memoryRegisterView;
			private ProgramCounterView pcView;
			private RegisterFileView registerFileView;
			private ProgramMemoryView programMemoryView;
			private DataMemoryView dataMemoryView;
			private ControlUnitView controlUnitView;
			private ForwardUnitView forwardUnitView;
			private MuxView[] forwardMuxViews;
			private MuxView pcMuxView, aluMuxView, destinationMuxView, writebackMuxView;
			private ArithmeticBlockView pcAdderView, branchAdderView, aluView;
			private CombinationalLogicBlockView signExtView, targetShifterView, immediateShifterView;
			#endregion
			
			private Label pcAddConstLabel;
			#endregion
			
			#region Properties
			public Processor Processor
			{
				get { return processor; }
				private set
				{
					processor = value;
					processor.ModeChanged += ChangeHazardMode;
					
					signalsView.Processor = processor;
					pcView.ProgramCounter = processor.PC;
					programMemoryView.ProgramMemorySynchronizer = processor.ProgramSynchronizer;
					fetchRegisterView.StageRegister = processor.FetchRegister;
					registerFileView.RegisterFile = processor.Registers;
					decodeRegisterView.StageRegister = processor.DecodeRegister;
					executeRegisterView.StageRegister = processor.ExecuteRegister;
					dataMemoryView.DataMemorySynchronizer = processor.DataSynchronizer;
					memoryRegisterView.StageRegister = processor.MemoryRegister;
					controlUnitView.Control = processor.Control;
					forwardUnitView.Control = processor.Control;
				}
			}
			#endregion
			
			#region Methods
			#region Event Handlers
			private void ChangeHazardMode(object sender, EventArgs e)
			{
				forwardMuxViews[0].Visible = (processor.Mode == HazardMode.StallAndForward);
				forwardMuxViews[1].Visible = (processor.Mode == HazardMode.StallAndForward);
				forwardUnitView.Visible = (processor.Mode == HazardMode.StallAndForward);
				Invalidate();
				return;
			}
			#endregion
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>ProcessorView</c> instance.
			/// </summary>
			/// <param name="processor">The <c>MIPSEmulator.Processor</c> to monitor.</param>
			#endregion
			public ProcessorView(Processor processor)
			{
				AutoScroll = true;
				
				#region Fetch Stage
				pcAdderView = new ArithmeticBlockView();
				pcAdderView.Orientation = Orientation.Horizontal;
				pcAdderView.Left = 25;
				pcAdderView.Top = 75;
				Controls.Add(pcAdderView);
				
				pcAddConstLabel = new Label();
				pcAddConstLabel.AutoSize = true;
				pcAddConstLabel.Text = "4";
				pcAddConstLabel.Top = pcAdderView.Top - pcAddConstLabel.Height - 10;
				pcAddConstLabel.Left = pcAdderView.Left + pcAdderView.Width / 5 - 5;
				Controls.Add(pcAddConstLabel);
				
				pcMuxView = new MuxView();
				pcMuxView.Orientation = Orientation.Horizontal;
				pcMuxView.Left = pcAdderView.Left;
				pcMuxView.Top = pcAdderView.Bottom + 35;
				Controls.Add(pcMuxView);
				
				pcView = new ProgramCounterView();
				pcView.Left = 15;
				pcView.Top = pcMuxView.Bottom + 20;
				Controls.Add(pcView);
				
				programMemoryView = new ProgramMemoryView();
				programMemoryView.Left = pcView.Left;
				programMemoryView.Top = pcView.Bottom + 30;
				Controls.Add(programMemoryView);
				
				fetchRegisterView = new FetchRegisterView();
				fetchRegisterView.Top = 60;
				fetchRegisterView.Left = programMemoryView.Right + 30;
				Controls.Add(fetchRegisterView);
				#endregion
				
				#region Decode Stage
				controlUnitView = new ControlUnitView();
				controlUnitView.Left = fetchRegisterView.Right + 70;
				controlUnitView.Top = 25;
				Controls.Add(controlUnitView);
				
				registerFileView = new RegisterFileView();
				registerFileView.Left = controlUnitView.Left + 10;
				registerFileView.Top = controlUnitView.Bottom + 60;
				Controls.Add(registerFileView);
				
				targetShifterView = new CombinationalLogicBlockView();
				targetShifterView.Text = "<<2";
				targetShifterView.Top = registerFileView.Top - 15;
				targetShifterView.Left = fetchRegisterView.Right + 15;
				Controls.Add(targetShifterView);
				
				signExtView = new CombinationalLogicBlockView();
				signExtView.Text = "Sign\nExt";
				signExtView.Left = registerFileView.Left + 50;
				signExtView.Top = registerFileView.Bottom + 5;
				Controls.Add(signExtView);
				
				decodeRegisterView = new DecodeRegisterView();
				decodeRegisterView.Top = fetchRegisterView.Top;
				decodeRegisterView.Left = registerFileView.Right + 50;
				Controls.Add(decodeRegisterView);
				#endregion
				
				#region Execute Stage
				forwardMuxViews = new MuxView[2];
				forwardMuxViews[0] = new MuxView();
				forwardMuxViews[0].Left = decodeRegisterView.Right + 30;
				forwardMuxViews[0].Top = decodeRegisterView.Top + 110;
				forwardMuxViews[0].Orientation = Orientation.Vertical;
				forwardMuxViews[1] = new MuxView();
				forwardMuxViews[1].Left = forwardMuxViews[0].Left;
				forwardMuxViews[1].Top = forwardMuxViews[0].Bottom + 20;
				forwardMuxViews[1].Orientation = Orientation.Vertical;
				Controls.AddRange(forwardMuxViews);
				
				immediateShifterView = new CombinationalLogicBlockView();
				immediateShifterView.Text = "<<2";
				immediateShifterView.Left = forwardMuxViews[0].Right + 40;
				immediateShifterView.Top = decodeRegisterView.Top + 85;
				Controls.Add(immediateShifterView);
				
				aluMuxView = new MuxView();
				aluMuxView.Left = immediateShifterView.Left + 5;
				aluMuxView.Top = forwardMuxViews[1].Top + 10;
				aluMuxView.Orientation = Orientation.Vertical;
				Controls.Add(aluMuxView);
				
				destinationMuxView = new MuxView();
				destinationMuxView.Left = aluMuxView.Left;
				destinationMuxView.Top = forwardMuxViews[1].Bottom + 35;
				destinationMuxView.Orientation = Orientation.Vertical;
				Controls.Add(destinationMuxView);
				
				forwardUnitView = new ForwardUnitView();
				forwardUnitView.Top = decodeRegisterView.Bottom + 40;
				forwardUnitView.Left = decodeRegisterView.Right + 55;
				Controls.Add(forwardUnitView);
				
				branchAdderView = new ArithmeticBlockView();
				branchAdderView.Orientation = Orientation.Vertical;
				branchAdderView.Left = immediateShifterView.Right + 20;
				branchAdderView.Top = decodeRegisterView.Top + 40;
				Controls.Add(branchAdderView);
				
				aluView = new ArithmeticBlockView();
				aluView.Orientation = Orientation.Vertical;
				aluView.Text = "ALU";
				aluView.Left = branchAdderView.Left;
				aluView.Top = branchAdderView.Bottom + 15;
				Controls.Add(aluView);
				
				executeRegisterView = new ExecuteRegisterView();
				executeRegisterView.Top = decodeRegisterView.Top;
				executeRegisterView.Left = aluView.Right + 30;
				Controls.Add(executeRegisterView);
				#endregion
				
				#region Memory Stage
				dataMemoryView = new DataMemoryView();
				dataMemoryView.Left = executeRegisterView.Right + 30;
				dataMemoryView.Top = aluView.Top + 15;
				Controls.Add(dataMemoryView);
				
				memoryRegisterView = new MemoryRegisterView();
				memoryRegisterView.Top = executeRegisterView.Top;
				memoryRegisterView.Left = dataMemoryView.Right + 30;
				Controls.Add(memoryRegisterView);
				#endregion
				
				#region Writeback Stage
				writebackMuxView = new MuxView();
				writebackMuxView.Left = memoryRegisterView.Right + 20;
				writebackMuxView.Orientation = Orientation.Vertical;
				writebackMuxView.Top = dataMemoryView.Bottom - writebackMuxView.Height;
				Controls.Add(writebackMuxView);
				#endregion
				
				signalsView = new SignalsView(this);
				signalsView.Dock = DockStyle.Fill;
				Controls.Add(signalsView);
				
				Processor = processor;
			}
			#endregion
			#endregion
		}
	}
}
