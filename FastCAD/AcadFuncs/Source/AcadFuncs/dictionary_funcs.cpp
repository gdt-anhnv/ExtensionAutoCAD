#include "dictionary_funcs.h"
#include "ARXFuncs.h"
#include "../wrap_header.h"
#include "DBObject.h"
#include "Geometry.h"
#include "AcadFuncs.h"
#include "acedCmdNF.h"
#include "UserFuncs.h"
#include "../constants.h"

AcDbObjectId DictionaryFuncs::IsExistedDict(const std::wstring& name, AcDbDatabase* db)
{
	AcDbObjectId dict_id = db->namedObjectsDictionaryId();
	ObjectWrap<AcDbDictionary> dict_ent(DBObject::OpenObjectById<AcDbDictionary>(dict_id));

	AcDbObjectId tmp_id = AcDbObjectId::kNull;
	if (Acad::eOk == dict_ent.object->getAt(name.c_str(), tmp_id))
		return tmp_id;

	return AcDbObjectId::kNull;
}

AcDbObjectId DictionaryFuncs::IsExistedDict(AcDbObjectId dict_id, const std::wstring & name)
{
	ObjectWrap<AcDbDictionary> dict_ent(DBObject::OpenObjectById<AcDbDictionary>(dict_id));

	AcDbObjectId tmp_id = AcDbObjectId::kNull;
	if (Acad::eOk == dict_ent.object->getAt(name.c_str(), tmp_id))
		return tmp_id;

	return AcDbObjectId::kNull;
}

AcDbObjectId DictionaryFuncs::CreateXRecord(const std::wstring& name, AcDbDatabase* db)
{
	AcDbObjectId dict_id = db->namedObjectsDictionaryId();
	ObjectWrap<AcDbDictionary> dict_ent(DBObject::OpenObjectById<AcDbDictionary>(dict_id));

	AcDbXrecord* xrecord = new AcDbXrecord();
	dict_ent.object->upgradeOpen();
	AcDbObjectId xrecord_id = AcDbObjectId::kNull;
	if (Acad::eOk == dict_ent.object->setAt(name.c_str(), xrecord, xrecord_id))
	{
		xrecord->close();
		return xrecord_id;
	}

	delete xrecord;
	return AcDbObjectId::kNull;
}

AcDbObjectId DictionaryFuncs::CreateXRecord(AcDbObjectId dict_id, const std::wstring & name)
{
	ObjectWrap<AcDbDictionary> dict_ent(DBObject::OpenObjectById<AcDbDictionary>(dict_id));

	AcDbXrecord* xrecord = new AcDbXrecord();
	dict_ent.object->upgradeOpen();
	AcDbObjectId xrecord_id = AcDbObjectId::kNull;
	if (Acad::eOk == dict_ent.object->setAt(name.c_str(), xrecord, xrecord_id))
	{
		xrecord->close();
		return xrecord_id;
	}

	delete xrecord;
	return AcDbObjectId::kNull;
}

AcDbObjectId DictionaryFuncs::AppendToDict(AcDbObject * obj, const std::wstring & name)
{
	AcDbObjectId dict_id = acdbCurDwg()->namedObjectsDictionaryId();
	ObjectWrap<AcDbDictionary> dict_ent(DBObject::OpenObjectById<AcDbDictionary>(dict_id));

	dict_ent.object->upgradeOpen();
	AcDbObjectId obj_id = AcDbObjectId::kNull;
	if (Acad::eOk == dict_ent.object->setAt(name.c_str(), obj, obj_id))
	{
		obj->close();
		return obj_id;
	}

	return AcDbObjectId::kNull;
}

void DictionaryFuncs::DeleteXRecord(const std::wstring & name)
{
	AcDbObjectId dict_id = acdbCurDwg()->namedObjectsDictionaryId();
	ObjectWrap<AcDbDictionary> dict_ent(DBObject::OpenObjectById<AcDbDictionary>(dict_id));

	AcDbObjectId xrecord_id = AcDbObjectId::kNull;
	dict_ent.object->getAt(name.c_str(), xrecord_id);

	ObjectWrap<AcDbXrecord> xrecord(DBObject::OpenObjectById<AcDbXrecord>(xrecord_id));
	xrecord.object->upgradeOpen();
	xrecord.object->erase();
}

void DictionaryFuncs::AppendResbufToXrecord(AcDbObjectId xrecord_id, resbuf * rb)
{
	ObjectWrap<AcDbXrecord> record(DBObject::OpenObjectById<AcDbXrecord>(xrecord_id));
	struct resbuf* curr_rb = NULL;
	record.object->rbChain(&curr_rb);

	if (NULL == curr_rb)
	{
		record.object->upgradeOpen();
		record.object->setFromRbChain(*rb);
		//acutRelRb(rb);
	}
	else
	{
		struct resbuf* tmp_rb = curr_rb;
		struct resbuf* last_rb = curr_rb;
		while (NULL != tmp_rb)
		{
			last_rb = tmp_rb;
			tmp_rb = tmp_rb->rbnext;
		}

		last_rb->rbnext = rb;

		record.object->upgradeOpen();
		record.object->setFromRbChain(*curr_rb);
		acutRelRb(curr_rb);
	}
}

struct resbuf* DictionaryFuncs::GetLastRb(resbuf * rb)
{
	struct resbuf* cur_rb = rb;
	while (NULL != cur_rb)
	{
		if (NULL != cur_rb->rbnext)
			cur_rb = cur_rb->rbnext;
		else
			return cur_rb;
	}

	return NULL;
}

resbuf * DictionaryFuncs::GetRbBeforeRb(resbuf * rb, struct resbuf* destination_rb)
{
	struct resbuf* tmp_rb = rb;
	if (tmp_rb == destination_rb)
		return NULL;
	while (NULL != tmp_rb && tmp_rb->rbnext != destination_rb)
		tmp_rb = tmp_rb->rbnext;

	return tmp_rb;
}
