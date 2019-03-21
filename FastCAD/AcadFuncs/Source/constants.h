#ifndef _ACAD_FUNS_CONSTANTS_H_
#define _ACAD_FUNS_CONSTANTS_H_

wchar_t* const ACTIVE_VIEWPORT = L"*Active";
#define WrapBlkTblRcd				ObjectWrap<AcDbBlockTableRecord>
#define OpenObjectId				DBObject::OpenObjectById
#define OpenBlkRef					DBObject::OpenObjectById<AcDbBlockReference>
#define OpenBlkTblRcd				DBObject::OpenObjectById<AcDbBlockTableRecord>

#endif