#include "../stdafx.h"
#include "staticstic_blk_layout.h"
#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

#include "acaplmgr.h"

#include <iostream>
#include <windows.h>
#include <vector>

#define BLK_STATISTIC_NAME				L"DANHSACHBANVE"
#define TITLE_ATT_NAME					L"TENBANVE-ENGTEXT"
#define SECOND_TITLE_ATT_NAME			L"TENBANVE-VNTEXT"
#define NAME_DRAWING_ATT_NAME			L"KIHIEUBANVE"
#define SCALE_ATT_NAME					L"TILEBANVE"

#define TABLE_ROW_HEIGHT				1000
#define TABLE_NAME_COL_WIDTH			10000
#define TABLE_ID_COL_WIDTH				2500

#define TEXT_HEIGHT						200.0

struct TextData
{
	std::wstring value;
	double text_height;
	std::wstring layer;

	TextData() :
		value(std::wstring(L"")),
		text_height(TEXT_HEIGHT),
		layer(std::wstring(L""))
	{

	}

	TextData(std::wstring val, double th, std::wstring lay) :
		value(val),
		text_height(th),
		layer(lay)
	{}

	static AcDbText* CreateText(const TextData& td, AcGePoint3d pos)
	{
		AcDbText* text = new AcDbText(pos, td.value.c_str(), AcDbObjectId::kNull, td.text_height);
		text->setLayer(td.layer.c_str());

		return text;
	}
};

struct StaticsticData
{
	TextData title;
	TextData second_title;
	TextData name;
	TextData scale;
};

static std::vector<StaticsticData> GetData();
static void DrawTable(const std::vector<StaticsticData>& data, const AcGePoint3d& ins_pnt);
static void Testing();
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

static AcDbDictionaryIterator* GetLayoutIter()
{
	ObjectWrap<AcDbDictionary> layout_dict_wrap(nullptr);
	if (Acad::eOk != acdbCurDwg()->getLayoutDictionary(layout_dict_wrap.object))
		return nullptr;

	return layout_dict_wrap.object->newIterator();
}

void LayoutFuncs::IndexLayout()
{
	try
	{
		auto layout_data = GetData();
		if (0 == layout_data.size())
			return;

		std::wstring name = layout_data[0].name.value;
		name = name.substr(0, name.length() - 2);

		std::list<std::wstring> curr_name = std::list<std::wstring>();
		std::list<AcDbHandle> handles = std::list<AcDbHandle>();
		//reset name layout
		int count = 0;
		{
			int index = 1;
			AcDbDictionaryIterator* dict_iter = GetLayoutIter();
			for (; !dict_iter->done(); dict_iter->next())
			{
				count++;
				std::wstring layout_name = L"our_layout-";
				layout_name += std::to_wstring(index++);

				ObjectWrap<AcDbObject> layout_wrap(nullptr);
				dict_iter->getObject(layout_wrap.object);
				AcDbLayout* layout = AcDbLayout::cast(layout_wrap.object);
				ACHAR* tmp;
				layout->getLayoutName(tmp);
				curr_name.push_back(tmp);
				AcDbHandle tmp_handle = AcDbHandle();
				layout_wrap.object->getAcDbHandle(tmp_handle);
				handles.push_back(tmp_handle);
				Acad::ErrorStatus err = layout_wrap.object->upgradeOpen();
				err = layout->setLayoutName(layout_name.c_str());
			}

			delete dict_iter;
		}

		{
			int index = 1;
			AcDbDictionaryIterator* dict_iter = GetLayoutIter();
			for (; !dict_iter->done(); dict_iter->next())
			{
				AcDbObjectId btr_id = AcDbObjectId::kNull;
				{
					ObjectWrap<AcDbObject> layout_wrap(nullptr);
					dict_iter->getObject(layout_wrap.object);
					AcDbLayout* layout = AcDbLayout::cast(layout_wrap.object);
					std::wstring layout_name = name;
					if (index < 10)
						layout_name += L"0";
					layout_name += std::to_wstring(index);

					Acad::ErrorStatus err = layout_wrap.object->upgradeOpen();
					ACHAR* test_name;
					layout->getLayoutName(test_name);
					err = layout->setLayoutName(layout_name.c_str());

					index++;
				}
			}

			delete dict_iter;
		}
		AcApLayoutManager* layout_mgr = (AcApLayoutManager*)acdbHostApplicationServices()->layoutManager();
		layout_mgr->updateLayoutTabs();
	}
	catch(...)
	{ }
}

void Testing()
{
	AcDbLayoutManager* layout_man = acdbHostApplicationServices()->layoutManager();
	AcDbDictionary* layout_dict = nullptr;
	if (Acad::eOk == acdbCurDwg()->getLayoutDictionary(layout_dict))
	{
		AcDbObject* obj = nullptr;
		ACHAR* layout_name;
		AcDbDictionaryIterator* dict_iter = layout_dict->newIterator();

		for (; !dict_iter->done(); dict_iter->next())
		{
			dict_iter->getObject(obj);
			AcDbLayout* layout = AcDbLayout::cast(obj);
			layout->getLayoutName(layout_name);
			layout->close();
		}

		delete dict_iter;
	}

	layout_dict->close();
}

std::vector<StaticsticData> GetData()
{
	std::vector<StaticsticData> ret = std::vector<StaticsticData>();

	ObjectWrap<AcDbDictionary> layout_dict_wrap(nullptr);
	if (Acad::eOk != acdbCurDwg()->getLayoutDictionary(layout_dict_wrap.object))
		return ret;

	AcDbDictionaryIterator* dict_iter = layout_dict_wrap.object->newIterator();
	for (; !dict_iter->done(); dict_iter->next())
	{
		AcDbObjectId btr_id = AcDbObjectId::kNull;
		{
			ObjectWrap<AcDbObject> layout_wrap(nullptr);
			dict_iter->getObject(layout_wrap.object);
			AcDbLayout* layout = AcDbLayout::cast(layout_wrap.object);

			btr_id = layout->getBlockTableRecordId();
		}
	
		ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd_wrap(DBObject::OpenObjectById<AcDbBlockTableRecord>(btr_id));

		AcString btr_name = AcString();
		blk_tbl_rcd_wrap.object->getName(btr_name);
		if (0 == btr_name.compare(ACDB_MODEL_SPACE))
			continue;

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
					data.title = TextData(attribute.object->textString(), TEXT_HEIGHT, attribute.object->layer());
				else if (0 == std::wstring(attribute.object->tag()).compare(NAME_DRAWING_ATT_NAME))
					data.name = TextData(attribute.object->textString(), TEXT_HEIGHT, attribute.object->layer());
				else if (0 == std::wstring(attribute.object->tag()).compare(SCALE_ATT_NAME))
					data.scale = TextData(attribute.object->textString(), TEXT_HEIGHT, attribute.object->layer());
				else if (0 == std::wstring(attribute.object->tag()).compare(SECOND_TITLE_ATT_NAME))
					data.second_title = TextData(attribute.object->textString(), TEXT_HEIGHT, attribute.object->layer());
			}

			ret.push_back(data);

		}

		delete rcd_iter;
	}

	delete dict_iter;
	return ret;
}

void DrawTable(const std::vector<StaticsticData>& data, const AcGePoint3d& ins_pnt)
{
	for (int i = 0; i < data.size(); i++)
	{
		DBObject::AppendToModelSpace(TextData::CreateText(data[i].second_title,
			ins_pnt - i * TABLE_ROW_HEIGHT * AcGeVector3d::kYAxis));

		DBObject::AppendToModelSpace(TextData::CreateText(data[i].title,
			ins_pnt - (i * TABLE_ROW_HEIGHT + 360.0)* AcGeVector3d::kYAxis));

		DBObject::AppendToModelSpace(TextData::CreateText(data[i].name,
			ins_pnt - i * TABLE_ROW_HEIGHT * AcGeVector3d::kYAxis + TABLE_NAME_COL_WIDTH * AcGeVector3d::kXAxis));

		DBObject::AppendToModelSpace(TextData::CreateText(data[i].scale,
			ins_pnt - i * TABLE_ROW_HEIGHT * AcGeVector3d::kYAxis + (TABLE_NAME_COL_WIDTH + TABLE_ID_COL_WIDTH) * AcGeVector3d::kXAxis));
	}
}
