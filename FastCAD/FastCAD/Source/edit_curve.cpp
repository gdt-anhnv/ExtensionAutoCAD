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

		AcDbLine* line = new AcDbLine(fline.object->startPoint(), fline.object->endPoint());

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
