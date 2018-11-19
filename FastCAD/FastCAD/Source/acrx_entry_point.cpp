#include "../stdafx.h"
#include "contour_line.h"
#include "fast_dim.h"

#define GROUPNAME					L"PTFasterCommand"
#define CONTOURLINE					L"ContourLine"
#define FASTDIM						L"fd"
#define MERGE_DIM					L"md"

std::wstring groupname = GROUPNAME;
std::wstring command_name[] = {
	//CONTOURLINE,
	FASTDIM,
	MERGE_DIM
};

static AcRxFunctionPtr GetFuncs(std::wstring cmd)
{
	if (0 == cmd.compare(CONTOURLINE))
		return ContourLine::DoContourLine;
	if (0 == cmd.compare(FASTDIM))
		return FastDim::DoFastDim;
	if (0 == cmd.compare(MERGE_DIM))
		return FastDim::DoMergeDim;
}

static void InitCommands()
{
	int num_cmd = sizeof(command_name) / sizeof(std::wstring);
	for (int i = 0; i < num_cmd; i++)
	{
		acedRegCmds->addCommand(groupname.c_str(),
			command_name[i].c_str(),
			command_name[i].c_str(),
			ACRX_CMD_MODAL,
			GetFuncs(command_name[i]));
	}
}

static void DestroyCommands()
{
	acedRegCmds->removeGroup(groupname.c_str());
}

extern "C" AcRx::AppRetCode acrxEntryPoint(AcRx::AppMsgCode msg, void* appId)
{
	switch (msg)
	{
	case AcRx::kInitAppMsg:
		acrxUnlockApplication(appId);
		acrxRegisterAppMDIAware(appId);
		acutPrintf(_T("\n PTFaster (by Hicas Solution) loaded! \n"));
		InitCommands();
		break;
	case AcRx::kUnloadAppMsg:
		acutPrintf(_T("\n PTFaster (by Hicas Solution) unloaded! \n"));
		DestroyCommands();
		break;
	case AcRx::kLoadDwgMsg:
		break;
	}

	return AcRx::kRetOK;
}