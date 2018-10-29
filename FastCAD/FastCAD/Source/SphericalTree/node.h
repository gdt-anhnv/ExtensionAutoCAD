#ifndef _NODE_H_
#define _NODE_H_

#include "point2d.h"
#include "vector2d.h"

#include <iostream>
#include <list>

#define MAX_CHILDREN	3

template <typename T> class Node
{
private:
	STPoint2d center;
	double radius;
	std::list<Node*> child_nodes;

public:
	Node(const STPoint2d& cen, double rad);
	~Node();

	void Expand(STPoint2d pnt, double rad);
	void Remove(Node<T>*);
	void AddNode(Node<T>* n);
	Node<T>* InsertInto(Node<T>* n);
	std::list<Node<T>*> Split();
	void Shrink();
	bool IsFull();
	bool IsEmpty();
	bool ContainPnt(const STPoint2d&);
	std::list<Node<T>*> Query(const STPoint2d& pnt);
	void CalibrateBoundary();

	const STPoint2d Center() { return center; }
	double Radius() { return radius; }
	std::list<Node*> ChildNodes() { return child_nodes; }
	void SetCenter(const STPoint2d& pnt) { center = pnt; }
	void SetRadius(double rad) { radius = rad; }
};

template<typename T>
inline Node<T>::Node(const STPoint2d& cen, double rad) :
	center(cen),
	radius(rad),
	child_nodes(std::list<Node<T>*>())
{
}

template<typename T>
inline Node<T>::~Node()
{
	for (auto iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
		delete (*iter);
	child_nodes.clear();
}

template<typename T>
inline void Node<T>::Expand(STPoint2d pnt, double rad)
{
	double distance = center.Distance(pnt);
	if (distance + rad < radius)
		return;

	if (distance + radius < rad)
	{
		center = pnt;
		radius = rad;
		return;
	}

	if (pnt.EqualPnt(center))
	{
		radius = rad > radius ? rad : radius;
		return;
	}

	//calculate center
	STVector2d vec(center, pnt);
	vec.Normalize();

	center.Translate(vec * (distance * 0.5 + (rad - radius) * 0.5));

	//calculate radius
	radius = (rad + radius + distance) * 0.5;
}

template<typename T>
inline void Node<T>::Remove(Node<T> *n)
{
	child_nodes.remove(n);
}

template<typename T>
inline void Node<T>::AddNode(Node<T>* n)
{
	child_nodes.push_back(n);
}

template<typename T>
inline Node<T> * Node<T>::InsertInto(Node<T>* n)
{
	if (!IsEmpty())
	{
		Node<T>* ret = child_nodes.front();
		for (std::list<Node<T>*>::iterator iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
		{
			if (ret->center.Distance(n->center) > (*iter)->center.Distance(n->center))
				ret = *iter;
		}

		return ret;
	}
	return nullptr;
}

template<typename T>
inline std::list<Node<T>*> Node<T>::Split()
{
	if (IsEmpty())
		return std::list<Node<T>*>();

	int num_node = (MAX_CHILDREN + 1) / 2;
	Node<T>* n1 = new Node<T>(child_nodes.front()->center, child_nodes.front()->radius);

	n1->AddNode(child_nodes.front());
	n1->Expand(child_nodes.front()->center, child_nodes.front()->radius);
	child_nodes.pop_front();

	for (int i = 1; i < num_node; i++)
	{
		std::list<Node<T>*>::iterator nn = child_nodes.begin();
		for (std::list<Node<T>*>::iterator iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
		{
			if (n1->center.Distance((*nn)->center) < n1->center.Distance((*iter)->center))
			{
				nn = iter;
			}
		}

		child_nodes.remove(*nn);
		n1->AddNode(*nn);
		n1->Expand((*nn)->center, (*nn)->radius);
	}

	Node<T>* n2 = new Node<T>(child_nodes.front()->center, child_nodes.front()->radius);
	for (std::list<Node<T>*>::iterator iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
	{
		n2->AddNode(*iter);
		n2->Expand((*iter)->center, (*iter)->radius);
	}

	std::list<Node<T>*> ret = std::list<Node<T>*>();
	ret.push_back(n1);
	ret.push_back(n2);

	child_nodes.clear();

	return ret;
}

template<typename T>
inline void Node<T>::Shrink()
{
	std::list<Node<T>*> nodes = std::list<Node<T>*>();
	for (auto iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
	{
		for (auto sub_iter = (*iter)->child_nodes.begin(); sub_iter != (*iter)->child_nodes.end(); sub_iter++)
		{
			nodes.push_back(*sub_iter);
		}
	}

	if (nodes.size() > MAX_CHILDREN)
		return;

	for (auto iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
	{
		(*iter)->child_nodes.clear();
		delete *iter;
	}

	child_nodes.clear();

	if (0 == nodes.size())
		return;

	Node<T>* shrink = new Node<T>(nodes.front()->center, nodes.front()->radius);
	for (auto iter = nodes.begin(); iter != nodes.end(); iter++)
	{
		shrink->AddNode(*iter);
		shrink->Expand((*iter)->center, (*iter)->radius);
	}

	child_nodes.push_back(shrink);
}

template<typename T>
inline bool Node<T>::IsFull()
{
	return child_nodes.size() > MAX_CHILDREN;
}

template<typename T>
inline bool Node<T>::IsEmpty()
{
	return 0 == child_nodes.size();
}

template<typename T>
inline bool Node<T>::ContainPnt(const STPoint2d & pnt)
{
	return center.Distance(pnt) - 0.001 < radius;
}

template<typename T>
inline std::list<Node<T>*> Node<T>::Query(const STPoint2d & pnt)
{
	std::list<Node<T>*> ns = std::list<Node<T>*>();
	for (auto iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
	{
		if ((*iter)->ContainPnt(pnt))
			ns.push_back(*iter);
	}

	return std::move(ns);
}

template<typename T>
inline void Node<T>::CalibrateBoundary()
{
	if (0 == child_nodes.size())
		return;

	center = child_nodes.front()->center;
	radius = child_nodes.front()->radius;

	for (auto iter = child_nodes.begin(); iter != child_nodes.end(); iter++)
		Expand((*iter)->center, (*iter)->radius);
}

#endif
