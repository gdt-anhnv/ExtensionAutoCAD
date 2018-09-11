#ifndef _TOOLARXFUNCS_H_
#define _TOOLARXFUNCS_H_

#include "../stdafx.h"
#include "acad_funcs_header.h"
#include "sstream"
class ToolArxFuncs
{
	public:
		static std::wstring GetString(std::wstring promt);
		static AcDbObjectId DrawCirleMarker(AcGePoint3d center_, double rad);
		static void CheckProxyEntity();
		static void SearchEntityById();
		//......
		static void GetTextNear();
};

#endif // !_TOOLARXFUNCS_H_

