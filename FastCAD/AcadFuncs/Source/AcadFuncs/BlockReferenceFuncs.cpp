#include "BlockReferenceFuncs.h"
#include "../wrap_header.h"
#include "../AcadFuncs/DBObject.h"
#include "../AcadFuncs/AcadFuncs.h"
#include "../AcadFuncs/AttributeFuncs.h"
#include "../constants.h"

std::wstring BlockReferenceFuncs::GetBlkRefName(const AcDbObjectId & id)
{
	AcDbObjectId blk_tbl_rcd_id = AcDbObjectId::kNull;
	{
		ObjectWrap<AcDbBlockReference> bl_ref_wrap(OpenBlkRef(id));
		if (NULL == bl_ref_wrap.object)
			return L"";
		AcDbDynBlockReference * br(new AcDbDynBlockReference(bl_ref_wrap.object));
		if (NULL != br)
			blk_tbl_rcd_id = br->dynamicBlockTableRecord();
		else
		{
			blk_tbl_rcd_id = bl_ref_wrap.object->blockTableRecord();
		}

		delete br;
	}

	ObjectWrap<AcDbBlockTableRecord> bl_rc(OpenBlkTblRcd(blk_tbl_rcd_id));
	if (NULL == bl_rc.object)
		return L"";

	ACHAR* name = new ACHAR();
	bl_rc.object->getName(name);
	std::wstring result = std::wstring(name);
	delete name;
	return result;
}

AcDbObjectIdArray BlockReferenceFuncs::ExplodeBlkRef(AcDbDatabase * db, AcDbObjectId id, bool del_obj)
{

	AcDbObjectIdArray result = AcDbObjectIdArray();

	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL != obj_wrap.object)
	{
		if (obj_wrap.object->isKindOf(AcDbBlockReference::desc()))
		{
			AcDbBlockReference* br = AcDbBlockReference::cast(obj_wrap.object);
			AcDbVoidPtrArray ents;
			Acad::ErrorStatus err = br->explode(ents);

			WrapBlkTblRcd ms_wrap(DBObject::GetModelSpace(db));
			ms_wrap.object->upgradeOpen();
			AcDbObjectId ins_id = AcDbObjectId::kNull;
			for (int i = 0; i < ents.length(); i++)
			{
				AcDbEntity* sub_ent = AcDbEntity::cast((AcRxObject*)ents[i]);
				if (NULL != sub_ent)
					err = ms_wrap.object->appendAcDbEntity(
						ins_id, ObjectWrap<AcDbEntity>(sub_ent).object);

				if (AcDbObjectId::kNull != ins_id)
					result.append(ins_id);

			}
		}

		if (del_obj)
		{
			obj_wrap.object->upgradeOpen();
			obj_wrap.object->erase();
		}
	}
	return result;
}


void BlockReferenceFuncs::RemoveBlkRef(AcDbDatabase * db, const wchar_t * br_name)
{
	AcDbObjectId br_id = DBObject::FindBlockByName(db, br_name);
	if (AcDbObjectId::kNull == br_id)
		return;

	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(br_id));
	if (NULL == obj_wrap.object)
		return;

	obj_wrap.object->upgradeOpen();
	obj_wrap.object->erase();
}


void BlockReferenceFuncs::ScaleBlockReference(const AcDbObjectId & id, const double & val)
{
	ObjectWrap<AcDbBlockReference> br_wrap(OpenBlkRef(id));
	if (NULL == br_wrap.object)
		return;
	br_wrap.object->upgradeOpen();

	AcGeMatrix3d trans = br_wrap.object->blockTransform();
	trans.setToScaling(val, br_wrap.object->position());
	br_wrap.object->transformBy(trans);
}

AcGePoint3d BlockReferenceFuncs::GetBlkRefPos(const AcDbObjectId & br_id)
{
	ObjectWrap<AcDbBlockReference> br_wrap(OpenBlkRef(br_id));
	if (NULL == br_wrap.object)
		return AcGePoint3d::kOrigin;

	return br_wrap.object->position();
}

void BlockReferenceFuncs::SetBlkRefPos(const AcDbObjectId & br_id, const AcGePoint3d & pnt)
{
	ObjectWrap<AcDbBlockReference> object(DBObject::OpenObjectById<AcDbBlockReference>(br_id));
	if (object.object == NULL)
		return;
	object.object->upgradeOpen();
	AcGePoint3d pos = object.object->position();
	AcGeVector3d vec = pnt - pos;
	AcGeMatrix3d mat = AcGeMatrix3d();
	mat.setToTranslation(vec);
	object.object->transformBy(mat);
}

AcDbObjectId BlockReferenceFuncs::AppendBlockReference(const TCHAR * blk_name,
	const AcGePoint3d & ins_pnt, double scale_num, double rota)
{
	AcDbObjectId btr_id = DBObject::FindBlockByName(acdbCurDwg(), blk_name);
	if (btr_id.isNull())
		return AcDbObjectId::kNull;

	AcDbObjectId br_id = AcDbObjectId::kNull;
	{
		ObjectWrap<AcDbBlockReference> br_wrap(new AcDbBlockReference(ins_pnt, btr_id));
		br_wrap.object->setRotation(rota);
		WrapBlkTblRcd model_space(DBObject::GetModelSpace(acdbCurDwg()));
		model_space.object->upgradeOpen();

		if (Acad::eOk != model_space.object->appendAcDbEntity(br_id, br_wrap.object))
		{
			delete br_wrap.object;
			return AcDbObjectId::kNull;
		}
	}

	ScaleBlockReference(br_id, scale_num);

	AttributeFuncs::CloneAttributes(br_id, btr_id);
	return br_id;
}

void BlockReferenceFuncs::SetDoubleDynBlk(const AcDbObjectId & br_id, const wchar_t * name, double val)
{
	AcDbBlockReference* blk_ref = OpenBlkRef(br_id);
	ObjectWrap<AcDbBlockReference> blk_ref_wrap(blk_ref);
	if (NULL == blk_ref)
		return;


	AcDbDynBlockReference* dbr(new AcDbDynBlockReference(blk_ref));

	AcDbDynBlockReferencePropertyArray props = AcDbDynBlockReferencePropertyArray();
	if (!dbr->isDynamicBlock())
		return;

	dbr->getBlockProperties(props);
	for (int i = 0; i < props.length(); i++)
	{
		if (props.at(i).propertyName() == AcString(name))
		{
			AcDbDynBlockReferenceProperty prop = props.at(i);
			AcDbEvalVariant prop_val = AcDbEvalVariant(val);
			Acad::ErrorStatus err = prop.setValue(prop_val);
			return;
		}
	}
}

void BlockReferenceFuncs::SetBlkRefPosByAttPos(const AcDbObjectId & br_id, const wchar_t * att_tag, const AcGePoint3d & ins_pnt)
{
	BlockReferenceFuncs::SetBlkRefPos(br_id, ins_pnt);
	AcGePoint3d att_pnt = AttributeFuncs::GetAttributePosition(br_id, att_tag);

	double distance = ins_pnt.distanceTo(att_pnt);

	AcGeVector3d vec = att_pnt - ins_pnt;
	vec = vec.normal();

	BlockReferenceFuncs::SetBlkRefPos(br_id, ins_pnt - vec * distance);
}
