#include "fast_dim.h"
#include "../../AcadFuncs/Source/AcadFuncs/UserFuncs.h"
#include "../../AcadFuncs/Source/AcadFuncs/ARXFuncs.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"
#include "../../AcadFuncs/Source/AcadFuncs/DBObject.h"
#include "../../AcadFuncs/Source/AcadFuncs/Geometry.h"

#include <iostream>
#include <list>


struct DimInfo
{
	AcGePoint3d origin_point;
	AcGePoint3d dim_point;

	DimInfo(const AcGePoint3d& op, const AcGePoint3d& dp) :
		origin_point(op),
		dim_point(dp)
	{}
};

static AcGeVector3d DetectAlongVector(const AcDbObjectId& pl_id);
static AcGePoint3d PointOnLine(const AcGePoint3d& bp, const AcGeVector3d& vec, const AcGePoint3d& pnt);
void FastDim::DoFastDim()
{
	try
	{
		//pick polyline
		auto obj_ids = ARXFuncs::GetObjIdsByPicking();
		if (0 == obj_ids.length())
			return;

		//is polyline
		{
			ObjectWrap<AcDbPolyline> pl(DBObject::OpenObjectById<AcDbPolyline>(obj_ids[0]));
			if (nullptr == pl.object)
				return;
		}

		//pick base point
		AcGePoint3d base_pnt = UserFuncs::UserGetPoint(L"Chọn 1 điểm:");

		//Detect all the circles
		auto ent_ids = ARXFuncs::GetEntsInsidePolyline(obj_ids[0]);
		if (0 == ent_ids.length())
			return;

		//detect vectors
		AcGeVector3d vec = DetectAlongVector(obj_ids[0]);
		if (vec.length() < 0.001)
			return;

		//detect 2 bound
		{
			auto pnts = GeoFuncs::GetListVertexOfPolyline(obj_ids[0]);

		}

		//Order circles
		std::list<DimInfo> dim_infos = std::list<DimInfo>();
		for (int i = 0; i < ent_ids.length(); i++)
		{
			ObjectWrap<AcDbCircle> circle(DBObject::OpenObjectById<AcDbCircle>(ent_ids[i]));
			if (nullptr == circle.object)
				continue;

			AcGePoint3d pnt = PointOnLine(base_pnt, vec, circle.object->center());
			dim_infos.push_back(DimInfo(circle.object->center(), pnt));
		}

		//Dim position
	}
	catch (...)
	{

	}
}

static AcGeVector3d DetectAlongVector(const AcDbObjectId& pl_id)
{
	AcGePoint3dArray pl_verts = GeoFuncs::GetListVertexOfPolyline(pl_id);
	pl_verts.append(pl_verts[0]);

	AcGeVector3d ret = AcGeVector3d::kXAxis;
	double max_length = 0.0;
	for (int i = 0; i < pl_verts.length() - 1; i++)
	{
		AcGeVector3d cur = AcGeVector3d(pl_verts[i + 1].x - pl_verts[i].x,
			pl_verts[i + 1].y - pl_verts[i].y,
			pl_verts[i + 1].z - pl_verts[i].z);

		double length = cur.length();
		if (length > max_length)
		{
			max_length = length;
			ret = cur.normal();
		}
	}

	return ret;
}

AcGePoint3d PointOnLine(const AcGePoint3d & bp, const AcGeVector3d & vec, const AcGePoint3d & pnt)
{
	double k = vec.y * (pnt.x - bp.x) - vec.x * (pnt.y - bp.y);
	double ret_x = bp.x - k * vec.x;
	double ret_y = bp.y - k * vec.y;

	return AcGePoint3d(ret_x, ret_y, 0.0);
}
