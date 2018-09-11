#include "acad_manager.h"
#include "ToolArxFuncs.h"

#define TATESTING			L"TESTING"
#define SEARCH_ENTITY		L"SEARCH_ENTITY_BY_ID"
#define GET_TEXT_NEAR		L"GET_TEXT_NEAR"

std::wstring ToolArxCmd::groupname = L"EnbarGroup";
std::wstring ToolArxCmd::command_name[] = {
	TATESTING,
	SEARCH_ENTITY,
	GET_TEXT_NEAR
};



void ToolArxCmd::InitCommands()
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

void ToolArxCmd::DestroyCommands()
{
	acedRegCmds->removeGroup(groupname.c_str());
}



static void Testing()
{
	
}

AcRxFunctionPtr ToolArxCmd::GetFuncs(std::wstring cmd)
{
	if (cmd.compare(TATESTING) == 0)
		return Testing;
	if (cmd.compare(SEARCH_ENTITY) == 0)
		return AcRxFunctionPtr();
	if (cmd.compare(GET_TEXT_NEAR) == 0)
		return ToolArxFuncs::GetTextNear;
	else
		return nullptr;
}
