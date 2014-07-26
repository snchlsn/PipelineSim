#include "inst_node.h"
#include <stdlib.h>

inst_node* make_inst_node(const unsigned int inst)
{
	inst_node* node = (inst_node*)malloc(sizeof(inst_node));
	(*node).next = NULL;
	(*node).inst = inst;
	return node;
}
