#include "DBObject.h"
#include "../wrap_header.h"
#include <sstream>

AcDbBlockTable * DBObject::GetBlockTable(AcDbDatabase * db)
{
	AcDbBlockTable* blk_tbl = NULL;
	if (Acad::eOk != db->getBlockTable(blk_tbl, AcDb::OpenMode::kForRead))
		throw std::exception("Failed when get Block Table");
	return blk_tbl;
}

AcDbBlockTableRecord * DBObject::GetModelSpace(AcDbDatabase * db)
{
	return GetBlkTblRcd(db, ACDB_MODEL_SPACE);
}

AcDbBlockTableRecord * DBObject::GetBlkTblRcd(AcDbDatabase * db, const wchar_t * blk_name)
{
	AcDbBlockTableRecord* blk_tbl_rcd = NULL;
	{
		ObjectWrap<AcDbBlockTable> blk_tbl_wrap(GetBlockTable(db));
		if (Acad::eOk != blk_tbl_wrap.object->getAt(blk_name, blk_tbl_rcd, AcDb::kForRead))
			throw std::exception("Failed when get Block Table Record");
	}
	return blk_tbl_rcd;
}

AcDbObjectId DBObject::FindBlockByName(AcDbDatabase * db, const wchar_t * bn)
{
		ObjectWrap<AcDbBlockTable> blk_tbl_wrap(DBObject::GetBlockTable(db));
		AcDbObjectId blk_id = AcDbObjectId::kNull;
		blk_tbl_wrap.object->getAt(bn, blk_id);

		return blk_id;
}

AcDbObjectIdArray DBObject::FindBlockRefsByName(AcDbDatabase * db, const wchar_t * name)
{
	AcDbObjectIdArray br_ids;

	AcDbObjectId obj_id = FindBlockByName(db, name);
	ObjectWrap<AcDbObject> obj_wrap(DBObject::OpenObjectById<AcDbObject>(obj_id));

	if (NULL != obj_wrap.object && obj_wrap.object->isKindOf(AcDbBlockTableRecord::desc()))
	{
		AcDbBlockTableRecord::cast(obj_wrap.object)->getBlockReferenceIds(br_ids);
	}

	return br_ids;
}

ErrorStatus DBObject::Erase(const AcDbObjectId & id)
{
	ObjectWrap<AcDbEntity> ent = OpenObjectById<AcDbEntity>(id);

	if (NULL != ent.object)
	{
		ent.object->upgradeOpen();
		if (!ent.object->isErased())
			return ent.object->erase();
	}
	return ErrorStatus::eCannotBeErasedByCaller;
}

AcDbObjectId DBObject::CopyEntity(const AcDbObjectId & id)
{
	AcDbIdMapping mapping;
	AcDbObjectIdArray ids = AcDbObjectIdArray();
	ids.append(id);

	AcDbObjectId model_space_id = AcDbObjectId::kNull;
	{
		ObjectWrap<AcDbBlockTableRecord> model_space(GetModelSpace(acdbCurDwg()));
		model_space_id = model_space.object->id();
	}
	acdbCurDwg()->deepCloneObjects(ids, model_space_id, mapping);

	AcDbIdMappingIter mapping_iter(mapping);
	AcDbIdPair pair;
	for (mapping_iter.start(); !mapping_iter.done(); mapping_iter.next())
	{
		if (!mapping_iter.getMap(pair))
			continue;
		if (!pair.isCloned())
			continue;
		if (pair.key() == id)
			return pair.value();
		//return pair.value();
	}

	return AcDbObjectId::kNull;
}

AcDbObjectId DBObject::AppendToModelSpace(AcDbEntity * ent)
{
	AcDbObjectId id = AcDbObjectId::kNull;
	ObjectWrap<AcDbBlockTableRecord> rcd_wrap(DBObject::GetModelSpace(acdbCurDwg()));
	rcd_wrap.object->upgradeOpen();

	if (Acad::eOk == rcd_wrap.object->appendAcDbEntity(id, ent))
	{
		ent->close();
	}
	else
		return AcDbObjectId::kNull;

	return id;
}

AcDbObjectIdArray DBObject::GetNextEnts(const AcDbObjectId & id)
{
	AdsNameWrap flag;
	acdbGetAdsName(flag.ads, id);

	AcDbObjectIdArray ids = AcDbObjectIdArray();
	AcDbObjectId tmp_id = AcDbObjectId::kNull;
	do
	{
		acdbEntNext(flag.ads, flag.ads);

		tmp_id = AcDbObjectId::kNull;

		if (Acad::eOk == acdbGetObjectId(tmp_id, flag.ads))
			ids.append(tmp_id);

	} while (AcDbObjectId::kNull != tmp_id);

	return ids;
}

AcDbObjectId DBObject::GetObjIdFromHandle(const AcDbHandle& handle)
{
	AcString handle_str = GetHandleStr(handle);

	ads_name entries = { 0, 0 };
	acdbHandEnt(handle_str.constPtr(), entries);

	AcDbObjectId obj_id = AcDbObjectId::kNull;
	acdbGetObjectId(obj_id, entries);
	return obj_id;
}

AcString DBObject::GetHandleStr(const AcDbHandle& handle)
{
	ACHAR p[33];
#ifdef ACAD_2012
	handle.getIntoAsciiBuffer(p);
#else
	handle.getIntoAsciiBuffer(p, 33);
#endif

	return AcString(p);
}

void DBObject::CreatePickingName(const AcDbObjectIdArray & ids, ads_name & name)
{
	acedSSAdd(NULL, NULL, name);

	for (int i = 0; i < ids.length(); i++)
	{
		ads_name tmp = { 0, 0 };
		acdbGetAdsName(tmp, ids.at(i));
		acedSSAdd(tmp, name, name);
	}
}

//AcDbObjectId DBObject::GetObjectidFromHandle(const int & handle)
//{
//	AdsNameWrap ads_name;
//	std::wstringstream ss;
//	ss << std::hex << handle;
//	std::wstring whandle = ss.str();
//
//	acdbHandEnt(whandle.c_str(), ads_name.ads);
//	AcDbObjectId id = AcDbObjectId::kNull;
//	acdbGetObjectId(id, ads_name.ads);
//	return id;
//}



