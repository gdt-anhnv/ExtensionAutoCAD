#ifndef _TRIANGLE_H_
#define _TRIANGLE_H_

#include "SphericalTree\point2d.h"
#include "SphericalTree\node.h"

#include <iostream>
#include <list>

struct EdgeLine;
struct VertexInfo
{
	STPoint2d position;
	double value;

	VertexInfo(const STPoint2d& pos, double val);

	VertexInfo(const VertexInfo& vi);

	VertexInfo();
};

class Triangle : public Node<Triangle>
{
private:
	VertexInfo vertice[3];
public:
	Triangle(const VertexInfo&, const VertexInfo& p2, const VertexInfo& p3);
	~Triangle();

	std::list<EdgeLine> GetEdges();
	std::list<EdgeLine> DrawContour(int step);
	bool IsValidTriangle();
};

struct EdgeLine
{
	VertexInfo fp;
	VertexInfo sp;

	EdgeLine(const VertexInfo& _fp, const VertexInfo& _sp) :
		fp(_fp),
		sp(_sp)
	{}

	bool operator==(const EdgeLine& et)
	{
		if (fp.position == et.fp.position && sp.position == et.sp.position)
			return true;

		if (fp.position == et.sp.position && sp.position == et.fp.position)
			return true;

		return false;
	}
};


#endif