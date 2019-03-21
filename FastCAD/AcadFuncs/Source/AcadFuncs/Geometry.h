#ifndef _GEOMETRY_H_
#define _GEOMETRY_H_

#include "../acad_header.h"

class GeoFuncs
{
public:
	static double GetLengthPolyline(const AcDbObjectId& id);
	static double GetLengthPolyline(const AcDbObjectId & id,
		const AcGePoint3d &vertex1, const AcGePoint3d &vertex2);
	static double GetDisToStartOfPolyline(const AcDbObjectId& id, const AcGePoint3d& pnt);
	static double GetDisTwoPointOfPolyline(const AcDbObjectId& id,
		const AcGePoint3d& start, const AcGePoint3d& end);
	static AcGePoint3dArray GetListVertexOfPolyline(AcDbPolyline* pline);
	static AcGePoint3dArray GetListVertexOfPolyline(const AcDbObjectId& id);
	static AcDbObjectId MergeTwoEntityPE(AcDbDatabase* db, AcDbObjectId id_1, AcDbObjectId id_2);
	static bool IsPointOnStraightline(const AcGePoint3d& sp, const AcGePoint3d& ep, const AcGePoint3d& pnt);
	static bool IsPointInsidePolyline(const AcGePoint3dArray& pnts, const AcGePoint3d& check_pnt);
	static bool IsPointInsidePolyline(const AcDbObjectId& id, const AcGePoint3d& check_pnt);
	static bool IsPointBelongPolyline(AcGePoint3dArray poly_pnts, const AcGePoint3d& check_pnt);
	static bool IsPointBelongPolyline(const AcDbObjectId& id, const AcGePoint3d& check_pnt);
	static AcDbPolyline * CreatePolyline(const AcGePoint3dArray & pnts);
	static double GetLengthLine(const AcDbObjectId & line);
	static double GetLengthCurve(const AcDbObjectId& id);
	static AcGePoint3d GetCurveEndPnt(const AcDbObjectId& id);
	static AcGePoint3d GetCurveStartPnt(const AcDbObjectId& id);

};

#endif