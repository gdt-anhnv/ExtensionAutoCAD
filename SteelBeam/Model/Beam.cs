using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;

namespace SteelBeam
{
    class Beam
    {
        public const int cover = 25;
        private int width_beam;
        public int Width
        {
            get { return width_beam; }
            set { width_beam = value; }
        }
        private int height_beam;
        public int Height
        {
            get { return height_beam; }
            set { height_beam = value; }
        }
        private int length_beam;
        public int Length
        {
            get { return length_beam; }
            set { length_beam = value; }
        }

        public Beam()
        {
            width_beam = 0;
            height_beam = 0;
            length_beam = 0;
        }

        private List<AcadDB.Entity> DrawBoundBeam(AcadGeo.Point3d ins_pnt)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            AcadGeo.Point3d up_ins_pnt = ins_pnt + AcadGeo.Vector3d.YAxis * 0.5 * height_beam;
            ents.Add(new AcadDB.Line(up_ins_pnt + AcadGeo.Vector3d.XAxis * 0.5 * width_beam,
                up_ins_pnt - AcadGeo.Vector3d.XAxis * 0.5 * width_beam));

            AcadGeo.Point3d down_ins_pnt = ins_pnt - AcadGeo.Vector3d.YAxis * 0.5 * height_beam;
            ents.Add(new AcadDB.Line(down_ins_pnt + AcadGeo.Vector3d.XAxis * 0.5 * width_beam,
                down_ins_pnt - AcadGeo.Vector3d.XAxis * 0.5 * width_beam));

            AcadGeo.Point3d right_ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 0.5 * width_beam;
            ents.Add(new AcadDB.Line(right_ins_pnt + AcadGeo.Vector3d.YAxis * 0.5 * height_beam,
                right_ins_pnt - AcadGeo.Vector3d.YAxis * 0.5 * height_beam));

            AcadGeo.Point3d left_ins_pnt = ins_pnt - AcadGeo.Vector3d.XAxis * 0.5 * width_beam;
            ents.Add(new AcadDB.Line(left_ins_pnt + AcadGeo.Vector3d.YAxis * 0.5 * height_beam,
                left_ins_pnt - AcadGeo.Vector3d.YAxis * 0.5 * height_beam));

            return ents;
        }

        public List<AcadDB.Entity> DrawDauGoi(AcadGeo.Point3d ins_pnt)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            ents.AddRange(DrawBoundBeam(ins_pnt));

            return ents;
        }

        public List<AcadDB.Entity> DrawMiddleBeam(AcadGeo.Point3d ins_pnt)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            ents.AddRange(DrawBoundBeam(ins_pnt));

            return ents;
        }
    }
}
