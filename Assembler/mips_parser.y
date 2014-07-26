%{
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include "inst_node.h"
#include "label_tree.h"

#define ILLEGAL_LEXEME 0
#define SYNTAX_ERROR 1
#define UNRECOGNIZED_INSTRUCTION 2

#ifdef DEBUG
#define YYDEBUG 1 //Enable compilation of the debug code.
int yydebug = 1; //Tell bison to print a trace of the parse.
#endif

unsigned int get_machine_code(char* file_name, unsigned int** machine_code_ptr, char*** errors_ptr);
void machine_code_to_array(); //Converts the list at machine_code_head to an array, and stores it in machine_code_array.
void delete_machine_code_head(); //Calls delete_machine_code() on machine_code_head.
void delete_machine_code(inst_node* head);
void delete_machine_code_array();

extern int yylex();
extern void yyerror(const char* msg);

extern int yylineno;
extern FILE* yyin;
extern int err_type;
extern char** errors;
extern unsigned int errors_len;
unsigned int inst_num = 0;
inst_node* machine_code_head = NULL;
unsigned int* machine_code_array = NULL; //Used for passing assembled machine code to client code.
%}

%error-verbose //Tell bison to give useful error messages.
%define parse.lac full

%union
{
	inst_node* node;
	int iVal;
	unsigned int uiVal;
	char* label;
}

%token ADDIU ADDU AND ANDI BEQ J LW NOP NOR OR ORI SLLV SRAV SRLV SUBU SW XOR XORI REG NUM LABEL

%nonassoc LOW
%nonassoc LABEL
%nonassoc MED
%nonassoc HIGH

%%

instructions:	instructions instruction_incr	{ if ($<node>1 == NULL) machine_code_head = $<node>2; else (*$<node>1).next = $<node>2; $<node>$ = $<node>2; }	|
				instructions unrecognized_inst	{ sprintf(yymsg, "unrecognized instruction \"%s\"", $<label>2); yyerror(yymsg); free($<label>2); }	|
				instructions error '\n'	{ yyclearin; yyerrok; }	|
				instructions '\n'	|
				/* Empty */	{ $<node>$ = NULL; }	;

instruction_incr:	instruction	{ ++inst_num; $<node>$ = $<node>1; }	|
					LABEL ':' instruction	{	try_add_label_node(inst_num++ << 2, $<label>1);
												#ifdef DEBUG
												printf("\nInstruction %d is labeled %s.\n", inst_num - 1, $<label>1);
												#endif
												free($<label>1);
												$<node>$ = $<node>3;
											}	%prec HIGH;

unrecognized_inst:	LABEL REG ',' REG ',' REG	{ $<label>$ = $<label>1; }	%prec MED	|
					LABEL REG ',' REG ',' NUM	{ $<label>$ = $<label>1; }	%prec MED	|
					LABEL REG ',' NUM '(' REG ')'	{ $<label>$ = $<label>1; }	%prec MED	|
					LABEL REG ',' REG ',' LABEL	{ $<label>$ = $<label>1; free($<label>2); }	%prec MED	|
					LABEL LABEL	{ $<label>$ = $<label>1; free($<label>2); }	%prec MED	|
					LABEL	{ $<label>$ = $<label>1; }	%prec LOW	;

instruction:	r_op REG ',' REG ',' REG	{ $<node>$ = make_inst_node(($<uiVal>4 << 21) | ($<uiVal>6 << 16) | ($<uiVal>2 << 11) | $<uiVal>1); }	|
				sv_op REG ',' REG ',' REG	{ $<node>$ = make_inst_node(($<uiVal>6 << 21) | ($<uiVal>4 << 16) | ($<uiVal>2 << 11) | $<uiVal>1); }	|
				i_op REG ',' REG ',' NUM	{ $<node>$ = make_inst_node(($<uiVal>1 << 26) | ($<uiVal>4 << 21) | ($<uiVal>2 << 16) | ($<uiVal>6 & 0xFFFF)); } 	|
				mem_op REG ',' NUM '(' REG ')'	{ $<node>$ = make_inst_node(($<uiVal>1 << 26) | ($<uiVal>6 << 21) | ($<uiVal>2 << 16) | (($<uiVal>4) & 0xFFFF)); }	|
				BEQ REG ',' REG ',' LABEL	{ $<node>$ = make_inst_node((0x04 << 26) | ($<uiVal>2 << 21) | ($<uiVal>4 << 16)); add_jb_ref_node($<node>$, inst_num << 2, $<label>6); free($<label>6); }	|
				J LABEL	{ $<node>$ = make_inst_node(0x02 << 26); add_jb_ref_node($<node>$, inst_num << 2, $<label>2); free($<label>2); }	|
				NOP	{ $<node>$ = make_inst_node(0x25); }	;

r_op:	ADDU	{ $<uiVal>$ = 0x21; }	|
		AND	{ $<uiVal>$ = 0x24; }	|
		NOR	{ $<uiVal>$ = 0x27; }	|
		OR	{ $<uiVal>$ = 0x25; }	|
		SUBU	{ $<uiVal>$ = 0x23; }	|
		XOR	{ $<uiVal>$ = 0x26; }	;

sv_op:	SLLV	{ $<uiVal>$ = 0x04; }	|
		SRAV	{ $<uiVal>$ = 0x07; }	|
		SRLV	{ $<uiVal>$ = 0x06; }	;

i_op:	ADDIU	{ $<uiVal>$ = 0x09; }	|
		ANDI	{ $<uiVal>$ = 0x0C; }	|
		ORI	{ $<uiVal>$ = 0x0D; }	|
		XORI	{ $<uiVal>$ = 0x0E; }	;

mem_op:	LW	{ $<uiVal>$ = 0x23; }	|
		SW	{ $<uiVal>$ = 0x2B; }	;

%%

unsigned int get_machine_code(char* file_path, unsigned int** machine_code_ptr, char*** errors_ptr)
{
	inst_num = 0;
	yylineno = 1;
	yyin = fopen(file_path, "r");
	yyparse();
	fclose(yyin);
	
	if (errors == NULL)
	{
		process_refs_root();
		machine_code_to_array();
		delete_machine_code_head();
		delete_label_tree_root();
	}
	
	(*machine_code_ptr) = machine_code_array;
	(*errors_ptr) = errors;
	return (errors == NULL ? inst_num : errors_len);
}

void machine_code_to_array()
{
	inst_node* curr_node = machine_code_head;
	unsigned int i = 0;

	if (inst_num > 0)
	{
		machine_code_array = (unsigned int*)malloc(sizeof(unsigned int) * inst_num);
		for (; curr_node != NULL; curr_node = (*curr_node).next)
			machine_code_array[i++] = (*curr_node).inst;
	}

	return;
}

void delete_machine_code_head()
{
	if (machine_code_head != NULL)
	{
		delete_machine_code(machine_code_head);
		machine_code_head = NULL;
	}
	return;
}

void delete_machine_code(inst_node* head)
{
	if ((*head).next != NULL)
		delete_machine_code((*head).next);

	free(head);
	return;
}

void delete_machine_code_array()
{
	if (machine_code_array != NULL)
	{
		free(machine_code_array);
		machine_code_array = NULL;
	}

	if (errors != NULL)
	{
		free(errors);
		errors = NULL;
		errors_len = 0;
	}
	return;
}

#ifdef DEBUG
int main(int argc, char** argv)
{
	unsigned int* test_code;
	unsigned int i;

	if (argc != 2)
	{
		fprintf(stderr, "Usage:\nmips_parser filename\nwhere filename is the name of the file to be parsed.\n");
		return 1;
	}

	get_machine_code(argv[1], &test_code, &errors);
	
	for (i = 0; i < errors_len; ++i)
		printf("%s\n", errors[i]);
	
	delete_machine_code_array();
	return 0;
}
#endif
