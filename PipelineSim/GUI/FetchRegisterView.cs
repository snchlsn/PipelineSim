#region Using Directives
using System;
using System.Text;
using System.Windows.Forms;
using MIPSEmulator;

using StageRegister = MIPSEmulator.Processor.StageRegister;
using FetchRegister = MIPSEmulator.Processor.FetchStageRegister;
#endregion

namespace PipelineSim.GUI
{
	public sealed partial class PipelineSimForm: Form
	{
		private partial class ProcessorView: UserControl
		{
			protected sealed class FetchRegisterView: StageRegisterView
			{
				#region Fields
				private const string stageName = "IF/ID";
				#endregion
				
				#region Properties
				protected override string StageName
				{
					get { return stageName; }
				}
				#endregion
				
				#region Methods
				#region Event Handlers
				protected override void SetToolTip(object sender, EventArgs e)
				{
					FetchRegister fetchRegister = (FetchRegister)StageRegister;
					StringBuilder toolTipText = new StringBuilder(68);
					
					toolTipText.Append("Instruction: ").Append(Globals.HexPrefix).Append(fetchRegister.Instruction.ToString(Globals.UintFormat)).AppendLine();
					toolTipText.Append("Following instruction address: ").Append(Globals.HexPrefix).Append(fetchRegister.FollowingInstructionAddress.ToString(Globals.UintFormat)).AppendLine();
					toolTip.SetToolTip(this, toolTipText.ToString());
					
					#if DEBUG
					if (toolTipText.Capacity > 68)
						throw new Exception("StringBuilder exceeded initial capacity.");
					#endif
					
					base.SetToolTip(sender, e);
					return;
				}
				#endregion
				
				#region Constructors
				#region XML Header
				/// <summary>
				/// Creates and initializes a new <c>FetchRegisterView</c> instance.
				/// </summary>
				#endregion
				public FetchRegisterView()
				{
				}
				#endregion
				#endregion
			}
		}
	}
}
