#include "../stdafx.h"
//#include "acad_funcs_header.h"

class ToolArxCmd
{
public:
	static void InitCommands();
	static void DestroyCommands();
private:
	static AcRxFunctionPtr GetFuncs(std::wstring cmd);
private:
	static std::wstring groupname;
	static std::wstring command_name[];
};