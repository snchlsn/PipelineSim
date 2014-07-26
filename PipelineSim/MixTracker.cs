#region Using Directives
using System;
using System.Configuration;
using System.IO;
using MIPSEmulator;
using MIPSEmulator.Assembly;
using PipelineSim.Config;
using PipelineSim.GUI;
#endregion

namespace PipelineSim
{
	public class MixTracker
	{
		#region Fields
		private const string fileName = "mix records.csv";
		private const string separator = ",\t";
		
		private readonly PipelineSimForm owner;
		private readonly Processor processor;
		
		private uint[] instructionCounts = new uint[Enum.GetValues(typeof(InstructionType)).Length];
		private bool enabled = false;
		#endregion
		
		#region Properties
		private bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					if (value)
						processor.ProgramFinished += WriteRecord;
					else
						processor.ProgramFinished -= WriteRecord;
					
					enabled = value;
				}
			}
		}
		#endregion
		
		#region Methods
		#region Event Handlers
		private void WriteRecord(object sender, EventArgs e)
		{
			FileInfo mixFile;
			StreamWriter mixWriter = null;
			
			try
			{
				mixFile = new FileInfo(fileName);
				
				if (!mixFile.Exists)
				{
					mixWriter = new StreamWriter(mixFile.Create());
					
					mixWriter.Write("Date");
					mixWriter.Write(separator);
					mixWriter.Write("File");
					foreach (string typeName in Enum.GetNames(typeof(InstructionType)))
					{
						mixWriter.Write(separator);
						mixWriter.Write(typeName);
					}
				}
				else
					mixWriter = mixFile.AppendText();
				
				mixWriter.WriteLine();
				mixWriter.Write(DateTime.Now.ToString("o"));
				mixWriter.Write(separator);
				mixWriter.Write(owner.OpenFileName);
				
				foreach (InstructionType type in Enum.GetValues(typeof(InstructionType)))
				{
					mixWriter.Write(separator);
					mixWriter.Write(instructionCounts[(int)type]);
				}
			}
			catch {}
			finally
			{
				if (mixWriter != null)
				{
					mixWriter.Flush();
					mixWriter.Close();
				}
			}
			
			return;
		}
		#endregion
		
		#region Constructors
		public MixTracker(PipelineSimForm owner, Processor processor)
		{
			EventHandler setEnabled = (sender, e) => { Enabled = GeneralSettings.GetSection(ConfigurationUserLevel.None).MixTracking; };
			
			this.owner = owner;
			this.processor = processor;
			
			processor.ExecutionStarted += (sender, e) => { for (byte i = 0; i < instructionCounts.Length; ++i) instructionCounts[i] = 0; };
			processor.ControlPathUpdated += (sender, e) => { if (!processor.MemoryRegister.HasBubble) ++instructionCounts[(int)processor.Program.GetInstruction(processor.MemoryRegister.InstructionAddress).Type]; };
			
			GeneralSettings.ChangesSaved += setEnabled;
			setEnabled(null, null);
		}
		#endregion
		#endregion
	}
}
