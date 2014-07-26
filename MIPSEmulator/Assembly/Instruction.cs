#region Using Directives
using System;
using System.ComponentModel;
using System.Text;
#endregion

namespace MIPSEmulator.Assembly
{
	#region XML Header
	/// <summary>
	/// Represents a MIPS machine instruction.
	/// </summary>
	#endregion
	public struct Instruction
	{
		#region Fields
		#region XML Header
		/// <summary>
		/// The instruction's 32-bit machine code.
		/// </summary>
		#endregion
		private readonly uint rawInstruction;
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets the instruction's function code field.
		/// </summary>
		/// <value>The 6-bit function code, cast as a <c>MIPSEmulator.Assembly.Function</c>.</value>
		/// <remarks>This property is only meaningful for R-type instructions.</remarks>
		#endregion
		public Function Function
		{
			get { return (Function)(rawInstruction & 0x3F); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the instruction's immediate value field.
		/// </summary>
		/// <value>The 16-bit immediate value field, cast as a <c>short</c>.</value>
		/// <remarks>This property is only meaningful for I-type instructions.</remarks>
		#endregion
		public short Immediate
		{
			get { return BitConverter.ToInt16(BitConverter.GetBytes((ushort)(rawInstruction & 0xFFFF)), 0); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the instruction's opcode field.
		/// </summary>
		/// <value>The 6-bit opcode field, cast as a <c>MIPSEmulator.Assembly.Opcode</c>.</value>
		#endregion
		public Opcode Opcode
		{
			get { return (Opcode)(rawInstruction >> 26); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the instruction's rd field.
		/// </summary>
		/// <value>The 5-bit rd field, cast as a <c>MIPSEmulator.Assembly.Register</c>.</value>
		/// <remarks>This property is only meaningful for R-type instructions.</remarks>
		#endregion
		public Register RD
		{
			get { return (Register)((rawInstruction >> 11) & 0x1F); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the instruction's rs field.
		/// </summary>
		/// <value>The 5-bit rs field, cast as a <c>MIPSEmulator.Assembly.Register</c>.</value>
		/// <remarks>This property is not meaningful for jump instructions.</remarks>
		#endregion
		public Register RS
		{
			get { return (Register)((rawInstruction >> 21) & 0x1F); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the instruction's rt field.
		/// </summary>
		/// <value>The 5-bit rt field, cast as a <c>MIPSEmulator.Assembly.Register</c>.</value>
		/// <remarks>This property is not meaningful for jump instructions.</remarks>
		#endregion
		public Register RT
		{
			get { return (Register)((rawInstruction >> 16) & 0x1F); }
		}
		
		#region XML Header
		/// <summary>
		/// Gets the instruction's jump target address.
		/// </summary>
		/// <value>The 26-bit jump target field, shifted left 2 bits and cast as a <c>uint</c>.</value>
		/// <remarks>This property is only meaningful for jump instructions.</remarks>
		#endregion
		public uint Target
		{
			get { return (rawInstruction & 0x03FFFFFF) << 2; }
		}
		
		public InstructionType Type
		{
			get
			{
				switch (Opcode)
				{
					case Opcode.J:
						return InstructionType.Jump;
					
					case Opcode.Beq:
						return InstructionType.Branch;
					
					case Opcode.RType:
						return InstructionType.Register;
					
					case Opcode.Lw:
						return InstructionType.Memory;
					
					case Opcode.Sw:
						return InstructionType.Memory;
					
					default:
						return InstructionType.Immediate;
				}
			}
		}
		#endregion
		
		#region Operators
		public static explicit operator Instruction(uint val)
		{
			try
			{
				return new Instruction(val);
			}
			catch (ArgumentException e)
			{
				throw new InvalidCastException("The uint being cast does not represent a valid instruction.", e);
			}
		}
		
		public static explicit operator uint(Instruction inst)
		{
			return inst.rawInstruction;
		}
		#endregion
		
		#region Methods
		#region XML Header
		/// <summary>
		/// Gets a <c>string</c> representation of the <c>Instruction</c>.
		/// </summary>
		/// <returns>
		/// A <c>string</c> containing the MIPS Assembly equivalent of the <c>Instruction</c>.
		/// </returns>
		#endregion
		public override string ToString()
		{
			StringBuilder assemblyInstruction = new StringBuilder(25);
			
			if (Opcode == Opcode.RType)
			{
				assemblyInstruction.Append(Enum.GetName(typeof(Function), Function).ToLower());
				assemblyInstruction.Append(" $").Append(Enum.GetName(typeof(Register), RD).ToLower());
				assemblyInstruction.Append(", $");
				
				if (Function == Function.Sllv || Function == Function.Srav || Function == Function.Srlv)
				{
					assemblyInstruction.Append(Enum.GetName(typeof(Register), RT).ToLower());
					assemblyInstruction.Append(", $").Append(Enum.GetName(typeof(Register), RS).ToLower());
				}
				else
				{
					assemblyInstruction.Append(Enum.GetName(typeof(Register), RS).ToLower());
					assemblyInstruction.Append(", $").Append(Enum.GetName(typeof(Register), RT).ToLower());
				}
			}
			else
			{
				assemblyInstruction.Append(Enum.GetName(typeof(Opcode), Opcode).ToLower()).Append(' ');
				
				if (Opcode == Opcode.Lw || Opcode == Opcode.Sw)
				{
					assemblyInstruction.Append('$').Append(Enum.GetName(typeof(Register), RT).ToLower()).Append(", ");
					assemblyInstruction.Append(Immediate.ToString()).Append("($").Append(Enum.GetName(typeof(Register), RS).ToLower()).Append(')');
				}
				else if (Opcode == Opcode.J)
					assemblyInstruction.Append("0x").Append(Target.ToString("X7"));
				else if (Opcode == Opcode.Beq)
				{
					assemblyInstruction.Append('$').Append(Enum.GetName(typeof(Register), RS).ToLower());
					assemblyInstruction.Append(", $").Append(Enum.GetName(typeof(Register), RT).ToLower());
					assemblyInstruction.Append(", ").Append(Immediate.ToString());
				}
				else
				{
					assemblyInstruction.Append('$').Append(Enum.GetName(typeof(Register), RT).ToLower());
					assemblyInstruction.Append(", $").Append(Enum.GetName(typeof(Register), RS).ToLower());
					assemblyInstruction.Append(", ").Append(Immediate.ToString());
				}
			}
			
			#if DEBUG
			if (assemblyInstruction.Capacity > 25)
				throw new Exception("StringBuilder exceeded initial capacity.");
			#endif
			
			return assemblyInstruction.ToString();
		}

		
		#region Constructors
		public Instruction(uint rawInstruction)
		{
			this.rawInstruction = rawInstruction;
			
			#region Exceptions
			if (!Enum.IsDefined(typeof(Opcode), Opcode))
			    throw new ArgumentException("The opcode (" + ((byte)Opcode).ToString() + ") is invalid.", "rawInstruction");
			if (Opcode == Opcode.RType && !Enum.IsDefined(typeof(Function), Function))
				throw new ArgumentException("The instruction is an R-type and the function field (" + ((byte)Function).ToString() + ") is invalid.", "rawInstruction");
			#endregion
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>Instruction</c> instance representing a MIPS R-type instruction.
		/// </summary>
		/// <param name="function">
		/// A <c>MIPSEmulator.Assembly.Function</c> value specifying the instruction's function code field.
		/// </param>
		/// <param name="rd">
		/// A <c>MIPSEmulator.Assembly.Register</c> value specifying the instruction's rd field.
		/// </param>
		/// <param name="rs">
		/// A <c>MIPSEmulator.Assembly.Register</c> value specifying the instruction's rs field.
		/// </param>
		/// <param name="rt">
		/// A <c>MIPSEmulator.Assembly.Register</c> value specifying the instruction's rs field.
		/// </param>
		/// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
		/// <paramref name="function"/> is not one of the <c>MIPSEmulator.Assembly.Function</c> values -or-
		/// <paramref name="rd"/> is not one of the <c>MIPSEmulator.Assembly.Register</c> values -or-
		/// <paramref name="rs"/> is not one of the <c>MIPSEmulator.Assembly.Register</c> values -or-
		/// <paramref name="rt"/> is not one of the <c>MIPSEmulator.Assembly.Register</c> values.
		/// </exception>
		#endregion
		public Instruction(Function function, Register rd, Register rs, Register rt)
		{
			#region Exceptions
			if (!Enum.IsDefined(typeof(Function), function))
				throw new InvalidEnumArgumentException("function", (int)function, typeof(Function));
			if (!Enum.IsDefined(typeof(Register), rd))
				throw new InvalidEnumArgumentException("rd", (int)rd, typeof(Register));
			if (!Enum.IsDefined(typeof(Register), rs))
				throw new InvalidEnumArgumentException("rs", (int)rs, typeof(Register));
			if (!Enum.IsDefined(typeof(Register), rt))
				throw new InvalidEnumArgumentException("rt", (int)rt, typeof(Register));
			#endregion
			
			rawInstruction = ((uint)rs << 21) | ((uint)rt << 16) | ((uint)rd << 11) | (uint)function;
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>Instruction</c> instance representing a MIPS I-type instruction.
		/// </summary>
		/// <param name="opcode">
		/// A <c>MIPSEmulator.Assembly.Opcode</c> value specifying the instruction's opcode field.
		/// </param>
		/// <param name="rs">
		/// A <c>MIPSEmulator.Assembly.Register</c> value specifying the instruction's rs field.
		/// </param>
		/// <param name="rt">
		/// A <c>MIPSEmulator.Assembly.Register</c> value specifying the instruction's rs field.
		/// </param>
		/// <param name="immediate">
		/// A <c>short</c> specifying the instruction's immediate value field.
		/// </param>
		/// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
		/// <paramref name="opcode"/> is not one of the <c>MIPSEmulator.Assembly.Opcode</c> values -or-
		/// <paramref name="rs"/> is not one of the <c>MIPSEmulator.Assembly.Register</c> values -or-
		/// <paramref name="rt"/> is not one of the <c>MIPSEmulator.Assembly.Register</c> values.
		/// </exception>
		#endregion
		public Instruction(Opcode opcode, Register rs, Register rt, short immediate)
		{
			#region Exceptions
			if (!Enum.IsDefined(typeof(Opcode), opcode))
				throw new InvalidEnumArgumentException("opcode", (int)opcode, typeof(Opcode));
			if (!Enum.IsDefined(typeof(Register), rs))
				throw new InvalidEnumArgumentException("rs", (int)rs, typeof(Register));
			if (!Enum.IsDefined(typeof(Register), rt))
				throw new InvalidEnumArgumentException("rt", (int)rt, typeof(Register));
			#endregion
			
			rawInstruction = ((uint)opcode << 26) | ((uint)rs << 21) | ((uint)rt << 16) | (uint)BitConverter.ToUInt16(BitConverter.GetBytes(immediate), 0);
		}
		
		#region XML Header
		/// <summary>
		/// Creates and initializes a new <c>Instruction</c> instance representing a MIPS jump instruction.
		/// </summary>
		/// <param name="target">The target address of the jump.</param>
		/// <param name="jump">Dummy argument.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="target"/> is not word-aligned.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="target"/> is greater than <c>0x0FFFFFFC</c>.
		/// </exception>
		#endregion
		public Instruction(uint target, bool jump)
		{
			#region Exceptions
			if ((target & 0x00000003) != 0)
				throw new ArgumentException("The target address must be word-aligned.", "target");
			if ((target & 0xF0000000) != 0)
				throw new ArgumentOutOfRangeException("target", target, "The target address must be no greater than 0x0FFFFFFC.");
			#endregion
			
			rawInstruction = ((uint)Opcode.J << 26) | (target >> 2);
		}
		#endregion
		#endregion
	}
}
