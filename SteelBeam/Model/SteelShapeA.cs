using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;

namespace SteelBeam
{
    class SteelShapeA
    {
        private int corner_rad = 30;
        private double PI = 3.1415;
        private int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        private int height;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        private int diameter;
        public int Diamter
        {
            get { return diameter; }
            set { diameter = value; }
        }
        private int spacing;
        public int Spacing
        {
            get { return spacing; }
            set { spacing = value; }
        }

        public SteelShapeA()
        {
            width = 0;
            height = 0;
            diameter = 0;
            spacing = 0;
        }

        public List<AcadDB.Entity> DrawShapeA(AcadGeo.Point3d ins_pnt)
        {
            if (width < 2 * corner_rad || height < 2 * corner_rad)
                throw new Autodesk.AutoCAD.Runtime.Exception(Autodesk.AutoCAD.Runtime.ErrorStatus.OK, "Failed");

            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            int inner_w = width - 2 * corner_rad;
            int inner_h = height - 2 * corner_rad;
            AcadGeo.Point3d up_ins_pnt = ins_pnt + AcadGeo.Vector3d.YAxis * 0.5 * height;
            ents.Add(new AcadDB.Line(up_ins_pnt + AcadGeo.Vector3d.XAxis * 0.5 * inner_w,
                up_ins_pnt - AcadGeo.Vector3d.XAxis * 0.5 * inner_w));

            AcadGeo.Point3d down_ins_pnt = ins_pnt - AcadGeo.Vector3d.YAxis * 0.5 * height;
            ents.Add(new AcadDB.Line(down_ins_pnt + AcadGeo.Vector3d.XAxis * 0.5 * inner_w,
                down_ins_pnt - AcadGeo.Vector3d.XAxis * 0.5 * inner_w));

            AcadGeo.Point3d right_ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 0.5 * width;
            ents.Add(new AcadDB.Line(right_ins_pnt + AcadGeo.Vector3d.YAxis * 0.5 * inner_h,
                right_ins_pnt - AcadGeo.Vector3d.YAxis * 0.5 * inner_h));

            AcadGeo.Point3d left_ins_pnt = ins_pnt - AcadGeo.Vector3d.XAxis * 0.5 * width;
            ents.Add(new AcadDB.Line(left_ins_pnt + AcadGeo.Vector3d.YAxis * 0.5 * inner_h,
                left_ins_pnt - AcadGeo.Vector3d.YAxis * 0.5 * inner_h));

            AcadGeo.Point3d corner_pnt1 = ins_pnt +
                AcadGeo.Vector3d.XAxis * (width * 0.5 - corner_rad) +
                AcadGeo.Vector3d.YAxis * (height * 0.5 - corner_rad);
            ents.Add(new AcadDB.Arc(corner_pnt1, AcadGeo.Vector3d.ZAxis, corner_rad, 0.0, PI * 0.5));

            AcadGeo.Point3d corner_pnt2 = ins_pnt -
                AcadGeo.Vector3d.XAxis * (width * 0.5 - corner_rad) +
                AcadGeo.Vector3d.YAxis * (height * 0.5 - corner_rad);
            ents.Add(new AcadDB.Arc(corner_pnt2, AcadGeo.Vector3d.ZAxis, corner_rad, PI * 0.5, PI));

            AcadGeo.Point3d corner_pnt3 = ins_pnt -
                AcadGeo.Vector3d.XAxis * (width * 0.5 - corner_rad) -
                AcadGeo.Vector3d.YAxis * (height * 0.5 - corner_rad);
            ents.Add(new AcadDB.Arc(corner_pnt3, AcadGeo.Vector3d.ZAxis, corner_rad, PI, PI * 1.5));

            AcadGeo.Point3d corner_pnt4 = ins_pnt +
                AcadGeo.Vector3d.XAxis * (width * 0.5 - corner_rad) -
                AcadGeo.Vector3d.YAxis * (height * 0.5 - corner_rad);
            ents.Add(new AcadDB.Arc(corner_pnt4, AcadGeo.Vector3d.ZAxis, corner_rad, PI * 1.5, PI * 2.0));

            return ents;
        }
    }
}
