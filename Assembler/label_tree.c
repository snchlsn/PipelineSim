#include "label_tree.h"
#include <string.h>
#include <stdio.h>

jb_ref_node* make_jb_ref_node(inst_node* jb_node, const unsigned int address)
{
	jb_ref_node* node = (jb_ref_node*)malloc(sizeof(jb_ref_node));
	(*node).next = NULL;
	(*node).jb_node = jb_node;
	(*node).address = address;
	return node;
}

label_node* make_label_node(const char* label)
{
	label_node* node = (label_node*)malloc(sizeof(label_node));
	(*node).left_child = NULL;
	(*node).right_child = NULL;
	(*node).jb_refs = NULL;
	(*node).address = 1;
	(*node).label = (char*)malloc(strlen(label) + 1);
	strcpy((*node).label, label);
	return node;
}

void add_jb_ref_node(inst_node* jb_node, const unsigned int address, const char* label)
{
	label_node* l_node = force_get_label_node_root(label);
	jb_ref_node* r_node = make_jb_ref_node(jb_node, address);

	#ifdef DEBUG
	printf("\nAdded a reference to label %s at address %d.\n", label, address);
	#endif

	(*r_node).next = (*l_node).jb_refs;
	(*l_node).jb_refs = r_node;
	return;
}

int try_add_label_node(const unsigned int address, const char* label)
{
	label_node* l_node = get_label_node_root(label);

	if (l_node == NULL)
	{
		l_node = force_get_label_node_root(label);
		(*l_node).address = address;
		return 0;
	}
	else if ((*l_node).address == 1)
	{
		(*l_node).address = address;
		return 0;
	}
	else
		return 1;
}

label_node* get_label_node_root(const char* label)
{
	return get_label_node(label, label_tree_root);
}

label_node* get_label_node(const char* label, label_node* root)
{
	int cmp_result;

	if (root == NULL)
		return NULL;

	cmp_result = strcmp(label, (*root).label);
	if (cmp_result > 1)
		return get_label_node(label, (*root).right_child);
	else if (cmp_result < 0)
		return get_label_node(label, (*root).left_child);
	else
		return root;
}

label_node* force_get_label_node_root(const char* label)
{
	if (label_tree_root == NULL)
	{
		label_tree_root = make_label_node(label);
		return label_tree_root;
	}
	return force_get_label_node(label, label_tree_root);
}

label_node* force_get_label_node(const char* label, label_node* root)
{
	int cmp_result;

	cmp_result = strcmp(label, (*root).label);
	if (cmp_result > 1)
	{
		if ((*root).right_child == NULL)
		{
			(*root).right_child = make_label_node(label);
			return (*root).right_child;
		}
		return get_label_node(label, (*root).right_child);
	}
	else if (cmp_result < 0)
	{
		if ((*root).left_child == NULL)
		{
			(*root).left_child = make_label_node(label);
			return (*root).left_child;
		}
		return get_label_node(label, (*root).left_child);
	}
	else
		return root;
}

void process_refs_root()
{
	if (label_tree_root != NULL)
		process_refs(label_tree_root);
	return;
}

void process_refs(label_node* root)
{
	if ((*root).left_child != NULL)
		process_refs((*root).left_child);

	if ((*root).right_child != NULL)
		process_refs((*root).right_child);

	if ((*root).jb_refs != NULL)
		process_refs_label((*root).jb_refs, (*root).address);

	return;
}

void process_refs_label(jb_ref_node* head, const unsigned int label_address)
{
	if ((*head).next != NULL)
		process_refs_label((*head).next, label_address);

	if ((*((*head).jb_node)).inst == 0x8000000)
		(*((*head).jb_node)).inst |= label_address >> 2;
	else
	{
	    (*((*head).jb_node)).inst |= ((label_address - (*head).address - 4) >> 2) & 0xFFFF;
	    printf("\nBranch offset: %X\n", ((label_address - (*head).address - 4) >> 2) & 0xFFFF);
	}

	return;
}

void delete_label_tree_root()
{
	if (label_tree_root != NULL)
	{
		delete_label_tree(label_tree_root);
		label_tree_root = NULL;
	}
	return;
}

void delete_label_tree(label_node* root)
{
	if ((*root).left_child != NULL)
		delete_label_tree((*root).left_child);

	if ((*root).right_child != NULL)
		delete_label_tree((*root).right_child);

	if ((*root).jb_refs != NULL)
		delete_jb_ref_list((*root).jb_refs);

	free((*root).label);
	free(root);
	return;
}

void delete_jb_ref_list(jb_ref_node* head)
{
	if ((*head).next != NULL)
		delete_jb_ref_list((*head).next);

	free(head);
	return;
}
