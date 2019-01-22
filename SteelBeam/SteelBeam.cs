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
        public const double angle_info_steel = 60.0 * 3.1415 / 360.0;
        public const double OFFSET_TEXT_POS = 750.0;

        [CommandMethod("ReadBeam")]
        public void ReadInfoBeam()
        {

        }

        public List<AcadDB.Entity> Beam1a(AcadGeo.Point3d ins_pnt, Beam beam)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            ents.AddRange(beam.DrawDauGoi(ins_pnt));

            {
                SteelShapeA shape_a = new SteelShapeA();
                shape_a.Width = beam.Width - 2 * Beam.cover;
                shape_a.Height = beam.Height - 2 * Beam.cover;
                shape_a.Diamter = 8;
                shape_a.Spacing = 150;
                ents.AddRange(shape_a.DrawShapeA(ins_pnt));

                InfoSteel2 info_s = new InfoSteel2();
                info_s.Number = 1;
                info_s.Content = shape_a.GetContent();
                info_s.Position = ins_pnt - AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS -
                    AcadGeo.Vector3d.YAxis * 30.0;
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a.Width * 0.5 -
                    AcadGeo.Vector3d.YAxis * 30.0);
                ents.AddRange(info_s.Draw());
            }

            {
                SteelShapeB shape_b1 = new SteelShapeB();
                shape_b1.NumberSteel = 4;
                shape_b1.Diameter = 22;
                shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b1.Draw());

                InfoSteel info_s1 = new InfoSteel();
                info_s1.Number = 1;
                info_s1.Content = shape_b1.GetContent();
                info_s1.SteelPos = shape_b1.QuerySteelPos();
                info_s1.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b1.StartPoint.X + shape_b1.EndPoint.X),
                    0.5 * (shape_b1.StartPoint.Y + shape_b1.EndPoint.Y),
                    0.5 * (shape_b1.StartPoint.Z + shape_b1.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s1.Position = info_pos;
                ents.AddRange(info_s1.Draw());
            }

            {
                SteelShapeB shape_b2 = new SteelShapeB();
                shape_b2.NumberSteel = 4;
                shape_b2.Diameter = 22;
                shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b2.Draw());

                InfoSteel info_s2 = new InfoSteel();
                info_s2.Number = 1;
                info_s2.Content = shape_b2.GetContent();
                info_s2.SteelPos = shape_b2.QuerySteelPos();
                info_s2.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b2.StartPoint.X + shape_b2.EndPoint.X),
                    0.5 * (shape_b2.StartPoint.Y + shape_b2.EndPoint.Y),
                    0.5 * (shape_b2.StartPoint.Z + shape_b2.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s2.Position = info_pos;
                ents.AddRange(info_s2.Draw());
            }

            {
                SteelShapeB shape_b3 = new SteelShapeB();
                shape_b3.NumberSteel = 2;
                shape_b3.Diameter = 22;
                shape_b3.StartPoint = ins_pnt +
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b3.EndPoint = ins_pnt +
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b3.Draw());

                InfoSteel info_s3 = new InfoSteel();
                info_s3.Number = 1;
                info_s3.Content = shape_b3.GetContent();
                info_s3.SteelPos = shape_b3.QuerySteelPos();
                info_s3.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b3.StartPoint.X + shape_b3.EndPoint.X),
                    0.5 * (shape_b3.StartPoint.Y + shape_b3.EndPoint.Y),
                    0.5 * (shape_b3.StartPoint.Z + shape_b3.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s3.Position = info_pos;
                ents.AddRange(info_s3.Draw());
            }

            return ents;
        }

        public List<AcadDB.Entity> Beam1b(AcadGeo.Point3d ins_pnt, Beam beam)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();
            beam.DrawDauGoi(ins_pnt);

            {
                SteelShapeA shape_a = new SteelShapeA();
                shape_a.Width = beam.Width - 2 * Beam.cover;
                shape_a.Height = beam.Height - 2 * Beam.cover;
                shape_a.Diamter = 8;
                shape_a.Spacing = 150;
                ents.AddRange(shape_a.DrawShapeA(ins_pnt));

                InfoSteel2 info_s = new InfoSteel2();
                info_s.Number = 1;
                info_s.Content = shape_a.GetContent();
                info_s.Position = ins_pnt - AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS +
                    AcadGeo.Vector3d.YAxis * 60.0;
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a.Width * 0.5 +
                    AcadGeo.Vector3d.YAxis * 60.0);
                ents.AddRange(info_s.Draw());
            }
            {
                SteelShapeB shape_b1 = new SteelShapeB();
                shape_b1.NumberSteel = 4;
                shape_b1.Diameter = 22;
                shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b1.Draw());

                InfoSteel info_s1 = new InfoSteel();
                info_s1.Number = 1;
                info_s1.Content = shape_b1.GetContent();
                info_s1.SteelPos = shape_b1.QuerySteelPos();
                info_s1.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b1.StartPoint.X + shape_b1.EndPoint.X),
                    0.5 * (shape_b1.StartPoint.Y + shape_b1.EndPoint.Y),
                    0.5 * (shape_b1.StartPoint.Z + shape_b1.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s1.Position = info_pos;
                ents.AddRange(info_s1.Draw());
            }

            {
                SteelShapeB shape_b2 = new SteelShapeB();
                shape_b2.NumberSteel = 4;
                shape_b2.Diameter = 22;
                shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b2.Draw());

                InfoSteel info_s2 = new InfoSteel();
                info_s2.Number = 1;
                info_s2.Content = shape_b2.GetContent();
                info_s2.SteelPos = shape_b2.QuerySteelPos();
                info_s2.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b2.StartPoint.X + shape_b2.EndPoint.X),
                    0.5 * (shape_b2.StartPoint.Y + shape_b2.EndPoint.Y),
                    0.5 * (shape_b2.StartPoint.Z + shape_b2.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s2.Position = info_pos;
                ents.AddRange(info_s2.Draw());
            }

            {
                SteelShapeB shape_b3 = new SteelShapeB();
                shape_b3.NumberSteel = 2;
                shape_b3.Diameter = 22;
                shape_b3.StartPoint = ins_pnt -
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b3.EndPoint = ins_pnt -
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b3.Draw());

                InfoSteel info_s3 = new InfoSteel();
                info_s3.Number = 1;
                info_s3.Content = shape_b3.GetContent();
                info_s3.SteelPos = shape_b3.QuerySteelPos();
                info_s3.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b3.StartPoint.X + shape_b3.EndPoint.X),
                    0.5 * (shape_b3.StartPoint.Y + shape_b3.EndPoint.Y),
                    0.5 * (shape_b3.StartPoint.Z + shape_b3.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s3.Position = info_pos;
                ents.AddRange(info_s3.Draw());
            }

            return ents;
        }

        [CommandMethod("DrawBeam1")]
        public void DrawBeam1()
        {
            try
            {
                Beam beam = new Beam();
                beam.Width = 300;
                beam.Height = 500;
                beam.Length = 5000;

                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");
                List<AcadDB.Entity> ents = Beam1a(ins_pnt, beam);

                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
                ents.AddRange(Beam1b(ins_pnt, beam));

                foreach (var ent in ents)
                {
                    AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
        }

        public List<AcadDB.Entity> Beam2a(AcadGeo.Point3d ins_pnt, Beam beam)
        {
            const double down_footer = 50.0;
            const double up_footer = 100.0;
            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            double spacing_height = beam.Height - 2 * Beam.cover -
                2 * SteelShapeC.corner_rad - down_footer - up_footer;

            {
                SteelShapeA shape_a = new SteelShapeA();
                shape_a.Width = beam.Width - 2 * Beam.cover;
                shape_a.Height = beam.Height - 2 * Beam.cover;
                shape_a.Diamter = 8;
                shape_a.Spacing = 150;
                ents.AddRange(shape_a.DrawShapeA(ins_pnt));

                InfoSteel2 info_s = new InfoSteel2();
                info_s.Number = 1;
                info_s.Content = shape_a.GetContent();
                info_s.Position = ins_pnt - AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS -
                    AcadGeo.Vector3d.YAxis * (spacing_height / 3.0 - down_footer);
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a.Width * 0.5 -
                    AcadGeo.Vector3d.YAxis * (spacing_height / 3.0 - down_footer));
                ents.AddRange(info_s.Draw());
            }

            {
                SteelShapeB shape_b1 = new SteelShapeB();
                shape_b1.NumberSteel = 5;
                shape_b1.Diameter = 22;
                shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b1.Draw());

                InfoSteel info_s1 = new InfoSteel();
                info_s1.Number = 1;
                info_s1.Content = shape_b1.GetContent();
                info_s1.SteelPos = shape_b1.QuerySteelPos();
                info_s1.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b1.StartPoint.X + shape_b1.EndPoint.X),
                    0.5 * (shape_b1.StartPoint.Y + shape_b1.EndPoint.Y),
                    0.5 * (shape_b1.StartPoint.Z + shape_b1.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s1.Position = info_pos;
                ents.AddRange(info_s1.Draw());
            }

            {
                SteelShapeB shape_b2 = new SteelShapeB();
                shape_b2.NumberSteel = 5;
                shape_b2.Diameter = 22;
                shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b2.Draw());

                InfoSteel info_s2 = new InfoSteel();
                info_s2.Number = 1;
                info_s2.Content = shape_b2.GetContent();
                info_s2.SteelPos = shape_b2.QuerySteelPos();
                info_s2.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b2.StartPoint.X + shape_b2.EndPoint.X),
                    0.5 * (shape_b2.StartPoint.Y + shape_b2.EndPoint.Y),
                    0.5 * (shape_b2.StartPoint.Z + shape_b2.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s2.Position = info_pos;
                ents.AddRange(info_s2.Draw());
            }

            {
                SteelShapeB shape_b3 = new SteelShapeB();
                shape_b3.NumberSteel = 5;
                shape_b3.Diameter = 22;
                shape_b3.StartPoint = ins_pnt +
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b3.EndPoint = ins_pnt +
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b3.Draw());

                InfoSteel info_s3 = new InfoSteel();
                info_s3.Number = 1;
                info_s3.Content = shape_b3.GetContent();
                info_s3.SteelPos = shape_b3.QuerySteelPos();
                info_s3.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b3.StartPoint.X + shape_b3.EndPoint.X),
                    0.5 * (shape_b3.StartPoint.Y + shape_b3.EndPoint.Y),
                    0.5 * (shape_b3.StartPoint.Z + shape_b3.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s3.Position = info_pos;
                ents.AddRange(info_s3.Draw());
            }

            {
                SteelShapeC shape_c1 = new SteelShapeC(MissingDir.RIGHT);
                shape_c1.Width = SteelShapeC.corner_rad * 2;
                shape_c1.Height = beam.Height - 2 * Beam.cover;
                shape_c1.Diamter = 8;
                shape_c1.Spacing = 150;
                ents.AddRange(shape_c1.DrawShapeC(ins_pnt));

                InfoSteel2 info_s4 = new InfoSteel2();
                info_s4.Number = 1;
                info_s4.Content = shape_c1.GetContent();
                info_s4.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * SteelShapeC.corner_rad -
                    AcadGeo.Vector3d.YAxis * (2.5 * spacing_height / 3.0 - down_footer));
                AcadGeo.Point3d info_pos = ins_pnt;
                info_pos -= AcadGeo.Vector3d.YAxis * (2.5 * spacing_height / 3.0 - down_footer);
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s4.Position = info_pos;
                ents.AddRange(info_s4.Draw());
            }

            {
                SteelShapeC shape_c2 = new SteelShapeC(MissingDir.DOWN);
                shape_c2.Width = beam.Width - 2 * Beam.cover;
                shape_c2.Height = SteelShapeC.corner_rad * 2;
                shape_c2.Diamter = 8;
                shape_c2.Spacing = 150;
                AcadGeo.Point3d shape_ins_pnt =
                    ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad);
                ents.AddRange(shape_c2.DrawShapeC(shape_ins_pnt));

                InfoSteel info_s5 = new InfoSteel();
                info_s5.Number = 1;
                info_s5.Content = shape_c2.GetContent();
                List<AcadGeo.Point3d> steel_pos = new List<AcadGeo.Point3d>();
                steel_pos.Add(shape_ins_pnt + AcadGeo.Vector3d.YAxis * SteelShapeC.corner_rad);
                info_s5.SteelPos = steel_pos;
                info_s5.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = shape_ins_pnt;
                info_pos += AcadGeo.Vector3d.YAxis * 320.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s5.Position = info_pos;
                ents.AddRange(info_s5.Draw());
            }

            return ents;
        }

        public List<AcadDB.Entity> Beam2b(AcadGeo.Point3d ins_pnt, Beam beam)
        {
            const double down_footer = 100.0;
            const double up_footer = 50.0;
            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);
            double spacing_height = beam.Height - 2 * Beam.cover -
                2 * SteelShapeC.corner_rad - down_footer - up_footer;

            {
                SteelShapeA shape_a = new SteelShapeA();
                shape_a.Width = beam.Width - 2 * Beam.cover;
                shape_a.Height = beam.Height - 2 * Beam.cover;
                shape_a.Diamter = 8;
                shape_a.Spacing = 150;
                ents.AddRange(shape_a.DrawShapeA(ins_pnt));

                InfoSteel2 info_s = new InfoSteel2();
                info_s.Number = 1;
                info_s.Content = shape_a.GetContent();
                info_s.Position = ins_pnt - AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS +
                    AcadGeo.Vector3d.YAxis * (spacing_height / 3.0 + up_footer - down_footer);
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a.Width * 0.5 +
                    AcadGeo.Vector3d.YAxis * (spacing_height / 3.0 + up_footer - down_footer));
                ents.AddRange(info_s.Draw());
            }

            {
                SteelShapeB shape_b1 = new SteelShapeB();
                shape_b1.NumberSteel = 5;
                shape_b1.Diameter = 22;
                shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b1.Draw());

                InfoSteel info_s1 = new InfoSteel();
                info_s1.Number = 1;
                info_s1.Content = shape_b1.GetContent();
                info_s1.SteelPos = shape_b1.QuerySteelPos();
                info_s1.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b1.StartPoint.X + shape_b1.EndPoint.X),
                    0.5 * (shape_b1.StartPoint.Y + shape_b1.EndPoint.Y),
                    0.5 * (shape_b1.StartPoint.Z + shape_b1.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s1.Position = info_pos;
                ents.AddRange(info_s1.Draw());
            }

            {
                SteelShapeB shape_b2 = new SteelShapeB();
                shape_b2.NumberSteel = 5;
                shape_b2.Diameter = 22;
                shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b2.Draw());

                InfoSteel info_s2 = new InfoSteel();
                info_s2.Number = 1;
                info_s2.Content = shape_b2.GetContent();
                info_s2.SteelPos = shape_b2.QuerySteelPos();
                info_s2.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b2.StartPoint.X + shape_b2.EndPoint.X),
                    0.5 * (shape_b2.StartPoint.Y + shape_b2.EndPoint.Y),
                    0.5 * (shape_b2.StartPoint.Z + shape_b2.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s2.Position = info_pos;
                ents.AddRange(info_s2.Draw());
            }

            {
                SteelShapeB shape_b3 = new SteelShapeB();
                shape_b3.NumberSteel = 5;
                shape_b3.Diameter = 22;
                shape_b3.StartPoint = ins_pnt -
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b3.EndPoint = ins_pnt -
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b3.Draw());

                InfoSteel info_s3 = new InfoSteel();
                info_s3.Number = 1;
                info_s3.Content = shape_b3.GetContent();
                info_s3.SteelPos = shape_b3.QuerySteelPos();
                info_s3.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b3.StartPoint.X + shape_b3.EndPoint.X),
                    0.5 * (shape_b3.StartPoint.Y + shape_b3.EndPoint.Y),
                    0.5 * (shape_b3.StartPoint.Z + shape_b3.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s3.Position = info_pos;
                ents.AddRange(info_s3.Draw());
            }

            {
                SteelShapeC shape_c1 = new SteelShapeC(MissingDir.RIGHT);
                shape_c1.Width = SteelShapeC.corner_rad * 2;
                shape_c1.Height = beam.Height - 2 * Beam.cover;
                shape_c1.Diamter = 8;
                shape_c1.Spacing = 150;
                ents.AddRange(shape_c1.DrawShapeC(ins_pnt));

                InfoSteel2 info_s4 = new InfoSteel2();
                info_s4.Number = 1;
                info_s4.Content = shape_c1.GetContent();
                info_s4.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * SteelShapeC.corner_rad +
                    AcadGeo.Vector3d.YAxis * (2.5 * spacing_height / 3.0 + up_footer - down_footer));
                AcadGeo.Point3d info_pos = ins_pnt;
                info_pos += AcadGeo.Vector3d.YAxis * (2.5 * spacing_height / 3.0 + up_footer - down_footer);
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s4.Position = info_pos;
                ents.AddRange(info_s4.Draw());
            }

            {
                SteelShapeC shape_c2 = new SteelShapeC(MissingDir.UP);
                shape_c2.Width = beam.Width - 2 * Beam.cover;
                shape_c2.Height = SteelShapeC.corner_rad * 2;
                shape_c2.Diamter = 8;
                shape_c2.Spacing = 150;
                AcadGeo.Point3d shape_ins_pnt =
                    ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad);
                ents.AddRange(shape_c2.DrawShapeC(shape_ins_pnt));

                InfoSteel info_s5 = new InfoSteel();
                info_s5.Number = 1;
                info_s5.Content = shape_c2.GetContent();
                List<AcadGeo.Point3d> steel_pos = new List<AcadGeo.Point3d>();
                steel_pos.Add(shape_ins_pnt - AcadGeo.Vector3d.YAxis * SteelShapeC.corner_rad);
                info_s5.SteelPos = steel_pos;
                info_s5.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = shape_ins_pnt;
                info_pos -= AcadGeo.Vector3d.YAxis * 320.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s5.Position = info_pos;
                ents.AddRange(info_s5.Draw());
            }

            return ents;
        }

        [CommandMethod("DrawBeam2")]
        public void DrawBeam2()
        {
            try
            {
                Beam beam = new Beam();
                beam.Width = 300;
                beam.Height = 500;
                beam.Length = 5000;

                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");
                List<AcadDB.Entity> ents = Beam2a(ins_pnt, beam);

                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
                ents.AddRange(Beam2b(ins_pnt, beam));

                foreach (var ent in ents)
                {
                    AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
        }

        public List<AcadDB.Entity> Beam3a(AcadGeo.Point3d ins_pnt, Beam beam)
        {
            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);

            {
                SteelShapeA shape_a1 = new SteelShapeA();
                shape_a1.Width = beam.Width - 2 * Beam.cover;
                shape_a1.Height = beam.Height - 2 * Beam.cover;
                shape_a1.Diamter = 8;
                shape_a1.Spacing = 150;
                ents.AddRange(shape_a1.DrawShapeA(ins_pnt));

                SteelShapeA shape_a2 = new SteelShapeA();
                int spacing = beam.Width - 2 * Beam.cover - 2 * SteelShapeB.offset;
                spacing = spacing / 5 + 1;
                shape_a2.Width = spacing + 2 * SteelShapeB.offset;
                shape_a2.Height = beam.Height - 2 * Beam.cover;
                shape_a2.Diamter = 8;
                shape_a2.Spacing = 150;
                ents.AddRange(shape_a2.DrawShapeA(ins_pnt));

                InfoSteel2 info_s = new InfoSteel2();
                info_s.Number = 1;
                info_s.Content = shape_a1.GetContent();
                info_s.Position = ins_pnt - AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS -
                    AcadGeo.Vector3d.YAxis * 210.0;
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a1.Width * 0.5 -
                    AcadGeo.Vector3d.YAxis * 210.0);
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a2.Width * 0.5 -
                    AcadGeo.Vector3d.YAxis * 210.0);
                ents.AddRange(info_s.Draw());
            }

            {
                SteelShapeB shape_b1 = new SteelShapeB();
                shape_b1.NumberSteel = 6;
                shape_b1.Diameter = 22;
                shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b1.Draw());

                InfoSteel info_s1 = new InfoSteel();
                info_s1.Number = 1;
                info_s1.Content = shape_b1.GetContent();
                info_s1.SteelPos = shape_b1.QuerySteelPos();
                info_s1.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b1.StartPoint.X + shape_b1.EndPoint.X),
                    0.5 * (shape_b1.StartPoint.Y + shape_b1.EndPoint.Y),
                    0.5 * (shape_b1.StartPoint.Z + shape_b1.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s1.Position = info_pos;
                ents.AddRange(info_s1.Draw());
            }

            {
                SteelShapeB shape_b2 = new SteelShapeB();
                shape_b2.NumberSteel = 6;
                shape_b2.Diameter = 22;
                shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b2.Draw());

                InfoSteel info_s2 = new InfoSteel();
                info_s2.Number = 1;
                info_s2.Content = shape_b2.GetContent();
                info_s2.SteelPos = shape_b2.QuerySteelPos();
                info_s2.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b2.StartPoint.X + shape_b2.EndPoint.X),
                    0.5 * (shape_b2.StartPoint.Y + shape_b2.EndPoint.Y),
                    0.5 * (shape_b2.StartPoint.Z + shape_b2.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s2.Position = info_pos;
                ents.AddRange(info_s2.Draw());
            }

            {
                SteelShapeB shape_b3 = new SteelShapeB();
                shape_b3.NumberSteel = 6;
                shape_b3.Diameter = 22;
                shape_b3.StartPoint = ins_pnt +
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b3.EndPoint = ins_pnt +
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b3.Draw());

                InfoSteel info_s3 = new InfoSteel();
                info_s3.Number = 1;
                info_s3.Content = shape_b3.GetContent();
                info_s3.SteelPos = shape_b3.QuerySteelPos();
                info_s3.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b3.StartPoint.X + shape_b3.EndPoint.X),
                    0.5 * (shape_b3.StartPoint.Y + shape_b3.EndPoint.Y),
                    0.5 * (shape_b3.StartPoint.Z + shape_b3.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s3.Position = info_pos;
                ents.AddRange(info_s3.Draw());
            }

            {
                SteelShapeB shape_b4 = new SteelShapeB();
                shape_b4.NumberSteel = 2;
                shape_b4.Diameter = 22;
                shape_b4.StartPoint = ins_pnt -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b4.EndPoint = ins_pnt +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b4.Draw());

                InfoSteel info_s4 = new InfoSteel();
                info_s4.Number = 1;
                info_s4.Content = shape_b4.GetContent();
                info_s4.SteelPos = shape_b4.QuerySteelPos();
                info_s4.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b4.StartPoint.X + shape_b4.EndPoint.X),
                    0.5 * (shape_b4.StartPoint.Y + shape_b4.EndPoint.Y),
                    0.5 * (shape_b4.StartPoint.Z + shape_b4.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s4.Position = info_pos;
                ents.AddRange(info_s4.Draw());
            }

            {
                SteelShapeC shape_c1 = new SteelShapeC(MissingDir.DOWN);
                shape_c1.Width = beam.Width - 2 * Beam.cover;
                shape_c1.Height = SteelShapeC.corner_rad * 2;
                shape_c1.Diamter = 8;
                shape_c1.Spacing = 150;
                ents.AddRange(shape_c1.DrawShapeC(ins_pnt));

                InfoSteel info_s4 = new InfoSteel();
                info_s4.Number = 1;
                info_s4.Content = shape_c1.GetContent();
                List<AcadGeo.Point3d> steel_pos = new List<AcadGeo.Point3d>();
                steel_pos.Add(ins_pnt + AcadGeo.Vector3d.YAxis * SteelShapeC.corner_rad);
                info_s4.SteelPos = steel_pos;
                info_s4.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = ins_pnt;
                info_pos += AcadGeo.Vector3d.YAxis * (100.0 + SteelShapeC.corner_rad);
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s4.Position = info_pos;
                ents.AddRange(info_s4.Draw());
            }

            {
                SteelShapeC shape_c2 = new SteelShapeC(MissingDir.DOWN);
                shape_c2.Width = beam.Width - 2 * Beam.cover;
                shape_c2.Height = SteelShapeC.corner_rad * 2;
                shape_c2.Diamter = 8;
                shape_c2.Spacing = 150;
                AcadGeo.Point3d shape_ins_pnt =
                    ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad);
                ents.AddRange(shape_c2.DrawShapeC(shape_ins_pnt));

                InfoSteel info_s5 = new InfoSteel();
                info_s5.Number = 1;
                info_s5.Content = shape_c2.GetContent();
                List<AcadGeo.Point3d> steel_pos = new List<AcadGeo.Point3d>();
                steel_pos.Add(shape_ins_pnt + AcadGeo.Vector3d.YAxis * SteelShapeC.corner_rad);
                info_s5.SteelPos = steel_pos;
                info_s5.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = shape_ins_pnt;
                info_pos += AcadGeo.Vector3d.YAxis * 320.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s5.Position = info_pos;
                ents.AddRange(info_s5.Draw());
            }

            return ents;
        }

        public List<AcadDB.Entity> Beam3b(AcadGeo.Point3d ins_pnt, Beam beam)
        {
            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);

            {
                SteelShapeA shape_a1 = new SteelShapeA();
                shape_a1.Width = beam.Width - 2 * Beam.cover;
                shape_a1.Height = beam.Height - 2 * Beam.cover;
                shape_a1.Diamter = 8;
                shape_a1.Spacing = 150;
                ents.AddRange(shape_a1.DrawShapeA(ins_pnt));

                SteelShapeA shape_a2 = new SteelShapeA();
                int spacing = beam.Width - 2 * Beam.cover - 2 * SteelShapeB.offset;
                spacing = spacing / 5 + 1;
                shape_a2.Width = spacing + 2 * SteelShapeB.offset;
                shape_a2.Height = beam.Height - 2 * Beam.cover;
                shape_a2.Diamter = 8;
                shape_a2.Spacing = 150;
                ents.AddRange(shape_a2.DrawShapeA(ins_pnt));

                InfoSteel2 info_s = new InfoSteel2();
                info_s.Number = 1;
                info_s.Content = shape_a1.GetContent();
                info_s.Position = ins_pnt - AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS -
                    AcadGeo.Vector3d.YAxis * 130.0;
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a1.Width * 0.5 -
                    AcadGeo.Vector3d.YAxis * 130.0);
                info_s.AddSteelPos(ins_pnt - AcadGeo.Vector3d.XAxis * shape_a2.Width * 0.5 -
                    AcadGeo.Vector3d.YAxis * 130.0);
                ents.AddRange(info_s.Draw());
            }

            {
                SteelShapeB shape_b1 = new SteelShapeB();
                shape_b1.NumberSteel = 6;
                shape_b1.Diameter = 22;
                shape_b1.StartPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b1.EndPoint = ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b1.Draw());

                InfoSteel info_s1 = new InfoSteel();
                info_s1.Number = 1;
                info_s1.Content = shape_b1.GetContent();
                info_s1.SteelPos = shape_b1.QuerySteelPos();
                info_s1.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b1.StartPoint.X + shape_b1.EndPoint.X),
                    0.5 * (shape_b1.StartPoint.Y + shape_b1.EndPoint.Y),
                    0.5 * (shape_b1.StartPoint.Z + shape_b1.EndPoint.Z));
                info_pos -= AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s1.Position = info_pos;
                ents.AddRange(info_s1.Draw());
            }

            {
                SteelShapeB shape_b2 = new SteelShapeB();
                shape_b2.NumberSteel = 6;
                shape_b2.Diameter = 22;
                shape_b2.StartPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b2.EndPoint = ins_pnt + AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b2.Draw());

                InfoSteel info_s2 = new InfoSteel();
                info_s2.Number = 1;
                info_s2.Content = shape_b2.GetContent();
                info_s2.SteelPos = shape_b2.QuerySteelPos();
                info_s2.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b2.StartPoint.X + shape_b2.EndPoint.X),
                    0.5 * (shape_b2.StartPoint.Y + shape_b2.EndPoint.Y),
                    0.5 * (shape_b2.StartPoint.Z + shape_b2.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 125.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s2.Position = info_pos;
                ents.AddRange(info_s2.Draw());
            }

            {
                SteelShapeB shape_b3 = new SteelShapeB();
                shape_b3.NumberSteel = 6;
                shape_b3.Diameter = 22;
                shape_b3.StartPoint = ins_pnt -
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b3.EndPoint = ins_pnt -
                    AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - SteelShapeB.offset - offset_steel_layer) +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b3.Draw());

                InfoSteel info_s3 = new InfoSteel();
                info_s3.Number = 1;
                info_s3.Content = shape_b3.GetContent();
                info_s3.SteelPos = shape_b3.QuerySteelPos();
                info_s3.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b3.StartPoint.X + shape_b3.EndPoint.X),
                    0.5 * (shape_b3.StartPoint.Y + shape_b3.EndPoint.Y),
                    0.5 * (shape_b3.StartPoint.Z + shape_b3.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s3.Position = info_pos;
                ents.AddRange(info_s3.Draw());
            }

            {
                SteelShapeB shape_b4 = new SteelShapeB();
                shape_b4.NumberSteel = 2;
                shape_b4.Diameter = 22;
                shape_b4.StartPoint = ins_pnt -
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                shape_b4.EndPoint = ins_pnt +
                    AcadGeo.Vector3d.XAxis * (beam.Width * 0.5 - Beam.cover - SteelShapeB.offset);
                ents.AddRange(shape_b4.Draw());

                InfoSteel info_s4 = new InfoSteel();
                info_s4.Number = 1;
                info_s4.Content = shape_b4.GetContent();
                info_s4.SteelPos = shape_b4.QuerySteelPos();
                info_s4.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = new AcadGeo.Point3d(
                    0.5 * (shape_b4.StartPoint.X + shape_b4.EndPoint.X),
                    0.5 * (shape_b4.StartPoint.Y + shape_b4.EndPoint.Y),
                    0.5 * (shape_b4.StartPoint.Z + shape_b4.EndPoint.Z));
                info_pos += AcadGeo.Vector3d.YAxis * 50.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s4.Position = info_pos;
                ents.AddRange(info_s4.Draw());
            }

            {
                SteelShapeC shape_c1 = new SteelShapeC(MissingDir.DOWN);
                shape_c1.Width = beam.Width - 2 * Beam.cover;
                shape_c1.Height = SteelShapeC.corner_rad * 2;
                shape_c1.Diamter = 8;
                shape_c1.Spacing = 150;
                ents.AddRange(shape_c1.DrawShapeC(ins_pnt));

                InfoSteel info_s4 = new InfoSteel();
                info_s4.Number = 1;
                info_s4.Content = shape_c1.GetContent();
                List<AcadGeo.Point3d> steel_pos = new List<AcadGeo.Point3d>();
                steel_pos.Add(ins_pnt + AcadGeo.Vector3d.YAxis * SteelShapeC.corner_rad);
                info_s4.SteelPos = steel_pos;
                info_s4.Angle = -angle_info_steel;
                AcadGeo.Point3d info_pos = ins_pnt;
                info_pos += AcadGeo.Vector3d.YAxis * (150.0 + SteelShapeC.corner_rad);
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s4.Position = info_pos;
                ents.AddRange(info_s4.Draw());
            }

            {
                SteelShapeC shape_c2 = new SteelShapeC(MissingDir.UP);
                shape_c2.Width = beam.Width - 2 * Beam.cover;
                shape_c2.Height = SteelShapeC.corner_rad * 2;
                shape_c2.Diamter = 8;
                shape_c2.Spacing = 150;
                AcadGeo.Point3d shape_ins_pnt =
                    ins_pnt - AcadGeo.Vector3d.YAxis * (beam.Height * 0.5 - Beam.cover - offset_steel_layer - SteelShapeC.corner_rad);
                ents.AddRange(shape_c2.DrawShapeC(shape_ins_pnt));

                InfoSteel info_s5 = new InfoSteel();
                info_s5.Number = 1;
                info_s5.Content = shape_c2.GetContent();
                List<AcadGeo.Point3d> steel_pos = new List<AcadGeo.Point3d>();
                steel_pos.Add(shape_ins_pnt - AcadGeo.Vector3d.YAxis * SteelShapeC.corner_rad);
                info_s5.SteelPos = steel_pos;
                info_s5.Angle = angle_info_steel;
                AcadGeo.Point3d info_pos = shape_ins_pnt;
                info_pos -= AcadGeo.Vector3d.YAxis * 320.0;
                info_pos -= AcadGeo.Vector3d.XAxis * OFFSET_TEXT_POS;
                info_s5.Position = info_pos;
                ents.AddRange(info_s5.Draw());
            }

            return ents;
        }

        [CommandMethod("DrawBeam3")]
        public void DrawBeam3()
        {
            try
            {
                Beam beam = new Beam();
                beam.Width = 600;
                beam.Height = 800;
                beam.Length = 5000;

                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");
                List<AcadDB.Entity> ents = Beam3a(ins_pnt, beam);

                ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
                ents.AddRange(Beam3b(ins_pnt, beam));

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
