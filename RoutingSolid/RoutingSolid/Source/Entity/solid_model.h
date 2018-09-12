#pragma once
#include "stdafx.h"

#include <list>
#include "Node.h"

class SolidModel
{
private:
	std::list<Node*> nodes;

public:

	/*static void NotCut(Node* node,  const AcGeLine3d & line);
	static void IntersectTwoVertex(Node* node, const AcGeLine3d & line);
	static void Cut(Node* node, const AcGeLine3d & line);*/

	void ParseRouting(const AcGeLine3d & line);

	SolidModel();
	~SolidModel();
};

