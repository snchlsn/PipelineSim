#region Functional Requirements
/*
1. Implement the following machine instructions: addiu, addu, subu, and, andi, nor, or, ori, xor, xori, sllv, srav, srlv, sw, lw, j, beq. [1]
2. Implement the nop pseudo-instruction.
3. Display in its main window a block diagram of the processor being emulated.
4. Provide an option to switch between a version of the processor that supports stalls and forwarding, a version that supports only stalls, and a version that supports neither.
5. Be able to parse and execute scripts written in MIPS assembly and limited to the implemented instructions.
6. Allow manual stepping through instructions, via keyboard shortcut or menu interface.
7. Allow automatic stepping through instructions at a set rate.
8. Allow the emulator to be reset and returned to the start of the loaded script.
9. Display, in hexadecimal, the current contents of the instruction pointer.
10. Display, in hexadecimal, the current contents of each register separating two stages of the pipeline.
11. Display mouse-over text for each pipeline register, giving the name of each bit field and its current value.
12. Display mouse-over text for each signal or bus, giving its name and current value.
13. Draw data forwarding buses in green rather than black during cycles when they are being used.
14. Provide a window listing all registers and their current contents, in hexadecimal and signed decimal.
15. Provide a window listing all used locations in data memory and their current contents, in hexadecimal and signed decimal.
16. Display a message giving the line number of the error if an unimplemented instruction or syntax error is encountered.
17. Cease emulation and display an error message if stalls and forwarding are disabled and a hazard is detected.
18. Track relative frequency of instruction types in each executed script, and save results to a file.
19. Provide a window giving a complete disassembly listing of the loaded script.
*/
#endregion

#region Functional Non-requirements
/*
2.	Maintain a list of recently opened scripts.
	STATUS: Implemented and functioning correctly.
*/
#endregion

#region Using Directives
using System;
using System.Windows.Forms;
#endregion

namespace PipelineSim
{
	#region XML Header
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	#endregion
	internal sealed class Program
	{
		#region XML Header
		/// <summary>
		/// Program entry point.
		/// </summary>
		#endregion
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PipelineSim.GUI.PipelineSimForm());
		}
	}
}
