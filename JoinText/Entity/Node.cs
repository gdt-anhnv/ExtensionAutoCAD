using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcadGeo = Autodesk.AutoCAD.Geometry;

namespace RoutingSolid
{
	class Node
	{
		private AcadGeo.Point3d position;
		private List<Connection> connections;

		public Node(AcadGeo.Point3d ver)
		{
			position = ver;
			connections = new List<Connection>();
		}

		//get set Vertex
		public AcadGeo.Point3d Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
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

		public Node GetConnection(Connection conn)
		{
			if (conn.Source == this)
				return conn.Target;
			else
				return conn.Source;
		}

	}
}
