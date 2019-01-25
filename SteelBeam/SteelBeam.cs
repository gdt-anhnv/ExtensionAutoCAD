using Autodesk.AutoCAD.Runtime;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteelBeam
{
    public class SteelBeam
    {
        public const double offset_steel_layer = 50.0;
        public const double angle_info_steel = 60.0 * 3.1415 / 360.0;
        public const double OFFSET_TEXT_POS = 750.0;

        public const double OFFSET_BLK_TK = 225.0;
        public const string BLK_THEP_THANG = "TK_2";
        public const string BLK_THEP_COGS = "TK_9";
        public const string BLK_THEP_COG = "TK_6";
        public const string BLK_THEP_DAI = "TK_14";
        public const string BLK_THKL = "TK_THKL";
        public const string BLK_TK_TITLE = "TK_TITLE";

        [CommandMethod("ReadBeam")]
        public void ReadInfoBeam()
        {
            try
            {
                var data = ReadExcel.DoReadExcel(@"C:\Users\nguye\OneDrive\Desktop\Data.xls");
                AcadGeo.Point3d ins_pnt = AcadFuncsCSharp.UserInput.PickPoint("Chọn điểm");

                foreach (var d in data)
                {
                    if (2 == d.dau_goi_data.type_beam)
                        DrawBeam1(d, ins_pnt);
                    else if (3 == d.dau_goi_data.type_beam)
                        DrawBeam2(d, ins_pnt);
                    else if (4 == d.dau_goi_data.type_beam)
                        DrawBeam3(d, ins_pnt);

                    ins_pnt -= AcadGeo.Vector3d.YAxis * (d.Height + 2000.0);
                }
            }
            catch(Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
        }

        public List<AcadDB.Entity> Beam1a(AcadGeo.Point3d ins_pnt, Beam beam, BeamDauGoiData data)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            ents.AddRange(beam.DrawDauGoi(ins_pnt));

            {
                SteelShapeA shape_a = new SteelShapeA();
                shape_a.Width = beam.Width - 2 * Beam.cover;
                shape_a.Height = beam.Height - 2 * Beam.cover;
                shape_a.Diamter = data.dai_diameter;
                shape_a.Spacing = data.spacing;
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
                shape_b1.NumberSteel = data.number_steel_first_layer;
                shape_b1.Diameter = data.first_layer_diameter;
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
                shape_b2.NumberSteel = data.number_steel_first_layer;
                shape_b2.Diameter = data.first_layer_diameter;
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
                shape_b3.NumberSteel = data.number_steel_second_layer;
                shape_b3.Diameter = data.second_layer_diameter;
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

        public List<AcadDB.Entity> Beam1b(AcadGeo.Point3d ins_pnt, Beam beam, BeamGiuaData data)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();
            beam.DrawDauGoi(ins_pnt);

            {
                SteelShapeA shape_a = new SteelShapeA();
                shape_a.Width = beam.Width - 2 * Beam.cover;
                shape_a.Height = beam.Height - 2 * Beam.cover;
                shape_a.Diamter = data.dai_diameter;
                shape_a.Spacing = data.spacing;
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
                shape_b1.NumberSteel = data.number_steel_first_layer;
                shape_b1.Diameter = data.first_layer_diameter;
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
                shape_b2.NumberSteel = data.number_steel_first_layer;
                shape_b2.Diameter = data.first_layer_diameter;
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
                shape_b3.NumberSteel = data.number_steel_second_layer;
                shape_b3.Diameter = data.second_layer_diameter;
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

        public List<AcadDB.Entity> StatisticsBeam1(AcadGeo.Point3d ins_pnt, BeamData beam_data)
        {
            List<AcadDB.Entity> ents = new List<AcadDB.Entity>();

            AcadDB.ObjectId blk_thep_thang = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_THEP_THANG);
            AcadDB.ObjectId blk_thep_cogs = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_THEP_COGS);
            AcadDB.ObjectId blk_thep_cog = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_THEP_COG);
            AcadDB.ObjectId blk_thep_dai = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_THEP_DAI);
            AcadDB.ObjectId blk_tkkl = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_THKL);
            AcadDB.ObjectId blk_thep_title = AcadFuncsCSharp.BlkRefFuncs.GetBlock(BLK_TK_TITLE);
            if(AcadDB.ObjectId.Null == blk_thep_thang ||
                AcadDB.ObjectId.Null == blk_thep_cogs ||
                AcadDB.ObjectId.Null == blk_thep_cog ||
                AcadDB.ObjectId.Null == blk_thep_dai ||
                AcadDB.ObjectId.Null ==  blk_tkkl ||
                AcadDB.ObjectId.Null == blk_thep_title)
            {
                MessageBox.Show("Khong co block thep");
                throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.OK, "Failed");
            }

            AcadDB.BlockReference blk_ref_title = new AcadDB.BlockReference(ins_pnt, blk_thep_title);
            ents.Add(blk_ref_title);
            ins_pnt -= AcadGeo.Vector3d.YAxis * 1100 - AcadGeo.Vector3d.XAxis * 375;

            AcadDB.BlockReference blk_ref1 = new AcadDB.BlockReference(ins_pnt, blk_thep_thang);
            ents.Add(blk_ref1);
            ins_pnt -= AcadGeo.Vector3d.YAxis * OFFSET_BLK_TK;

            AcadDB.BlockReference blk_ref2 = new AcadDB.BlockReference(ins_pnt, blk_thep_thang);
            ents.Add(blk_ref2);
            ins_pnt -= AcadGeo.Vector3d.YAxis * OFFSET_BLK_TK;

            AcadDB.BlockReference blk_ref3 = new AcadDB.BlockReference(ins_pnt, blk_thep_cogs);
            ents.Add(blk_ref3);
            ins_pnt -= AcadGeo.Vector3d.YAxis * OFFSET_BLK_TK;

            AcadDB.BlockReference blk_ref4 = new AcadDB.BlockReference(ins_pnt, blk_thep_cog);
            ents.Add(blk_ref4);
            ins_pnt -= AcadGeo.Vector3d.YAxis * OFFSET_BLK_TK;

            AcadDB.BlockReference blk_ref5 = new AcadDB.BlockReference(ins_pnt, blk_thep_dai);
            ents.Add(blk_ref5);
            ins_pnt -= AcadGeo.Vector3d.YAxis * OFFSET_BLK_TK;

            AcadDB.BlockReference ins_tkkl = new AcadDB.BlockReference(ins_pnt, blk_tkkl);
            ents.Add(ins_tkkl);
            ins_pnt -= AcadGeo.Vector3d.YAxis * OFFSET_BLK_TK;

            return ents;
        }

        public void DrawBeam1(BeamData beam_data, AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = beam_data.Width;
            beam.Height = beam_data.Height;
            beam.Length = beam_data.Length;

            List<AcadDB.Entity> ents = Beam1a(ins_pnt, beam, beam_data.dau_goi_data);

            ins_pnt += AcadGeo.Vector3d.XAxis * 2000.0;
            ents.AddRange(Beam1b(ins_pnt, beam, beam_data.giua_data));

            ins_pnt += AcadGeo.Vector3d.XAxis * 2000.0 + AcadGeo.Vector3d.YAxis * 500.0;
            StatisticsBeam1(ins_pnt, beam_data);

            foreach (var ent in ents)
            {
                AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
            }
        }

        public List<AcadDB.Entity> Beam2a(AcadGeo.Point3d ins_pnt, Beam beam, BeamDauGoiData data)
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
                shape_a.Diamter = data.dai_diameter;
                shape_a.Spacing = data.spacing;
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
                shape_b1.NumberSteel = data.number_steel_first_layer;
                shape_b1.Diameter = data.first_layer_diameter;
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
                shape_b2.NumberSteel = data.number_steel_first_layer;
                shape_b2.Diameter = data.first_layer_diameter;
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
                shape_b3.NumberSteel = data.number_steel_second_layer;
                shape_b3.Diameter = data.second_layer_diameter;
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
                shape_c1.Diamter = data.dai_diameter;
                shape_c1.Spacing = data.spacing;
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
                shape_c2.Diamter = data.dai_diameter2;
                shape_c2.Spacing = data.spacing2;
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

        public List<AcadDB.Entity> Beam2b(AcadGeo.Point3d ins_pnt, Beam beam, BeamGiuaData data)
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
                shape_a.Diamter = data.dai_diameter;
                shape_a.Spacing = data.spacing;
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
                shape_b1.NumberSteel = data.number_steel_first_layer;
                shape_b1.Diameter = data.first_layer_diameter;
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
                shape_b2.NumberSteel = data.number_steel_first_layer;
                shape_b2.Diameter = data.first_layer_diameter;
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
                shape_b3.NumberSteel = data.number_steel_second_layer;
                shape_b3.Diameter = data.second_layer_diameter;
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
                shape_c1.Diamter = data.dai_diameter;
                shape_c1.Spacing = data.spacing;
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
                shape_c2.Diamter = data.dai_diameter2;
                shape_c2.Spacing = data.spacing2;
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

        public void DrawBeam2(BeamData beam_data, AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = beam_data.Width;
            beam.Height = beam_data.Height;
            beam.Length = beam_data.Length;

            List<AcadDB.Entity> ents = Beam2a(ins_pnt, beam, beam_data.dau_goi_data);

            ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
            ents.AddRange(Beam2b(ins_pnt, beam, beam_data.giua_data));

            foreach (var ent in ents)
            {
                AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
            }
        }

        public List<AcadDB.Entity> Beam3a(AcadGeo.Point3d ins_pnt, Beam beam, BeamDauGoiData data)
        {
            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);

            {
                SteelShapeA shape_a1 = new SteelShapeA();
                shape_a1.Width = beam.Width - 2 * Beam.cover;
                shape_a1.Height = beam.Height - 2 * Beam.cover;
                shape_a1.Diamter = data.dai_diameter;
                shape_a1.Spacing = data.spacing;
                ents.AddRange(shape_a1.DrawShapeA(ins_pnt));

                SteelShapeA shape_a2 = new SteelShapeA();
                int spacing = beam.Width - 2 * Beam.cover - 2 * SteelShapeB.offset;
                spacing = spacing / data.number_steel_first_layer + (0 == data.number_steel_first_layer / 2 ? 1 : 2);
                shape_a2.Width = spacing + 2 * SteelShapeB.offset;
                shape_a2.Height = beam.Height - 2 * Beam.cover;
                shape_a2.Diamter = data.dai_diameter;
                shape_a2.Spacing = data.spacing;
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
                shape_b1.NumberSteel = data.number_steel_first_layer;
                shape_b1.Diameter = data.first_layer_diameter;
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
                shape_b2.NumberSteel = data.number_steel_first_layer;
                shape_b2.Diameter = data.first_layer_diameter;
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
                shape_b3.NumberSteel = data.number_steel_second_layer;
                shape_b3.Diameter = data.second_layer_diameter;
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
                shape_b4.Diameter = 14;
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
                shape_c1.Diamter = data.dai_diameter2;
                shape_c1.Spacing = data.spacing2;
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
                shape_c2.Spacing = 600;
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

        public List<AcadDB.Entity> Beam3b(AcadGeo.Point3d ins_pnt, Beam beam, BeamGiuaData data)
        {
            List<AcadDB.Entity> ents = beam.DrawDauGoi(ins_pnt);

            {
                SteelShapeA shape_a1 = new SteelShapeA();
                shape_a1.Width = beam.Width - 2 * Beam.cover;
                shape_a1.Height = beam.Height - 2 * Beam.cover;
                shape_a1.Diamter = data.dai_diameter;
                shape_a1.Spacing = data.spacing;
                ents.AddRange(shape_a1.DrawShapeA(ins_pnt));

                SteelShapeA shape_a2 = new SteelShapeA();
                int spacing = beam.Width - 2 * Beam.cover - 2 * SteelShapeB.offset;
                spacing = spacing / data.number_steel_first_layer + (0 == data.number_steel_first_layer / 2 ? 1 : 2);
                shape_a2.Width = spacing + 2 * SteelShapeB.offset;
                shape_a2.Height = beam.Height - 2 * Beam.cover;
                shape_a2.Diamter = data.dai_diameter;
                shape_a2.Spacing = data.spacing;
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
                shape_b1.NumberSteel = data.number_steel_first_layer;
                shape_b1.Diameter = data.first_layer_diameter;
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
                shape_b2.NumberSteel = data.number_steel_first_layer;
                shape_b2.Diameter = data.first_layer_diameter;
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
                shape_b3.NumberSteel = data.number_steel_second_layer;
                shape_b3.Diameter = data.second_layer_diameter;
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
                shape_b4.Diameter = 14;
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
                shape_c1.Diamter = data.dai_diameter2;
                shape_c1.Spacing = data.spacing2;
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

        public void DrawBeam3(BeamData beam_data, AcadGeo.Point3d ins_pnt)
        {
            Beam beam = new Beam();
            beam.Width = beam_data.Width;
            beam.Height = beam_data.Height;
            beam.Length = beam_data.Length;

            List<AcadDB.Entity> ents = Beam3a(ins_pnt, beam, beam_data.dau_goi_data);

            ins_pnt = ins_pnt + AcadGeo.Vector3d.XAxis * 2000.0;
            ents.AddRange(Beam3b(ins_pnt, beam, beam_data.giua_data));

            foreach (var ent in ents)
            {
                AcadFuncsCSharp.AcadFuncs.AddNewEnt(ent);
            }
        }

    }
}
