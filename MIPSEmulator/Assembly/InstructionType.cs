namespace MIPSEmulator.Assembly
{
	public enum InstructionType: byte
	{
		Immediate = 0,
		Register,
		Jump,
		Branch,
		Memory
	}
}
