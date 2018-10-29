#ifndef _MTREE_H_
#define _MTREE_H_

#include "node.h"
#include "point2d.h"

#include <iostream>
#include <list>
#include <queue>
#include <stack>

template <typename T> class MTree
{
private:
	Node<T>* root;

public:
	MTree();
	~MTree();

	void Insert(Node<T>*);
	bool Remove(Node<T>* t);
	std::list<Node<T>*> Query(const STPoint2d&);
	std::list<T*> AllLeaves();

private:
	bool Remove(Node<T>* node, Node<T>* in_node);
};

template<typename T>
inline MTree<T>::MTree() :
	root(new Node<T>(STPoint2d(0.0, 0.0), 0.0))
{
}

template<typename T>
inline MTree<T>::~MTree()
{
	if (nullptr != root)
		delete root;
	root = nullptr;
}

template<typename T>
inline void MTree<T>::Insert(Node<T>* n)
{
	if (root->IsEmpty())
	{
		root->AddNode(n);
		root->SetCenter(n->Center());
		root->SetRadius(n->Radius());
		return;
	}

	std::stack<Node<T>*> trails;
	Node<T>* cur = root;
	Node<T>* tmp = root;

	while (!tmp->IsEmpty())
	{
		cur = tmp;
		cur->Expand(n->Center(), n->Radius());
		trails.push(cur);
		tmp = tmp->InsertInto(n);
	}

	cur->AddNode(n);
	//trails.push(cur);

	if (cur->IsFull())
	{
		while (trails.size() > 0)
		{
			cur = trails.top();
			trails.pop();

			if (cur->IsFull())
			{
				std::list<Node<T>*> split_nodes = cur->Split();
				Node<T>* parent = nullptr;
				if (trails.size() > 0)
				{
					parent = trails.top();
					parent->Remove(cur);
					delete cur;
				}
				else
				{
					parent = root;
				}

				for (std::list <Node<T>*>::iterator iter = split_nodes.begin(); iter != split_nodes.end(); iter++)
				{
					parent->AddNode(*iter);
					parent->CalibrateBoundary();
				}
			}
			else
			{
				return;
			}
		}
	}
}

template<typename T>
inline bool MTree<T>::Remove(Node<T>* t)
{
	bool ret = Remove(t, root);

	{
		if (1 == root->ChildNodes().size() && !root->ChildNodes().front()->IsEmpty())
		{
			Node<T>* tmp = root;
			root = root->ChildNodes().front();
			tmp->Remove(root);
			delete tmp;
		}
	}

	return ret;
}

template<typename T>
inline bool MTree<T>::Remove(Node<T>* node, Node<T>* in_node)
{
	std::list<Node<T>*> ns = in_node->Query(node->Center());
	for (auto iter = ns.begin(); iter != ns.end(); iter++)
	{
		if (*iter == node)
		{
			in_node->Remove(node);
			return true;
		}
		else if (Remove(node, *iter))
		{
			if ((*iter)->IsEmpty())
			{
				in_node->Remove(*iter);
				delete *iter;
				in_node->Shrink();
			}
			in_node->CalibrateBoundary();
			return true;
		}
	}

	return false;
}



template<typename T>
inline std::list<Node<T>*> MTree<T>::Query(const STPoint2d & pnt)
{
	std::list<Node<T>*> ret = std::list<Node<T>*>();
	std::stack<Node<T>*> trails = std::stack<Node<T>*>();
	if (root->ContainPnt(pnt))
		trails.push(root);

	while (trails.size() > 0)
	{
		Node<T>* n = trails.top();
		trails.pop();

		if (n->IsEmpty())
		{
			if (n->ContainPnt(pnt))
				ret.push_back(n);
		}
		else
		{
			std::list<Node<T>*> ns = n->Query(pnt);
			for (auto iter = ns.begin(); iter != ns.end(); iter++)
				trails.push(*iter);
		}
	}

	return ret;
}

template<typename T>
inline std::list<T*> MTree<T>::AllLeaves()
{
	std::list<T*> leaves = std::list<T*>();
	std::stack<Node<T>*> trails = std::stack<Node<T>*>();
	trails.push(root);

	while (trails.size() > 0)
	{
		Node<T>* cur = trails.top();
		trails.pop();

		if (cur->IsEmpty())
		{
			T* t_cur = static_cast<T*>(cur);
			if (nullptr != t_cur)
				leaves.push_back(t_cur);
		}

		std::list<Node<T>*> sub = cur->ChildNodes();
		for (auto iter = sub.begin(); iter != sub.end(); iter++)
			trails.push(*iter);
	}

	return leaves;
}

#endif
