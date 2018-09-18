using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickGraph;

using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.DatabaseServices;

namespace RoutingSolid
{
    class Connection : Edge<Node>
    {
		private AcadApp.Curve curve;
		public Connection(Node n1, Node n2) : base(n1, n2)
		{

		}

		public AcadGeo.Point3d ConnectedPos(AcadGeo.Point3d pos)
		{
			if (Source.Position.IsEqualTo(pos))
				return Target.Position;
			else
				return Source.Position;
		}
	}
}
