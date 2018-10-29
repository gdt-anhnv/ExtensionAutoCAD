#include "contour_line.h"
#include "SphericalTree\point2d.h"
#include "SphericalTree\mtree.h"
#include "SphericalTree\node.h"
#include "triangle.h"

#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

#include <iostream>
#include <list>
#include <vector>

struct ContourData
{
	STPoint2d point;
	double value;

	ContourData(const STPoint2d& pnt, double val) :
		point(pnt),
		value(val)
	{}
};

static std::list<ContourData> PrepareData()
{
	AcDbObjectIdArray ids = ARXFuncs::GetObjIdsInSelected();
	std::list<ContourData> data = std::list<ContourData>();
	for (int i = 0; i < ids.length(); i++)
	{
		ObjectWrap<AcDbText> txt(DBObject::OpenObjectById<AcDbText>(ids.at(i)));
		if (nullptr == txt.object)
			continue;
		data.push_back(ContourData(STPoint2d(txt.object->position().x, txt.object->position().y),
			10.0 * std::wcstod(txt.object->textString(), nullptr)));
	}

	return data;
}

static std::vector<STPoint2d> OutlineTriangle()
{
	std::vector<STPoint2d> pnts = std::vector<STPoint2d>();
	AcGePoint3d pnt1 = UserFuncs::UserGetPoint(L"First point:");
	pnts.push_back(STPoint2d(pnt1.x, pnt1.y));
	AcGePoint3d pnt2 = UserFuncs::UserGetPoint(L"First point:");
	pnts.push_back(STPoint2d(pnt2.x, pnt2.y));
	AcGePoint3d pnt3 = UserFuncs::UserGetPoint(L"First point:");
	pnts.push_back(STPoint2d(pnt3.x, pnt3.y));

	return pnts;
}

static std::list<EdgeLine> GetEdges(const std::list<Triangle*>& ents)
{
	std::list<EdgeLine> edges = std::list<EdgeLine>();

	for (auto iter = ents.begin(); iter != ents.end(); iter++)
	{
		std::list<EdgeLine> sub_edges = (*iter)->GetEdges();
		for (auto tri_iter = sub_edges.begin(); tri_iter != sub_edges.end(); tri_iter++)
		{
			bool existed = false;
			for (auto edge_iter = edges.begin(); edge_iter != edges.end(); edge_iter++)
			{
				if (*edge_iter == *tri_iter)
				{
					edges.remove(*tri_iter);
					existed = true;
					break;
				}
			}

			if (!existed)
				edges.push_back(*tri_iter);
		}
	}

	return std::move(edges);
}

static std::list<Triangle*> GetTriangles(const std::list<Node<Triangle>*>& ents)
{
	std::list<Triangle*> triangles = std::list<Triangle*>();
	for (auto iter = ents.begin(); iter != ents.end(); iter++)
		triangles.push_back((Triangle*)*iter);

	return std::move(triangles);
}

static MTree<Triangle>* DoDelauneyTriangulation(const std::list<ContourData>& data, const std::vector<STPoint2d>& pnts)
{
	MTree<Triangle>* mtree = new MTree<Triangle>();
	mtree->Insert(new Triangle(VertexInfo(pnts.at(0), 0.0), VertexInfo(pnts.at(1), 0.0), VertexInfo(pnts.at(2), 0.0)));

	for (auto iter = data.begin(); iter != data.end(); iter++)
	{
		std::list<Triangle*> ents = GetTriangles(mtree->Query(iter->point));
		for (auto ent_iters = ents.begin(); ent_iters != ents.end(); ent_iters++)
			mtree->Remove(*ent_iters);

		std::list<EdgeLine> edges = GetEdges(ents);
		for (std::list<EdgeLine>::iterator edge_iter = edges.begin(); edge_iter != edges.end(); edge_iter++)
			mtree->Insert(new Triangle(VertexInfo(iter->point, iter->value), edge_iter->fp, edge_iter->sp));
	}

	for (int i = 0; i < pnts.size(); i++)
	{
		auto rt = mtree->Query(pnts.at(i));
		for (auto iter = rt.begin(); iter != rt.end(); iter++)
			mtree->Remove(*iter);
	}

	return mtree;
}

static void DrawLeaves(const std::list<Triangle*>& tris)
{
	ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
	model_space.object->upgradeOpen();

	for (auto iter = tris.begin(); iter != tris.end(); iter++)
	{
		Triangle* tri = dynamic_cast<Triangle*>(*iter);
		if (nullptr == tri)
			continue;

		auto edges = tri->GetEdges();
		for (auto edge_iter = edges.begin(); edge_iter != edges.end(); edge_iter++)
		{
			AcDbLine* line = new AcDbLine(
				AcGePoint3d(edge_iter->fp.position.x, edge_iter->fp.position.y, 0.0),
				AcGePoint3d(edge_iter->sp.position.x, edge_iter->sp.position.y, 0.0));
			model_space.object->appendAcDbEntity(line);
			line->close();
		}

		//AcDbCircle* cir = new AcDbCircle(AcGePoint3d(tri->Center().x, tri->Center().y, 0.0), AcGeVector3d::kZAxis, tri->Radius());
		//model_space.object->appendAcDbEntity(cir);
		//cir->close();
	}
}

static void DrawContourLine(const std::list<Triangle*>& tris, double step)
{
	ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
	model_space.object->upgradeOpen();

	for (auto iter = tris.begin(); iter != tris.end(); iter++)
	{
		if (!(*iter)->IsValidTriangle())
			continue;
		STPoint2d pnt1(0.0, 0.0);
		STPoint2d pnt2(0.0, 0.0);
		auto lines = (*iter)->DrawContour(step);
		for(auto iter = lines.begin(); iter != lines.end(); iter++)
		{
			AcDbLine* line = new AcDbLine(
				AcGePoint3d(iter->fp.position.x, iter->fp.position.y, 0.0),
				AcGePoint3d(iter->sp.position.x, iter->sp.position.y, 0.0));
			model_space.object->appendAcDbEntity(line);
			line->close();
		}
		//else
		//{
		//	auto edges = (*iter)->GetEdges();
		//	for (auto edge_iter = edges.begin(); edge_iter != edges.end(); edge_iter++)
		//	{
		//		AcDbLine* line = new AcDbLine(
		//			AcGePoint3d(edge_iter->fp.position.x, edge_iter->fp.position.y, 0.0),
		//			AcGePoint3d(edge_iter->sp.position.x, edge_iter->sp.position.y, 0.0));
		//		model_space.object->appendAcDbEntity(line);
		//		line->close();
		//	}
		//}
	}
}

void ContourLine::DoContourLine()
{
	try
	{
		//get all data
		std::list<ContourData> data = PrepareData();

		//get outline pnts
		std::vector<STPoint2d> pnts = OutlineTriangle();

		int step = UserFuncs::GetInt(L"Nhập step:");

		//Do Delauney Triangulations
		MTree<Triangle>* tris = DoDelauneyTriangulation(data, pnts);

		//Debug: Draw all rectangles
		//DrawLeaves(tris->AllLeaves());
		DrawContourLine(tris->AllLeaves(), step);
	}
	catch (...)
	{

	}
}