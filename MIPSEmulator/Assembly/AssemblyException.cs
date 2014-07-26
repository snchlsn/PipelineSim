
using System;
using System.Runtime.Serialization;

namespace MIPSEmulator.Assembly
{
	public class AssemblyException : Exception, ISerializable
	{
		public AssemblyException()
		{
		}

	 	public AssemblyException(string message) : base(message)
		{
		}

		public AssemblyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected AssemblyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}