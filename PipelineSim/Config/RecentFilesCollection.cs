#region Using Directives
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security;
using System.Windows.Forms;
#endregion

namespace PipelineSim.Config
{
	#region XML Header
	/// <summary>
	/// A collection of <c>RecentFileElement</c>s.
	/// </summary>
	#endregion
	internal sealed class RecentFilesCollection: ConfigurationElementCollection
	{
		#region Fields
		#region XML Header
		/// <summary>
		/// A hard cap on the value of the <c>Capacity</c> property.
		/// </summary>
		#endregion
		public const int MaxCapacity = 20;
		
		private const string elementName = "mipsScript";
		private const string capacityName = "capacity";
		
		private bool modified = false;
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets the type of the <c>ConfigurationElementCollection</c>.
		/// </summary>
		/// <returns>
		/// One of the <c>System.Configuration.ConfigurationElementCollectionType</c> values.
		/// </returns>
		#endregion
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		#region XML Header
		/// <summary>
		/// Gets the name of elements of the collection.
		/// </summary>
		/// <returns>A <c>System.String</c> containing the element name.</returns>
		#endregion
		protected override string ElementName
		{
			get { return elementName; }
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the maximum number of <c>RecentFileElement</c>s to store.
		/// </summary>
		/// <returns>The maximum number of <c>RecentFileElement</c>s.</returns>
		/// <exception cref="System.Exception">
		/// The assigned value is less than <c>1</c> or greater than <c>MaxCapacity</c>.
		/// </exception>
		#endregion
		[ConfigurationProperty(capacityName, IsKey = true, DefaultValue = 5)]
		[IntegerValidator(MinValue = 1, MaxValue = MaxCapacity, ExcludeRange = false)]
		public int Capacity
		{
			get
			{
				return (int)this[capacityName];
			}
			set
			{
				if ((int)this[capacityName] == value)
				{
					this[capacityName] = value;
					modified = true;
				}
			}
		}
		#endregion
	   
		#region Operators
		#region XML Header
		/// <summary>
		/// Retrieve an file path in the collection by index.
		/// </summary>
		/// <param name="index">The index location of the file path to return.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is less than <c>0</c> -or- <paramref name="index"/> is greater than or equal
		/// to <c>Count</c>.
		/// </exception>
		#endregion
		public string this[int index]
		{
			get
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index", index, "index cannot be less than zero.");
				if (index >= Count)
					throw new ArgumentOutOfRangeException("index", index, "No file path exists at the specified index.");
				
				return ((RecentFileElement)BaseGet(index)).FilePath;
			}
			set
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index", index, "index cannot be less than zero.");
				if (index >= Count)
					throw new ArgumentOutOfRangeException("index", index, "No file path exists at the specified index.");
				
				if (((RecentFileElement)BaseGet(index)).FilePath != value)
				{
					if (BaseGet(index) != null)
						BaseRemoveAt(index);
					
					BaseAdd(index, new RecentFileElement(value));
					modified = true;
				}
			}
		}
		#endregion

		#region Methods
		#region XML Header
		/// <summary>
		/// Adds a <c>RecentFileElement</c> to the configuration file.
		/// </summary>
		/// <param name="element">The <c>RecentFileElement</c> to add.</param>
		#endregion
		private void Add(RecentFileElement element)
		{
			RecentFileElement[] newFiles = new RecentFileElement[Count];
			int capacity = Capacity;
			int i = 1;
			
			CopyTo(newFiles, 0);
			Clear();
			BaseAdd(element);
			foreach (RecentFileElement curFile in newFiles)
				if (curFile.FilePath != element.FilePath)
				{
					BaseAdd(curFile);
					if (++i >= capacity)
						break;
				}
			
			modified = true;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Adds a recent file to the configuration file.
		/// </summary>
		/// <param name="filePath">
		/// The path - relative or absolute - of the file to be added.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="filePath"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="filePath"/> is empty, contains only white space, contains one or more of the
		/// invalid characters defined by <c>System.IO.Path.GetInvalidPathChars</c>, or contains a colon that
		/// is not part of a volume identifier.
		/// </exception>
		/// <exception cref="System.IO.PathTooLongException">
		/// <paramref name="filePath"/> exceeds the length limits defined by the host operating system.
		/// </exception>
		/// <exception cref="System.Security.SecurityException">
		/// The caller does not have the permissions required to access the file specified by
		/// <paramref name="filePath"/>.
		/// </exception>
		#endregion
		public void Add(string filePath)
		{
			if (filePath == null)
				throw new ArgumentNullException("filePath");
			
			
			Add(new RecentFileElement(Path.GetFullPath(filePath)));
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Adds a collection of file paths to the configuration file.
		/// </summary>
		/// <param name="files">
		/// A <c>System.Collections.Generic.ICollection</c> of type <c>string</c>.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="files"/> is <c>null</c>.
		/// </exception>
		#endregion
		public void AddRange(ICollection<string> files)
		{
			if (files == null)
				throw new ArgumentNullException("files");
			
			foreach (string file in files)
				Add(file);
			
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Clears all recent files from the collection.
		/// </summary>
		#endregion
		public void Clear()
		{
			if (Count != 0)
			{
				BaseClear();
				modified = true;
			}
			return;
		}
	   
		#region XML Header
		/// <summary>
		/// Creates a new <c>RecentFileElement</c>.
		/// </summary>
		/// <returns>A new <c>RecentFileElement</c>.</returns>
		#endregion
		protected override ConfigurationElement CreateNewElement()
		{
			return new RecentFileElement();
		}
			   
		#region XML Header
		/// <summary>
		/// Checks for the existance of a given file in the <c>RecentFilesCollection</c>.
		/// </summary>
		/// <param name="filePath">The relative or absolute path of the file to check for.</param>
		/// <returns><c>true</c> if the file exists; <c>false</c> otherwise.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="filePath"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="filePath"/> is empty, contains only white space, contains one or more of the
		/// invalid characters defined by <c>System.IO.Path.GetInvalidPathChars</c>, or contains a colon that
		/// is not part of a volume identifier.
		/// </exception>
		/// <exception cref="System.IO.PathTooLongException">
		/// <paramref name="filePath"/> exceeds the length limits defined by the host operating system.
		/// </exception>
		/// <exception cref="System.Security.SecurityException">
		/// The caller does not have the permissions required to access the file specified by
		/// <paramref name="filePath"/>.
		/// </exception>
		#endregion
		public bool FileExists(string filePath)
		{
			if (filePath == null)
				throw new ArgumentNullException("filePath");
			
			filePath = Path.GetFullPath(filePath);
			foreach (RecentFileElement file in this)
				if (file.FilePath == filePath)
					return true;
			
			return false;
		}
		
		#region XML Header
		/// <summary>
		/// Gets the key of an element.
		/// </summary>
		/// <param name="element">The <c>ConfigurationElement</c> to get the key of.</param>
		/// <returns>The key of <paramref name="element"/>.</returns>
		#endregion
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((RecentFileElement)element).FilePath;
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
		public new bool IsModified()
		{
			return modified;
		}
	   
		#region XML Header
		/// <summary>
		/// Removes a <c>RecentFileElement</c> with the given name.
		/// </summary>
		/// <param name="filePath">
		/// The relative or absolute path of the <c>RecentFileElement</c> to remove.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="filePath"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.Exception">
		/// No <c>RecentFileElement</c> for the specified path exists in the collection.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="filePath"/> is empty, contains only white space, contains one or more of the
		/// invalid characters defined by <c>System.IO.Path.GetInvalidPathChars</c>, or contains a colon that
		/// is not part of a volume identifier.
		/// </exception>
		/// <exception cref="System.IO.PathTooLongException">
		/// <paramref name="filePath"/> exceeds the length limits defined by the host operating system.
		/// </exception>
		/// <exception cref="System.Security.SecurityException">
		/// The caller does not have the permissions required to access the file specified by
		/// <paramref name="filePath"/>.
		/// </exception>
		#endregion
		public void Remove(string filePath)
		{
			if (filePath == null)
				throw new ArgumentNullException("filePath");
			
			BaseRemove(Path.GetFullPath(filePath));
			modified = true;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Marks the current <c>RecentFilesCollection</c> as having no configuration properties that have been
		/// modified since they were last saved.
		/// </summary>
		#endregion
		public new void ResetModified()
		{
			modified = false;
		}
		#endregion
	}
}
