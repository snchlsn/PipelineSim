#ifndef LABEL_TREE
#define LABEL_TREE

#include "inst_node.h"
#include <stdlib.h>

typedef struct jb_ref_node_t
{
	struct jb_ref_node_t* next;
	inst_node* jb_node;
	unsigned int address;
} jb_ref_node;

typedef struct label_node_t
{
	struct label_node_t* left_child;
	struct label_node_t* right_child;
	char* label;
	unsigned int address;
	jb_ref_node* jb_refs;
} label_node;

jb_ref_node* make_jb_ref_node(inst_node* jb_node, const unsigned int address);
label_node* make_label_node(const char* label);
void add_jb_ref_node(inst_node* jb_node, const unsigned int address, const char* label);
int try_add_label_node(const unsigned int address, const char* label);
label_node* get_label_node_root(const char* label);
label_node* get_label_node(const char* label, label_node* root);
label_node* force_get_label_node_root(const char* label);
label_node* force_get_label_node(const char* label, label_node* root);
void process_refs_root();
void process_refs(label_node* root);
void process_refs_label(jb_ref_node* head, const unsigned int label_address);
void delete_label_tree_root();
void delete_label_tree(label_node* root);
void delete_jb_ref_list(jb_ref_node* head);

label_node* label_tree_root;

#endif
