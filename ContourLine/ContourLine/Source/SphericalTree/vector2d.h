#ifndef _VECTOR2D_H_
#define _VECTOR2D_H_

#include "point2d.h"

class STVector2d
{
private:
	double x;
	double y;

public:
	STVector2d(double _x, double _y);
	STVector2d(const STPoint2d& pnt1, const STPoint2d& pnt2);
	STVector2d(const STVector2d&);
	const STVector2d& operator+(const STVector2d&);
	~STVector2d();

	const STVector2d operator*(double);

	void Normalize();
	double Length();
	friend 	void STPoint2d::Translate(const STVector2d&);
	bool IsParallel(const STVector2d& vec);
};

#endif