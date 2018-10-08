#include "vector2d.h"

#include <iostream>

STVector2d::STVector2d(double _x, double _y) :
	x(_x),
	y(_y)
{
}

STVector2d::STVector2d(const STPoint2d & pnt1, const STPoint2d & pnt2) :
	x(pnt2.x - pnt1.x),
	y(pnt2.y - pnt1.y)
{
}

STVector2d::STVector2d(const STVector2d & vec)
{
	x = vec.x;
	y = vec.y;
}

const STVector2d & STVector2d::operator+(const STVector2d & vec)
{
	return STVector2d(x + vec.x, y + vec.y);
}

STVector2d::~STVector2d()
{
}

const STVector2d STVector2d::operator*(double scale)
{
	return STVector2d(x * scale, y * scale);
}

void STVector2d::Normalize()
{
	double len = Length();
	if (len < 0.0001)
		return;

	x /= len;
	y /= len;
}

double STVector2d::Length()
{
	return sqrt(x * x + y * y);
}

bool STVector2d::IsParallel(const STVector2d & vec)
{
	return (abs(abs(x) - abs(vec.x)) < 0.001) && (abs(abs(y) - abs(vec.y)) < 0.001);
}
