using Autodesk.AutoCAD.Runtime;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelBeam
{
    public class SteelBeam
    {
        public const double offset_steel_layer = 50.0;
        public List<AcadDB.Entity> Beam1a(AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = 300;
            beam.Height = 500;
            beam.Length = 5000;

            SteelShapeA shape_a = new SteelShapeA();
            shape_a.Width = beam.Width - 2 * Beam.cover;
            shape_a.Height = beam.Height - 2 * Beam.cover;
            shape_a.Diamter = 8;
            shape_a.Spacing = 150;

            SteelShapeB shape_b = new SteelShapeB();
            shape_b.NumberSteel = 4;
            shape_b.Diameter = 22;
            shape_b.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_c = new SteelShapeB();
            shape_c.NumberSteel = 4;
            shape_c.Diameter = 22;
            shape_c.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_c.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_d = new SteelShapeB();
            shape_d.NumberSteel = 2;
            shape_d.Diameter = 22;
            shape_d.StartPoint = ins_pnt +
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_d.EndPoint = ins_pnt +
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            ents.AddRange(shape_a.DrawShapeA(ins_pnt));
            ents.AddRange(shape_b.Draw());
            ents.AddRange(shape_c.Draw());
            ents.AddRange(shape_d.Draw());

            return ents;
        }

        public List<AcadDB.Entity> Beam1b(AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = 300;
            beam.Height = 500;
            beam.Length = 5000;

            SteelShapeA shape_a = new SteelShapeA();
            shape_a.Width = beam.Width - 2 * Beam.cover;
            shape_a.Height = beam.Height - 2 * Beam.cover;
            shape_a.Diamter = 8;
            shape_a.Spacing = 150;

            SteelShapeB shape_b = new SteelShapeB();
            shape_b.NumberSteel = 4;
            shape_b.Diameter = 22;
            shape_b.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_c = new SteelShapeB();
            shape_c.NumberSteel = 4;
            shape_c.Diameter = 22;
            shape_c.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_c.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_d = new SteelShapeB();
            shape_d.NumberSteel = 2;
            shape_d.Diameter = 22;
            shape_d.StartPoint = ins_pnt -
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_d.EndPoint = ins_pnt -
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            ents.AddRange(shape_a.DrawShapeA(ins_pnt));
            ents.AddRange(shape_b.Draw());
            ents.AddRange(shape_c.Draw());
            ents.AddRange(shape_d.Draw());

            return ents;
        }

        [CommandMethod("DrawBeam1")]
        public void DrawBeam1()
        {
            try
            {
                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");
                List<AcadDB.Entity> ents = Beam1a(ins_pnt);

                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
                ents.AddRange(Beam1b(ins_pnt));

                foreach (var ent in ents)
                {
                    AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
        }

        public List<AcadDB.Entity> Beam2a(AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = 300;
            beam.Height = 500;
            beam.Length = 5000;

            SteelShapeA shape_a = new SteelShapeA();
            shape_a.Width = beam.Width - 2 * Beam.cover;
            shape_a.Height = beam.Height - 2 * Beam.cover;
            shape_a.Diamter = 8;
            shape_a.Spacing = 150;

            SteelShapeB shape_b1 = new SteelShapeB();
            shape_b1.NumberSteel = 5;
            shape_b1.Diameter = 22;
            shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b2 = new SteelShapeB();
            shape_b2.NumberSteel = 5;
            shape_b2.Diameter = 22;
            shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b3 = new SteelShapeB();
            shape_b3.NumberSteel = 5;
            shape_b3.Diameter = 22;
            shape_b3.StartPoint = ins_pnt +
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b3.EndPoint = ins_pnt +
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeC shape_c1 = new SteelShapeC(MissingDir.RIGHT);
            shape_c1.Width = SteelShapeC.corner_rad * 2;
            shape_c1.Height = beam.Height - 2 * Beam.cover;
            shape_c1.Diamter = 8;
            shape_c1.Spacing = 150;

            SteelShapeC shape_c2 = new SteelShapeC(MissingDir.DOWN);
            shape_c2.Width = beam.Width - 2 * Beam.cover;
            shape_c2.Height = SteelShapeC.corner_rad * 2;
            shape_c2.Diamter = 8;
            shape_c2.Spacing = 150;

            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            ents.AddRange(shape_a.DrawShapeA(ins_pnt));
            ents.AddRange(shape_b1.Draw());
            ents.AddRange(shape_b2.Draw());
            ents.AddRange(shape_b3.Draw());
            ents.AddRange(shape_c1.DrawShapeC(ins_pnt));
            ents.AddRange(shape_c2.DrawShapeC(
                ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad)));

            return ents;
        }

        public List<AcadDB.Entity> Beam2b(AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = 300;
            beam.Height = 500;
            beam.Length = 5000;

            SteelShapeA shape_a = new SteelShapeA();
            shape_a.Width = beam.Width - 2 * Beam.cover;
            shape_a.Height = beam.Height - 2 * Beam.cover;
            shape_a.Diamter = 8;
            shape_a.Spacing = 150;

            SteelShapeB shape_b = new SteelShapeB();
            shape_b.NumberSteel = 5;
            shape_b.Diameter = 22;
            shape_b.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_c = new SteelShapeB();
            shape_c.NumberSteel = 5;
            shape_c.Diameter = 22;
            shape_c.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_c.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_d = new SteelShapeB();
            shape_d.NumberSteel = 5;
            shape_d.Diameter = 22;
            shape_d.StartPoint = ins_pnt -
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_d.EndPoint = ins_pnt -
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeC shape_c1 = new SteelShapeC(MissingDir.RIGHT);
            shape_c1.Width = SteelShapeC.corner_rad * 2;
            shape_c1.Height = beam.Height - 2 * Beam.cover;
            shape_c1.Diamter = 8;
            shape_c1.Spacing = 150;

            SteelShapeC shape_c2 = new SteelShapeC(MissingDir.UP);
            shape_c2.Width = beam.Width - 2 * Beam.cover;
            shape_c2.Height = SteelShapeC.corner_rad * 2;
            shape_c2.Diamter = 8;
            shape_c2.Spacing = 150;

            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            ents.AddRange(shape_a.DrawShapeA(ins_pnt));
            ents.AddRange(shape_b.Draw());
            ents.AddRange(shape_c.Draw());
            ents.AddRange(shape_d.Draw());
            ents.AddRange(shape_c1.DrawShapeC(ins_pnt));
            ents.AddRange(shape_c2.DrawShapeC(
                ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad)));

            return ents;
        }

        [CommandMethod("DrawBeam2")]
        public void DrawBeam2()
        {
            try
            {
                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");
                List<AcadDB.Entity> ents = Beam2a(ins_pnt);

                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
                ents.AddRange(Beam2b(ins_pnt));

                foreach (var ent in ents)
                {
                    AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
        }

        public List<AcadDB.Entity> Beam3a(AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = 600;
            beam.Height = 800;
            beam.Length = 5000;

            SteelShapeA shape_a = new SteelShapeA();
            shape_a.Width = beam.Width - 2 * Beam.cover;
            shape_a.Height = beam.Height - 2 * Beam.cover;
            shape_a.Diamter = 8;
            shape_a.Spacing = 150;

            SteelShapeB shape_b1 = new SteelShapeB();
            shape_b1.NumberSteel = 6;
            shape_b1.Diameter = 22;
            shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b2 = new SteelShapeB();
            shape_b2.NumberSteel = 6;
            shape_b2.Diameter = 22;
            shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b3 = new SteelShapeB();
            shape_b3.NumberSteel = 6;
            shape_b3.Diameter = 22;
            shape_b3.StartPoint = ins_pnt +
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b3.EndPoint = ins_pnt +
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b4 = new SteelShapeB();
            shape_b4.NumberSteel = 2;
            shape_b4.Diameter = 22;
            shape_b4.StartPoint = ins_pnt +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b4.EndPoint = ins_pnt -
                AcadGeo.Vector3d.XAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeC shape_c1 = new SteelShapeC(MissingDir.DOWN);
            shape_c1.Width = beam.Width - 2 * Beam.cover;
            shape_c1.Height = SteelShapeC.corner_rad * 2;
            shape_c1.Diamter = 8;
            shape_c1.Spacing = 150;

            SteelShapeC shape_c2 = new SteelShapeC(MissingDir.DOWN);
            shape_c2.Width = beam.Width - 2 * Beam.cover;
            shape_c2.Height = SteelShapeC.corner_rad * 2;
            shape_c2.Diamter = 8;
            shape_c2.Spacing = 150;

            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            ents.AddRange(shape_a.DrawShapeA(ins_pnt));
            ents.AddRange(shape_b1.Draw());
            ents.AddRange(shape_b2.Draw());
            ents.AddRange(shape_b3.Draw());
            ents.AddRange(shape_b4.Draw());
            ents.AddRange(shape_c1.DrawShapeC(ins_pnt));
            ents.AddRange(shape_c2.DrawShapeC(
                ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad)));

            return ents;
        }

        public List<AcadDB.Entity> Beam3b(AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = 600;
            beam.Height = 800;
            beam.Length = 5000;

            SteelShapeA shape_a = new SteelShapeA();
            shape_a.Width = beam.Width - 2 * Beam.cover;
            shape_a.Height = beam.Height - 2 * Beam.cover;
            shape_a.Diamter = 8;
            shape_a.Spacing = 150;

            SteelShapeB shape_b1 = new SteelShapeB();
            shape_b1.NumberSteel = 5;
            shape_b1.Diameter = 22;
            shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b2 = new SteelShapeB();
            shape_b2.NumberSteel = 5;
            shape_b2.Diameter = 22;
            shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b3 = new SteelShapeB();
            shape_b3.NumberSteel = 5;
            shape_b3.Diameter = 22;
            shape_b3.StartPoint = ins_pnt -
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b3.EndPoint = ins_pnt -
                AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeB shape_b4 = new SteelShapeB();
            shape_b4.NumberSteel = 2;
            shape_b4.Diameter = 22;
            shape_b4.StartPoint = ins_pnt +
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
            shape_b4.EndPoint = ins_pnt -
                AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);

            SteelShapeC shape_c1 = new SteelShapeC(MissingDir.DOWN);
            shape_c1.Width = beam.Width - 2 * Beam.cover;
            shape_c1.Height = SteelShapeC.corner_rad * 2;
            shape_c1.Diamter = 8;
            shape_c1.Spacing = 150;

            SteelShapeC shape_c2 = new SteelShapeC(MissingDir.UP);
            shape_c2.Width = beam.Width - 2 * Beam.cover;
            shape_c2.Height = SteelShapeC.corner_rad * 2;
            shape_c2.Diamter = 8;
            shape_c2.Spacing = 150;

            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            ents.AddRange(shape_a.DrawShapeA(ins_pnt));
            ents.AddRange(shape_b1.Draw());
            ents.AddRange(shape_b2.Draw());
            ents.AddRange(shape_b3.Draw());
            ents.AddRange(shape_c1.DrawShapeC(ins_pnt));
            ents.AddRange(shape_c2.DrawShapeC(
                ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad)));

            return ents;
        }

        [CommandMethod("DrawBeam3")]
        public void DrawBeam3()
        {
            try
            {
                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");
                List<AcadDB.Entity> ents = Beam3a(ins_pnt);

                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
                ents.AddRange(Beam3b(ins_pnt));

                foreach (var ent in ents)
                {
                    AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
        }

    }
}
