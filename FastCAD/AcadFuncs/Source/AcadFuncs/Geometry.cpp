#include "Geometry.h"
#include "AcadFuncs.h"
#include "DBObject.h"

static AcDbObjectId DrawPolyline(AcDbDatabase* db, AcGePoint3dArray pnts)
{
	AcDbObjectId id = AcDbObjectId::kNull;

	ObjectWrap<AcDbPolyline> pline(new AcDbPolyline());

	for (int i = 0; i < pnts.length(); i++)
	{
		pline.object->addVertexAt(i, AcGePoint2d(pnts.at(i).x, pnts.at(i).y));
	}

	{
		ObjectWrap<AcDbBlockTableRecord> rcd_wrap(DBObject::GetModelSpace(db));
		rcd_wrap.object->upgradeOpen();
		rcd_wrap.object->appendAcDbEntity(id, pline.object);
	}

	return id;
}

double GeoFuncs::GetLengthPolyline(const AcDbObjectId & id)
{
	if (AcDbObjectId::kNull != id)
	{
		double length = 0.0;
		AcDbPolyline* pl = DBObject::OpenObjectById<AcDbPolyline>(id);
		ObjectWrap<AcDbPolyline> pl_wrap(pl);

		if (NULL == pl)
			throw std::exception("Could not get the length of the polyline");

		double paramEnd;
		pl->getEndParam(paramEnd);
		pl->getDistAtParam(paramEnd, length);

		return length;
	}
	else
		throw std::exception("Could not get the length of the polyline");
}

double GeoFuncs::GetLengthPolyline(const AcDbObjectId & id, const AcGePoint3d & vertex1, const AcGePoint3d & vertex2)
{
	if (AcDbObjectId::kNull != id)
	{
		double length = 0.0;
		AcDbPolyline* pl = DBObject::OpenObjectById<AcDbPolyline>(id);
		if (NULL != pl)
		{
			AcGePolyline3d pl1 = AcGePolyline3d(GetListVertexOfPolyline(pl));
			double param1 = pl1.paramOf(vertex1);
			double param2 = pl1.paramOf(vertex2);
			length = abs(pl1.length(param1, param2));
		}
		return length;
	}
	else
		throw std::exception("Could not get the length of the polyline");
}

double GeoFuncs::GetDisToStartOfPolyline(const AcDbObjectId & id, const AcGePoint3d & pnt)
{
	AcDbPolyline* pl = DBObject::OpenObjectById<AcDbPolyline>(id);

	if (NULL == pl)
		return 0.0;

	double ret = 0.0;
	for (int i = 0; i < pl->numVerts() - 1; i++)
	{
		AcGePoint2d tmp_pnt1 = AcGePoint2d::kOrigin;
		AcGePoint2d tmp_pnt2 = AcGePoint2d::kOrigin;

		pl->getPointAt(i, tmp_pnt1);
		pl->getPointAt(i + 1, tmp_pnt2);

		AcGePoint3d pnt1 = AcGePoint3d(tmp_pnt1.x, tmp_pnt1.y, 0.0);
		AcGePoint3d pnt2 = AcGePoint3d(tmp_pnt2.x, tmp_pnt2.y, 0.0);

		if (0.1 > std::abs(pnt1.distanceTo(pnt) + pnt2.distanceTo(pnt) - pnt1.distanceTo(pnt2)))
		{
			AcGePoint2d tmp_sp = AcGePoint2d::kOrigin;
			pl->getPointAt(0, tmp_sp);
			return GetLengthPolyline(id, AcGePoint3d(tmp_sp.x, tmp_sp.y, 0.0), pnt1) + pnt1.distanceTo(pnt);
		}
	}

	return 0.0;
}

double GeoFuncs::GetDisTwoPointOfPolyline(const AcDbObjectId & id, const AcGePoint3d & start, const AcGePoint3d & end)
{
	return std::abs(GetDisToStartOfPolyline(id, start) - GetDisToStartOfPolyline(id, end));
}

AcGePoint3dArray GeoFuncs::GetListVertexOfPolyline(AcDbPolyline * pline)
{
	AcGePoint3dArray pnts;

	for (int i = 0; i < pline->numVerts(); i++)
	{
		AcGePoint3d pnt(AcGePoint3d::kOrigin);
		pline->getPointAt(i, pnt);
		pnts.append(pnt);
	}

	return pnts;
}

AcGePoint3dArray GeoFuncs::GetListVertexOfPolyline(const AcDbObjectId & id)
{
	AcGePoint3dArray pnts;
	if (AcDbObjectId::kNull != id)
	{
		ObjectWrap<AcDbObject> pl_w(DBObject::OpenObjectById<AcDbObject>(id));
		if (NULL != pl_w.object && pl_w.object->isKindOf(AcDbPolyline::desc()))
		{
			AcDbPolyline* pl = AcDbPolyline::cast(pl_w.object);
			for (int i = 0; i < pl->numVerts(); i++)
			{
				AcGePoint3d pnt(AcGePoint3d::kOrigin);
				pl->getPointAt(i, pnt);
				pnts.append(pnt);
			}
		}
	}
	return pnts;
}

AcDbObjectId GeoFuncs::MergeTwoEntityPE(AcDbDatabase* db, AcDbObjectId id_1, AcDbObjectId id_2)
{
	ObjectWrap<AcDbEntity> obj_1(DBObject::OpenObjectById<AcDbEntity>(id_1));
	ObjectWrap<AcDbEntity> obj_2(DBObject::OpenObjectById<AcDbEntity>(id_2));

	if (NULL == obj_1.object || NULL == obj_2.object)
		return AcDbObjectId::kNull;

	
	AcDbJoinEntityPE* join = AcDbJoinEntityPE::cast(obj_1.object->queryX(AcDbJoinEntityPE::desc()));
	if (join == NULL)
		return AcDbObjectId::kNull;

	obj_1.object->upgradeOpen();
	obj_2.object->upgradeOpen();

	Acad::ErrorStatus ret = join->joinEntity(obj_1.object, obj_2.object);
	if (Acad::eOk == ret)
	{
		obj_2.object->erase();
		obj_1.object->downgradeOpen();
		return obj_1.object->id();
	}

	return AcDbObjectId::kNull;
}

bool GeoFuncs::IsPointOnStraightline(const AcGePoint3d & sp, const AcGePoint3d & ep, const AcGePoint3d & pnt)
{
	AcGeTol tol = AcGeTol();
	tol.setEqualPoint(1.0);
	if (pnt.isEqualTo(sp, tol) || pnt.isEqualTo(ep, tol))
		return true;
	tol.setEqualVector(1e-3);
	AcGeVector3d vec_b(ep.x - sp.x, ep.y - sp.y, ep.z - sp.z);
	AcGeVector3d vec_check(pnt.x - sp.x, pnt.y - sp.y, pnt.z - sp.z);
	AcGeVector3d v1 = vec_check.normal();
	AcGeVector3d v2 = vec_b.normal();
	if (vec_check.normal().isEqualTo(vec_b.normal(), tol))
	{
		if (sp.distanceTo(pnt) < sp.distanceTo(ep))
			return true;
		return false;
	}
	return false;
}

static bool GetMaxMinPoint(const AcGePoint3dArray& poly_point, AcGePoint3d& point_max, AcGePoint3d& point_min)
{
	if (0 == poly_point.length())
		return false;
	point_max = poly_point.at(0);
	point_min = poly_point.at(0);
	for (int i = 0; i < poly_point.length(); i++)
	{
		// Max point
		if (poly_point.at(i).x > point_max.x)
			point_max.x = poly_point.at(i).x;
		if (poly_point.at(i).y > point_max.y)
			point_max.y = poly_point.at(i).y;
		//Min point
		if (poly_point.at(i).x < point_min.x)
			point_min.x = poly_point.at(i).x;
		if (poly_point.at(i).y < point_min.y)
			point_min.y = poly_point.at(i).y;
	}
	return true;
}

static AcGeVector3d MakeVetor(AcGePoint3dArray pnts, const AcGePoint3d& pnt)
{
	AcGePoint3d pnt_2(0.0, 0.0, 0.0);
	AcGeVector3d vec = AcGeVector3d();
	pnts.append(pnts.first());
	for (int i = 0; i < pnts.length(); i++)
	{
		pnt_2 = AcGePoint3d((pnts.at(i).x + pnts.at(i + 1).x) / 2.0,
			(pnts.at(i).y + pnts.at(i + 1).y) / 2.0,
			(pnts.at(i).z + pnts.at(i + 1).z) / 2.0);
		AcGeLine3d line(pnt, pnt_2);
		int check = 0;
		for (int i = 0; i < pnts.length(); i++)
		{
			double param = 0.0;
			if (line.isOn(pnts.at(i), param) && line.isOn(pnts.at(i + 1), param))
				check += 1;
		}
		if (0 == check)
		{
			vec = AcGeVector3d(pnt_2.x - pnt.x, pnt_2.y - pnt.y, pnt_2.z - pnt.z).normal();
			return vec;
		}
	}
	return vec;
}

static bool IntersectPolyline(AcGePoint3dArray pl_pnts,
	AcGePoint3d& sp, AcGePoint3d& ep,
	AcGePoint3dArray& intersected_pnts)
{
	AcDbLine ray(sp, ep);
	pl_pnts.append(pl_pnts.first());

	for (int i = 0; i < pl_pnts.length() - 1; i++)
	{
		AcDbLine check_l(pl_pnts.at(i), pl_pnts.at(i + 1));
		int num_check = 0;
		AcGePoint3dArray intersect_points = AcGePoint3dArray();
		ray.intersectWith(&check_l, AcDb::Intersect::kOnBothOperands, intersect_points);
		if (intersect_points.length() > 0)
		{
			if (intersect_points.length() == 1)
			{
				intersected_pnts.append(intersect_points.at(0));
			}
			else
			{
				AcGePoint3d tmp(0.0, 0.0, 0.0);
				tmp.x = (pl_pnts.at(i).x + pl_pnts.at(i + 1).x)*0.5;
				tmp.y = (pl_pnts.at(i).y + pl_pnts.at(i + 1).y)*0.5;
				intersected_pnts.append(tmp);
			}
		}
	}
	if (intersected_pnts.length() == 0)
		return false;
	return true;
}

bool GeoFuncs::IsPointInsidePolyline(const AcGePoint3dArray& pnts, const AcGePoint3d& check_pnt)
{
	// is out side of max min polyline
	AcGePoint3d max_point = AcGePoint3d();
	AcGePoint3d min_point = AcGePoint3d();
	{
		if (!GetMaxMinPoint(pnts, max_point, min_point))
			return false; // throw when polyline is null
		if (check_pnt.x > max_point.x || check_pnt.x < min_point.x ||
			check_pnt.y > max_point.y || check_pnt.y < min_point.y)
			return false; // throw when outside max min rectange of polygon
	}

	AcGePoint3d start_ray = check_pnt;
	AcGePoint3d end_ray(0.0, 0.0, 0.0);
	AcGePoint3dArray intersect_point = AcGePoint3dArray();
	AcGeVector3d ray_vec = MakeVetor(pnts, check_pnt);
	double radius = min_point.distanceTo(max_point);
	ray_vec *= radius;
	end_ray = start_ray + ray_vec;
	if (IntersectPolyline(pnts, start_ray, end_ray, intersect_point))
	{
		int num_inter = intersect_point.length();
		// xet dieu kien cong them dem giao
		for (int i = 0; i < num_inter; i++)
		{
			for (int j = 0; j < pnts.length(); j++)
			{
				AcGePoint3d tmp = pnts.at(j);
				if (intersect_point.at(i).isEqualTo(tmp))
					num_inter += 1;
			}
		}
		if (0 == num_inter % 2)
			return false;
		else
			return true;
	}
	else
		return false;
}

bool GeoFuncs::IsPointInsidePolyline(const AcDbObjectId & id, const AcGePoint3d & check_pnt)
{
	AcGePoint3dArray pnts = GetListVertexOfPolyline(id);

	// is out side of max min polyline
	AcGePoint3d max_point = AcGePoint3d(AcGePoint3d::kOrigin);
	AcGePoint3d min_point = AcGePoint3d(AcGePoint3d::kOrigin);
	{
		if (!GetMaxMinPoint(pnts, max_point, min_point))
			return false; // throw when polyline is null
		if (check_pnt.x > max_point.x || check_pnt.x < min_point.x ||
			check_pnt.y > max_point.y || check_pnt.y < min_point.y)
			return false; // throw when outside max min rectange of polygon
	}

	AcGePoint3d start_ray = check_pnt;
	AcGePoint3d end_ray(0.0, 0.0, 0.0);
	AcGePoint3dArray intersect_point = AcGePoint3dArray();
	AcGeVector3d ray_vec = MakeVetor(pnts, check_pnt);
	double radius = min_point.distanceTo(max_point);
	ray_vec *= radius;
	end_ray = start_ray + ray_vec;
	if (IntersectPolyline(pnts, start_ray, end_ray, intersect_point))
	{
		int num_inter = intersect_point.length();
		// xet dieu kien cong them dem giao
		for (int i = 0; i < num_inter; i++)
		{
			for (int j = 0; j < pnts.length(); j++)
			{
				AcGePoint3d tmp = pnts.at(j);
				if (intersect_point.at(i).isEqualTo(tmp))
					num_inter += 1;
			}
		}
		if (0 == num_inter % 2)
			return false;
		else
			return true;
	}
	else
		return false;
}

bool GeoFuncs::IsPointBelongPolyline(AcGePoint3dArray poly_pnts, const AcGePoint3d& check_pnt)
{
	if (poly_pnts.length() <= 0)
		return false;
	poly_pnts.append(poly_pnts.at(0));
	for (int i = 1; i < poly_pnts.length(); i++)
	{
		// check point_check is vertex of polyline or not
		if (check_pnt.isEqualTo(poly_pnts.at(i - 1)))
			return true;

		//check point chek on egde of polyline
		if (IsPointOnStraightline(poly_pnts.at(i - 1), poly_pnts.at(i), check_pnt))
			return true;
	}
	return false;
}

bool GeoFuncs::IsPointBelongPolyline(const AcDbObjectId & id, const AcGePoint3d & check_pnt)
{
	AcGeTol tol = AcGeTol();
	tol.setEqualPoint(1.0);
	AcGePoint3dArray poly_pnts = AcGePoint3dArray();
	{
		ObjectWrap<AcDbPolyline> obj_wrap(DBObject::OpenObjectById<AcDbPolyline>(id));
		if (obj_wrap.object != NULL)
		{
			AcDbPolyline* pl = AcDbPolyline::cast(obj_wrap.object);
			poly_pnts = GetListVertexOfPolyline(pl);
			AcGePoint3d tmp_pnt = AcGePoint3d::kOrigin;
			pl->getClosestPointTo(check_pnt, tmp_pnt);
			double d = check_pnt.distanceTo(tmp_pnt);
			if (d <= 0.1)
				return true;
		}
		else
		{
			return false;
		}
	}
	poly_pnts.append(poly_pnts.at(0));
	for (int i = 1; i < poly_pnts.length(); i++)
	{
		// check point_check is vertex of polyline or not
		if (check_pnt.isEqualTo(poly_pnts.at(i - 1), tol))
			return true;

		//check point chek on egde of polyline
		/*if (IsPointOnStraightline(poly_pnts.at(i - 1), poly_pnts.at(i), check_pnt))
		return true;*/
	}
	return false;
}

AcDbPolyline * GeoFuncs::CreatePolyline(const AcGePoint3dArray & pnts)
{
	AcDbPolyline * pline = new AcDbPolyline();
	
	for (int i = 0; i < pnts.length(); i++)
	{
		pline->addVertexAt(i, AcGePoint2d(pnts.at(i).x, pnts.at(i).y));
	}

	return pline;
}

double GeoFuncs::GetLengthCurve(const AcDbObjectId & id)
{
	if (id.isNull())
		throw int(1);

	double length = 0.0;
	ObjectWrap<AcDbCurve> curve_wrap(DBObject::OpenObjectById<AcDbCurve>(id));

	if (NULL == curve_wrap.object)
		throw int(1);

	double start_param = 0.0;
	curve_wrap.object->getStartParam(start_param);

	double end_param = 0.0;
	curve_wrap.object->getEndParam(end_param);

	curve_wrap.object->getDistAtParam(end_param, length);

	return length;
}

double GeoFuncs::GetLengthLine(const AcDbObjectId & line)
{
	ObjectWrap<AcDbLine> l = DBObject::OpenObjectById<AcDbLine>(line);

	return l.object->endPoint().distanceTo(l.object->startPoint());
}

AcGePoint3d GeoFuncs::GetCurveEndPnt(const AcDbObjectId& id)
{
	ObjectWrap<AcDbCurve> curve(DBObject::OpenObjectById<AcDbCurve>(id));
	double end_param = 0.0;
	curve.object->getEndParam(end_param);

	AcGePoint3d pnt = AcGePoint3d::kOrigin;
	curve.object->getPointAtParam(end_param, pnt);

	return pnt;
}

AcGePoint3d GeoFuncs::GetCurveStartPnt(const AcDbObjectId& id)
{
	ObjectWrap<AcDbCurve> curve(DBObject::OpenObjectById<AcDbCurve>(id));
	double start_param = 0.0;
	curve.object->getStartParam(start_param);

	AcGePoint3d pnt = AcGePoint3d::kOrigin;
	curve.object->getPointAtParam(start_param, pnt);

	return pnt;

}

