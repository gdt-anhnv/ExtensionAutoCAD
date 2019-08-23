#ifndef _BLOCK_REFERNCE_FUNCS_H_
#define _BLOCK_REFERNCE_FUNCS_H_

#include "../acad_header.h"

class BlockReferenceFuncs
{
public:
	static std::wstring GetBlkRefName(const AcDbObjectId &id);
	static AcDbObjectIdArray ExplodeBlkRef(AcDbDatabase* db, AcDbObjectId id, bool del_obj);
	static void RemoveBlkRef(AcDbDatabase* db, const wchar_t* br_name);
	static void ScaleBlockReference(const AcDbObjectId & id, const double & val);
	static AcGePoint3d GetBlkRefPos(const AcDbObjectId & br_id);
	static void SetBlkRefPos(const AcDbObjectId & br_id, const AcGePoint3d & pnt);
	static AcDbObjectId AppendBlockReference(const TCHAR * blk_name,
		const AcGePoint3d & ins_pnt, double scale_num, double rota = 0.0);
	static void SetDoubleDynBlk(const AcDbObjectId & br_id, const wchar_t * name, double val);

	static void SetBlkRefPosByAttPos(const AcDbObjectId & br_id, const wchar_t * att_tag, const AcGePoint3d & ins_pnt);

	
};

#endif // _ATTRIBUTE_FUNCS_H_
