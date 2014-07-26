%{
#include <string.h>
#include "mips_parser.h"

extern int yyerrstatus;

void yyerror(const char* msg);
void yywarning(const char* msg);

int err_type = -1;
char** errors = NULL;
unsigned int errors_len = 0;
%}

%option noyywrap
%option yylineno

%%

  /* Comment rules */;
"//".*$	++yylineno;
  /* "/*"([^\*]|\*[^/])*"*//*";

  /* Whitespace rules */;
\n	{ /*++yylineno*/; return '\n'; }	;
[ \t\r]+	;
<<EOF>>	{ static int once = 0; return ((once = !once) ? '\n' : 0); }	;

  /* Mnemonic rules */;
(?i:addiu)	return ADDIU;
(?i:addu)	return ADDU;
(?i:and)	return AND;
(?i:andi)	return ANDI;
(?i:beq)	return BEQ;
(?i:j)	return J;
(?i:lw)	return LW;
(?i:nop)	return NOP;
(?i:nor)	return NOR;
(?i:or)	return OR;
(?i:ori)	return ORI;
(?i:sllv)	return SLLV;
(?i:srav)	return SRAV;
(?i:srlv)	return SRLV;
(?i:subu)	return SUBU;
(?i:sw)	return SW;
(?i:xor)	return XOR;
(?i:xori)	return XORI;

  /* Number and register rules */;
\-?[0-9]+	{ yylval.iVal = atoi(yytext); return NUM; }

"$"t[0-7]	{ yylval.uiVal = yytext[2] + 8 - 0x30; return REG; }	;
"$"t[89]	{ yylval.uiVal = yytext[2] + 24 - 0x30; return REG; }	;
"$"v[01]	{ yylval.uiVal = yytext[2] + 2 - 0x30; return REG; }	;
"$"a[0-3]	{ yylval.uiVal = yytext[2] + 4 - 0x30; return REG; }	;
"$"k[01]	{ yylval.uiVal = yytext[2] + 26 - 0x30; return REG; }	;
"$"s[0-7]	{ yylval.uiVal = yytext[2] + 16 - 0x30; return REG; }	;
"$"((s8)|(fp))	{ yylval.uiVal = 30; return REG; }	;
"$"at	{ yylval.uiVal = 1; return REG; }	;
"$"gp	{ yylval.uiVal = 28; return REG; }	;
"$"sp	{ yylval.uiVal = 29; return REG; }	;
"$"ra	{ yylval.uiVal = 31; return REG; }	;
"$"zero	{ yylval.uiVal = 0; return REG; }	;
"$"([0-9]|[12][0-9]|3[01])	{ yylval.uiVal = atoi(&(yytext[1])); return REG; }	;

  /* Label rule */;
[[:alpha:]][[:alnum:]]*	{ yylval.label = (char*)malloc(sizeof(yytext) + 1); strcpy(yylval.label, yytext); return LABEL; }	;

  /* Punctuation rules */;
"("	return '(';
")"	return ')';
","	return ',';
:	return ':';

  /* Illegal input rule */;
[^(),: \t\n\r\-]+	{
						char msg[255];

						sprintf(msg, "illegal lexeme \"%s\"", yytext);
						yyerror(msg);
					}	;

%%

void yyerror(const char* msg)
{
	char** new_errors;
	char err_buf[255];
	int err_buf_len, i;

	/*
	switch (err_type)
	{
		case ILLEGAL_LEXEME:
			err_buf_len = sprintf(err_buf, "Illegal lexeme \"%s\" near line %d", yytext, yylineno);
			break;
		case SYNTAX_ERROR:
			err_buf_len = sprintf(err_buf, "Syntax error near \"%s\", line %d", yytext, yylineno);
			break;
		case UNRECOGNIZED_INSTRUCTION:
			err_buf_len = sprintf(err_buf, "Unrecognized instruction \"%s\" near line %d", msg, yylineno);
			break;
		default:
			return;
	}

	if (errors != NULL)
		if (strcmp(errors[errors_len - 1], err_buf) == 0)
		{
			err_type = -1;
			return;
		}
	*/

	err_buf_len = sprintf(err_buf, "Near line %d: %s", yylineno, msg);

	new_errors = (char**)malloc(++errors_len * sizeof(char*));
	for (i = 0; i < errors_len - 1; ++i)
		new_errors[i] = errors[i];

	new_errors[errors_len - 1] = (char*)malloc(err_buf_len + 1);

	if (errors != NULL)
		free(errors);

	strcpy(new_errors[errors_len - 1], err_buf);
	errors = new_errors;
	//err_type = -1;
	return;
}
