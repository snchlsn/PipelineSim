#if TEST
#region Using Directives
using System;
using System.IO;
using NUnit.Framework;
#endregion

namespace MIPSEmulator.Assembly
{
	#region XML Header
	/// <summary>
	/// Provides methods for testing the <c>Assembler</c> class.
	/// </summary>
	#endregion
	[TestFixture]
	public sealed class AssemblerTest
	{
		#region Fields
		private const string scriptName = "test_script.txt";
		private const string dumpName = "test_dump.masm";
		#endregion
		
		#region Test Methods
		#region XML Header
		/// <summary>
		/// Compiles a test script and dumps the machine code to a file.
		/// </summary>
		#endregion
		[Test]
		public void CompileAndDump()
		{
			Instruction[] machineCode = Assembler.AssembleScript(scriptName);
			byte[] machineCodeBytes = new byte[machineCode.Length << 2];
			
			for (int i = 0; i < machineCode.Length; ++i)
				Array.ConstrainedCopy(BitConverter.GetBytes((uint)machineCode[i]), 0, machineCodeBytes, i << 2, 4);
			
			File.WriteAllBytes(dumpName, machineCodeBytes);
			return;
		}
		#endregion
	}
}
#endif
