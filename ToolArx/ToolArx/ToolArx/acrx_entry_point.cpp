#include "../Source/stdafx.h"
#include "../Source/Autocad/acad_manager.h"
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
		acutPrintf(_T("\n ToolArx loaded! \n"));
		ToolArxCmd::InitCommands();
		break;
	case AcRx::kUnloadAppMsg:
		acutPrintf(_T("\n ToolArx unloaded! \n"));
		ToolArxCmd::DestroyCommands();
		break;
	case AcRx::kLoadDwgMsg:
		break;
	}

	return AcRx::kRetOK;
}
