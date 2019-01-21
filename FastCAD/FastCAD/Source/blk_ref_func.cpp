#include "blk_ref_func.h"
#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

#include <iostream>
#include <windows.h>

void BlkRefFunc::CopyBlkName()
{
	try
	{
		AcDbObjectIdArray ids = ARXFuncs::GetObjIdsByPicking();
		if (0 == ids.length())
			return;

		AcDbObjectId picked_id = ids[0];
		ObjectWrap<AcDbBlockReference> blk_ref(DBObject::OpenObjectById<AcDbBlockReference>(picked_id));
		if (nullptr == blk_ref.object)
			return;

		AcDbObjectId blk_id = blk_ref.object->blockTableRecord();
		ObjectWrap<AcDbBlockTableRecord> source_blk(DBObject::OpenObjectById<AcDbBlockTableRecord>(blk_id));
		if (nullptr == source_blk.object)
			return;

		AcString blk_name = AcString();
		source_blk.object->getName(blk_name);
		//std::string test = std::string("abcadsfa");
		int len = (blk_name.length() + 1) * sizeof(wchar_t);
		HGLOBAL hMem = GlobalAlloc(GMEM_MOVEABLE, len);
		memcpy((LPWSTR)GlobalLock(hMem), blk_name.constPtr(), len);
		GlobalUnlock(hMem);
		OpenClipboard(0);
		EmptyClipboard();
		SetClipboardData(CF_UNICODETEXT, hMem);
		CloseClipboard();
	}
	catch (...)
	{

	}
}

AcString replace_blk_name = AcString();
void BlkRefFunc::CopyBlk()
{
	try
	{
		AcDbObjectIdArray ids = ARXFuncs::GetObjIdsByPicking();
		if (0 == ids.length())
			return;

		AcDbObjectId picked_id = ids[0];
		ObjectWrap<AcDbBlockReference> blk_ref(DBObject::OpenObjectById<AcDbBlockReference>(picked_id));
		if (nullptr == blk_ref.object)
			return;

		AcDbObjectId blk_id = blk_ref.object->blockTableRecord();
		ObjectWrap<AcDbBlockTableRecord> source_blk(DBObject::OpenObjectById<AcDbBlockTableRecord>(blk_id));
		if (nullptr == source_blk.object)
			return;

		//AcString blk_name = AcString();
		source_blk.object->getName(replace_blk_name);

		ads_name ads;
		acedSSAdd(NULL, NULL, ads);
		ads_name tmp = { 0, 0 };
		Acad::ErrorStatus ret = acdbGetAdsName(tmp, ids[0]);
		acedSSAdd(tmp, ads, ads);

		acedCommandS(RTSTR, L"COPYCLIP", RTPICKS, ads, RTSTR, L"", 0);
	}
	catch (...)
	{

	}
}

static AcDbObjectId BlkNameExisted()
{
	try
	{
		ObjectWrap<AcDbBlockTable> blk_tbl(DBObject::GetBlockTable(acdbHostApplicationServices()->workingDatabase()));
		AcDbObjectId blk_id;
		if (Acad::eOk != blk_tbl.object->getAt(replace_blk_name.constPtr(), blk_id))
			return AcDbObjectId::kNull;

		ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd(DBObject::OpenObjectById<AcDbBlockTableRecord>(blk_id));
		if (nullptr == blk_tbl_rcd.object)
			return AcDbObjectId::kNull;

		return blk_id;
	}
	catch (...)
	{

	}

	return AcDbObjectId::kNull;
}

void BlkRefFunc::PasteBlk()
{
	try
	{
		AcDbObjectId blk_id = BlkNameExisted();
		AcDbObjectIdArray ids;
		if (AcDbObjectId::kNull != blk_id)
		{
			ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd(DBObject::OpenObjectById<AcDbBlockTableRecord>(blk_id));
			AcString rep_name = AcString(replace_blk_name);
			rep_name.append(L"_1");
			blk_tbl_rcd.object->upgradeOpen();
			blk_tbl_rcd.object->setName(rep_name.constPtr());
			blk_tbl_rcd.object->getBlockReferenceIds(ids);
		}

		acedCommandS(RTSTR, L"PASTECLIP", RTSTR, L"", 0);
		AcDbObjectId rep_id = BlkNameExisted();
		for (int i = 0; i < ids.length(); i++)
		{
			ObjectWrap<AcDbBlockReference> blk_ref(DBObject::OpenObjectById<AcDbBlockReference>(ids[i]));
			blk_ref.object->upgradeOpen();
			blk_ref.object->setBlockTableRecord(rep_id);
		}

		if (AcDbObjectId::kNull != blk_id)
		{
			ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd(DBObject::OpenObjectById<AcDbBlockTableRecord>(blk_id));
			blk_tbl_rcd.object->upgradeOpen();
			blk_tbl_rcd.object->erase();
		}
	}
	catch(...)
	{ }
}

void BlkRefFunc::ChangeBlkBase()
{
	try
	{
		AcDbObjectIdArray ids = ARXFuncs::GetObjIdsByPicking();
		if (0 == ids.length())
			return;

		AcGePoint3d ins_pnt = AcGePoint3d::kOrigin;
		AcDbObjectId btr_id = AcDbObjectId::kNull;
		{
			ObjectWrap<AcDbBlockReference> br_obj(DBObject::OpenObjectById<AcDbBlockReference>(ids[0]));
			if (nullptr == br_obj.object)
				return;
			ins_pnt = br_obj.object->position();
			btr_id = br_obj.object->blockTableRecord();
		}
		AcGePoint3d bp = UserFuncs::UserGetPoint(L"Pick new base point");

		AcGeMatrix3d trans = AcGeMatrix3d::kIdentity;
		trans.setToTranslation(ins_pnt - bp);

		ObjectWrap<AcDbBlockTableRecord> blk_tbl_rcd(DBObject::OpenObjectById<AcDbBlockTableRecord>(btr_id));
		AcDbBlockTableRecordIterator* iter = nullptr;
		blk_tbl_rcd.object->newIterator(iter);

		while (!iter->done())
		{
			AcDbEntity* ent = nullptr;
			if (Acad::eOk != iter->getEntity(ent, AcDb::kForRead))
				continue;

			ent->upgradeOpen();
			ent->transformBy(trans);
			ent->close();

			iter->step();
		}

		trans = trans.inverse();
		AcDbObjectIdArray br_ids = AcDbObjectIdArray();
		if (Acad::eOk == blk_tbl_rcd.object->getBlockReferenceIds(br_ids))
		{
			for (int i = 0; i < br_ids.length(); i++)
			{
				ObjectWrap<AcDbBlockReference> br_obj(DBObject::OpenObjectById<AcDbBlockReference>(br_ids[i]));
				br_obj.object->upgradeOpen();
				br_obj.object->recordGraphicsModified();
				br_obj.object->transformBy(trans);
			}
		}
	}
	catch(...) {}
}
