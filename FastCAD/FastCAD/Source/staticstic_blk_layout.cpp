#include "../stdafx.h"
#include "staticstic_blk_layout.h"
#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

#include <iostream>
#include <windows.h>

struct StaticsticData
{
	std::wstring title;
	std::wstring name;
	std::wstring scale;
};

void LayoutFuncs::StaticsticBlkLayout()
{
}

static void GetData()
{
	ObjectWrap<AcDbBlockTable> blk_tbl_wrap(DBObject::GetBlockTable(acdbCurDwg()));

	AcDbBlockTableIterator* iter = nullptr;
	blk_tbl_wrap.object->newIterator(iter);

	for (; !iter->done(); iter->step())
	{
		AcDbObjectId id = AcDbObjectId::kNull;
		iter->getRecordId(id);

		ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd_wrap(DBObject::OpenObjectById<AcDbBlockTableRecord>(id));
	}

	delete iter;
}
