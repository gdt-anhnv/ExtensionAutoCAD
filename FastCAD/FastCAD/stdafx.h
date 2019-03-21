#ifndef _STDAFX_H_
#define _STDAFX_H_

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
// ObjectARX Includes
#include "rxregsvc.h"
#include "acutads.h"
#include "dbmain.h"
//#include "dbxHeaders.h"
//#include "AcExtensionModule.h"
#include "aced.h"
#include "dbents.h"
#include "dbidar.h"
#include "acdocman.h"
#include "acedads.h" // used acedGetString
//#include "core_rxmfcapi.h"
#include "gepent3d.h"
#include "actrans.h"
#include "acedCmdNF.h"

#include <iostream>
#include <tchar.h>

static double KPI = 3.14159265359;

#endif