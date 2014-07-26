#!/bin/bash
flex --nounistd -o mips_lexer.c mips_lexer.l
bison -o mips_parser.h -r solved mips_parser.y