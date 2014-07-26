#region Using Directives
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FormsExtensions;
using MIPSEmulator;
using MIPSEmulator.Assembly;

using StageRegister = MIPSEmulator.Processor.StageRegister;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected abstract class StageRegisterView: ClockedComponentView
			{
				#region Fields
				private StageRegister stageRegister = null;
				
				private OrientedTextLabel stageNameLabel;
				protected ToolTip toolTip;
				#endregion
				
				#region Properties
				protected override Size DefaultSize
				{
					get { return new Size(20, 340); }
				}
				
				protected abstract string StageName
				{
					get;
				}
				
				public virtual StageRegister StageRegister
				{
					get { return stageRegister; }
					set
					{
						if (stageRegister != value)
						{
							if (stageRegister != null)
								stageRegister.StateChanged -= SetToolTip;
							
							stageRegister = value;
							
							if (stageRegister != null)
							{
								stageRegister.StateChanged += SetToolTip;
								SetToolTip();
							}
						}
					}
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				private void SetLabelSize(object sender, EventArgs e)
				{
					Label label = (Label)sender;
					
					label.AutoSize = false;
					label.SizeChanged -= SetLabelSize;
					label.Size = new Size(label.Height, label.Width);
				}
				
				protected virtual void SetToolTip(object sender = null, EventArgs e = null)
				{
					StringBuilder toolTipTitle = new StringBuilder(StageName, 31);
					
					toolTipTitle.Append(" - ");
					
					if (StageRegister.HasBubble)
						toolTipTitle.Append("Bubble");
					else
						toolTipTitle.Append(((Instruction)StageRegister.Owner.ProgramSynchronizer.Program.GetInstruction(StageRegister.InstructionAddress)).ToString());
					
					toolTip.ToolTipTitle = toolTipTitle.ToString();
					
					#if DEBUG
					if (toolTipTitle.Capacity > 31)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					toolTip.SetToolTip(stageNameLabel, toolTip.GetToolTip(this));
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>StageRegisterView</c> instance.
				/// </summary>
				#endregion
				public StageRegisterView(): base(false)
				{
					stageNameLabel = new OrientedTextLabel();
					stageNameLabel.AutoSize = true;
					stageNameLabel.Text = StageName;
					stageNameLabel.RotationAngle = 270;
					stageNameLabel.Top = 20;
					stageNameLabel.Left = 2;
					stageNameLabel.SizeChanged += SetLabelSize;
					Controls.Add(stageNameLabel);
					
					toolTip = new ToolTip();
					toolTip.InitialDelay = toolTipInitialDelay;
					toolTip.ReshowDelay = toolTipReshowDelay;
				}
				#endregion
				#endregion
			}
		}
	}
}
