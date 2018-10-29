#ifndef _POINT2D_H_
#define _POINT2D_H_

class STVector2d;
struct STPoint2d
{
public:
	double x;
	double y;

	STPoint2d(double _x, double _y);
	STPoint2d(const STPoint2d& pnt);

	double Distance(const STPoint2d& pnt) const;
	void Translate(const STVector2d&);
	bool EqualPnt(const STPoint2d&);

	bool operator==(const STPoint2d&);
};

#endif