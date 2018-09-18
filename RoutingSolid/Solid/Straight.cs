using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid.Solid
{
	class Straight : SolidEntity
	{
		protected AcadGeo.Point3d start_position;
		public AcadGeo.Point3d StartPosition
		{
			get { return start_position; }
			set { start_position = value; }
		}
		private AcadGeo.Point3d end_position;
		public AcadGeo.Point3d EndPosition
		{
			get { return end_position; }
			set { end_position = value; }
		}

		public Straight() :
			base()
		{
			start_position = new AcadGeo.Point3d();
			end_position = new AcadGeo.Point3d();
		}

		protected override List<PathInfo> GetSweptPath()
		{
			AcadDB.Line line = new AcadDB.Line(start_position, end_position);

			List<PathInfo> ents = new List<PathInfo>();
			profile.BasePoint = StartPosition;
			ents.Add(new PathInfo(profile.GetRegions().First(), profile.GetSubRegions().First(), line, profile.BasePoint));

			return ents;
		}
	}
}
