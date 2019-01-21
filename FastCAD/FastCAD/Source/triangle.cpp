#include "triangle.h"
#include "SphericalTree\vector2d.h"

#include <iostream>

static STPoint2d CalculateCircumcircleCenter(const STPoint2d& p1, const STPoint2d& p2, const STPoint2d& p3);
static double CalculateCircumcircleRadius(const STPoint2d& p1, const STPoint2d& p2, const STPoint2d& p3);
static double CalculateArea(const STPoint2d& p1, const STPoint2d& p2, const STPoint2d& p3);
Triangle::Triangle(const VertexInfo& p1, const VertexInfo& p2, const VertexInfo& p3) :
	Node<Triangle>(CalculateCircumcircleCenter(p1.position, p2.position, p3.position),
		CalculateCircumcircleRadius(p1.position, p2.position, p3.position)),
		vertice()
{
	vertice[0] = p1;
	vertice[1] = p2;
	vertice[2] = p3;
}

Triangle::~Triangle()
{
}

std::list<EdgeLine> Triangle::GetEdges()
{
	std::list<EdgeLine> edges = std::list<EdgeLine>();
	edges.push_back(EdgeLine(vertice[0], vertice[1]));
	edges.push_back(EdgeLine(vertice[1], vertice[2]));
	edges.push_back(EdgeLine(vertice[2], vertice[0]));

	return std::move(edges);
}

std::list<EdgeLine> Triangle::DrawContour(int step)
{
	VertexInfo max_ver = vertice[0];
	if (vertice[1].value > max_ver.value)
		max_ver = vertice[1];
	if (vertice[2].value > max_ver.value)
		max_ver = vertice[2];

	VertexInfo min_ver = vertice[0];
	if (vertice[1].value < min_ver.value)
		min_ver = vertice[1];
	if (vertice[2].value < min_ver.value)
		min_ver = vertice[2];

	VertexInfo mid_ver = vertice[0];
	if (mid_ver.position == max_ver.position || mid_ver.position == min_ver.position)
		mid_ver = vertice[1];
	if (mid_ver.position == max_ver.position || mid_ver.position == min_ver.position)
		mid_ver = vertice[2];

	double contour_val = floor(min_ver.value / step) * step;
	if (contour_val < min_ver.value)
		contour_val += step;

	double max_contour_val = floor(max_ver.value / step) * step;

	std::list<EdgeLine> lines = std::list<EdgeLine>();
	while (contour_val < max_contour_val + step * 0.5)
	{
		if (contour_val < mid_ver.value)
		{
			STPoint2d pos = min_ver.position;
			pos.Translate(STVector2d(min_ver.position, max_ver.position) * ((contour_val - min_ver.value) / (max_ver.value - min_ver.value)));
			STPoint2d pos2 = min_ver.position;
			pos2.Translate(STVector2d(min_ver.position, mid_ver.position) * ((contour_val - min_ver.value) / (mid_ver.value - min_ver.value)));
			lines.push_back(EdgeLine(VertexInfo(pos, 0.0), VertexInfo(pos2, 0.0)));
		}
		else
		{
			STPoint2d pos = min_ver.position;
			pos.Translate(STVector2d(min_ver.position, max_ver.position) * ((contour_val - min_ver.value) / (max_ver.value - min_ver.value)));
			STPoint2d pos2 = max_ver.position;
			pos2.Translate(STVector2d(max_ver.position, mid_ver.position) * ((max_ver.value - contour_val) / (max_ver.value - mid_ver.value)));
			lines.push_back(EdgeLine(VertexInfo(pos, 0.0), VertexInfo(pos2, 0.0)));
		}

		contour_val += step;
	}

	return lines;
}

bool Triangle::IsValidTriangle()
{
	if (vertice[0].position.Distance(vertice[1].position) > 50.0)
		return false;
	if (vertice[1].position.Distance(vertice[2].position) > 50.0)
		return false;
	if (vertice[2].position.Distance(vertice[0].position) > 50.0)
		return false;
	return true;
}


static STPoint2d CalculateCircumcircleCenter(const STPoint2d& p1, const STPoint2d& p2, const STPoint2d& p3)
{
	double b_x = 0.0;
	{
		double a = p1.x * p1.x + p1.y * p1.y;
		double b = p1.y;
		double c = 1;
		double d = p2.x * p2.x + p2.y * p2.y;
		double e = p2.y;
		double f = 1;
		double g = p3.x * p3.x + p3.y * p3.y;
		double h = p3.y;
		double i = 1;
		b_x = (a*e*i + b*f*g + c*d*h) - (a*h*f + b*d*i + c*e*g);
		b_x = -b_x;
	}

	double b_y = 0.0;
	{
		double a = p1.x * p1.x + p1.y * p1.y;
		double b = p1.x;
		double c = 1;
		double d = p2.x * p2.x + p2.y * p2.y;
		double e = p2.x;
		double f = 1;
		double g = p3.x * p3.x + p3.y * p3.y;
		double h = p3.x;
		double i = 1;
		b_y = (a*e*i + b*f*g + c*d*h) - (a*h*f + b*d*i + c*e*g);
	}

	double a_val = 0.0;
	{
		double a = p1.x;
		double b = p1.y;
		double c = 1;
		double d = p2.x;
		double e = p2.y;
		double f = 1;
		double g = p3.x;
		double h = p3.y;
		double i = 1;
		a_val = (a*e*i + b*f*g + c*d*h) - (a*h*f + b*d*i + c*e*g);
	}

	return STPoint2d(-b_x / (2 * a_val), -b_y / (2 * a_val));
}

static double CalculateCircumcircleRadius(const STPoint2d& p1, const STPoint2d& p2, const STPoint2d& p3)
{
	double l1 = p1.Distance(p2);
	double l2 = p2.Distance(p3);
	double l3 = p3.Distance(p1);

	return l1 * l2 * l3 / (4 * CalculateArea(p1, p2, p3));
}

static double CalculateArea(const STPoint2d& p1, const STPoint2d& p2, const STPoint2d& p3)
{
	double l1 = p1.Distance(p2);
	double l2 = p2.Distance(p3);
	double l3 = p3.Distance(p1);

	double a = 0.5 * (l1 + l2 + l3);
	return sqrt(a * (a - l1) * (a - l2) * (a - l3));
}

VertexInfo::VertexInfo(const STPoint2d & pos, double val) :
	position(pos),
	value(val)
{
}

VertexInfo::VertexInfo(const VertexInfo & vi) :
	position(vi.position),
	value(vi.value)
{
}

VertexInfo::VertexInfo() :
	position(STPoint2d(0.0, 0.0)),
	value(0.0)
{}