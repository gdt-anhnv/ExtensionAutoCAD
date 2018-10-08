using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadEd = Autodesk.AutoCAD.EditorInput;
using AcadServices = Autodesk.AutoCAD.ApplicationServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid
{
	class AcadFuncs
	{
		public static double kPI = 3.14159265359;

		public static AcadDB.Database GetActiveDB()
		{
			return AcadApp.DocumentManager.MdiActiveDocument.Database;
		}

		public static AcadEd.Editor GetEditor()
		{
			return AcadApp.DocumentManager.MdiActiveDocument.Editor;
		}

		static public AcadServices.Document GetActiveDoc()
		{
			return AcadApp.DocumentManager.MdiActiveDocument;
		}

		static AcadDB.BlockTableRecord GetModelSpace(AcadDB.Transaction tr)
		{
			return tr.GetObject(GetBlkTbl(tr)[AcadDB.BlockTableRecord.ModelSpace], AcadDB.OpenMode.ForRead) as AcadDB.BlockTableRecord;
		}

		static AcadDB.BlockTable GetBlkTbl(AcadDB.Transaction tr)
		{
			return tr.GetObject(GetActiveDB().BlockTableId, AcadDB.OpenMode.ForRead) as AcadDB.BlockTable;
		}

		static public void AddNewEnt(AcadDB.Entity ent)
		{
			AcadDB.Database db = GetActiveDB();

			using (AcadDB.Transaction tr = db.TransactionManager.StartTransaction())
			{
				AcadDB.BlockTableRecord model_space = GetModelSpace(tr);
				if (null == model_space)
					return;

				model_space.UpgradeOpen();
				model_space.AppendEntity(ent);

				tr.AddNewlyCreatedDBObject(ent, true);

				tr.Commit();
			}
		}

		public static List<AcadDB.ObjectId> PickEnts()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (AcadDB.Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					AcadEd.PromptSelectionResult prmpt_ret = AcadFuncs.GetEditor().GetSelection();
					if (AcadEd.PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return new List<AcadDB.ObjectId>();
					}
					tr.Commit();

					return prmpt_ret.Value.GetObjectIds().ToList();
				}
			}
		}

		public static bool GetDouble(ref double val, string message)
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (AcadDB.Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					AcadEd.PromptDoubleOptions prmpt_pnt = new AcadEd.PromptDoubleOptions(message);
					AcadEd.PromptDoubleResult prmpt_ret = AcadFuncs.GetEditor().GetDouble(prmpt_pnt);
					if (AcadEd.PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return false;
					}

					val = prmpt_ret.Value;
					tr.Commit();
				}
			}

			return true;
		}

		public static bool GetPoint(ref AcadGeo.Point3d val, string message)
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (AcadDB.Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					AcadEd.PromptPointOptions prmpt_pnt = new AcadEd.PromptPointOptions(message);
					AcadEd.PromptPointResult prmpt_ret = AcadFuncs.GetEditor().GetPoint(prmpt_pnt);
					if (AcadEd.PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return false;
					}

					val = prmpt_ret.Value;
					tr.Commit();
				}
			}

			return true;
		}

		public static bool GetPoint(ref AcadGeo.Point3d val, string message, AcadGeo.Point3d bp)
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (AcadDB.Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					AcadEd.PromptPointOptions prmpt_pnt = new AcadEd.PromptPointOptions(message);
					prmpt_pnt.UseBasePoint = true;
					prmpt_pnt.BasePoint = bp;
					AcadEd.PromptPointResult prmpt_ret = AcadFuncs.GetEditor().GetPoint(prmpt_pnt);
					if (AcadEd.PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return false;
					}

					val = prmpt_ret.Value;
					tr.Commit();
				}
			}

			return true;
		}

		public static AcadGeo.Vector3d GetVec(AcadGeo.Point3d pos1, AcadGeo.Point3d pos2)
		{
			return new AcadGeo.Vector3d(
				pos1.X - pos2.X,
				pos1.Y - pos2.Y,
				pos1.Z - pos2.Z).GetNormal();
		}

		public static AcadGeo.Point3d GetMidPoint(AcadGeo.Point3d pos1, AcadGeo.Point3d pos2)
		{
			return new AcadGeo.Point3d(
				0.5 * (pos1.X + pos2.X),
				0.5 * (pos1.Y + pos2.Y),
				0.5 * (pos1.Z + pos2.Z));
		}

	}
}
