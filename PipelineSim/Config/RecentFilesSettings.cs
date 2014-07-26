#region Using Directives
using System;   
using System.Configuration;
#endregion

namespace PipelineSim.Config
{
	internal sealed class RecentFilesSettings: ConfigurationSection
	{
		#region Fields
		private const string sectionName = "recentFilesSettings";
		private const string recentFilesName = "recentFilesCollection";
		
		private Configuration config;
		private RecentFilesCollection recentFiles;
		private bool modified;
		#endregion
		
		#region Configuration Properties
		#region XML Header
		/// <summary>
		/// Gets the collection of recent files.
		/// </summary>
		/// <returns>A <c>PipelineSim.Config.RecentFilesCollection</c>.</returns>
		#endregion
		[ConfigurationProperty(recentFilesName, IsDefaultCollection = true)]
		public RecentFilesCollection RecentFiles
		{
			get
			{
				if (recentFiles == null)
					recentFiles = (RecentFilesCollection)base[recentFilesName];
				
				return recentFiles;
			}
			private set
			{
				base[recentFilesName] = value;
			}
		}
		#endregion
		
		#region Events
		#region XML Header
		/// <summary>
		/// Occurs when changes to the configuration properties of an instance of <c>RecentFilesSettings</c>
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
			return modified || RecentFiles.IsModified();
		}
		
		#region XML Header
		/// <summary>
		/// Marks the current <c>RecentFilesSettings</c> as having no configuration properties that have been
		/// modified since they were last saved.
		/// </summary>
		#endregion
		protected override void ResetModified()
		{
			modified = false;
			RecentFiles.ResetModified();
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Saves the configuration to the config file.
		/// </summary>
		/// <param name="saveMode">
		/// A <c>System.Configuration.ConfigurationSaveMode</c> value that determines which property values to
		/// save.  The default is <c>ConfigurationSaveMode.Modified</c>.
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
				OnChangesSaved(EventArgs.Empty);
			}
			return;
		}
		
		/// <summary>
		/// Gets the current applications &lt;RecentFiles&gt; section.
		/// </summary>
		/// <param name="ConfigLevel">
		/// The &lt;ConfigurationUserLevel&gt; that the config file
		/// is retrieved from.
		/// </param>
		/// <returns>
		/// The configuration file's &lt;RecentFiles&gt; section.
		/// </returns>
		public static RecentFilesSettings GetSection(ConfigurationUserLevel configLevel)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(configLevel);
			RecentFilesSettings recentFilesSettings;
			
			recentFilesSettings = (RecentFilesSettings)config.GetSection(sectionName);
			if (recentFilesSettings == null)
			{
				recentFilesSettings = new RecentFilesSettings();
				config.Sections.Add(sectionName, recentFilesSettings);
			}
			recentFilesSettings.config = config;
			
			return recentFilesSettings;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>RecentFilesSettings</c> instance.
		/// </summary>
		#endregion
		private RecentFilesSettings()
		{
			SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
		}
		#endregion
		#endregion
	}
}
