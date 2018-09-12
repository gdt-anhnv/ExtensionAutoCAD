#include "solid_model.h"


static void IntersectTwoVertex(Node * node, const AcGeLine3d & line, const AcGePoint3d &pnt)
{
	



}

static bool IsVertexLine(const AcGeLine3d &line, const AcGePoint3d & pnt)
{
	AcGePoint3d start = AcGePoint3d::kOrigin;
	if (line.hasStartPoint(start))
		if (start.isEqualTo(pnt))
			return true;

	AcGePoint3d end = AcGePoint3d::kOrigin;
	if (line.hasStartPoint(end))
		if (end.isEqualTo(pnt))
			return true;

	return false;
}

static void CheckNode(Node * node, const AcGeLine3d & line)
{
	for (std::list<Node*>::iterator i = node->connections.begin(); i != node->connections.end(); ++i)
	{
		AcGePoint3d pnt = AcGePoint3d::kOrigin;
		AcGeLine3d node_line = AcGeLine3d(node->GetPos(), (*i)->GetPos());
		if(line.intersectWith(node_line, pnt))
		{
			if (IsVertexLine(line, pnt))
			{
				if (IsVertexLine(node_line, pnt)) //cham 2 dau
				{

				}
				else
				{

				}
			}
		}
		else
		{

		}
	}

}

void SolidModel::ParseRouting(const AcGeLine3d & line)
{
	if (nodes.empty())
	{

	}

	for (std::list<Node*>::iterator i = nodes.begin(); i != nodes.end(); ++i)
	{

	}

}

SolidModel::SolidModel()
{
}


SolidModel::~SolidModel()
{
}