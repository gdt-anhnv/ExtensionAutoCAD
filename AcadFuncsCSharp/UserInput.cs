using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AcadRuntime = Autodesk.AutoCAD.Runtime;

namespace AcadFuncsCSharp
{
    public class UserInput
    {
        public static AcadGeo.Point3d PickPoint(string mess)
        {
            AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
            using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
                {
                    PromptPointOptions prmpt_pnt = new PromptPointOptions(mess);
                    PromptPointResult prmpt_ret = AcadFuncs.GetEditor().GetPoint(prmpt_pnt);
                    if (PromptStatus.Cancel == prmpt_ret.Status)
                    {
                        tr.Abort();
                        tr.Dispose();
                        throw new AcadRuntime.Exception(AcadRuntime.ErrorStatus.OK, "failed");
                    }

                    tr.Commit();
                    return prmpt_ret.Value;
                }
            }
        }
    }
}
