#region Using Directives
using System;
using System.IO;
using System.Configuration;
#endregion

namespace PipelineSim.Config
{
	#region XML Header
	/// <summary>
	/// An XML representation of a recently opened file.
	/// </summary>
	#endregion
	internal sealed class RecentFileElement: ConfigurationElement
	{
		#region Fields
		private const string filePathName = "filePath";
		#endregion
		
		#region Configuration Properties
		#region XML Header
		/// <summary>
		/// Gets or sets the full path of the recently opened file.
		/// </summary>
		#endregion
		[ConfigurationProperty(filePathName, IsKey = true, IsRequired = true)]
		//[StringValidator(MinLength = 4, InvalidCharacters = "<>?\"*|")] //This apparently doesn't work like it's supposed to.
		public string FilePath
		{
			get { return (string)this[filePathName]; }
			set { this[filePathName] = value; }
		}
		#endregion
		
		#region Constructors
		public RecentFileElement(): base() {}
		
		public RecentFileElement(string fileName): base()
		{
			FilePath = fileName;
		}
		#endregion
	}
}
