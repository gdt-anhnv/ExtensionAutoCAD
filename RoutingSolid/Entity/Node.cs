using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid.Entity
{
	class Node
	{
		private AcadGeo.Point3d vertex;
		private List<Connection> connections;

		public Node(AcadGeo.Point3d ver)
		{
			vertex = ver;
			connections = new List<Connection>();
		}

		//get set Vertex
		public AcadGeo.Point3d Vertex
		{
			get
			{
				return vertex;
			}
			set
			{
				vertex = value;
			}
		}

		public List<Connection> Connections
		{
			get
			{
				return connections;
			}
			set
			{
				connections = value;
			}
		}

	}
}
