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

                tran.Commit();

                if (!blk_tbl.Has(name))
                    return ObjectId.Null;
                else
                    return blk_tbl[name];
            }
        }

        public static List<Entity> CloneAttribute(ObjectId blk_ref_id, ObjectId btr_id)
        {
            List<Entity> ents = new List<Entity>();
            //return ents;
            using (Transaction tran = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
            {
                BlockReference blk_ref = tran.GetObject(blk_ref_id, OpenMode.ForWrite) as BlockReference;
                BlockTableRecord btr = tran.GetObject(btr_id, OpenMode.ForRead) as BlockTableRecord;
                foreach (var obj_id in btr)
                {
                    AttributeDefinition att_def = obj_id.GetObject(OpenMode.ForRead) as AttributeDefinition;
                    if (null != att_def && !att_def.Constant)
                    {
                        AttributeReference att_ref = new AttributeReference();
                        att_ref.SetAttributeFromBlock(att_def, blk_ref.BlockTransform);
                        att_ref.Position = att_def.Position.TransformBy(blk_ref.BlockTransform);
                        att_ref.AlignmentPoint = att_def.AlignmentPoint.TransformBy(blk_ref.BlockTransform);

                        blk_ref.AttributeCollection.AppendAttribute(att_ref);
                        tran.AddNewlyCreatedDBObject(att_ref, true);
                    }
                }

                blk_ref.RecordGraphicsModified(true);

                tran.Commit();
            }

            return ents;
        }
    }
}
