#ifndef _USER_FUNCS_H_
#define _USER_FUNC_H_

#include "../acad_header.h"
#include <list>

class UserFuncs
{
public:

	static AcDbObjectIdArray GetIdsFromSelectionSet(const ads_name&);
	static AcDbObjectIdArray UserGetEnts();
	static AcDbObjectIdArray GetEntsFromSS(const ads_name& ss);
	static int GetInt(std::wstring promp);
	static int GetIntWithFilter(const std::wstring & promp, int filter_bit);
	static AcGePoint3d UserGetPoint(std::wstring prompt);
	static AcGePoint3d UserGetPointByPoint(std::wstring prompt, const AcGePoint3d & base_point);

	static int GetOption(const std::wstring &title, const std::list<std::wstring> & options);
};


#endif // !_USER_FUNCS_H_