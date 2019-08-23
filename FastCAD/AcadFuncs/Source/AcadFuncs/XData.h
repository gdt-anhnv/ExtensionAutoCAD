
#ifndef _XDATA_FUNCS_H_
#define _XDATA_FUNCS_H_

#include "../acad_header.h"
#include <list>

class XDataFuncs
{
public:
	static void AddXDataIntVal(const AcDbObjectId & id, const std::wstring & app_name, const int & value);
	static void AddXDataHandleVal(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring& value);
	static bool CheckXDataAppname(const AcDbObjectId & id, const std::wstring & app_name);
	static void AddXDataWstringVal(const AcDbObjectId & id, const std::wstring & app_name, const ACHAR * value);
	static bool CheckXDataIntVal(const AcDbObjectId &id, const std::wstring & app_name, const int & value);
	static bool CheckXDataDoubleVal(const AcDbObjectId &id, const std::wstring & app_name, const double & value);
	static bool CheckXDataStringVal(const AcDbObjectId &id, const std::wstring & app_name, const std::wstring & value);
	static void SetXData(AcDbDatabase* db, const AcDbObjectId& id, const resbuf* xdata);
	static void AddXDataDoubleVal(const AcDbObjectId & id, const std::wstring & app_name, const double & value);
	//make sure object already close when use this func
	static void DeleteXData(const AcDbObjectId & id, const std::wstring & app_name);
	static bool RemoveXData(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & xdata_tag);
	static std::wstring GetXData(const AcDbObjectId &id, const std::wstring & app_name, const std::wstring & tag);

	static std::list<std::wstring> GetXDataWstringVal(const AcDbObjectId &id, const std::wstring & app_name, 
		const std::wstring & tag);

	static std::list<int> GetXDataIntVal(const AcDbObjectId & id, const std::wstring & appname, const std::wstring &tag);

	static std::list<double> GetXDataDoubleVal(const AcDbObjectId & id, const std::wstring & appname, const std::wstring &tag);

	static AcDbObjectIdArray FindObjectsByXData(const std::wstring & app_name, const std::wstring & val);

	static bool ChangeXDataWstringVal(const AcDbObjectId & id,
		const std::wstring & app_name, const std::wstring & xdata_tag,const std::wstring & val);
	static bool ChangeXDataHandleVal(const AcDbObjectId & id,
		const std::wstring & app_name, const std::wstring & xdata_tag, const std::wstring & val);
	static bool ChangeXdataDoubleVal(AcDbObjectId id, const std::wstring & app_name,
		const std::wstring & xdata_tag, const double & value);
	static bool ChangeXdataIntVal(AcDbObjectId id, const std::wstring & app_name,
		const std::wstring & xdata_tag, const int & value);
	static void AddXdataHandle(const AcDbObjectId & des_id, const std::wstring & app_name, AcDbObjectId source_id);
	static AcDbObjectId GetObjId(struct resbuf* rb);

	static AcDbObjectIdArray GetObjectsByXdataHandle(const AcDbObjectId &id,
		const std::wstring & app_name, const std::wstring & tag);

	static bool AddXDataAppName(const AcDbObjectId & id, const std::wstring & app_name);
};

#endif