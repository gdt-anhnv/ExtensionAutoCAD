#include "edit_curve.h"
#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

void EditCurve::ConnectLine()
{
	try
	{
		AcDbObjectIdArray tmp_first_ids = ARXFuncs::GetObjIdsByPicking();
		if (0 == tmp_first_ids.length())
			return;

		AcDbObjectIdArray tmp_second_ids = ARXFuncs::GetObjIdsByPicking();
		if (0 == tmp_second_ids.length())
			return;

		AcDbObjectId fid = tmp_first_ids[0];
		AcDbObjectId sid = tmp_second_ids[0];

		if (fid == sid)
			return;

		ObjectWrap<AcDbLine> fline(DBObject::OpenObjectById<AcDbLine>(fid));
		ObjectWrap<AcDbLine> sline(DBObject::OpenObjectById<AcDbLine>(sid));

		AcString layer = fline.object->layer();

		AcDbLine* line = new AcDbLine(fline.object->startPoint(), fline.object->endPoint());
		line->setLayer(layer);

		if (fline.object->startPoint().distanceTo(sline.object->startPoint()) >
			line->startPoint().distanceTo(line->endPoint()))
		{
			line->setStartPoint(fline.object->startPoint());
			line->setEndPoint(sline.object->startPoint());
		}

		if (fline.object->startPoint().distanceTo(sline.object->endPoint()) >
			line->startPoint().distanceTo(line->endPoint()))
		{
			line->setStartPoint(fline.object->startPoint());
			line->setEndPoint(sline.object->endPoint());
		}

		if (fline.object->endPoint().distanceTo(sline.object->startPoint()) >
			line->startPoint().distanceTo(line->endPoint()))
		{
			line->setStartPoint(fline.object->endPoint());
			line->setEndPoint(sline.object->startPoint());
		}

		if (fline.object->endPoint().distanceTo(sline.object->endPoint()) >
			line->startPoint().distanceTo(line->endPoint()))
		{
			line->setStartPoint(fline.object->endPoint());
			line->setEndPoint(sline.object->endPoint());
		}

		ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(
			acdbHostApplicationServices()->workingDatabase()));
		model_space.object->upgradeOpen();
		model_space.object->appendAcDbEntity(line);
		line->close();

		fline.object->upgradeOpen();
		fline.object->erase();

		sline.object->upgradeOpen();
		sline.object->erase();
	}
	catch (...)
	{
	}
}

void EditCurve::ScraperNum()
{
	AcDbObjectIdArray ids = UserFuncs::UserGetEnts();
	double total_len = 0.0;
	for (int i = 0; i < ids.length(); i++)
	{
		ObjectWrap<AcDbLine> line_wrap(DBObject::OpenObjectById<AcDbLine>(ids[i]));
		if (nullptr != line_wrap.object)
		{
			total_len +=
				line_wrap.object->startPoint().distanceTo(line_wrap.object->endPoint());
			continue;
		}

		ObjectWrap<AcDbArc> curve_wrap(DBObject::OpenObjectById<AcDbArc>(ids[i]));
		if (nullptr != curve_wrap.object)
		{
			total_len += curve_wrap.object->length();
			continue;
		}
	}

	ads_real div;
	acedGetReal(L"Nhập số chia: ", &div);
	acutPrintf(L"%f", total_len / div);
}
