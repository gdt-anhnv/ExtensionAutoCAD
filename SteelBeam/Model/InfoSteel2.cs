using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
namespace SteelBeam
{
    public class InfoSteel2
    {
        private const double cir_radius = 62.5;
        private const double text_height = 62.5;
        private const double text_w_factor = 0.75;
        private const double length_line = 15.0;
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

        public InfoSteel2()
        {
            number = 0;
            content = "";
            position = AcadGeo.Point3d.Origin;
            steel_pos = new List<AcadGeo.Point3d>();
        }

        public void AddSteelPos(AcadGeo.Point3d pnt)
        {
            steel_pos.Add(pnt);
        }

        public List<AcadDB.Entity> Draw()
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            if (0 == steel_pos.Count)
                return ents;

            AcadGeo.Point3d tmp_line = steel_pos[0];
            foreach(var sp in steel_pos)
            {
                AcadDB.Line line = new AcadDB.Line();
                line.StartPoint = sp + length_line * AcadGeo.Vector3d.XAxis + length_line * AcadGeo.Vector3d.YAxis;
                line.EndPoint = sp - length_line * AcadGeo.Vector3d.XAxis - length_line * AcadGeo.Vector3d.YAxis;
                ents.Add(line);

                if (position.DistanceTo(sp) > position.DistanceTo(tmp_line))
                    tmp_line = sp;
            }
            ents.Add(new AcadDB.Line(position, tmp_line));

            ents.Add(new AcadDB.Circle(position - AcadGeo.Vector3d.XAxis * cir_radius, AcadGeo.Vector3d.ZAxis, cir_radius));

            //Draw number
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
