PipelineSim
===========

Simplified MIPS pipeline simulator (Software Senior Project)

This program, intended to be a learning tool, parses spripts written in a subset of MIPS assembly and displays a block diagram of a virtual processor, showing graphically how the instructions pass through the pipeline.  It supports most 32-bit unsigned int arithmetic and logical operations (no mult/div), memory load/store, branch if zero, the unconditional jump, and the nop pseudo-instruction.  Branches and jumps have one delay slot.  Note that while most of the project is implemented in C#, the assembler is written in C with Flex and Bison, and must be compiled separately.  For more details, see the project proposal in the docs directory.
