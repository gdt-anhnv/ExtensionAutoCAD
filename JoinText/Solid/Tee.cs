using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid.Solid
{
	class Tee : SolidEntity
	{
		private AcadGeo.Point3d position;
		public AcadGeo.Point3d Position
		{
			get { return position; }
			set { position = value; }
		}

		private AcadGeo.Point3d main_branch_position;
		public AcadGeo.Point3d MainBranchPosition
		{
			get { return main_branch_position; }
			set { main_branch_position = value; }
		}

		private AcadGeo.Point3d branch_position;
		public AcadGeo.Point3d BranchPosition
		{
			get { return branch_position; }
			set { branch_position = value; }
		}

		private double radius;
		public double Radius
		{
			get { return radius; }
			set { radius = value; }
		}

		public Tee() : base()
		{
			position = new AcadGeo.Point3d();
			main_branch_position = new AcadGeo.Point3d();
			branch_position = new AcadGeo.Point3d();
			radius = 0.0;
		}

		private AcadGeo.Point3d CalculateCenter()
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(
				CalculateBranchPos(position, main_branch_position, radius),
				CalculateBranchPos(position, branch_position, radius));
			AcadGeo.Point3d pos = position + AcadFuncs.GetVec(main_branch_position, position) * radius;
			AcadGeo.Plane plane = new AcadGeo.Plane(pos, NormalVec().CrossProduct(vec));

			return position.Mirror(plane);
		}

		private static AcadGeo.Point3d CalculateBranchPos(AcadGeo.Point3d pos, AcadGeo.Point3d tmp_bp, double radius)
		{
			return pos + AcadFuncs.GetVec(tmp_bp, pos) * radius;
		}

		private AcadGeo.Vector3d NormalVec()
		{
			AcadGeo.Vector3d v1 = AcadFuncs.GetVec(position, main_branch_position);

			AcadGeo.Vector3d v2 = AcadFuncs.GetVec(branch_position, position);

			return v1.CrossProduct(v2).GetNormal();
		}

		private double GetStartAngle(AcadGeo.Point3d pos, AcadGeo.Point3d center, AcadGeo.Vector3d ref_vec, AcadGeo.Vector3d up_vec)
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(CalculateBranchPos(pos, main_branch_position, radius), center);
			return ref_vec.GetAngleTo(vec, up_vec);
		}
		private double GetEndAngle(AcadGeo.Point3d pos, AcadGeo.Point3d center, AcadGeo.Vector3d ref_vec, AcadGeo.Vector3d up_vec)
		{
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(CalculateBranchPos(pos, branch_position, radius), center);

			return ref_vec.GetAngleTo(vec, up_vec);
		}

		protected override List<PathInfo> GetSweptPath()
		{
			AcadGeo.Point3d center = CalculateCenter();
			AcadGeo.Vector3d ref_vec = AcadFuncs.GetVec(CalculateBranchPos(position, main_branch_position, radius), center);
			AcadGeo.CircularArc3d arc = new AcadGeo.CircularArc3d(
				center, NormalVec(), ref_vec, radius,
				GetStartAngle(position, center, ref_vec, NormalVec()), GetEndAngle(position, center, ref_vec, NormalVec()));

			//AcadFuncs.AddNewEnt(AcadDB.Arc.CreateFromGeCurve(arc));

			List<PathInfo> ents = new List<PathInfo>();
			AcadGeo.Point3d bp = CalculateBranchPos(position,
				profile.NormalVector.GetNormal().IsParallelTo(ref_vec) ? branch_position : main_branch_position,
				radius);

			profile.BasePoint = new AcadGeo.Point3d(bp.X, bp.Y, bp.Z);
			ents.Add(new PathInfo(profile.GetRegions().First(), profile.GetSubRegions().First(), AcadDB.Arc.CreateFromGeCurve(arc), bp));

			{
				AcadGeo.Plane plane = new AcadGeo.Plane(position, AcadFuncs.GetVec(position, branch_position));
				AcadGeo.CircularArc3d tmp_arc = new AcadGeo.CircularArc3d(
				arc.Center, arc.Normal, arc.ReferenceVector, arc.Radius, arc.StartAngle, arc.EndAngle);
				tmp_arc.Mirror(plane);

				AcadGeo.Point3d tmp_bp = new AcadGeo.Point3d(bp.X, bp.Y, bp.Z);
				if (tmp_bp.IsEqualTo(CalculateBranchPos(position, branch_position, radius)))
				{
					tmp_bp += AcadFuncs.GetVec(position, branch_position) * radius * 2.0;
					profile.BasePoint = tmp_bp;
				}
				//AcadFuncs.AddNewEnt(AcadDB.Arc.CreateFromGeCurve(tmp_arc));

				ents.Add(new PathInfo(profile.GetRegions().First(), profile.GetSubRegions().First(), AcadDB.Arc.CreateFromGeCurve(tmp_arc), tmp_bp));
			}

			{
				AcadGeo.Point3d tmp_pos = CalculateBranchPos(position, branch_position, radius);
				AcadGeo.Point3d tmp_end_pos = tmp_pos + AcadFuncs.GetVec(position, branch_position) * radius * 2.0;

				AcadGeo.Point3d tmp_bp = new AcadGeo.Point3d(bp.X, bp.Y, bp.Z);
				if (tmp_bp.IsEqualTo(CalculateBranchPos(position, branch_position, radius)))
				{
					profile.BasePoint = CalculateBranchPos(position, branch_position, radius);
				}
				else
				{
					profile.BasePoint = new AcadGeo.Point3d(position.X, position.Y, position.Z);
					profile.Rotate(AcadFuncs.GetVec(position, branch_position));
					profile.BasePoint = CalculateBranchPos(position, branch_position, radius);
				}

				ents.Add(new PathInfo(profile.GetRegions().First(), profile.GetSubRegions().First(), new AcadDB.Line(tmp_pos, tmp_end_pos), tmp_bp));
			}


			return ents;
		}
	}
}
