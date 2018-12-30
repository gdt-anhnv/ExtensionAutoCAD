
/*****************************************************/
//only for objectarx 2018 and back then
// ObjectARX Includes
#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            // Exclude rarely-used stuff from Windows headers
#endif

#if defined(_DEBUG) && !defined(_FULLDEBUG_)
#define _DEBUG_WAS_DEFINED
#undef _DEBUG
#pragma message (" Compiling MFC header files in release mode.")
#endif

#include <SDKDDKVer.h>

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS      // some CString constructors will be explicit

#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions

#ifndef _AFX_NO_OLE_SUPPORT
#include <afxole.h>         // MFC OLE classes
#include <afxodlgs.h>       // MFC OLE dialog classes
#include <afxdisp.h>        // MFC Automation classes
#endif // _AFX_NO_OLE_SUPPORT

#ifndef _AFX_NO_DB_SUPPORT
#include <afxdb.h>                      // MFC ODBC database classes
#endif // _AFX_NO_DB_SUPPORT

#ifndef _AFX_NO_DAO_SUPPORT
#include <afxdao.h>                     // MFC DAO database classes
#endif // _AFX_NO_DAO_SUPPORT

#ifndef _AFX_NO_OLE_SUPPORT
#include <afxdtctl.h>           // MFC support for Internet Explorer 4 Common Controls
#endif
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>                     // MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

/***********************************************************************************/

#include "rxregsvc.h"
#include "acutads.h"
#include "dbmain.h"
//#include "dbxHeaders.h"
//#include "AcExtensionModule.h"
#include "aced.h"
#include "dbents.h"
#include "dbJoinEntityPE.h"
#include "dbdynblk.h"
#include "dbregion.h"
#include "geplin3d.h"
#include "geassign.h"
#include "dbapserv.h"
#include "dbidmap.h"
#include "adscodes.h"
#include "acedCmdNF.h"

#include <iostream>
#include <string>
#include <tchar.h>
#include <Windows.h>
