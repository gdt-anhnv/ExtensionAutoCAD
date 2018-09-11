#include "ToolArxFuncs.h"

static std::wstring ConvertIdToWstring(AcDbObjectId id)
{

	std::wstringstream ss;
	ss << id;
	std::wstring wstr = ss.str();
	for (int i = 0; i < wstr.length(); ++i)
	{
		int val = int(wstr[i]);
		if (val < 91 && val > 64)
		{
			val = val + 32;
			wstr[i] = wchar_t(val);
		}
	}
	return wstr;
}

static void PickObject()
{
	AcDbObjectIdArray ids = ARXFuncs::GetObjIdsInSelected();
	for (int i = 0; i < ids.length(); i++)
	{
		AcDbObjectId id = ids[i];
		std::wstringstream ss;
		ss << id;
		std::wstring wstr = ss.str();

	}
}

static ErrorStatus CheckIdExistInDatabase(AcDbObjectId & id, std::wstring str)
{
	AcDbBlockTableRecordIterator *iter = NULL;
	{
		ObjectWrap<AcDbBlockTableRecord> rcd_wrap(
			DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
		if (NULL == rcd_wrap.object)
		{
			acutPrintf(L"Model_Space is not found!");
			return ErrorStatus::eDelIsModelSpace;
		}
		rcd_wrap.object->newIterator(iter);
	}
	while (!iter->done())
	{
		AcDbObjectId ent_id = AcDbObjectId::kNull;
		iter->getEntityId(ent_id);
		std::wstring wst_id = ConvertIdToWstring(ent_id);
		if (wcscmp(wst_id.c_str(), str.c_str()) == 0)
		{
			id = ent_id;
			return ErrorStatus::eOk;
		}
		iter->step();
	}
	return ErrorStatus::eFileNotFound;
}

#define CAD14
std::wstring ToolArxFuncs::GetString(std::wstring prompt)
{
	ACHAR szResult[256];
	size_t size;
	int rc =
#ifdef CAD14
		acedGetString(true, prompt.c_str(), szResult);
#else
		acedGetString(true,prompt.c_str(), szResult, size);
#endif
	switch (rc)
	{
	case RTNORM:
		return std::wstring(szResult);
	case RTCAN:
		acutPrintf(L"\nUser canceled");
		return std::wstring();
	case RTERROR:
		acutPrintf(L"\nloi input ");
		return std::wstring();
		break;
	}
}

AcDbObjectId ToolArxFuncs::DrawCirleMarker(AcGePoint3d center_, double rad)
{
	AcDbObjectId object_id = AcDbObjectId::kNull;
	ObjectWrap<AcDbCircle> cir(new AcDbCircle(center_,AcGeVector3d::kZAxis, rad));
	if (cir.object == nullptr)
		return object_id;
	cir.object->setColorIndex(3);
	ObjectWrap<AcDbBlockTableRecord> modal_space(DBObject::GetModelSpace(
									acdbHostApplicationServices()->workingDatabase()));
	if (modal_space.object == nullptr)
		return object_id;
	modal_space.object->upgradeOpen();
	modal_space.object->appendAcDbEntity(object_id, cir.object);
	return object_id;
}

void ToolArxFuncs::CheckProxyEntity()
{
	ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(
									acdbHostApplicationServices()->workingDatabase()));
	ObjectWrap<AcDbBlockTableRecordIterator> *iter = NULL;

}

void ToolArxFuncs::SearchEntityById()
{
	std::wstring str_id = ToolArxFuncs::GetString(L"Input Entity Id: ");
	if (!str_id.empty())
	{
		for (int i = 0; i < str_id.length(); ++i)
		{
			if (str_id[i] == 'x')
			{
				str_id.erase(0, i + 1);
				break;
			}
		}
	}
	else
	{
		return;
	}
	AcDbObjectId ent_id = AcDbObjectId::kNull;
	ErrorStatus err = CheckIdExistInDatabase(ent_id, str_id);
	if (err == eOk)
	{
		if (!ent_id.isNull())
		{
			ObjectWrap<AcDbEntity> ent_wrap(NULL);
			if (ErrorStatus::eOk == acdbOpenAcDbEntity(ent_wrap.object, ent_id, AcDb::kForRead, false))
			{
				if (ent_wrap.object != NULL)
				{
					AcDbExtents ext = AcDbExtents();
					if (ErrorStatus::eOk == ent_wrap.object->getGeomExtents(ext))
					{
						AcDbPolyline * pl = new AcDbPolyline();
						pl->addVertexAt(0, AcGePoint2d(ext.minPoint().x, ext.minPoint().y));
						pl->addVertexAt(1, AcGePoint2d(ext.maxPoint().x, ext.minPoint().y));
						pl->addVertexAt(2, AcGePoint2d(ext.maxPoint().x, ext.maxPoint().y));
						pl->addVertexAt(3, AcGePoint2d(ext.minPoint().x, ext.maxPoint().y));
						pl->addVertexAt(4, AcGePoint2d(ext.minPoint().x, ext.minPoint().y));
						ObjectWrap<AcDbBlockTableRecord> rcd_wrap(
							DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
						if (NULL != rcd_wrap.object)
						{
							rcd_wrap.object->upgradeOpen();
							rcd_wrap.object->appendAcDbEntity(pl);
						}
						pl->close();
						ARXFuncs::ZoomIntoZone(ent_id);
					}
				}
			}
		}
	}
}