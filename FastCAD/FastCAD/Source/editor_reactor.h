#ifndef _EDITOR_REACTOR_H_
#define _EDITOR_REACTOR_H_

#include "../stdafx.h"
#include "dbmain.h"

class FCEditorReactor : public AcEditorReactor
{
public:
	ACRX_DECLARE_MEMBERS(FCEditorReactor);
	FCEditorReactor();
	~FCEditorReactor();

	virtual void commandWillStart(const ACHAR* cmdStr);
	virtual void commandEnded(const ACHAR* cmdStr);

};

#endif