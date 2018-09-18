using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid.Solid
{
	abstract class SolidEntity
	{
		protected Profile profile;
		public Profile Profile
		{
			set { profile = value; }
		}

		public SolidEntity()
		{
			profile = null;
		}

		public void Draw()
		{
			List<PathInfo> swept_paths = GetSweptPath();

			AcadDB.SweepOptionsBuilder swept_options = new AcadDB.SweepOptionsBuilder();
			swept_options.Align = AcadDB.SweepOptionsAlignOption.NoAlignment;

			using (AcadDB.Transaction tr = AcadFuncs.GetActiveDB().TransactionManager.StartTransaction())
			{
				AcadDB.Solid3d union_sol3d = null;
				AcadDB.Solid3d union_sub_sol3d = null;
				foreach (var pi in swept_paths)
				{
					if (null != pi.sub_region)
					{
						AcadDB.Solid3d solid = new AcadDB.Solid3d();
						swept_options.BasePoint = pi.start_position;
						solid.CreateSweptSolid(pi.sub_region, pi.path, swept_options.ToSweepOptions());
						if (null == union_sub_sol3d)
							union_sub_sol3d = solid;
						else
							union_sub_sol3d.BooleanOperation(AcadDB.BooleanOperationType.BoolUnite, solid);
					}

					if (null != pi.region)
					{
						AcadDB.Solid3d solid = new AcadDB.Solid3d();
						swept_options.BasePoint = pi.start_position;
						solid.CreateSweptSolid(pi.region, pi.path, swept_options.ToSweepOptions());
						if (null == union_sol3d)
							union_sol3d = solid;
						else
							union_sol3d.BooleanOperation(AcadDB.BooleanOperationType.BoolUnite, solid);
					}
				}

				union_sol3d.BooleanOperation(AcadDB.BooleanOperationType.BoolSubtract, union_sub_sol3d);

				if (!(this is Straight))
					union_sol3d.ColorIndex = 1;

				AcadFuncs.AddNewEnt(union_sol3d);

				tr.Commit();
			}
		}

		protected abstract List<PathInfo> GetSweptPath();
	}

	class PathInfo
	{
		public AcadDB.Entity region;
		public AcadDB.Entity sub_region;
		public AcadDB.Entity path;
		public AcadGeo.Point3d start_position;

		public PathInfo(AcadDB.Entity r, AcadDB.Entity sr, AcadDB.Entity p, AcadGeo.Point3d sp)
		{
			region = r;
			sub_region = sr;
			path = p;
			start_position = sp;
		}
	}
}
