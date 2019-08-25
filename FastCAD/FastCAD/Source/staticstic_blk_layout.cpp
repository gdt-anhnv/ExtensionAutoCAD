#include "../stdafx.h"
#include "staticstic_blk_layout.h"
#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

#include <iostream>
#include <windows.h>
#include <vector>

#define BLK_STATISTIC_NAME				L"TestingBlk"
#define TITLE_ATT_NAME					L"TITLE"
#define NAME_DRAWING_ATT_NAME			L"NAMEDRAWING"
#define SCALE_ATT_NAME					L"SCALE"

#define TABLE_ROW_HEIGHT				1000
#define TABLE_NAME_COL_WIDTH			10000
#define TABLE_ID_COL_WIDTH				2500

#define TEXT_HEIGHT						200.0

struct TextData
{
	std::wstring value;
	double text_height;

	TextData() :
		value(std::wstring(L"")),
		text_height(TEXT_HEIGHT)
	{

	}

	TextData(std::wstring val, double th) :
		value(val),
		text_height(th)
	{}

	static AcDbText* CreateText(const TextData& td, AcGePoint3d pos)
	{
		return new AcDbText(pos, td.value.c_str(), AcDbObjectId::kNull, td.text_height);
	}
};

struct StaticsticData
{
	TextData title;
	TextData name;
	TextData scale;
};

static std::vector<StaticsticData> GetData();
static void DrawTable(const std::vector<StaticsticData>& data, const AcGePoint3d& ins_pnt);
void LayoutFuncs::StaticsticBlkLayout()
{
	try
	{
		AcGePoint3d ins_pnt = UserFuncs::UserGetPoint(std::wstring(L"Pick diem de tao bang thong ke"));
		DrawTable(GetData(), ins_pnt);
	}
	catch (...)
	{

	}
}

std::vector<StaticsticData> GetData()
{
	std::vector<StaticsticData> ret = std::vector<StaticsticData>();

	ObjectWrap<AcDbBlockTable> blk_tbl_wrap(DBObject::GetBlockTable(acdbCurDwg()));

	AcDbBlockTableIterator* iter = nullptr;
	blk_tbl_wrap.object->newIterator(iter);

	for (; !iter->done(); iter->step())
	{
		AcDbObjectId id = AcDbObjectId::kNull;
		iter->getRecordId(id);

		ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd_wrap(DBObject::OpenObjectById<AcDbBlockTableRecord>(id));

		AcDbBlockTableRecordIterator* rcd_iter = nullptr;
		blk_tbl_rcd_wrap.object->newIterator(rcd_iter);
		for (; !rcd_iter->done(); rcd_iter->step())
		{
			AcDbObjectId ent_id = AcDbObjectId::kNull;
			rcd_iter->getEntityId(ent_id);
			{
				ObjectWrap<AcDbEntity> entity(DBObject::OpenObjectById<AcDbEntity>(ent_id));
				if (!entity.object->isKindOf(AcDbBlockReference::desc()))
					continue;
			}

			if (0 != BlockReferenceFuncs::GetBlkRefName(ent_id).compare(BLK_STATISTIC_NAME))
				continue;

			AcDbObjectIdArray att_ids = AttributeFuncs::GetAttribute(ent_id);
			StaticsticData data;
			for (int i = 0; i < att_ids.length(); i++)
			{
				ObjectWrap<AcDbAttribute> attribute(DBObject::OpenObjectById<AcDbAttribute>(att_ids[i]));
				if (0 == std::wstring(attribute.object->tag()).compare(TITLE_ATT_NAME))
					data.title = TextData(attribute.object->textString(), TEXT_HEIGHT);
				else if (0 == std::wstring(attribute.object->tag()).compare(NAME_DRAWING_ATT_NAME))
					data.name = TextData(attribute.object->textString(), TEXT_HEIGHT);
				else if (0 == std::wstring(attribute.object->tag()).compare(SCALE_ATT_NAME))
					data.scale = TextData(attribute.object->textString(), TEXT_HEIGHT);
			}

			ret.push_back(data);

		}

		delete rcd_iter;
	}

	delete iter;

	return ret;
}

void DrawTable(const std::vector<StaticsticData>& data, const AcGePoint3d& ins_pnt)
{
	for (int i = 0; i < data.size(); i++)
	{
		DBObject::AppendToModelSpace(TextData::CreateText(data[i].title,
			ins_pnt - i * TABLE_ROW_HEIGHT * AcGeVector3d::kYAxis));

		DBObject::AppendToModelSpace(TextData::CreateText(data[i].name,
			ins_pnt - i * TABLE_ROW_HEIGHT * AcGeVector3d::kYAxis + TABLE_NAME_COL_WIDTH * AcGeVector3d::kXAxis));

		DBObject::AppendToModelSpace(TextData::CreateText(data[i].scale,
			ins_pnt - i * TABLE_ROW_HEIGHT * AcGeVector3d::kYAxis + (TABLE_NAME_COL_WIDTH + TABLE_ID_COL_WIDTH) * AcGeVector3d::kXAxis));
	}
}
