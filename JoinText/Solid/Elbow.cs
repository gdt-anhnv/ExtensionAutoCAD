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

		private double angle;
		public double Angle
		{
			get { return angle; }
			set { radius = value; }
		}

		public Elbow() : base()
		{
			position = new AcadGeo.Point3d();
			branch_positions = new AcadGeo.Point3d[2];
			radius = 0.0;
			angle = AcadFuncs.kPI * 0.5;
		}

		public void CalculateAngle()
		{
			AcadGeo.Vector3d vec1 = AcadFuncs.GetVec(branch_positions[0], position);
			AcadGeo.Vector3d vec2 = AcadFuncs.GetVec(branch_positions[1], position);

			if (vec1.IsParallelTo(vec2))
				throw new Exception("Failed parse elbow");
			angle = vec1.GetAngleTo(vec2);
		}

		private AcadGeo.Point3d CalculateCenter()
		{
			//AcadGeo.Vector3d vec = AcadFuncs.GetVec(
			//	CalculateBranchPos(position, branch_positions[0], radius, angle),
			//	CalculateBranchPos(position, branch_positions[1], radius, angle));
			//AcadGeo.Point3d pos = position + AcadFuncs.GetVec(branch_positions[0], position) * (radius / Math.Sin(angle * 0.5));
			//AcadGeo.Plane plane = new AcadGeo.Plane(pos, NormalVec().CrossProduct(vec));

			//return position.Mirror(plane);
			AcadGeo.Vector3d vec1 = AcadFuncs.GetVec(branch_positions[0], position);
			AcadGeo.Vector3d vec2 = AcadFuncs.GetVec(branch_positions[1], position);
			AcadGeo.Vector3d tmp_vec = new AcadGeo.Vector3d(
				vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
			tmp_vec = tmp_vec.GetNormal();
			return position + tmp_vec * (radius / Math.Sin(angle * 0.5));
		}

		private static AcadGeo.Point3d CalculateBranchPos(AcadGeo.Point3d pos, AcadGeo.Point3d tmp_bp, double radius, double angle)
		{
			double len = radius / Math.Tan(angle * 0.5);
			return pos + AcadFuncs.GetVec(tmp_bp, pos) * len;
		}

		private AcadGeo.Vector3d NormalVec()
		{
			AcadGeo.Vector3d v1 = AcadFuncs.GetVec(position, branch_positions[0]);

			AcadGeo.Vector3d v2 = AcadFuncs.GetVec(branch_positions[1], position);

			return v1.CrossProduct(v2).GetNormal();
		}

		private double GetStartAngle(AcadGeo.Point3d pos, AcadGeo.Point3d center, AcadGeo.Vector3d ref_vec, AcadGeo.Vector3d up_vec)
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(CalculateBranchPos(pos, branch_positions[0], radius, angle), center);
			return ref_vec.GetAngleTo(vec, up_vec);
		}
		private double GetEndAngle(AcadGeo.Point3d pos, AcadGeo.Point3d center, AcadGeo.Vector3d ref_vec, AcadGeo.Vector3d up_vec)
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(CalculateBranchPos(pos, branch_positions[1], radius, angle), center);

			return ref_vec.GetAngleTo(vec, up_vec);
		}

		protected override List<PathInfo> GetSweptPath()
		{
			AcadGeo.Point3d center = CalculateCenter();
			AcadGeo.Vector3d ref_vec = AcadFuncs.GetVec(CalculateBranchPos(position, branch_positions[0], radius, angle), center);
			AcadGeo.CircularArc3d arc = new AcadGeo.CircularArc3d(
				center, NormalVec(), ref_vec, radius,
				GetStartAngle(position, center, ref_vec, NormalVec()), GetEndAngle(position, center, ref_vec, NormalVec()));

			AcadFuncs.AddNewEnt(AcadDB.Arc.CreateFromGeCurve(arc));

			List<PathInfo> ents = new List<PathInfo>();
			AcadGeo.Point3d bp = CalculateBranchPos(position,
				profile.NormalVector.GetNormal().IsParallelTo(AcadFuncs.GetVec(position, branch_positions[1])) ? branch_positions[1] : branch_positions[0],
				radius, angle);

			profile.BasePoint = bp;
			ents.Add(new PathInfo(profile.GetRegions().First(), profile.GetSubRegions().First(), AcadDB.Arc.CreateFromGeCurve(arc), bp));

			//{
			//	var regs = profile.GetRegions();
			//	AcadFuncs.AddNewEnt(regs[0]);
			//}

			return ents;
		}
	}
}
