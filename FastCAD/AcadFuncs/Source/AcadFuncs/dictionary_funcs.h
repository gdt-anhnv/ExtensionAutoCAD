#ifndef _DICTIONARY_FUNCS_H_
#define _DICTIONARY_FUNCS_H_

#include "../acad_header.h"
#include <iostream>
#include <string>

class DictionaryFuncs
{
public:
	static AcDbObjectId IsExistedDict(const std::wstring& name, AcDbDatabase* db = acdbCurDwg());
	static AcDbObjectId IsExistedDict(AcDbObjectId dict_id, const std::wstring& name);
	static AcDbObjectId CreateXRecord(const std::wstring& name, AcDbDatabase* db = acdbCurDwg());
	static AcDbObjectId CreateXRecord(AcDbObjectId dict_id, const std::wstring& name);
	static AcDbObjectId AppendToDict(AcDbObject* obj, const std::wstring& name);
	static void DeleteXRecord(const std::wstring& name);
	static void AppendResbufToXrecord(AcDbObjectId xrecord_id, resbuf* rb);
	static struct resbuf* GetLastRb(struct resbuf* rb);
	static struct resbuf* GetRbBeforeRb(struct resbuf* rb, struct resbuf* destination_rb);
};

#endif