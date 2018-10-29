#include "point2d.h"
#include "vector2d.h"

#include <iostream>

STPoint2d::STPoint2d(double _x, double _y) :
	x(_x),
	y(_y)
{

}

STPoint2d::STPoint2d(const STPoint2d & pnt) :
	x(pnt.x),
	y(pnt.y)
{
}

double STPoint2d::Distance(const STPoint2d & pnt) const
{
	return sqrt((pnt.x - x) * (pnt.x - x) + (pnt.y - y) * (pnt.y - y));
}

void STPoint2d::Translate(const STVector2d & vec)
{
	x += vec.x;
	y += vec.y;
}

bool STPoint2d::EqualPnt(const STPoint2d & pnt)
{
	if (abs(x - pnt.x) < 0.001 && abs(y - pnt.y) < 0.001)
		return true;
	return false;
}

bool STPoint2d::operator==(const STPoint2d & pnt)
{
	if (abs(x - pnt.x) < 0.001 && abs(y - pnt.y) < 0.001)
		return true;

	return false;
}
