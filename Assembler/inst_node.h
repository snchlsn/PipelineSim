#ifndef INST_NODE
#define INST_NODE

typedef struct inst_node_t
{
	struct inst_node_t* next;
	unsigned int inst;
} inst_node;

inst_node* make_inst_node(const unsigned int inst);

#endif
