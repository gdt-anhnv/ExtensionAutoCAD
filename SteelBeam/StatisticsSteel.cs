using Autodesk.AutoCAD.Runtime;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace SteelBeam
{
    public class StatisticsSteelType1
    {
        private static string ATT_SH = "SH";
        private static string ATT_DK = "DK";
        private static string ATT_DAI = "DAI";
        private static string ATT_SL1 = "SL1";
        private static string ATT_SLA = "SLA";
        private static string ATT_DT = "DT";
        private static string ATT_TL = "TL";
        private static string ATT_L1 = "L1";
        private static string ATT_L2 = "L2";
        private static string ATT_L3 = "L3";
        private static string ATT_M = "M";

        public static void StatisticsModel123_Steel1(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "1";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }

        public static void StatisticsModel123_Steel2(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "2";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = "2000";
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }

        public static void StatisticsModel123_Steel3(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "3";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L1 == att_ref.Tag || ATT_L3 == att_ref.Tag)
                        att_ref.TextString = (beam.Height - 2 * Beam.cover).ToString();
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }

        public static void StatisticsModel123_Steel4(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "4";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = "2000";
                    if (ATT_L1 == att_ref.Tag)
                        att_ref.TextString = (beam.Height - 2 * Beam.cover).ToString();
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }

        public static void StatisticsModel123_Steel5(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "5";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = (beam.Width - 2 * Beam.cover).ToString();
                    if (ATT_M == att_ref.Tag)
                        att_ref.TextString = "60";
                    if (ATT_L1 == att_ref.Tag)
                        att_ref.TextString = (beam.Height - 2 * Beam.cover).ToString();
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }

        public static void StatisticsModel2_Steel6(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "6";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = (beam.Width - 2 * Beam.cover).ToString();
                    if (ATT_M == att_ref.Tag)
                        att_ref.TextString = "60";
                    if (ATT_L1 == att_ref.Tag)
                        att_ref.TextString = (beam.Height - 2 * Beam.cover).ToString();
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }

        public static void StatisticsModel2_Steel7(BeamData beam, AcadDB.BlockReference blk_ref)
        {
            AcadDB.Database db = AcadApp.DocumentManager.MdiActiveDocument.Database;
            using (AcadDB.Transaction trans = db.TransactionManager.StartTransaction())
            {
                AcadDB.AttributeCollection atts = blk_ref.AttributeCollection;
                foreach (AcadDB.ObjectId att in atts)
                {
                    AcadDB.AttributeReference att_ref = trans.GetObject(att, AcadDB.OpenMode.ForWrite) as AcadDB.AttributeReference;
                    if (null == att_ref)
                        continue;

                    if (ATT_SH == att_ref.Tag)
                        att_ref.TextString = "7";
                    if (ATT_DK == att_ref.Tag)
                        att_ref.TextString = beam.dau_goi_data.first_layer_diameter.ToString();
                    if (ATT_DAI == att_ref.Tag)
                        att_ref.TextString = (beam.Length - 2 * Beam.cover).ToString();
                    if (ATT_L2 == att_ref.Tag)
                        att_ref.TextString = (beam.Width - 2 * Beam.cover).ToString();
                    if (ATT_M == att_ref.Tag)
                        att_ref.TextString = "60";
                    if (ATT_L1 == att_ref.Tag)
                        att_ref.TextString = (beam.Height - 2 * Beam.cover).ToString();
                    if (ATT_SL1 == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_SLA == att_ref.Tag)
                        att_ref.TextString = (beam.dau_goi_data.number_steel_first_layer).ToString();
                    if (ATT_DT == att_ref.Tag)
                    {
                        double total_len = beam.dau_goi_data.number_steel_first_layer * (beam.Length - 2 * Beam.cover);
                        total_len /= 1000.0;
                        att_ref.TextString = (total_len).ToString();
                    }
                    if (ATT_TL == att_ref.Tag)
                        att_ref.TextString = "...";
                }

                trans.Commit();
            }
        }
    }
}
