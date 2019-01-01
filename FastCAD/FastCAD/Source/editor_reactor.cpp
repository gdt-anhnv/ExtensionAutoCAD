#include "../stdafx.h"
#include "editor_reactor.h"

ACRX_NO_CONS_DEFINE_MEMBERS(FCEditorReactor, AcEditorReactor);

FCEditorReactor::FCEditorReactor()
{
	acedEditor->addReactor(this);
}

FCEditorReactor::~FCEditorReactor()
{
	acedEditor->removeReactor(this);
}

void FCEditorReactor::commandWillStart(const ACHAR * cmdStr)
{
}

void FCEditorReactor::commandEnded(const ACHAR * cmdStr)
{
	MessageBox(nullptr, cmdStr, L"AbC", MB_OK);
	if (0 == std::wcscmp(cmdStr, L"InstanceBalloon"))
		acDocManager->sendStringToExecute(curDoc(), L"_.Test ");
}
