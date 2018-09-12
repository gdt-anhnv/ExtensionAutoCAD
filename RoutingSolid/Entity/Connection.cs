using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickGraph;

using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid.Entity
{
    class Connection : Edge<Node>
    {
		public Connection(Node n1, Node n2) : base(n1, n2)
		{

		}
	}
}
