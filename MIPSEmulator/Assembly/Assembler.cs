#region Using Directives
using System;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace MIPSEmulator.Assembly
{
	#region XML Header
	/// <summary>
	/// Provides methods for assembling MIPS scripts (wrapper class for mips_assembler.dll).
	/// </summary>
	#endregion
	public sealed class Assembler
	{
		#region Fields
		private const string dllName = "mips_assembler.dll";
		#endregion
		
		#region Methods
		#region External Methods
		#region XML Header
		/// <summary>
		/// Assembles a MIPS script file.
		/// </summary>
		/// <param name="filePath">
		/// The absolute or relative path to the script file to be compiled, given as an ASCII-encoded
		/// byte array.
		/// </param>
		/// <param name="machineCode">
		/// An uninitialized <c>uint</c> pointer.  This will be set to point to an array containing
		/// the assembled machine code.
		/// </param>
		/// <param name="errors">
		/// An unititialized <c>byte**</c>.  This will be set to point to a array of null-terminated c strings
		/// describing any errors that were encountered, or <c>null</c> if there were no errors.
		/// </param>
		/// <returns>
		/// A <c>uint</c> specifying the number of instructions that were assembled -or- the length of the
		/// <paramref name="errors"/> array if it is not <c>null</c>.
		/// </returns>
		/// <remarks>
		/// <c>delete_machine_code_array()</c> should always be called immediately after
		/// processing the assembled machine code in order to prevent memory leaks.
		/// </remarks>
		/// <exception cref="System.DllNotFoundException">mips_assembler.dll could not be found.</exception>
		/// <exception cref="System.AccessViolationException">
		/// mips_assembler.dll needs to be debugged.
		/// </exception>
		#endregion
		[DllImport(dllName)]
		private unsafe static extern uint get_machine_code(byte[] filePath, out uint* machineCode, out byte** errors);
		
		#region XML Header
		/// <summary>
		/// Frees memory allocated by <c>get_machine_code()</c> to hold assembled machine code.
		/// </summary>
		/// <exception cref="System.DllNotFoundException">mips_assembler.dll could not be found.</exception>
		#endregion
		[DllImport(dllName)]
		private static extern void delete_machine_code_array();
		#endregion
		
		#region XML Header
		/// <summary>
		/// Assembles a MIPS script file.
		/// </summary>
		/// <param name="filePath">
		/// A <c>string</c> containing the path to the script to assemble.
		/// </param>
		/// <returns>
		/// An array of <c>MIPSEmulator.Assembly.Instruction</c>s representing the assembled machine code.
		/// </returns>
		/// <exception cref="MIPSEmulator.Assembly.AssemblyException">
		/// A syntax error or illegal lexeme was encountered.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="filePath"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.DllNotFoundException">mips_assembler.dll could not be found.</exception>
		/// <exception cref="System.AccessViolationException">
		/// mips_assembler.dll needs to be debugged.
		/// </exception>
		#endregion
		public static Instruction[] AssembleScript(string filePath)
		{
			Instruction[] machineCode;
			StringBuilder errors;
			
			if (filePath == null)
				throw new ArgumentNullException(filePath);
			
			unsafe
			{
				bool dllNotFound = false;
				uint* machineCodePtr = null;
				byte** errorsPtr = null;
				
				try
				{
					uint returnCount = get_machine_code(Encoding.ASCII.GetBytes(filePath), out machineCodePtr, out errorsPtr);
					
					if (errorsPtr == null)
					{
						machineCode = new Instruction[returnCount];
						for (uint i = 0; i < returnCount; ++i)
							machineCode[i] = (Instruction)(machineCodePtr[i]);
					}
					else
					{
						errors = new StringBuilder((int)returnCount);
						for (uint i = 0; i < returnCount; ++i)
						{
							for (byte* c = errorsPtr[i]; *c != 0; ++c)
								errors.Append((char)(*c));
							
							errors.Append('\n');
						}
						
						throw new AssemblyException(errors.ToString());
					}
				}
				catch (DllNotFoundException e)
				{
					dllNotFound = true;
					throw e;
				}
				finally
				{
					if (!dllNotFound)
						delete_machine_code_array();
				}
			}
			
			return machineCode;
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>Assembler</c> instance.
		/// Should never be used, as all members are static.
		/// </summary>
		#endregion
		private Assembler() {}
		#endregion
		#endregion
	}
}
