using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickGraph;

using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.DatabaseServices;

namespace RoutingSolid.Entity
{
	class Model
	{
		private BidirectionalGraph<Node, Connection> routers;

		public void Parse(AcadApp.Curve curve)
		{
			if (null != curve)
			{
				ParseCurve(curve);
			}
		}


		private bool IsVertex(AcadApp.Curve curve, AcadGeo.Point3d pnt)
		{
			if (pnt.Equals(curve.StartPoint) || pnt.Equals(curve.EndPoint))
				return true;

			return false;
		}

		private AcadGeo.Point3d GetAnotherVertex(AcadApp.Curve curve, AcadGeo.Point3d pnt)
		{
			if (pnt.Equals(curve.StartPoint))
				return curve.EndPoint;
			if (pnt.Equals(curve.EndPoint))
				return curve.StartPoint;

			return AcadGeo.Point3d.Origin;
		}

		private Node FindNode(AcadGeo.Point3d pnt)
		{
			List<Node> nodes = routers.Vertices.ToList<Node>();

			foreach (Node node in nodes)
			{
				if (pnt.Equals(node.Vertex))
					return node;
			}
			return null;
		}

		

		private void ParseCurve(AcadApp.Curve newCurve)
		{
			List<Connection> connections = routers.Edges.ToList();

			foreach (Connection connection in connections)
			{
				AcadApp.Curve currentEdge = connection.Curve;
				//AcadDatabase.Line edgeLine = cur as AcadDatabase.Line;
				
					AcadGeo.Point3dCollection pnts = new AcadGeo.Point3dCollection();
					currentEdge.IntersectWith(newCurve, AcadApp.Intersect.OnBothOperands, pnts, 0, 0);
					if (0 != pnts.Count)
					{

						if (IsVertex(newCurve, pnts[0]) && IsVertex(currentEdge, pnts[0]))  //link
						{
							AcadGeo.Point3d anotherVertex = GetAnotherVertex(newCurve, pnts[0]);
							Node intersectNode = FindNode(pnts[0]);
							Node newNode = new Node(anotherVertex);

							if (AcadGeo.Point3d.Origin != anotherVertex && null != intersectNode)
							{
								routers.AddVertex(newNode);
								routers.AddEdge(new Connection(intersectNode, newNode));
							}
						}
						else
						if (IsVertex(newCurve, pnts[0]))
						{
							routers.RemoveEdge(connection);

							AcadApp.Curve tmp_cur = connection.Curve;
							tmp_cur.GetSplitCurves(pnts);

						}
						else
							if (IsVertex(currentEdge, pnts[0])) //
						{

						}
						else //piercing
						{

						}

				}

			}

			//Node n1 = new Node(line.StartPoint);
			//Node n2 = new Node(line.EndPoint);

			////is line intersected with anyone???
			////checkout the routers having a node at "position"???
			//routers.AddVertex(n1);
			//routers.AddVertex(n2);

			//Connection conn = new Connection(n1, n2);
			//routers.AddEdge(conn);
		}
	}
}
