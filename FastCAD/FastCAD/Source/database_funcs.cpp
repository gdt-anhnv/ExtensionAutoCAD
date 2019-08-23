#include "../stdafx.h"

#include "database_funcs.h"
#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

void DatabaseFuncs::SaveDXF()
{
	try
	{
		AcTransaction *trans = acdbTransactionManager->startTransaction();

		acedCommandS(RTSTR, L"_.ZOOM", RTSTR, L"_.A", RTSTR, L"", RTNONE);

		AcDbDatabase* db = acdbCurDwg();
		LayerFuncs::DeleteLayer(acdbCurDwg(), L"名称");
		AcDbObjectIdArray ids;
		db->purge(ids);

		AcDbObjectId bound_id = ARXFuncs::GetObjectByPicking(L"Chọn biên bản vẽ");
		if (bound_id.isNull())
			throw int(1);

		acedCommandS(RTSTR, L"_.SAVEAS", RTSTR, L"", RTNONE);

	}
	catch (...)
	{

	}
	acdbTransactionManager->abortTransaction();
}
