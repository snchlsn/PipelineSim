#region Using Directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using MIPSEmulator;
using MIPSEmulator.Assembly;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			#region XML Header
			/// <summary>
			/// The <c>Control</c> that draws all the signals in the <c>ProcessorView</c>.
			/// </summary>
			#endregion
			protected class SignalsView: Control
			{
				#region Fields
				private Processor processor;
				private Signal[] signals, muxInputs;
				private Signal focusedSignal;
				
				private ToolTip toolTip;
				
				private const byte arrowHeight = 6;
				private const byte halfArrowWidth = 4;
				#endregion
				
				#region Properties
				public Processor Processor
				{
					get { return processor; }
					set
					{
						if (processor != value)
						{
							if (processor != null)
							{
								processor.ControlPathUpdated -= UpdateMuxInputs;
								processor.ModeChanged -= Invalidate;
							}
							
							processor = value;
							
							if (processor != null)
							{
								UpdateMuxInputs();
								processor.ControlPathUpdated += UpdateMuxInputs;
								processor.ModeChanged += Invalidate;
							}
						}
					}
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void Invalidate(object sender, EventArgs e)
				{
					Invalidate();
					return;
				}
				
				#region XML Header
				/// <summary>
				/// Paints each <c>Signal</c> included in the current hazard mode.
				/// </summary>
				/// <param name="sender">The <c>object</c> that raised the event.</param>
				/// <param name="e">
				/// A <c>System.Windows.Forms.PaintEventArgs</c> that provides data for the event.
				/// </param>
				#endregion
				private void PaintSignals(object sender, PaintEventArgs e)
				{
					byte vertex_set = (byte)Processor.Mode;
					Pen pen = new Pen(Color.Black);
					
					foreach (Signal signal in signals)
						if (signal != null && signal.Vertices.Length > vertex_set && signal.Vertices[vertex_set] != null)
						{
							pen.Color = signal.Color;
							e.Graphics.DrawLines(pen, signal.Vertices[vertex_set]);
							if (!signal.IsBus)
							{
								Point[] arrow = new Point[3];
								Point[] lastLineSeg = new Point[2];
								
								lastLineSeg[0] = signal.Vertices[vertex_set][signal.Vertices[vertex_set].Length - 2];
								lastLineSeg[1] = signal.Vertices[vertex_set][signal.Vertices[vertex_set].Length - 1];
								arrow[2] = lastLineSeg[1];
								if (lastLineSeg[0].X == lastLineSeg[1].X)
								{
									if (lastLineSeg[1].Y > lastLineSeg[0].Y)
									{
										arrow[0] = new Point(lastLineSeg[1].X - halfArrowWidth, lastLineSeg[1].Y - arrowHeight);
										arrow[1] = new Point(lastLineSeg[1].X + halfArrowWidth, lastLineSeg[1].Y - arrowHeight);
									}
									else
									{
										arrow[0] = new Point(lastLineSeg[1].X - halfArrowWidth, lastLineSeg[1].Y + arrowHeight);
										arrow[1] = new Point(lastLineSeg[1].X + halfArrowWidth, lastLineSeg[1].Y + arrowHeight);
									}
								}
								else
								{
									if (lastLineSeg[1].X > lastLineSeg[0].X)
									{
										arrow[0] = new Point(lastLineSeg[1].X - arrowHeight, lastLineSeg[1].Y - halfArrowWidth);
										arrow[1] = new Point(lastLineSeg[1].X - arrowHeight, lastLineSeg[1].Y + halfArrowWidth);
									}
									else
									{
										arrow[0] = new Point(lastLineSeg[1].X + arrowHeight, lastLineSeg[1].Y - halfArrowWidth);
										arrow[1] = new Point(lastLineSeg[1].X + arrowHeight, lastLineSeg[1].Y + halfArrowWidth);
									}
								}
								e.Graphics.FillPolygon(new SolidBrush(pen.Color), arrow);
							}
						}
					
					return;
				}
				
				private void SetToolTip(object sender, MouseEventArgs e)
				{
					byte vertex_set = (byte)Processor.Mode;
					int x0 = e.X;
					int y0 = e.Y;
					
					foreach (Signal signal in signals)
					{
						//If the signal is included in the current hazard mode
						if (signal != null && signal.Vertices.Length > vertex_set && signal.Vertices[vertex_set] != null)
							for (byte i = 0; i < signal.Vertices[vertex_set].Length - 1; ++i)
							{
								int x1, y1, x2, y2;
								
								if (signal.Vertices[vertex_set][i].X < signal.Vertices[vertex_set][i + 1].X)
								{
									x1 = signal.Vertices[vertex_set][i].X;
									x2 = signal.Vertices[vertex_set][i + 1].X;
								}
								else
								{
									x1 = signal.Vertices[vertex_set][i + 1].X;
									x2 = signal.Vertices[vertex_set][i].X;
								}
								
								if (signal.Vertices[vertex_set][i].Y < signal.Vertices[vertex_set][i + 1].Y)
								{
									y1 = signal.Vertices[vertex_set][i].Y;
									y2 = signal.Vertices[vertex_set][i + 1].Y;
								}
								else
								{
									y1 = signal.Vertices[vertex_set][i + 1].Y;
									y2 = signal.Vertices[vertex_set][i].Y;
								}
								
								//Optimize calcualation with a bounding box check
								if (new Rectangle(x1 - 3, y1 - 3, x2 - x1 + 7, y2 - y1 + 7).Contains(x0, y0))
								{
									int len_sq = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
									float t = ((x0 - x1) * (x2 - x1) + (y0 - y1) * (y2 - y1)) / (float)len_sq;
									
									//If the distance from the mouse to the signal is no greater than 3, have it set the tool tip.
									if ((t < 0.0f ? Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)) : (t > 1.0f ? Math.Sqrt((x2 - x0) * (x2 - x0) + (y2 - y0) * (y2 - y0)) : Math.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)) / Math.Sqrt(len_sq))) <= 3.0f)
									{
										if (signal != focusedSignal)
										{
											if (focusedSignal != null)
											{
												focusedSignal.HasFocus = false;
												if (focusedSignal.ConnectedSignals != null)
													foreach (Signal connectedSignal in focusedSignal.ConnectedSignals)
														connectedSignal.HasFocus = false;
											}
											
											signal.Update();
											signal.SetToolTip(toolTip, this);
											signal.HasFocus = true;
											focusedSignal = signal;
											if (signal.ConnectedSignals != null)
												foreach (Signal connectedSignal in signal.ConnectedSignals)
													connectedSignal.HasFocus = true;
											
											Invalidate();
										}
										return;
									}
								}
							}
					}
					
					if (focusedSignal != null)
					{
						toolTip.RemoveAll();
						focusedSignal.HasFocus = false;
						if (focusedSignal.ConnectedSignals != null)
							foreach (Signal connectedSignal in focusedSignal.ConnectedSignals)
								connectedSignal.HasFocus = false;
						
						focusedSignal = null;
						Invalidate();
					}
					return;
				}
				
				#region XML Header
				/// <summary>
				/// Updates <c>Signal</c>s whose <c>IsActive</c> property is meaningful (i.e., those
				/// that feed into multiplexers).
				/// </summary>
				/// <param name="sender">
				/// The <c>object</c> that raised the event.  The default is <c>null</c>.
				/// </param>
				/// <param name="e">A <c>System.EventArgs</c>.  The default is <c>null</c>.</param>
				#endregion
				private void UpdateMuxInputs(object sender = null, EventArgs e = null)
				{
					bool wasActive;
					bool invalidate = false;
					
					foreach (Signal signal in muxInputs)
						if (signal != null)
						{
							wasActive = signal.IsActive;
							signal.Update();
							invalidate = invalidate || (wasActive ^ signal.IsActive);
						}
					
					if (invalidate)
						Invalidate();
					
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>SignalsView</c> instance.
				/// </summary>
				#endregion
				public SignalsView(ProcessorView owner)
				{
					signals = new Signal[100]; //TODO: Adjust length.
					muxInputs = new Signal[20];
					byte index = 0, muxIndex = 0;
					Point[][] vertices;
					Signal refSignal;
					
					DoubleBuffered = true;
					
					#region Signals
					#region Fetch Stage
					#region 4 to PC adder
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.pcAddConstLabel.Left, owner.pcAddConstLabel.Bottom);
					vertices[0][1] = new Point(vertices[0][0].X, owner.pcAdderView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index] = new Signal("Constant", 3, false, false, HazardMode.Fail, vertices,
					                            (signal) => {});
					signals[index++].Value = 4;
					#endregion
					
					#region PC mux to PC
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.pcMuxView.Left + (owner.pcMuxView.Width >> 1), owner.pcMuxView.Bottom);
					vertices[0][1] = new Point(vertices[0][0].X, owner.pcView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Address", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) =>	{
					                              					switch (processor.Control.PCSource)
					                              					{
					                              						case PCSource.Branch:
					                              							signal.Value = (uint)(processor.DecodeRegister.FollowingInstructionAddress + ((int)processor.DecodeRegister.Instruction.Immediate << 2));
					                              							break;
					                              						
					                              						case PCSource.Jump:
					                              							signal.Value = ((Instruction)processor.FetchRegister.Instruction).Target;
					                              							break;
					                              						
					                              						default:
					                              							signal.Value = processor.PC.Address + 4;
					                              							break;
					                              					}
					                              				}
					                             );
					#endregion
					
					#region PC bus
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.pcView.Left + (owner.pcView.Width >> 1), owner.pcView.Bottom);
					vertices[0][1] = new Point(vertices[0][0].X, owner.programMemoryView.Top - ((owner.programMemoryView.Top - vertices[0][0].Y) >> 1));
					vertices[2] = vertices[1] = vertices[0];
					signals[index] = new Signal("Address", 32, false, true, HazardMode.Fail, vertices,
					                            (signal) => { signal.Value = processor.PC.Address; });
					signals[index++].ConnectedSignals = new Signal[2];
					#endregion
					
					#region PC bus to program memory
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.programMemoryView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					refSignal.ConnectedSignals[0] = signals[index];
					signals[index].ConnectedSignals = new Signal[1];
					signals[index++].ConnectedSignals[0] = refSignal;
					#endregion
					
					#region PC bus to PC adder
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[5];
					vertices[0][0] = signals[index - 1].Vertices[0][0];
					vertices[0][1] = new Point(owner.pcAdderView.Right + 5, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.pcAdderView.Top - 10);
					vertices[0][3] = new Point(owner.pcAdderView.Right - (owner.pcAdderView.Width / 5), vertices[0][2].Y);
					vertices[0][4] = new Point(vertices[0][3].X, owner.pcAdderView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index] = new Signal(refSignal, vertices);
					signals[index].ConnectedSignals = refSignal.ConnectedSignals;
					refSignal = signals[index - 2];
					refSignal.ConnectedSignals[1] = signals[index++];
					#endregion
					
					#region PC adder bus
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.pcAdderView.Left + (owner.pcAdderView.Width >> 1), owner.pcAdderView.Bottom);
					vertices[0][1] = new Point(vertices[0][0].X, vertices[0][0].Y + (owner.pcMuxView.Top - (vertices[0][0].Y) >> 1));
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Following address", 32, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.PC.Address + 4; });
					#endregion
					
					#region PC adder bus to IF/ID (following address)
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.fetchRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region PC adder bus to PC mux (following address)
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][0];
					vertices[0][1] = new Point(vertices[0][0].X, owner.pcMuxView.Top);
					vertices[2] = vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal, vertices,
					                                                      (signal) => { signal.Value = processor.PC.Address + 4; signal.IsActive = (processor.Control.PCSource == PCSource.Next);});
					#endregion
					
					#region Program memory to IF/ID
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.programMemoryView.Right, owner.programMemoryView.Bottom - 25);
					vertices[0][1] = new Point(owner.fetchRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Instruction", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.ProgramSynchronizer.Instruction; });
					#endregion
					#endregion
					
					#region Decode Stage
					#region IF/ID to ID/EX (following address)
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.fetchRegisterView.Right, signals[index - 3].Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Following address", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.FetchRegister.FollowingInstructionAddress; });
					#endregion
					
					#region IF/ID to instruction bus
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(owner.fetchRegisterView.Right, signals[index - 2].Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.registerFileView.Left - 25, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.decodeRegisterView.Bottom - 35);
					vertices[2] = vertices[1] = vertices[0];
					signals[index] = new Signal("Instruction", 32, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.FetchRegister.Instruction; });
					refSignal = signals[index++];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = signals[index - 1].Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.registerFileView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices);
					#endregion
					
					#region Instruction bus to control unit
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = signals[index - 1].Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.controlUnitView.Bottom - 10);
					vertices[0][2] = new Point(owner.controlUnitView.Left, vertices[0][1].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Op, Rs, Rt, Func", 22, false, false, HazardMode.Fail, vertices,
					                              (signal) =>	{
																	Instruction instruction = (Instruction)processor.FetchRegister.Instruction;
																	
																	signal.Value = ((uint)instruction.Opcode << 16) | ((uint)instruction.RS << 11) | ((uint)instruction.RT << 6) | (uint)instruction.Function;
																});
					#endregion
					
					#region Instruction bus to target shifter
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(signals[index - 1].Vertices[0][0].X, owner.targetShifterView.Bottom + 10);
					vertices[0][1] = new Point(owner.targetShifterView.Left + (owner.targetShifterView.Width >> 1), vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.targetShifterView.Bottom);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Jump Target", 26, false, false, HazardMode.Fail, vertices,
					                               (signal) => { signal.Value = ((Instruction)processor.FetchRegister.Instruction).Target >> 2; });
					#endregion
					
					#region Target shifter to PC mux
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[6];
					vertices[0][0] = new Point(refSignal.Vertices[0][2].X, owner.targetShifterView.Top);
					vertices[0][1] = new Point(vertices[0][0].X, owner.controlUnitView.Top + 10);
					vertices[0][2] = new Point(owner.pcAdderView.Left - 5, vertices[0][1].Y);
					vertices[0][3] = new Point(vertices[0][2].X, owner.pcAdderView.Bottom + 10);
					vertices[0][4] = new Point(owner.pcMuxView.Left + 25, vertices[0][3].Y);
					vertices[0][5] = new Point(vertices[0][4].X, owner.pcMuxView.Top);
					vertices[2] = vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices,
					                                                      (signal) => { signal.Value = ((Instruction)processor.FetchRegister.Instruction).Target; signal.IsActive = (processor.Control.PCSource == PCSource.Jump); });
					#endregion
					
					#region Instruction bus to register file (Rs)
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(signals[index - 2].Vertices[0][0].X, owner.registerFileView.Top + 10);
					vertices[0][1] = new Point(owner.registerFileView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Rs", 5, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((Instruction)processor.FetchRegister.Instruction).RS; });
					#endregion
					
					#region Instruction bus to register file (Rt)
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(refSignal.Vertices[0][0].X, refSignal.Vertices[0][0].Y + 40);
					vertices[0][1] = new Point(owner.registerFileView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Rt", 5, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((Instruction)processor.FetchRegister.Instruction).RT; });
					#endregion
					
					#region Instruction bus to sign extender
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(signals[index - 1].Vertices[0][0].X, owner.signExtView.Top + (owner.signExtView.Height >> 1));
					vertices[0][1] = new Point(owner.signExtView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Immediate", 16, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = BitConverter.ToUInt16(BitConverter.GetBytes(((Instruction)processor.FetchRegister.Instruction).Immediate), 0); });
					#endregion
					
					#region Instruction bus to ID/EX (Rt)
					refSignal = signals[index - 2];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(signals[index - 1].Vertices[0][0].X, owner.signExtView.Bottom + 10);
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices);
					#endregion
					
					#region Instruction bus to ID/EX (Rd)
					refSignal = signals[index - 9];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Rd", 5, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((Instruction)processor.FetchRegister.Instruction).RD; });
					#endregion
					
					#region Instruction bus to ID/EX (Rs)
					refSignal = signals[index - 10];
					vertices = new Point[1][]; //Forwarding signal
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(vertices[0][0].X, vertices[0][0].Y + 10);
					vertices[0][2] = new Point(owner.decodeRegisterView.Left, vertices[0][1].Y);
					signals[index++] = new Signal("Rs", 5, false, false, HazardMode.StallAndForward, vertices,
					                              (signal) => { signal.Value = (uint)((Instruction)processor.FetchRegister.Instruction).RS; });
					#endregion
					
					#region Register file to ID/EX (read value 1)
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.registerFileView.Right, signals[index - 6].Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + ((owner.decodeRegisterView.Left - vertices[0][0].X) >> 1), vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, vertices[0][1].Y + 5);
					vertices[0][3] = new Point(owner.decodeRegisterView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Read Value 1", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.Registers[(Register)((Instruction)processor.FetchRegister.Instruction).RS]; });
					#endregion
					
					#region Register file to ID/EX (read value 2)
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.registerFileView.Right, signals[index - 6].Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + ((owner.decodeRegisterView.Left - vertices[0][0].X) >> 1), vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.forwardMuxViews[1].Top + 10);
					vertices[0][3] = new Point(owner.decodeRegisterView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Read Value 2", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.Registers[(Register)((Instruction)processor.FetchRegister.Instruction).RT]; });
					#endregion
					
					#region Sign extender to ID/EX
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.signExtView.Right, signals[index - 6].Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Immediate", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = BitConverter.ToUInt32(BitConverter.GetBytes((int)((Instruction)processor.FetchRegister.Instruction).Immediate), 0); });
					#endregion
					
					#region Control to PC mux
					vertices = new Point[3][]; 
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.controlUnitView.Left, owner.controlUnitView.Top + 5);
					vertices[0][1] = new Point(10, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.pcMuxView.Top + (owner.pcMuxView.Height >> 1));
					vertices[0][3] = new Point(owner.pcMuxView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("PC Src", 2, true, false, HazardMode.Fail, vertices,
					                             (signal) => { signal.Value = (uint)processor.Control.PCSource; });
					#endregion
					
					#region Control to PC (PC write)
					vertices = new Point[2][]; //Stall signal
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.controlUnitView.Left, owner.fetchRegisterView.Top - 10);
					vertices[0][1] = new Point(owner.fetchRegisterView.Left - 15, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.pcView.Top + (owner.pcView.Height >> 1));
					vertices[0][3] = new Point(owner.pcView.Right, vertices[0][2].Y);
					vertices[1] = vertices[0];
					signals[index++] = new Signal("PC Write", 1, true, false, HazardMode.Stall, vertices,
					                              (signal) => { signal.Value = (uint)(processor.Control.StallPC ? 0 : 1); });
					#endregion
					
					#region Control to IF/ID (write, flush)
					vertices = new Point[2][]; //Stall signal
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.controlUnitView.Left, owner.fetchRegisterView.Top);
					vertices[0][1] = new Point(owner.fetchRegisterView.Right, vertices[0][0].Y);
					vertices[1] = vertices[0];
					signals[index++] = new Signal("IF/ID Write, Flush", 2, true, false, HazardMode.Stall, vertices,
					                              (signal) => { signal.Value = (uint)((processor.Control.StallFetch ? 0 : 2) | (processor.Control.FlushFetch ? 1 : 0)); });
					#endregion
					
					#region Control to ID/EX (writeback signals)
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.controlUnitView.Right, owner.decodeRegisterView.Top + 15);
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Writeback signals", 2, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.Control.Signals.HasFlag(ControlSignals.MemRead) ? 2 : 0) | (processor.Control.Signals.HasFlag(ControlSignals.RegWrite) ? 1 : 0)); });
					#endregion
					
					#region Control to ID/EX (memory signals)
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.controlUnitView.Right, signals[index - 1].Vertices[0][0].Y + 10);
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Memory signals", 2, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.Control.Signals.HasFlag(ControlSignals.MemWrite) ? 2 : 0) | (processor.Control.Signals.HasFlag(ControlSignals.MemRead) ? 1 : 0)); });
					#endregion
					
					#region Control to ID/EX (execute signals)
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.controlUnitView.Right, signals[index - 1].Vertices[0][0].Y + 10);
					vertices[0][1] = new Point(owner.decodeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Execute signals", 7, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.Control.Signals.HasFlag(ControlSignals.Branch) ? 0x40 : 0) | (processor.Control.Signals.HasFlag(ControlSignals.RegDst) ? 0x20 : 0) | (processor.Control.Signals.HasFlag(ControlSignals.ALUSrc) ? 0x10 : 0)) | (uint)processor.Control.ALUOp; });
					#endregion
					#endregion
					
					#region Execute Stage
					#region ID/EX to EX/MEM (writeback signals)
					refSignal = signals[index - 3];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.executeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices,
					                              (signal) => { signal.Value = (uint)((processor.DecodeRegister.Signals.HasFlag(ControlSignals.MemRead) ? 2 : 0) | (processor.DecodeRegister.Signals.HasFlag(ControlSignals.RegWrite) ? 1 : 0)); });
					#endregion
					
					#region ID/EX to memory signals bus
					refSignal = signals[index - 3];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + 30, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 2, true, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.DecodeRegister.Signals.HasFlag(ControlSignals.MemWrite) ? 2 : 0) | (processor.DecodeRegister.Signals.HasFlag(ControlSignals.MemRead) ? 1 : 0)); });
					#endregion
					
					#region Memory signals bus to EX/MEM
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.executeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 2, true, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Memory signals bus to control unit
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][0];
					vertices[0][1] = new Point(vertices[0][0].X, owner.controlUnitView.Top + 20);
					vertices[0][2] = new Point(owner.controlUnitView.Right, vertices[0][1].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices);
					#endregion
					
					#region ID/EX to execute signals bus
					refSignal = signals[index - 5];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.immediateShifterView.Right + 5, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.destinationMuxView.Top - 10);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 7, true, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.DecodeRegister.Signals.HasFlag(ControlSignals.Branch) ? 0x40 : 0) | (processor.DecodeRegister.Signals.HasFlag(ControlSignals.RegDst) ? 0x20 : 0) | (processor.DecodeRegister.Signals.HasFlag(ControlSignals.ALUSrc) ? 0x10 : 0)) | (uint)processor.DecodeRegister.ALUOp; });
					#endregion
					
					#region Execute signals bus to control unit
					refSignal = signals[index - 1];
					vertices = new Point[2][]; //Stall signal
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(refSignal.Vertices[0][0].X + 15, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X, owner.controlUnitView.Top + 30);
					vertices[0][2] = new Point(owner.controlUnitView.Right, vertices[0][1].Y);
					vertices[1] = vertices[0];
					signals[index++] = new Signal("Branch", 1, true, false, HazardMode.Stall, vertices,
					                              (signal) => { signal.Value = (uint)(processor.DecodeRegister.Signals.HasFlag(ControlSignals.Branch) ? 1 : 0); });
					#endregion
					
					#region Execute signals bus to ALU
					refSignal = signals[index - 2];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(refSignal.Vertices[0][1].X, owner.aluView.Top);
					vertices[0][1] = new Point(owner.aluView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("ALUOp", 4, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)processor.DecodeRegister.ALUOp; });
					#endregion
					
					#region Execute signals bus to ALU mux
					refSignal = signals[index - 3];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(refSignal.Vertices[0][1].X, owner.aluMuxView.Top - 10);
					vertices[0][1] = new Point(owner.aluMuxView.Left + (owner.aluMuxView.Width >> 1), vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.aluMuxView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("ALUSrc", 1, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)(processor.DecodeRegister.Signals.HasFlag(ControlSignals.ALUSrc) ? 1 : 0); });
					#endregion
					
					#region Execute signals bus to destination mux
					refSignal = signals[index - 4];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(owner.destinationMuxView.Left + (owner.destinationMuxView.Width >> 1), vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.destinationMuxView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("RegDst", 1, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)(processor.DecodeRegister.Signals.HasFlag(ControlSignals.RegDst) ? 1 : 0); });
					#endregion
					
					#region ID/EX to adder (following address)
					refSignal = signals[index - 30];
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + 15, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.branchAdderView.Top + owner.branchAdderView.Height / 5);
					vertices[0][3] = new Point(owner.branchAdderView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices,
					                              (signal) => { signal.Value = processor.DecodeRegister.FollowingInstructionAddress; });
					#endregion
					
					#region ID/EX to forward mux A or ALU (read value 1)
					refSignal = signals[index - 19];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[1] = new Point[4];
					vertices[1][0] = vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][3].Y);
					vertices[1][1] = vertices[0][1] = new Point(owner.forwardMuxViews[0].Left, vertices[0][0].Y);
					vertices[1][2] = new Point(vertices[1][1].X, owner.aluView.Top + owner.aluView.Height / 5);
					vertices[1][3] = new Point(owner.aluView.Left, vertices[1][2].Y);
					vertices[2] = vertices[1];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal, vertices,
					                                                      (signal) => { signal.Value = processor.DecodeRegister.RegisterReadValue1; signal.IsActive = (processor.Mode != HazardMode.StallAndForward || processor.Control.ForwardA == ForwardSource.Decode); });
					#endregion
					
					#region Forward mux A to ALU
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.forwardMuxViews[0].Right, owner.aluView.Top + owner.aluView.Height / 5);
					vertices[0][1] = new Point(owner.aluView.Left, vertices[0][0].Y);
					signals[index++] = new Signal("Operand 1", 32, false, false, HazardMode.StallAndForward, vertices,
					                              (signal) =>	{
																	if (processor.DecodeRegister.Signals.HasFlag(ControlSignals.ALUSrc))
																		signal.Value = (uint)BitConverter.ToUInt16(BitConverter.GetBytes(processor.DecodeRegister.Instruction.Immediate), 0);
																	else if (processor.Mode != HazardMode.StallAndForward)
																		signal.Value = processor.DecodeRegister.RegisterReadValue1;
																	else switch (processor.Control.ForwardA)
																	{
																		case ForwardSource.Execute:
																			signal.Value = processor.ExecuteRegister.ALUResult;
																			break;
																		
																		case ForwardSource.Memory:
																			signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult);
																			break;
																		
																		default:
																			signal.Value = processor.DecodeRegister.RegisterReadValue1;
																			break;
																	}
																});
					#endregion
					
					#region ID/EX to Read value 2 bus
					refSignal = signals[index - 20];
					vertices = new Point[3][];
					vertices[0] = null;
					vertices[1] = new Point[4];
					vertices[1][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][3].Y);
					vertices[1][1] = new Point(owner.forwardMuxViews[1].Left, vertices[1][0].Y);
					vertices[1][2] = new Point(vertices[1][1].X, owner.aluMuxView.Bottom - 10);
					vertices[1][3] = new Point(owner.aluMuxView.Left - 15, vertices[1][2].Y);
					vertices[2] = vertices[1];
					signals[index++] = new Signal(refSignal.Name, 32, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.DecodeRegister.RegisterReadValue2; });
					#endregion
					
					#region Read value 2 bus to forward mux B or ALU mux
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[1] = new Point[2];
					vertices[0][0] = refSignal.Vertices[1][0];
					vertices[0][1] = refSignal.Vertices[1][1];
					vertices[1][0] = refSignal.Vertices[1][3];
					vertices[1][1] = new Point(owner.aluMuxView.Left, vertices[1][0].Y);
					vertices[2] = vertices[1];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices,
					                                                      (signal) => { signal.Value = processor.DecodeRegister.RegisterReadValue2; signal.IsActive = (processor.Mode != HazardMode.StallAndForward || processor.Control.ForwardB == ForwardSource.Decode); });
					#endregion
					
					#region Read value 2 bus to EX/MEM
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = null;
					vertices[1] = new Point[3];
					vertices[1][0] = refSignal.Vertices[1][0];
					vertices[1][1] = new Point(vertices[1][0].X, owner.aluMuxView.Bottom + 5);
					vertices[1][2] = new Point(owner.executeRegisterView.Left, vertices[1][1].Y);
					vertices[2] = vertices[1];
					signals[index++] = new Signal(refSignal, vertices);
					#endregion
					
					#region Forward mux B bus
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.forwardMuxViews[1].Right, owner.forwardMuxViews[1].Bottom - 10);
					vertices[0][1] = new Point(vertices[0][0].X + 30, vertices[0][0].Y);
					signals[index++] = new Signal("Read Value 2", 32, false, true, HazardMode.StallAndForward, vertices,
					                              (signal) =>	{
																	switch (processor.Control.ForwardB)
																	{
																		case ForwardSource.Execute:
																			signal.Value = processor.ExecuteRegister.ALUResult;
																			break;
																		
																		case ForwardSource.Memory:
																			signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult);
																			break;
																		
																		default:
																			signal.Value = processor.DecodeRegister.RegisterReadValue2;
																			break;
																	}
																});
					#endregion
					
					#region Forward mux B bus to ALU mux
					refSignal = signals[index - 1];
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.aluMuxView.Left, vertices[0][0].Y);
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.StallAndForward, vertices,
					                                                      (signal) =>	{
																							switch (processor.Control.ForwardB)
																							{
																								case ForwardSource.Execute:
																									signal.Value = processor.ExecuteRegister.ALUResult;
																									break;
																								
																								case ForwardSource.Memory:
																									signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult);
																									break;
																								
																								default:
																									signal.Value = processor.DecodeRegister.RegisterReadValue2;
																									break;
																							}
																							signal.IsActive = !processor.DecodeRegister.Signals.HasFlag(ControlSignals.ALUSrc);
																						});
					#endregion
					
					#region Forward mux B bus to EX/MEM
					refSignal = signals[index - 2];
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.aluMuxView.Bottom + 5);
					vertices[0][2] = new Point(owner.executeRegisterView.Left, vertices[0][1].Y);
					signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.StallAndForward, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region ALU mux to ALU
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.aluMuxView.Right, owner.aluView.Bottom - owner.aluView.Height / 5);
					vertices[0][1] = new Point(owner.aluView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Operand 2", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) =>	{
																	if (!processor.DecodeRegister.Signals.HasFlag(ControlSignals.ALUSrc))
																		switch (processor.Control.ForwardB)
																		{
																			case ForwardSource.Execute:
																				signal.Value = processor.ExecuteRegister.ALUResult;
																				break;
																			
																			case ForwardSource.Memory:
																				signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult);
																				break;
																			
																			default:
																				signal.Value = processor.DecodeRegister.RegisterReadValue2;
																				break;
																		}
																	else
																		signal.Value = (uint)BitConverter.ToUInt16(BitConverter.GetBytes(processor.DecodeRegister.Instruction.Immediate), 0);
																});
					#endregion
					
					#region ID/EX to immediate value bus
					refSignal = signals[index - 26];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.forwardMuxViews[1].Right + 20, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.aluMuxView.Top + 10);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 32, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = BitConverter.ToUInt32(BitConverter.GetBytes((int)processor.DecodeRegister.Instruction.Immediate), 0); });
					#endregion
					
					#region Immediate value bus to shifter
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(vertices[0][0].X, owner.immediateShifterView.Top + (owner.immediateShifterView.Height >> 1));
					vertices[0][2] = new Point(owner.immediateShifterView.Left, vertices[0][1].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Immediate value bus to ALU mux
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][0];
					vertices[0][1] = new Point(owner.aluMuxView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices,
					                                                      (signal) => { signal.Value = BitConverter.ToUInt32(BitConverter.GetBytes((int)processor.DecodeRegister.Instruction.Immediate), 0); signal.IsActive = processor.DecodeRegister.Signals.HasFlag(ControlSignals.ALUSrc); });
					#endregion
					
					#region ID/EX to Rt bus
					refSignal = signals[index - 34];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[2] = null;
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.forwardUnitView.Left - 25, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.forwardUnitView.Top + (owner.forwardUnitView.Height >> 1));
					vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)processor.DecodeRegister.Instruction.RT; });
					#endregion
					
					#region Rt bus to forwarding unit
					refSignal = signals[index - 1];
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(owner.forwardUnitView.Left, vertices[0][0].Y);
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.StallAndForward, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Rt bus to control unit
					vertices = new Point[2][]; //Stall signal
					vertices[0] = new Point[4];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(owner.fetchRegisterView.Right + 10, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.fetchRegisterView.Top + 10);
					vertices[0][3] = new Point(owner.controlUnitView.Left, vertices[0][2].Y);
					vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.Stall, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Rt bus to destination mux
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[2] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[2][0] = refSignal.Vertices[0][0];
					vertices[2][1] = vertices[0][1] = new Point(owner.destinationMuxView.Left, vertices[0][0].Y);
					vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.Fail, vertices,
					                                                      (signal) => { signal.Value = (uint)processor.DecodeRegister.Instruction.RT; signal.IsActive = !processor.DecodeRegister.Signals.HasFlag(ControlSignals.RegDst); });
					#endregion
					
					#region ID/EX to destination mux (Rd)
					refSignal = signals[index - 37];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.destinationMuxView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal, vertices,
					                                                      (signal) => { signal.Value = (uint)processor.DecodeRegister.Instruction.RD; signal.IsActive = processor.DecodeRegister.Signals.HasFlag(ControlSignals.RegDst); });
					#endregion
					
					#region ID/EX to forwarding unit (Rs)
					refSignal = signals[index - 37];
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.decodeRegisterView.Right, refSignal.Vertices[0][2].Y);
					vertices[0][1] = new Point(owner.forwardUnitView.Left - 15, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.forwardUnitView.Top + 10);
					vertices[0][3] = new Point(owner.forwardUnitView.Left, vertices[0][2].Y);
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.StallAndForward, vertices,
					                              (signal) => { signal.Value = (uint)processor.DecodeRegister.Instruction.RS; });
					#endregion
					
					#region Destination mux to destination register bus
					vertices = new Point[2][]; //Stall signal
					vertices[0] = null;
					vertices[1] = new Point[2];
					vertices[1][0] = new Point(owner.destinationMuxView.Right, owner.destinationMuxView.Top + (owner.destinationMuxView.Height >> 1));
					vertices[1][1] = new Point(vertices[1][0].X + 20, vertices[1][0].Y);
					signals[index++] = new Signal("Destination Register", 5, false, true, HazardMode.Stall, vertices,
					                              (signal) => { signal.Value = (uint)(processor.DecodeRegister.Signals.HasFlag(ControlSignals.RegDst) ? processor.DecodeRegister.Instruction.RD : processor.DecodeRegister.Instruction.RT); });
					#endregion
					
					#region Destination register bus to EX/MEM
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[1] = new Point[2];
					vertices[0][0] = refSignal.Vertices[1][0];
					vertices[1][0] = refSignal.Vertices[1][1];
					vertices[1][1] = vertices[0][1] = new Point(owner.executeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Destination register bus to control unit
					vertices = new Point[2][]; //Stall signal
					vertices[0] = null;
					vertices[1] = new Point[6];
					vertices[1][0] = refSignal.Vertices[1][1];
					vertices[1][1] = new Point(vertices[1][0].X, owner.decodeRegisterView.Bottom + 10);
					vertices[1][2] = new Point(owner.registerFileView.Right + 10, vertices[1][1].Y);
					vertices[1][3] = new Point(vertices[1][2].X, owner.registerFileView.Top - 20);
					vertices[1][4] = new Point(owner.controlUnitView.Right - 20, vertices[1][3].Y);
					vertices[1][5] = new Point(vertices[1][4].X, owner.controlUnitView.Bottom);
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.Stall, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Shifter to branch adder
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.immediateShifterView.Right, owner.branchAdderView.Bottom - owner.branchAdderView.Height / 5);
					vertices[0][1] = new Point(owner.branchAdderView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Branch Offset", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = BitConverter.ToUInt32(BitConverter.GetBytes(((int)processor.DecodeRegister.Instruction.Immediate) << 2), 0); });
					#endregion
					
					#region Branch adder to PC mux
					vertices = new Point[3][];
					vertices[0] = new Point[7];
					vertices[0][0] = new Point(owner.branchAdderView.Right, owner.branchAdderView.Top + (owner.branchAdderView.Height >> 1));
					vertices[0][1] = new Point(vertices[0][0].X + 15, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, 5);
					vertices[0][3] = new Point(15, vertices[0][2].Y);
					vertices[0][4] = new Point(vertices[0][3].X, owner.pcAdderView.Bottom + 20);
					vertices[0][5] = new Point(owner.pcMuxView.Left + 10, vertices[0][4].Y);
					vertices[0][6] = new Point(vertices[0][5].X, owner.pcMuxView.Top);
					vertices[2] = vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal("Branch Target", 32, false, false, HazardMode.Fail, vertices,
					                                                      (signal) => { signal.Value = unchecked(BitConverter.ToUInt32(BitConverter.GetBytes(((int)processor.DecodeRegister.Instruction.Immediate) << 2), 0) + processor.DecodeRegister.FollowingInstructionAddress); signal.IsActive = (processor.Control.PCSource == PCSource.Branch); });
					#endregion
					
					#region ALU to EX/MEM (result)
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.aluView.Right, owner.aluView.Top + (owner.aluView.Height >> 1));
					vertices[0][1] = new Point(owner.executeRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("ALU Result", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.ALU.Result; });
					#endregion
					
					#region ALU to control unit (zero)
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.aluView.Right, owner.aluView.Top + (owner.aluView.Height >> 2));
					vertices[0][1] = new Point(vertices[0][0].X + 5, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.controlUnitView.Top + 10);
					vertices[0][3] = new Point(owner.controlUnitView.Right, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Zero", 1, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)(processor.ALU.Result == 0 ? 1 : 0); });
					#endregion
					
					#region Forwarding unit to forward mux B
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.forwardUnitView.Left + 10, owner.forwardUnitView.Top);
					vertices[0][1] = new Point(vertices[0][0].X, owner.destinationMuxView.Top + (owner.destinationMuxView.Height >> 1));
					vertices[0][2] = new Point(owner.forwardMuxViews[1].Left + (owner.forwardMuxViews[1].Width >> 1), vertices[0][1].Y);
					vertices[0][3] = new Point(vertices[0][2].X, owner.forwardMuxViews[1].Bottom);
					signals[index++] = new Signal("ForwardB", 2, true, false, HazardMode.StallAndForward, vertices,
					                              (signal) => { signal.Value = (uint)processor.Control.ForwardB; });
					#endregion
					
					#region Forwarding unit to forward mux A
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[6];
					vertices[0][0] = new Point(owner.forwardUnitView.Left + 20, owner.forwardUnitView.Top);
					vertices[0][1] = new Point(vertices[0][0].X, owner.destinationMuxView.Top - 10);
					vertices[0][2] = new Point(owner.forwardMuxViews[0].Right + 10, vertices[0][1].Y);
					vertices[0][3] = new Point(vertices[0][2].X, owner.forwardMuxViews[0].Bottom + 10);
					vertices[0][4] = new Point(owner.forwardMuxViews[0].Left + (owner.forwardMuxViews[0].Width >> 1), vertices[0][3].Y);
					vertices[0][5] = new Point(vertices[0][4].X, owner.forwardMuxViews[0].Bottom);
					signals[index++] = new Signal("ForwardA", 2, true, false, HazardMode.StallAndForward, vertices,
					                              (signal) => { signal.Value = (uint)processor.Control.ForwardA; });
					#endregion
					#endregion
					
					#region Memory Stage
					#region EX/MEM to writeback signals bus
					refSignal = signals[index - 37];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.executeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point((vertices[0][0].X + owner.memoryRegisterView.Left) >> 1, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 2, true, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.ExecuteRegister.Signals.HasFlag(ControlSignals.MemRead) ? 2 : 0) | (processor.ExecuteRegister.Signals.HasFlag(ControlSignals.RegWrite) ? 1 : 0)); });
					#endregion
					
					#region Writeback signals bus to MEM/WB
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.memoryRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 2, true, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Writeback signals bus to forwarding unit
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[5];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.memoryRegisterView.Top - 10);
					vertices[0][2] = new Point(owner.writebackMuxView.Right + 20, vertices[0][1].Y);
					vertices[0][3] = new Point(vertices[0][2].X, owner.forwardUnitView.Top + 30);
					vertices[0][4] = new Point(owner.forwardUnitView.Right, vertices[0][3].Y);
					signals[index++] = new Signal("RegWrite", 1, true, false, HazardMode.StallAndForward, vertices,
					                              (signal) => { signal.Value = (uint)(processor.ExecuteRegister.Signals.HasFlag(ControlSignals.RegWrite) ? 1 : 0); });
					#endregion
					
					#region EX/MEM to data memory (control signals)
					refSignal = signals[index - 38];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(owner.executeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.dataMemoryView.Left + (owner.dataMemoryView.Width >> 1), vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.dataMemoryView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("MemWrite, MemRead", 2, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.ExecuteRegister.Signals.HasFlag(ControlSignals.MemWrite) ? 2 : 0) | (processor.ExecuteRegister.Signals.HasFlag(ControlSignals.MemRead) ? 1 : 0)); });
					#endregion
					
					#region EX/MEM to ALU result bus
					refSignal = signals[index - 8];
					vertices = new Point[3][];
					vertices[0] = new Point[5];
					vertices[1] = new Point[2];
					vertices[1][0] = vertices[0][0] = new Point(owner.executeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[1][1] = vertices[0][1] = new Point(owner.dataMemoryView.Left - 10, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.executeRegisterView.Bottom + 10);
					vertices[0][3] = new Point(owner.decodeRegisterView.Right + 20, vertices[0][2].Y);
					vertices[0][4] = new Point(vertices[0][3].X, owner.forwardMuxViews[1].Top + 25);
					vertices[2] = vertices[1];
					signals[index++] = new Signal("ALU Result", 32, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.ExecuteRegister.ALUResult; });
					#endregion
					
					#region ALU result bus to data memory
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.dataMemoryView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region ALU result bus to MEM/WB
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[1] = new Point[3];
					vertices[1][0] = refSignal.Vertices[0][0];
					vertices[1][1] = vertices[0][0] = new Point(vertices[1][0].X, owner.dataMemoryView.Bottom + 10);
					vertices[1][2] = vertices[0][1] = new Point(owner.memoryRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1];
					signals[index++] = new Signal(refSignal, vertices);
					#endregion
					
					#region ALU result bus to forward mux B
					refSignal = signals[index - 3];
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][4];
					vertices[0][1] = new Point(owner.forwardMuxViews[1].Left, vertices[0][0].Y);
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.StallAndForward, vertices,
					                                                      (signal) => { signal.Value = processor.ExecuteRegister.ALUResult; signal.IsActive = (processor.Control.ForwardB == ForwardSource.Execute); });
					#endregion
					
					#region ALU result bus to forward mux A
					vertices = new Point[1][];
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][4];
					vertices[0][1] = new Point(vertices[0][0].X, owner.forwardMuxViews[0].Top + 25);
					vertices[0][2] = new Point(owner.forwardMuxViews[0].Left, vertices[0][1].Y);
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.StallAndForward, vertices,
					                                                      (signal) => { signal.Value = processor.ExecuteRegister.ALUResult; signal.IsActive = (processor.Control.ForwardA == ForwardSource.Execute); });
					#endregion
					
					#region EX/MEM to data memory (write data)
					refSignal = signals[index - 29];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.executeRegisterView.Right, refSignal.Vertices[0][2].Y);
					vertices[0][1] = new Point(owner.dataMemoryView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices,
					                              (signal) => { signal.Value = processor.ExecuteRegister.MemoryWriteValue; });
					#endregion
					
					#region EX/MEM to destination register bus
					refSignal = signals[index - 18];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.executeRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + 30, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)processor.ExecuteRegister.DestinationRegister; });
					#endregion
					
					#region Destination register bus to MEM/WB
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.memoryRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Destination register bus to control unit
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.executeRegisterView.Right + 10, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X, owner.controlUnitView.Top - 10);
					vertices[0][2] = new Point(owner.controlUnitView.Right - 20, vertices[0][1].Y);
					vertices[0][3] = new Point(vertices[0][2].X, owner.controlUnitView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices);
					#endregion
					
					#region Destination register bus to forwarding unit
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][0];
					vertices[0][1] = new Point(vertices[0][0].X, owner.forwardUnitView.Top + 10);
					vertices[0][2] = new Point(owner.forwardUnitView.Right, vertices[0][1].Y);
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.StallAndForward, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Data memory to MEM/WB
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.dataMemoryView.Right, owner.writebackMuxView.Top + 10);
					vertices[0][1] = new Point(owner.memoryRegisterView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("Read Data", 32, false, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = processor.DataSynchronizer.ReadValue; });
					#endregion
					#endregion
					
					#region Writeback Stage
					#region MEM/WB to writeback signals bus
					refSignal = signals[index - 14];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.memoryRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.writebackMuxView.Left + (owner.writebackMuxView.Width >> 1), vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 2, true, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)((processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? 2 : 0) | (processor.MemoryRegister.Signals.HasFlag(ControlSignals.RegWrite) ? 1 : 0)); });
					#endregion
					
					#region Writeback signals bus to writeback register
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.writebackMuxView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("MemToReg", 1, true, false, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)(processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? 1 : 0); });
					#endregion
					
					#region Writeback signals bus to RegWrite bus
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(vertices[0][0].X, owner.controlUnitView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal("RegWrite", 1, true, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)(processor.MemoryRegister.Signals.HasFlag(ControlSignals.RegWrite) ? 1 : 0); });
					#endregion
					
					#region RegWrite bus to register file
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.registerFileView.Right - 10, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.registerFileView.Top);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 1, true, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region RegWrite bus to forwarding unit
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[4];
					vertices[0][0] = refSignal.Vertices[0][1];
					vertices[0][1] = new Point(owner.writebackMuxView.Right + 30, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.forwardUnitView.Top + 40);
					vertices[0][3] = new Point(owner.forwardUnitView.Right, vertices[0][2].Y);
					signals[index++] = new Signal(refSignal.Name, 1, true, false, HazardMode.StallAndForward, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region MEM/WB to writeback mux (read data)
					refSignal = signals[index - 6];
					vertices = new Point[3][];
					vertices[0] = new Point[2];
					vertices[0][0] = new Point(owner.memoryRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(owner.writebackMuxView.Left, vertices[0][0].Y);
					vertices[2] = vertices[1] = vertices[0];
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal, vertices,
					                                                      (signal) => { signal.Value = processor.MemoryRegister.ReadValue; signal.IsActive = processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead); });
					#endregion
					
					#region MEM/WB to writeback mux (ALU result)
					refSignal = signals[index - 15];
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = new Point(owner.memoryRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + 10, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.writebackMuxView.Bottom - 10);
					vertices[0][3] = new Point(owner.writebackMuxView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal, vertices,
					                              (signal) => { signal.Value = processor.MemoryRegister.ALUResult; signal.IsActive = !processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead); });
					#endregion
					
					#region Writeback mux bus
					vertices = new Point[3][];
					vertices[0] = new Point[5];
					vertices[1] = new Point[4];
					vertices[1][0] = vertices[0][0] = new Point(owner.writebackMuxView.Right, owner.writebackMuxView.Top + (owner.writebackMuxView.Height >> 1));
					vertices[1][1] = vertices[0][1] = new Point(vertices[0][0].X + 10, vertices[0][0].Y);
					vertices[1][2] = vertices[0][2] = new Point(vertices[0][1].X, owner.decodeRegisterView.Bottom + 30);
					vertices[1][3] = vertices[0][3] = new Point(owner.decodeRegisterView.Right + 10, vertices[0][2].Y);
					vertices[0][4] = new Point(vertices[0][3].X, owner.forwardMuxViews[1].Bottom - 10);
					vertices[2] = vertices[1];
					signals[index++] = new Signal("Writeback Value", 32, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult); });
					#endregion
					
					#region Writeback mux bus to register file
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = refSignal.Vertices[0][3];
					vertices[0][1] = new Point(owner.fetchRegisterView.Right + 20, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.registerFileView.Bottom - 35);
					vertices[0][3] = new Point(owner.registerFileView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Writeback mux bus to forward mux B
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[2];
					vertices[0][0] = refSignal.Vertices[0][4];
					vertices[0][1] = new Point(owner.forwardMuxViews[1].Left, vertices[0][0].Y);
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.StallAndForward, vertices,
					                                                      (signal) => { signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult); signal.IsActive = (processor.Control.ForwardB == ForwardSource.Memory); });
					#endregion
					
					#region Writeback mux bus to forward mux A
					refSignal = signals[index - 1];
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][0];
					vertices[0][1] = new Point(vertices[0][0].X, owner.forwardMuxViews[0].Bottom - 10);
					vertices[0][2] = new Point(owner.forwardMuxViews[1].Left, vertices[0][1].Y);
					muxInputs[muxIndex++] = signals[index++] = new Signal(refSignal.Name, 32, false, false, HazardMode.StallAndForward, vertices,
					                                                      (signal) => { signal.Value = (processor.MemoryRegister.Signals.HasFlag(ControlSignals.MemRead) ? processor.MemoryRegister.ReadValue : processor.MemoryRegister.ALUResult); signal.IsActive = (processor.Control.ForwardA == ForwardSource.Memory); });
					#endregion
					
					#region MEM/WB to destination register bus
					refSignal = signals[index - 15];
					vertices = new Point[3][];
					vertices[0] = new Point[3];
					vertices[0][0] = new Point(owner.memoryRegisterView.Right, refSignal.Vertices[0][0].Y);
					vertices[0][1] = new Point(vertices[0][0].X + 10, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.memoryRegisterView.Bottom + 20);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, true, HazardMode.Fail, vertices,
					                              (signal) => { signal.Value = (uint)processor.MemoryRegister.DestinationRegister; });
					#endregion
					
					#region Destination register bus to register file
					refSignal = signals[index - 1];
					vertices = new Point[3][];
					vertices[0] = new Point[4];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(owner.fetchRegisterView.Right + 30, vertices[0][0].Y);
					vertices[0][2] = new Point(vertices[0][1].X, owner.registerFileView.Bottom - 15);
					vertices[0][3] = new Point(owner.registerFileView.Left, vertices[0][2].Y);
					vertices[2] = vertices[1] = vertices[0];
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.Fail, vertices, refSignal.UpdateDelegate);
					#endregion
					
					#region Destination register bus to forwarding unit
					vertices = new Point[1][]; //Forward signal
					vertices[0] = new Point[3];
					vertices[0][0] = refSignal.Vertices[0][2];
					vertices[0][1] = new Point(vertices[0][0].X, owner.forwardUnitView.Top + 20);
					vertices[0][2] = new Point(owner.forwardUnitView.Right, vertices[0][1].Y);
					signals[index++] = new Signal(refSignal.Name, 5, false, false, HazardMode.StallAndForward, vertices, refSignal.UpdateDelegate);
					#endregion
					#endregion
					#endregion
					
					toolTip = new ToolTip();
					toolTip.InitialDelay = toolTipInitialDelay;
					toolTip.ReshowDelay = toolTipReshowDelay;
					
					Paint += PaintSignals;
					MouseMove += SetToolTip;
				}
				#endregion
				#endregion
			}
		}
	}
}
