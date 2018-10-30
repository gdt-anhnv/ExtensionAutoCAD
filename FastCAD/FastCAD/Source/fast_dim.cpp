#include "fast_dim.h"
#include "../../AcadFuncs/Source/AcadFuncs/UserFuncs.h"
#include "../../AcadFuncs/Source/AcadFuncs/ARXFuncs.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"
#include "../../AcadFuncs/Source/AcadFuncs/DBObject.h"
#include "../../AcadFuncs/Source/AcadFuncs/Geometry.h"
#include "../../AcadFuncs/Source/AcadFuncs.h"

#include <iostream>
#include <list>
#include <vector>
#include <math.h>
#include <string>

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
		AcGePoint3d min_pnt = AcGePoint3d::kOrigin;
		AcGePoint3d max_pnt = AcGePoint3d::kOrigin;
		{
			auto pnts = GeoFuncs::GetListVertexOfPolyline(obj_ids[0]);
			std::vector<DimInfo> pl_bound = std::vector<DimInfo>();
			for (int i = 0; i < pnts.length(); i++)
			{
				pl_bound.push_back(DimInfo(pnts[i], PointOnLine(base_pnt, vec, pnts[i])));
			}

			pl_bound.push_back(DimInfo(pnts[0], PointOnLine(base_pnt, vec, pnts[0])));

			double max_length = 0.0;
			min_pnt = pl_bound.front().origin_point;
			max_pnt = pl_bound.front().origin_point;

			for (int i = 0; i < pl_bound.size() - 1; i++)
			{
				if (pl_bound[i].dim_point.distanceTo(pl_bound[i + 1].dim_point) > max_length)
				{
					max_length = pl_bound[i].dim_point.distanceTo(pl_bound[i + 1].dim_point);
					min_pnt = pl_bound[i].origin_point;
					max_pnt = pl_bound[i + 1].origin_point;
				}
			}
		}

		//Order circles
		std::list<DimInfo> dim_infos = std::list<DimInfo>();
		dim_infos.push_back(DimInfo(min_pnt, PointOnLine(base_pnt, vec, min_pnt)));
		dim_infos.push_back(DimInfo(max_pnt, PointOnLine(base_pnt, vec, max_pnt)));
		for (int i = 0; i < ent_ids.length(); i++)
		{
			ObjectWrap<AcDbCircle> circle(DBObject::OpenObjectById<AcDbCircle>(ent_ids[i]));
			if (nullptr == circle.object)
				continue;

			AcGePoint3d pnt = PointOnLine(base_pnt, vec, circle.object->center());
			dim_infos.push_back(DimInfo(circle.object->center(), pnt));
		}

		AcGePoint3d cmp_pnt = PointOnLine(base_pnt, vec, min_pnt);
		dim_infos.sort([cmp_pnt] (const DimInfo& di1, const DimInfo& di2) {
			return di1.dim_point.distanceTo(cmp_pnt) < di2.dim_point.distanceTo(cmp_pnt);
		});

		//Dim position
		ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
		model_space.object->upgradeOpen();
		//for (auto iter = dim_infos.begin(); iter != dim_infos.end(); iter++)
		//{
		//	AcDbPoint* pnt = new AcDbPoint(iter->dim_point);
		//	model_space.object->appendAcDbEntity(pnt);
		//	pnt->close();
		//}
		for (auto iter = dim_infos.begin(); iter != dim_infos.end(); iter++)
		{
			auto tmp_iter = iter;
			tmp_iter++;

			if (tmp_iter == dim_infos.end())
				break;
			double rotated_val = AcGeVector3d::kXAxis.angleTo(vec);
			//if (rotated_val > KPI * 0.5 - 0.001)
			//	rotated_val -= KPI * 0.5;
			AcDbRotatedDimension* rotated_dim = new AcDbRotatedDimension(
				rotated_val,
				iter->origin_point,
				tmp_iter->origin_point,
				iter->dim_point,
				nullptr);

			model_space.object->appendAcDbEntity(rotated_dim);
			rotated_dim->close();
		}
	}
	catch (...)
	{

	}
}

struct MergeDimInfo
{
	double dim_length;
	AcGePoint3d point_1;
	AcGePoint3d point_2;
	AcGePoint3d dim_position;

	MergeDimInfo(double dl, const AcGePoint3d& p1, const AcGePoint3d& p2, const AcGePoint3d& dp) :
		dim_length(dl),
		point_1(p1),
		point_2(p2),
		dim_position(dp)
	{}
};

enum MergeDimType
{
	kEqualLength,
	kNaiveMerge
};

static MergeDimType MergeType(const std::list<MergeDimInfo>& dims);
static std::wstring GetDimContent(const std::list<MergeDimInfo>& dims, MergeDimType type);
void FastDim::DoMergeDim()
{
	auto obj_ids = ARXFuncs::GetObjIdsInSelected();
	std::list<MergeDimInfo> len_dims = std::list<MergeDimInfo>();
	double total_len = 0.0;
	for (int i = 0; i < obj_ids.length();)
	{
		ObjectWrap<AcDbRotatedDimension> rot_dim(DBObject::OpenObjectById<AcDbRotatedDimension>(obj_ids[i]));
		if (nullptr == rot_dim.object)
		{
			obj_ids.removeAt(i);
			continue;
		}

		double value = 0.0;
		rot_dim.object->measurement(value);
		total_len += value;
		len_dims.push_back(MergeDimInfo(value,
			rot_dim.object->xLine1Point(),
			rot_dim.object->xLine2Point(),
			rot_dim.object->dimLinePoint()));

		i++;
	}

	if (len_dims.size() < 2)
		return;

	//{
	//	ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
	//	model_space.object->upgradeOpen();

	//	for (auto iter = len_dims.begin(); iter != len_dims.end(); iter++)
	//	{
	//		AcDbPoint* pnt = new AcDbPoint(iter->dim_position);
	//		model_space.object->appendAcDbEntity(pnt);
	//		pnt->close();
	//	}
	//}
	//return;

	std::list<DimInfo> dim_infos = std::list<DimInfo>();
	double rotation_dim = 0.0;
	AcGeVector3d vec = AcGeVector3d::kYAxis;
	{
		ObjectWrap<AcDbRotatedDimension> rot_dim(DBObject::OpenObjectById<AcDbRotatedDimension>(obj_ids[0]));
		vec = vec.rotateBy(rot_dim.object->rotation(), AcGeVector3d::kZAxis);
		rotation_dim = rot_dim.object->rotation();
	}
	for (auto iter = len_dims.begin(); iter != len_dims.end(); iter++)
	{
		dim_infos.push_back(DimInfo(iter->point_1, PointOnLine(iter->point_1, vec, len_dims.front().dim_position)));
		dim_infos.push_back(DimInfo(iter->point_2, PointOnLine(iter->point_2, vec, len_dims.front().dim_position)));
	}

	//{
	//	ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
	//	model_space.object->upgradeOpen();

	//	for (auto iter = dim_infos.begin(); iter != dim_infos.end(); iter++)
	//	{
	//		AcDbPoint* pnt = new AcDbPoint(iter->dim_point);
	//		model_space.object->appendAcDbEntity(pnt);
	//		pnt->close();

	//		AcDbPoint* pnt2 = new AcDbPoint(iter->origin_point);
	//		model_space.object->appendAcDbEntity(pnt2);
	//		pnt2->close();
	//	}
	//}
	//return;

	AcGePoint3d pivot_pnt = dim_infos.front().dim_point;
	pivot_pnt = AcGePoint3d(pivot_pnt.x - vec.y * total_len * 2.0, pivot_pnt.y + vec.x * total_len * 2.0, 0.0);
	dim_infos.sort([pivot_pnt](const DimInfo& di1, const DimInfo& di2) {
		return di1.dim_point.distanceTo(pivot_pnt) > di2.dim_point.distanceTo(pivot_pnt);
	});

	AcDbRotatedDimension* ret_dim = new AcDbRotatedDimension(
		rotation_dim,
		dim_infos.front().origin_point,
		dim_infos.back().origin_point,
		dim_infos.front().dim_point);

	MergeDimType dim_type = MergeType(len_dims);
	if (MergeDimType::kEqualLength == dim_type)
	{
		ret_dim->setDimensionText(GetDimContent(len_dims, dim_type).c_str());
	}

	ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
	model_space.object->upgradeOpen();
	model_space.object->appendAcDbEntity(ret_dim);

	ret_dim->close();

	for (int i = 0; i < obj_ids.length(); i++)
	{
		ObjectWrap<AcDbEntity> ent(DBObject::OpenObjectById<AcDbEntity>(obj_ids[i]));
		ent.object->upgradeOpen();
		ent.object->erase();
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

	if (ret.crossProduct(AcGeVector3d::kXAxis).normal().isEqualTo(AcGeVector3d::kZAxis))
		ret = -ret;

	return ret;
}

AcGePoint3d PointOnLine(const AcGePoint3d & bp, const AcGeVector3d & vec, const AcGePoint3d & pnt)
{
	double ret_x = bp.x * vec.y * vec.y - bp.y * vec.x * vec.y + pnt.x * vec.x * vec.x + pnt.y * vec.x * vec.y;
	double ret_y = pnt.x * vec.x * vec.y + pnt.y * vec.y * vec.y - bp.x * vec.x * vec.y + bp.y * vec.x * vec.x;

	return AcGePoint3d(ret_x, ret_y, 0.0);
}

MergeDimType MergeType(const std::list<MergeDimInfo>& dims)
{
	if (0 == dims.size())
		throw std::exception("Failed");

	for (auto iter = dims.begin(); iter != dims.end(); iter++)
	{
		if (std::abs(iter->dim_length - dims.front().dim_length) > 1.0)
			return MergeDimType::kNaiveMerge;
	}

	return MergeDimType::kEqualLength;
}

std::wstring GetDimContent(const std::list<MergeDimInfo>& dims, MergeDimType type)
{
	std::wstring content = L"";
	content.append(std::to_wstring(std::round(dims.front().dim_length)));
	size_t index = content.find(L".");
	content.erase(index);

	content.append(L"x");
	content.append(std::to_wstring(dims.size()));
	content.append(L"=<>");
	return content;
}
