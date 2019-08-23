#ifndef _ATTRIBUTE_FUNCS_H_
#define _ATTRIBUTE_FUNCS_H_
#include "../acad_header.h"


class AttributeFuncs
{
public:
	static AcDbObjectIdArray GetAttribute(const AcDbObjectId & br_id);

	static std::wstring GetAttributeTag(const AcDbObjectId & br_id, const AcDbObjectId & att_id);
	static const std::wstring GetAttValueWithTag(
		AcDbDatabase*db, const wchar_t* entry_name, const wchar_t* tag);
	static const std::wstring GetAttValueWithTag(
		AcDbObjectId blk_id, const wchar_t* tag);
	static const std::wstring GetAttValueWithTag(
		const AcDbBlockReference* br, const wchar_t* tag);
	static AcDbObjectId GetAttributeByName(AcDbObjectId br_id, const wchar_t* att_name);
	static void CloneAttributes(const AcDbObjectId & des_id,const AcDbObjectId & source_id);
	static void AssignAttributeVal(
		const AcDbObjectId& br_id, const wchar_t* tag, const wchar_t* val);
	static void RotateAttribute(
		const AcDbObjectId& br_id, const wchar_t* att_name, double rot_val);
	static AcGePoint3d GetAttributePosition(
		const AcDbObjectId& br_id, const wchar_t* att_name);
	static void SetAttributeAlignmentPoint(
		AcDbObjectId br_id, const wchar_t * tag, AcGePoint3d pos);
	static void SetAttibuteHeight(AcDbObjectId br_id, const wchar_t * tag, double height);
	static void SetAttributeColor(AcDbObjectId br_id, const wchar_t * tag, int color_index);
	static std::wstring GetPropValue(const AcDbObjectId& id, const wchar_t* tag);
	static ErrorStatus SetPropValue(const AcDbObjectId& id, const wchar_t* tag, const wchar_t* val);
	static double GetPropRotation(const AcDbObjectId& id, const wchar_t* tag);
	static void SetPropRotation(const AcDbObjectId& id, const wchar_t* tag, double rota);
	static void InvisibleAttribute(const AcDbObjectId& br_id, const wchar_t* tag, bool invi);
	static void SetLockPositionAllAttribute(const AcDbObjectId & br_id, bool lock);
	static AcDbObjectId GetAttID(const AcDbObjectId& id, const wchar_t* tag);
	static void SetAttributePosition(const AcDbObjectId & id, const wchar_t* tag, const AcGePoint3d & pnt);
};

#endif // _ATTRIBUTE_FUNCS_H_
