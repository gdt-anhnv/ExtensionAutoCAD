using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;

namespace RoutingSolid.Solid
{
	class Profile
	{
		private AcadGeo.Point3d base_point;
		public AcadGeo.Point3d BasePoint
		{
			get { return base_point; }
			set { base_point = value; }
		}
		private double width;
		public double Width
		{
			get { return width; }
			set { width = value; }
		}

		private double height;
		public double Height
		{
			get { return height; }
			set { height = value; }
		}

		private double thickness;
		public double Thickness
		{
			set { thickness = value; }
		}

		private AcadGeo.Vector3d up_vector;
		public AcadGeo.Vector3d UpVector
		{
			get { return up_vector; }
			set { up_vector = value; }
		}
		private AcadGeo.Vector3d normal_vector;
		public AcadGeo.Vector3d NormalVector
		{
			get { return normal_vector; }
			set { normal_vector = value; }
		}

		public Profile()
		{
			base_point = new AcadGeo.Point3d();
			width = 0.0;
			height = 0.0;
			thickness = 0.0;
			up_vector = new AcadGeo.Vector3d();
			normal_vector = new AcadGeo.Vector3d();
		}

		public Profile(Profile p)
		{
			base_point = p.base_point;
			width = p.width;
			height = p.height;
			thickness = p.thickness;
			up_vector = p.up_vector;
			normal_vector = p.normal_vector;
		}

		public void Translate(AcadGeo.Vector3d vec)
		{
			base_point += vec;
		}

		public void Rotate(AcadGeo.Vector3d vec)
		{
			AcadGeo.Vector3d axe = vec.CrossProduct(normal_vector);
			if (axe.IsEqualTo(new AcadGeo.Vector3d(0.0, 0.0, 0.0)))
				return;

			double angle = normal_vector.GetAngleTo(vec, axe);
			up_vector = up_vector.RotateBy(angle, axe);
			normal_vector = normal_vector.RotateBy(angle, axe);
		}

		public List<AcadDB.Region> GetRegions()
		{
			List<AcadDB.Region> regions = new List<AcadDB.Region>();

			AcadGeo.Vector3d tmp_vec = up_vector.CrossProduct(normal_vector);

			AcadDB.DBObjectCollection objs = new AcadDB.DBObjectCollection();

			AcadGeo.Vector3d hf = up_vector * height * 0.5;
			AcadGeo.Vector3d wf = tmp_vec * width * 0.5;
			objs.Add(new AcadDB.Line(base_point + hf + wf, base_point + hf - wf));
			objs.Add(new AcadDB.Line(base_point + hf - wf, base_point - hf - wf));
			objs.Add(new AcadDB.Line(base_point - hf - wf, base_point - hf + wf));
			objs.Add(new AcadDB.Line(base_point - hf + wf, base_point + hf + wf));

			AcadDB.DBObjectCollection regs = AcadDB.Region.CreateFromCurves(objs);
			foreach(var reg in regs)
			{
				if (reg is AcadDB.Region)
					regions.Add(reg as AcadDB.Region);
			}

			return regions;
		}

		public List<AcadDB.Region> GetSubRegions()
		{
			List<AcadDB.Region> regions = new List<AcadDB.Region>();

			AcadGeo.Vector3d tmp_vec = up_vector.CrossProduct(normal_vector);

			AcadDB.DBObjectCollection objs = new AcadDB.DBObjectCollection();

			AcadGeo.Vector3d hf = up_vector * height * 0.5;
			AcadGeo.Vector3d wf = tmp_vec * (width * 0.5 - thickness);
			AcadGeo.Vector3d tf = up_vector * thickness;
			objs.Add(new AcadDB.Line(base_point + hf + wf, base_point + hf - wf));
			objs.Add(new AcadDB.Line(base_point + hf - wf, base_point - hf - wf + tf));
			objs.Add(new AcadDB.Line(base_point - hf - wf + tf, base_point - hf + wf + tf));
			objs.Add(new AcadDB.Line(base_point - hf + wf + tf, base_point + hf + wf));

			{
				AcadDB.DBObjectCollection regs = AcadDB.Region.CreateFromCurves(objs);

				foreach (var reg in regs)
				{
					if (reg is AcadDB.Region)
						regions.Add(reg as AcadDB.Region);
				}
			}

			return regions;
		}
	}
}