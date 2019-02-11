using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
namespace SteelBeam
{
    public class InfoSteel
    {
        private const double cir_radius = 62.5;
        private const double text_height = 62.5;
        private const double text_w_factor = 0.75;
        private int number;
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        private string content;
        public string Content
        {
            set { content = value; }
        }
        private AcadGeo.Point3d position;
        public AcadGeo.Point3d Position
        {
            get { return position; }
            set { position = value; }
        }
        private List<AcadGeo.Point3d> steel_pos;
        public List<AcadGeo.Point3d> SteelPos
        {
            set { steel_pos = value; }
        }
        private double angle;
        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public InfoSteel()
        {
            number = 0;
            content = "";
            position = AcadGeo.Point3d.Origin;
            steel_pos = new List<AcadGeo.Point3d>();
            angle = 0.0;
        }

        public List<AcadDB.Entity> Draw()
        {
            AcadGeo.Line3d xline = new AcadGeo.Line3d(position, AcadGeo.Vector3d.XAxis);

            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            AcadGeo.Point3d pnt = new AcadGeo.Point3d(position.X, position.Y, position.Z);
            foreach(var sp in steel_pos)
            {
                AcadGeo.Line3d steel_line = new AcadGeo.Line3d(sp, new AcadGeo.Vector3d(Math.Sin(angle), Math.Cos(angle), 0.0));
                AcadGeo.Point3d[] intersected_pnts = xline.IntersectWith(steel_line);

                if (intersected_pnts.Length > 0)
                {
                    ents.Add(new AcadDB.Line(intersected_pnts[0], sp));
                    if (intersected_pnts[0].DistanceTo(position) > pnt.DistanceTo(position))
                        pnt = intersected_pnts[0];
                }
            }

            ents.Add(new AcadDB.Line(position, pnt));

            ents.Add(new AcadDB.Circle(position - AcadGeo.Vector3d.XAxis * cir_radius, AcadGeo.Vector3d.ZAxis, cir_radius));

            //draw value
            {
                AcadDB.DBText text = new AcadDB.DBText();
                text.TextString = number.ToString();
                text.Justify = AcadDB.AttachmentPoint.MiddleCenter;
                text.Height = text_height;
                text.WidthFactor = text_w_factor;
                text.AlignmentPoint = position - AcadGeo.Vector3d.XAxis * cir_radius;
                ents.Add(text);
            }

            //draw content
            {
                AcadDB.DBText text = new AcadDB.DBText();
                text.TextString = content;
                text.Justify = AcadDB.AttachmentPoint.BottomLeft;
                text.Height = text_height;
                text.WidthFactor = text_w_factor;
                text.AlignmentPoint = position + AcadGeo.Vector3d.XAxis * 10.0 + AcadGeo.Vector3d.YAxis * 10.0;
                ents.Add(text);
            }

            return ents;
        }
    }
}
