#ifndef _STD_AFX_H_
#define _STD_AFX_H_

//
//////////////////////////////////////////////////////////////////////////////
//
//  Copyright 2016 Autodesk, Inc.  All rights reserved.
//
//  Use of this software is subject to the terms of the Autodesk license 
//  agreement provided at the time of installation or download, or which 
//  otherwise accompanies this software in either electronic or hard copy form.   
//
//////////////////////////////////////////////////////////////////////////////
//
// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#if defined(_DEBUG) && !defined(AC_FULL_DEBUG)
#pragma message("Building debug version of this module to be used with non-debug/Prod AutoCAD")
#define DEBUG_THIS_ONLY
#undef _DEBUG
#endif

#pragma warning(disable: 4275)

#define VC_EXTRALEAN        // Exclude rarely-used stuff from Windows headers

#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions

#ifndef _AFX_NO_OLE_SUPPORT
#include <afxole.h>         // MFC OLE classes
#include <afxodlgs.h>       // MFC OLE dialog classes
#include <afxdisp.h>        // MFC OLE automation classes
#endif // _AFX_NO_OLE_SUPPORT


#ifndef _AFX_NO_DB_SUPPORT
#include <afxdb.h>            // MFC ODBC database classes
#endif // _AFX_NO_DB_SUPPORT

#ifndef _AFX_NO_DAO_SUPPORT
#pragma warning(push)
#pragma warning(disable:4265)//disable missing virtual destructor warning
#include <afxdao.h>         // MFC DAO database classes
#pragma warning(pop) // C4265
#endif // _AFX_NO_DAO_SUPPORT

#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>            // MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

#include <math.h>
//
//////////////////////////////////////////////////////////////////////////////
//
//  Copyright 2016 Autodesk, Inc.  All rights reserved.
//
//  Use of this software is subject to the terms of the Autodesk license 
//  agreement provided at the time of installation or download, or which 
//  otherwise accompanies this software in either electronic or hard copy form.   
//
//////////////////////////////////////////////////////////////////////////////
//
// include all of the ARX header files in our stdafx.h


#include "accmd.h"
#include "acdb.h"
#include "acdbabb.h"
#include "aced.h"
#include "acedinpt.h"
#include "acestext.h"
#include "acgi.h"
#include "acgiutil.h"
#include "AcGsManager.h"
#include "acgs.h"
#include "gs.h"
#include "AcDbLMgr.h"
#include "actrans.h"
#include "adesk.h"
#include "adeskabb.h"
#include "ads.h"
#include "adscodes.h"
#include "adsdef.h"
#include "adsdlg.h"
#include "adslib.h"
#include "adsrxdef.h"
#include "appinfo.h"
#include "dbacis.h"
#include "dbapserv.h"
#include "dbaudita.h"
#include "dbbody.h"
#include "dbboiler.h"
#include "dbcfilrs.h"
#include "dbcurve.h"
#include "dbdate.h"
#include "dbdict.h"
#include "dbdictdflt.h"
#include "dbelipse.h"
#include "dbents.h"
#include "dbfcf.h"
#include "dbfiler.h"
#include "dbfilter.h"
#include "dbframe.h"
#include "dbgroup.h"
#include "dbhandle.h"
#include "dbhatch.h"
#include "dbid.h"
#include "dbidar.h"
#include "dbidmap.h"
#include "dbid_ops.h"
#include "dbimage.h"
#include "dbindex.h"
#include "dbintar.h"
#include "dbjig.h"
#include "dblayout.h"
#include "dblead.h"
#include "dbltrans.h"
#include "dblyfilt.h"
#include "dblyindx.h"
#include "dbmain.h"
#include "dbmline.h"
#include "dbmstyle.h"
#include "dbmtext.h"
#include "dbole.h"
#include "dbpl.h"
#include "dbplhldr.h"
#include "dbplotsettings.h"
#include "dbproxy.h"
#include "dbptrar.h"
#include "dbray.h"
#include "dbregion.h"
#include "dbsol3d.h"
#include "dbspline.h"
#include "dbspfilt.h"
#include "dbspindx.h"
#include "dbsubeid.h"
#include "dbsymtb.h"
#include "dbxline.h"
#include "dbxrecrd.h"
#include "dbxutil.h"
#include "geapln3d.h"
#include "gearc2d.h"
#include "gearc3d.h"
#include "geassign.h"
#include "geblok2d.h"
#include "geblok3d.h"
#include "gebndpln.h"
#include "gecbndry.h"
#include "gecint2d.h"
#include "gecint3d.h"
#include "gecomp2d.h"
#include "gecomp3d.h"
#include "gecone.h"
#include "gecspl2d.h"
#include "gecspl3d.h"
#include "gecurv2d.h"
#include "gecurv3d.h"
#include "gecylndr.h"
#include "gedblar.h"
#include "gedll.h"
#include "gedwgio.h"
#include "gedxfio.h"
#include "geell2d.h"
#include "geell3d.h"
#include "geent2d.h"
#include "geent3d.h"
#include "geextc2d.h"
#include "geextc3d.h"
#include "geextsf.h"
#include "gefileio.h"
#include "gefiler.h"
#include "gegbl.h"
#include "gegblabb.h"
#include "geintarr.h"
#include "geintrvl.h"
#include "gekvec.h"
#include "gelent2d.h"
#include "gelent3d.h"
#include "gelibver.h"
#include "geline2d.h"
#include "geline3d.h"
#include "gelnsg2d.h"
#include "gelnsg3d.h"
#include "gemat2d.h"
#include "gemat3d.h"
#include "genurb2d.h"
#include "genurb3d.h"
#include "genurbsf.h"
#include "geoffc2d.h"
#include "geoffc3d.h"
#include "geoffsf.h"
#include "gepent2d.h"
#include "gepent3d.h"
#include "geplanar.h"
#include "geplane.h"
#include "geplin2d.h"
#include "geplin3d.h"
#include "gepnt2d.h"
#include "gepnt3d.h"
#include "geponc2d.h"
#include "geponc3d.h"
#include "geponsrf.h"
#include "gepos2d.h"
#include "gepos3d.h"
#include "gept2dar.h"
#include "gept3dar.h"
#include "geray2d.h"
#include "geray3d.h"
#include "gescl2d.h"
#include "gescl3d.h"
#include "gesent2d.h"
#include "gesent3d.h"
#include "gesphere.h"
#include "gesurf.h"
#include "getol.h"
#include "getorus.h"
#include "gevc2dar.h"
#include "gevc3dar.h"
#include "gevec2d.h"
#include "gevec3d.h"
#include "gevptar.h"
#include "gexbndsf.h"
#include "graph.h"
#include "imgdef.h"
#include "imgent.h"
#include "imgvars.h"
#include "lngtrans.h"
//#include "migrtion.h"
#include "oleaprot.h"
#include "ol_errno.h"
#include "rxboiler.h"
#include "rxclass.h"
#include "rxdefs.h"
#include "rxdict.h"
#include "rxditer.h"
#include "rxdlinkr.h"
#include "rxiter.h"
#include "rxkernel.h"
#include "rxmfcapi.h"
#include "rxnames.h"
#include "rxobject.h"
#include "rxregsvc.h"
#include "rxsrvice.h"
#include "xgraph.h"
#include "adui.h"
#include "acui.h"
#include "acuiDialog.h"

// Turn on the _DEBUG symbol if it was defined, before including
// non-MFC header files.
//
#ifdef DEBUG_THIS_ONLY
#define _DEBUG
#undef DEBUG_THIS_ONLY
#endif

#endif