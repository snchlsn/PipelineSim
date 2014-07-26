#region Using Directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected class Signal
			{
				#region Fields
				#region XML Header
				/// <summary>
				/// The name of the signal.
				/// </summary>
				#endregion
				public readonly string Name;
				
				#region XML Header
				/// <summary>
				/// <c>true</c> if the signal is a bus; <c>false</c> otherwise.
				/// </summary>
				#endregion
				public readonly bool IsBus;
				
				#region XML Header
				/// <summary>
				/// <c>true</c> if the signal is a control signal; <c>false</c> if it is a data signal.
				/// </summary>
				#endregion
				public readonly bool IsControl;
				
				#region XML Header
				/// <summary>
				/// The width of the signal, in bits.
				/// </summary>
				#endregion
				public readonly byte BitCount;
				
				#region XML Header
				/// <summary>
				/// The least advanced mode in which the <c>Signal</c> is used.
				/// </summary>
				#endregion
				public readonly HazardMode Mode;
				
				#region XML Header
				/// <summary>
				/// A two-dimensional array of points specifying the <c>Signal</c>'s vertices in each hazard mode.
				/// </summary>
				/// <remarks>
				/// The first rank has <c>Length</c> <c>1</c>, <c>2</c>, or <c>3</c>.  The first element
				/// contains vertices for when stalling is not enabled, the second contains vertices for
				/// when only stalling is enabled, and the third contains vertices for when both stalling
				/// and forwarding are enabled.  The <c>Length</c> of the first rank may be <c>2</c>
				/// only if the <c>Signal</c> is only used when stalling is enabled, and <c>1</c> only if
				/// the <c>Signal</c> is only used when forwarding is enabled.
				/// </remarks>
				#endregion
				public readonly Point[][] Vertices;
				
				#region XML Header
				/// <summary>
				/// A delegate to a method that updates the <c>Signal</c>'s <c>Value</c>
				/// and <c>IsActive</c> properties.
				/// </summary>
				#endregion
				public readonly Action<Signal> UpdateDelegate;
				
				private Signal[] connectedSignals;
				private uint val;
				private bool isActive = true, hasFocus = false;
				#endregion
				
				#region Properties
				#region XML Header
				/// <summary>
				/// Gets the color used to draw the <c>Signal</c>.
				/// </summary>
				#endregion
				public Color Color
				{
					get
					{
						if (IsControl)
						{
							if (hasFocus)
								return focusedControlColor;
							else
								return unfocusedControlColor;
						}
						else
						{
							if (isActive)
							{
								if (hasFocus)
									return focusedActiveDataColor;
								else
									return unfocusedActiveDataColor;
							}
							else
							{
								if (hasFocus)
									return focusedInactiveDataColor;
								else
									return unfocusedInactiveDataColor;
							}
						}
					}
				}
				
				public Signal[] ConnectedSignals
				{
					get { return connectedSignals; }
					set { connectedSignals = value; }
				}
				
				#region XML Header
				/// <summary>
				/// Gets or sets a value indicating whether the <c>Signal</c> has focus (i.e., the mouse
				/// is positioned over the <c>Signal</c> or a connected <c>Signal</c>).
				/// </summary>
				/// <value>
				/// <c>true</c> if the <c>Signal</c> has focus; <c>false</c> otherwise.
				/// </value>
				#endregion
				public bool HasFocus
				{
					get { return hasFocus; }
					set { hasFocus = value; }
				}
				
				#region XML Header
				/// <summary>
				/// Gets or sets a value indicating whether the signal is active (i.e., its value is being
				/// propagated and affects the circuit's outputs).
				/// </summary>
				/// <value><c>true</c> if the signal is active; <c>false</c> otherwise.</value>
				#endregion
				public bool IsActive
				{
					get { return isActive; }
					set { isActive = value; }
				}
				
				public uint Value
				{
					get { return val; }
					set { val = value; }
				}
				#endregion
				
				#region Methods
				public void SetToolTip(ToolTip toolTip, Control owner)
				{
					toolTip.SetToolTip(owner, Globals.HexPrefix + Value.ToString("X" + ((BitCount + 3) >> 2).ToString()));
					toolTip.ToolTipTitle = Name + " (" + BitCount.ToString() + (BitCount == 1 ? " bit)" : " bits)");
					return;
				}
				
				#region XML Header
				/// <summary>
				/// Updates the <c>Signal</c>'s <c>Value</c> and <c>IsActive</c> properties.
				/// </summary>
				#endregion
				public void Update()
				{
					UpdateDelegate(this);
					return;
				}
				
				#region Constructors
				public Signal(string name, byte bitCount, bool control, bool bus, HazardMode mode, Point[][] vertices, Action<Signal> update)
				{
					Name = name;
					BitCount = bitCount;
					IsBus = bus;
					IsControl = control;
					Mode = mode;
					Vertices = vertices;
					UpdateDelegate = update;
				}
				
				public Signal(Signal refSignal, Point[][] vertices): this(refSignal.Name, refSignal.BitCount, refSignal.IsControl, refSignal.IsBus, refSignal.Mode, vertices, refSignal.UpdateDelegate) {}
				
				public Signal(Signal refSignal, Point[][] vertices, Action<Signal> update): this(refSignal.Name, refSignal.BitCount, refSignal.IsControl, refSignal.IsBus, refSignal.Mode, vertices, update) {}
				#endregion
				#endregion
			}
		}
	}
}
