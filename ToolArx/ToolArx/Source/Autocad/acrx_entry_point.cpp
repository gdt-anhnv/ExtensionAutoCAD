#include "stdafx.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#endif

extern "C" AcRx::AppRetCode acrxEntryPoint(AcRx::AppMsgCode msg, void* appId)
{
	switch (msg)
	{
	case AcRx::kInitAppMsg:
		acrxUnlockApplication(appId);
		acrxRegisterAppMDIAware(appId);
		acutPrintf(_T("\n Enbar (by Hicas Solution) loaded! \n"));
		break;
	case AcRx::kUnloadAppMsg:
		acutPrintf(_T("\n Enbar (by Hicas Solution) unloaded! \n"));
		break;
	case AcRx::kLoadDwgMsg:
		break;
	}

	return AcRx::kRetOK;
}
