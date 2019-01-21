using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;

namespace SteelBeam
{
    public class SteelShapeB
    {
        public const int offset = 20;
        public const int rad_blk_steel = 10;
        private const string BLK_NAME = "KCS_SNODE";
        private int number_steel;
        public int NumberSteel
        {
            get { return number_steel; }
            set { number_steel = value; }
        }
        private int diameter;
        public int Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }
        private AcadGeo.Point3d start_point;
        public AcadGeo.Point3d StartPoint
        {
            get { return start_point; }
            set { start_point = value; }
        }
        private AcadGeo.Point3d end_point;
        public AcadGeo.Point3d EndPoint
        {
            get { return end_point; }
            set { end_point = value; }
        }

        public SteelShapeB()
        {
            number_steel = 0;
            diameter = 0;
            start_point = AcadGeo.Point3d.Origin;
            end_point = AcadGeo.Point3d.Origin;
        }

        public List<AcadDB.Entity> Draw()
        {
            if (number_steel < 2)
                return new List<AcadDB.Entity>();

            double len = start_point.DistanceTo(end_point);
            double spacing = len / (number_steel - 1);

            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();
            AcadDB.ObjectId blk_id = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_NAME);
            if (AcadDB.ObjectId.Null == blk_id)
            {
                MessageBox.Show("Khong co block thep");
                throw new Autodesk.AutoCAD.Runtime.Exception(Autodesk.AutoCAD.Runtime.ErrorStatus.OK, "Failed");
            }
            AcadGeo.Point3d ins_pnt = start_point;
            for(int i = 0; i < number_steel; i++)
            {
                ents.Add(new AcadDB.BlockReference(ins_pnt, blk_id));
                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * spacing;
            }

            return ents;
        }
    }
}
