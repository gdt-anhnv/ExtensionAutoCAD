#include "../stdafx.h"
#include "contour_line.h"
#include "fast_dim.h"
#include "instance_balloon.h"
#include "blk_ref_func.h"

#define GROUPNAME					L"PTFasterCommand"
#define CONTOURLINE					L"ContourLine"
#define FASTDIM						L"fd"
#define MERGE_DIM					L"md"

#define INSTANCE_BALLOON			L"InstanceBalloon"
#define COPY_NAME_BLK				L"GetBlkName"
#define COPY_BLK					L"CopyBlk"
#define PASTE_BLK					L"PasteBlk"
#define TEST						L"Test"

std::wstring groupname = GROUPNAME;
std::wstring command_name[] = {
	INSTANCE_BALLOON,
	COPY_NAME_BLK,
	COPY_BLK,
	PASTE_BLK,
	TEST
};

static AcRxFunctionPtr GetFuncs(std::wstring cmd)
{
	if (0 == cmd.compare(CONTOURLINE))
		return ContourLine::DoContourLine;
	if (0 == cmd.compare(FASTDIM))
		return FastDim::DoFastDim;
	if (0 == cmd.compare(MERGE_DIM))
		return FastDim::DoMergeDim;
	if (0 == cmd.compare(INSTANCE_BALLOON))
		return InstanceBalloon::DrawBalloon;
	if (0 == cmd.compare(COPY_NAME_BLK))
		return BlkRefFunc::CopyBlkName;
	if (0 == cmd.compare(COPY_BLK))
		return BlkRefFunc::CopyBlk;
	if (0 == cmd.compare(PASTE_BLK))
		return BlkRefFunc::PasteBlk;
	if (0 == cmd.compare(TEST))
		return InstanceBalloon::Test;
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