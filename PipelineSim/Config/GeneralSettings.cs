#region Using Directives
using System;
using System.Configuration;
using MIPSEmulator;
#endregion

namespace PipelineSim.Config
{
	internal sealed class GeneralSettings: ConfigurationSection
	{
		#region Fields
		#region XML Header
		/// <summary>
		/// The maximum allowed period for the simulator's timed system clock, in tenths of a second.
		/// </summary>
		#endregion
		public const short MaxPeriod = 999;
		
		#region XML Header
		/// <summary>
		/// The minimum allowed period for the simulator's timed system clock, in tenths of a second.
		/// </summary>
		#endregion
		public const short MinPeriod = 1;
		
		private const string sectionName = "generalSettings";
		private const string clockPeriodName = "clockPeriod";
		private const string hazardModeName = "hazardMode";
		private const string editorCommandName = "editorCommand";
		private const string mixTrackingName = "mixTracking";
		
		private Configuration config;
		private bool modified;
		#endregion

		#region Configuration Properties
		#region XML Header
		//TODO: Document exceptions.
		/// <summary>
		/// Gets or sets the period of the system clock used to automate the simulator.
		/// </summary>
		/// <value>
		/// An <c>int</c> specifying the clock period, in tenths of a second.  The minimum value is <c>1</c>,
		/// the maximum value is <c>999</c>, and the default is <c>10</c>.
		/// </value>
		#endregion
		[ConfigurationProperty(clockPeriodName, IsKey = true, IsRequired = false, DefaultValue = 10)]
		[IntegerValidator(MinValue = 1, MaxValue = 999, ExcludeRange = false)]
		public int ClockPeriod
		{
			get { return (int)this[clockPeriodName]; }
			set
			{
				if (!this[clockPeriodName].Equals(value))
				{
					this[clockPeriodName] = value;
					modified = true;
				}
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the command used to launch the script editor.
		/// </summary>
		/// <value>
		/// A <c>string</c> containing the command used to launch the editor.
		/// </value>
		#endregion
		[ConfigurationProperty(editorCommandName, IsKey = true, IsRequired = false, DefaultValue = "notepad++")]
		public string EditorCommand
		{
			get { return (string)this[editorCommandName]; }
			set
			{
				if (!this[editorCommandName].Equals(value))
				{
					this[editorCommandName] = value;
					modified = true;
				}
			}
		}
		
		[ConfigurationProperty(mixTrackingName, IsKey = true, IsRequired = false, DefaultValue = false)]
		public bool MixTracking
		{
			get { return (bool)this[mixTrackingName]; }
			set
			{
				if ((bool)this[mixTrackingName] != value)
				{
					this[mixTrackingName] = value;
					modified = true;
				}
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the emulator's hazard mode.
		/// </summary>
		/// <value>
		/// A <c>MIPSEmulator.HazardMode</c> value specifying the emulator's capabilities in dealing
		/// with hazards.
		/// </value>
		#endregion
		[ConfigurationProperty(hazardModeName, IsKey = true, IsRequired = false, DefaultValue = HazardMode.Fail)]
		public HazardMode HazardMode
		{
			get { return (HazardMode)this[hazardModeName]; }
			set
			{
				if ((HazardMode)this[hazardModeName] != value)
				{
					this[hazardModeName] = value;
					modified = true;
				}
			}
		}
		#endregion
		
		#region Events
		#region XML Header
		/// <summary>
		/// Occurs when changes to the configuration properties of an instance of <c>GeneralSettings</c>
		/// are saved to the configuration file.
		/// </summary>
		#endregion
		public static event EventHandler ChangesSaved;
		#endregion

		#region Methods
		#region Event Raisers
		#region XML Header
		/// <summary>
		/// Raises the <c>ChangesSaved</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnChangesSaved(EventArgs e)
		{
			if (ChangesSaved != null)
				ChangesSaved(this, e);
			return;
		}
		#endregion
		
		public static GeneralSettings GetSection(ConfigurationUserLevel configLevel)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(configLevel);
			GeneralSettings generalSettings;
			
			generalSettings = (GeneralSettings)config.GetSection(sectionName);
			if (generalSettings == null)
			{
				generalSettings = new GeneralSettings();
				config.Sections.Add(sectionName, generalSettings);
			}
			generalSettings.config = config;
			
			return generalSettings;
		}
		
		#region XML Header
		/// <summary>
		/// Gets a value indicating whether any configuration properties have been modified since they were
		/// last saved.
		/// </summary>
		/// <returns>
		/// <c>true</c> if any configuration properties have been modified; <c>false</c> otherwise.
		/// </returns>
		#endregion
		protected override bool IsModified()
		{
			return modified;
		}
		
		#region XML Header
		/// <summary>
		/// Marks the current <c>GeneralSettings</c> as having no configuration properties that have been
		/// modified since they were last saved.
		/// </summary>
		#endregion
		protected override void ResetModified()
		{
			modified = false;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Saves the configuration to the config file.
		/// </summary>
		/// <param name="saveMode">
		/// A <c>System.Configuration.ConfigurationSaveMode</c> value that determines which property values to
		/// save.
		/// </param>
		/// <exception cref="System.Configuration.ConfigurationErrorsException">
		/// The configuration file could not be written -or- the configuration file has changed.
		/// </exception>
		#endregion
		public void Save(ConfigurationSaveMode saveMode = ConfigurationSaveMode.Modified)
		{
			if (IsModified() || saveMode == ConfigurationSaveMode.Full)
			{
				config.Save(saveMode);
				ResetModified();
				OnChangesSaved(EventArgs.Empty);
			}
			return;
		}

		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and intializes a new <c>GeneralSettings</c> instance.
		/// </summary>
		#endregion
		private GeneralSettings()
		{
			SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
		}
		#endregion
		#endregion
	}
}
