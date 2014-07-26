#region Using Directives
using System;
using System.Collections;
using System.Linq;
using MIPSEmulator.Assembly;
#endregion

namespace MIPSEmulator
{
	public sealed class ProgramMemory: IEnumerable
	{
		#region Fields
		private readonly Instruction[] program;
		#endregion
		
		#region Methods
		public IEnumerator GetEnumerator()
		{
			return program.GetEnumerator();
		}
		
		public Instruction GetInstruction(uint address)
		{
			uint index;
			
			if ((address & 3) != 0)
				throw new ArgumentException("Addresses must be word-aligned.", "address");
			
			index = address >> 2;
			if (index >= program.Length)
				throw new ArgumentOutOfRangeException("address", address, "No instruction exists at the specified address.");
			
			return program[index];
		}
		
		#region Constructors
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>ProgramMemory</c> instance.
		/// </summary>
		/// <param name="program">
		/// An array of <c>MIPSEmulator.Assembly.Instruction</c>s representing the program to be loaded.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="program"/> is <c>null</c>.
		/// </exception>
		#endregion
		public ProgramMemory(Instruction[] program)
		{
			if (program == null)
				throw new ArgumentNullException("program");
			
			this.program = program;
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>ProgramMemory</c> instance.
		/// </summary>
		/// <param name="program">
		/// An array of MIPS machine instructions, encoded as <c>uint</c>s, representing the program
		/// to be loaded.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="program"/> is <c>null</c>.
		/// </exception>
		#endregion
		public ProgramMemory(uint[] program)
		{
			if (program == null)
				throw new ArgumentNullException("program");
			
			this.program = program.Cast<Instruction>().ToArray();
		}
		#endregion
		#endregion
	}
}
