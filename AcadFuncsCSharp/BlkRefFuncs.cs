using Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.EditorInput;

namespace AcadFuncsCSharp
{
    public class BlkRefFuncs
    {
        public static ObjectId GetBlock(string name)
        {
            using (Transaction tran = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
            {
                BlockTable blk_tbl = tran.GetObject(AcadFuncs.GetActiveDb().BlockTableId, OpenMode.ForRead) as BlockTable;
                ObjectId blkRecId = ObjectId.Null;

                if (!blk_tbl.Has(name))
                    return ObjectId.Null;
                else
                    return blk_tbl[name];
            }
        }
    }
}
