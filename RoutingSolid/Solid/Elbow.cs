using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid.Solid
{
	class Elbow : SolidEntity
	{
		private AcadGeo.Point3d position;
		public AcadGeo.Point3d Position
		{
			get { return position; }
			set { position = value; }
		}
		private AcadGeo.Point3d[] branch_positions;
		public AcadGeo.Point3d[] BranchPos
		{
			get { return branch_positions; }
			set { branch_positions = value; }
		}
		private double radius;
		public double Radius
		{
			get { return radius; }
			set { radius = value; }
		}

		public Elbow() : base()
		{
			position = new AcadGeo.Point3d();
			branch_positions = new AcadGeo.Point3d[2];
			radius = 0.0;
		}

		private AcadGeo.Point3d CalculateCenter()
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(
				CalculateBranchPos(position, branch_positions[0], radius),
				CalculateBranchPos(position, branch_positions[1], radius));
			AcadGeo.Point3d pos = position + AcadFuncs.GetVec(branch_positions[0], position) * radius;
			AcadGeo.Plane plane = new AcadGeo.Plane(pos, NormalVec().CrossProduct(vec));

			return position.Mirror(plane);
		}

		private static AcadGeo.Point3d CalculateBranchPos(AcadGeo.Point3d pos, AcadGeo.Point3d tmp_bp, double radius)
		{
			return pos + AcadFuncs.GetVec(tmp_bp, pos) * radius;
		}

		private AcadGeo.Vector3d NormalVec()
		{
			AcadGeo.Vector3d v1 = AcadFuncs.GetVec(position, branch_positions[0]);

			AcadGeo.Vector3d v2 = AcadFuncs.GetVec(branch_positions[1], position);

			return v1.CrossProduct(v2).GetNormal();
		}

		private double GetStartAngle(AcadGeo.Point3d pos, AcadGeo.Point3d center, AcadGeo.Vector3d ref_vec, AcadGeo.Vector3d up_vec)
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(CalculateBranchPos(pos, branch_positions[0], radius), center);
			return ref_vec.GetAngleTo(vec, up_vec);
		}
		private double GetEndAngle(AcadGeo.Point3d pos, AcadGeo.Point3d center, AcadGeo.Vector3d ref_vec, AcadGeo.Vector3d up_vec)
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(CalculateBranchPos(pos, branch_positions[1], radius), center);

			return ref_vec.GetAngleTo(vec, up_vec);
		}

		protected override List<PathInfo> GetSweptPath()
		{
			AcadGeo.Point3d center = CalculateCenter();
			AcadGeo.Vector3d ref_vec = AcadFuncs.GetVec(CalculateBranchPos(position, branch_positions[0], radius), center);
			AcadGeo.CircularArc3d arc = new AcadGeo.CircularArc3d(
				center, NormalVec(), ref_vec, radius,
				GetStartAngle(position, center, ref_vec, NormalVec()), GetEndAngle(position, center, ref_vec, NormalVec()));

			//AcadFuncs.AddNewEnt(AcadDB.Arc.CreateFromGeCurve(arc));

			List<PathInfo> ents = new List<PathInfo>();
			AcadGeo.Point3d bp = CalculateBranchPos(position,
				profile.NormalVector.GetNormal().IsParallelTo(ref_vec) ? branch_positions[1] : branch_positions[0],
				radius);

			profile.BasePoint = bp;
			ents.Add(new PathInfo(profile.GetRegions().First(), profile.GetSubRegions().First(), AcadDB.Arc.CreateFromGeCurve(arc), bp));
			return ents;
		}
	}
}
