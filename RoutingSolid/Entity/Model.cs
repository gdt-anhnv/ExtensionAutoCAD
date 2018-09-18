using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickGraph;

using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using QuickGraph.Algorithms;

namespace RoutingSolid
{
	class Model
	{
		private int ELBOW_CONNS = 2;
		private int TEE_CONNS = 3;
		private int CROSSING_CONNS = 4;

		private UndirectedGraph<Node, Connection> routers;
		private List<Solid.SolidEntity> solids;
		private Solid.Profile profile;

		private double width;
		public double Width
		{
			set { width = value; }
		}

		private double height;
		public double Height
		{
			set { height = value; }
		}

		private double thickness;
		public double Thickness
		{
			set { thickness = value; }
		}

		public Model()
		{
			routers = new UndirectedGraph<Node, Connection>();
			solids = new List<Solid.SolidEntity>();
			profile = new Solid.Profile();

			width = 0.0;
			height = 0.0;
		}

		public void RoutingSolid()
		{
			foreach (var solid in solids)
			{
				try
				{
					solid.Draw();
				}
				catch(Autodesk.AutoCAD.Runtime.Exception ex)
				{
					string mess = ex.Message;
				}
			}
		}

		private Solid.Straight BuildStraight(Node sn, Node en)
		{
			double length = sn.Position.DistanceTo(en.Position);
			AcadGeo.Vector3d vec = AcadFuncs.GetVec(sn.Position, en.Position);
			Solid.Straight straight = new Solid.Straight();
			if (sn.Connections.Count > 1)
			{
				straight.StartPosition = sn.Position - vec * width;
				length -= width;
			}
			else
				straight.StartPosition = sn.Position;

			if (en.Connections.Count > 1)
				straight.EndPosition = en.Position + vec * width;
			else
				straight.EndPosition = en.Position;

			if (length > 0.0)
				return straight;
			else
				return null;
		}

		public void BuildRoutingSolid()
		{
			List<Node> nodes = routers.Vertices.ToList();
			foreach(var n in nodes.ToList())
			{
				nodes.Remove(n);
				if (n.Connections.Count > 4)
					continue;

				foreach(var conn in n.Connections)
				{
					Node sn = n.GetConnection(conn);
					if (null != nodes.Find(x => x == sn))
					{
						Solid.Straight solid = BuildStraight(n, sn);
						if (null != solid)
						{
							Solid.Profile cal_prol = CalculateProfile(n.Position);
							cal_prol.Rotate(AcadFuncs.GetVec(sn.Position, n.Position));
							solid.Profile = cal_prol;
							solids.Add(solid);
						}
					}
				}

				if(ELBOW_CONNS == n.Connections.Count)
				{
					Solid.Elbow elbow = new Solid.Elbow();
					elbow.Position = n.Position;
					AcadGeo.Point3d[] bps = new AcadGeo.Point3d[2];
					bps[0] = n.Connections[0].Source.Position == n.Position ?
						n.Connections[0].Target.Position : n.Connections[0].Source.Position;
					bps[1] = n.Connections[1].Source.Position == n.Position ?
						n.Connections[1].Target.Position : n.Connections[1].Source.Position;
					elbow.BranchPos = bps;
					elbow.Profile = CalculateProfile(n.Position);
					elbow.Radius = width;
					solids.Add(elbow);
				}
				else if(TEE_CONNS == n.Connections.Count)
				{
					Solid.Tee tee = new Solid.Tee();
					tee.Position = n.Position;
					tee.MainBranchPosition = GetMainTeePos(n.Position,
						n.Connections[0].ConnectedPos(n.Position),
						n.Connections[1].ConnectedPos(n.Position),
						n.Connections[2].ConnectedPos(n.Position));
					tee.BranchPosition = GetBranchTeePos(n.Position,
						n.Connections[0].ConnectedPos(n.Position),
						n.Connections[1].ConnectedPos(n.Position),
						n.Connections[2].ConnectedPos(n.Position));
					tee.Profile = CalculateProfile(n.Position);
					tee.Radius = width;
					solids.Add(tee);
				}
				else if(CROSSING_CONNS == n.Connections.Count)
				{
					Solid.Crossing crossing = new Solid.Crossing();
					crossing.Position = n.Position;
					crossing.MainBranchPosition = GetMainTeePos(n.Position,
						n.Connections[0].ConnectedPos(n.Position),
						n.Connections[1].ConnectedPos(n.Position),
						n.Connections[2].ConnectedPos(n.Position));
					crossing.BranchPosition = GetBranchTeePos(n.Position,
						n.Connections[0].ConnectedPos(n.Position),
						n.Connections[1].ConnectedPos(n.Position),
						n.Connections[2].ConnectedPos(n.Position));
					crossing.Profile = CalculateProfile(n.Position);
					crossing.Radius = width;
					solids.Add(crossing);

				}
			}
		}

		private AcadGeo.Point3d GetMainTeePos(AcadGeo.Point3d pos, AcadGeo.Point3d pos1, AcadGeo.Point3d pos2, AcadGeo.Point3d pos3)
		{
			AcadGeo.Vector3d vec1 = AcadFuncs.GetVec(pos, pos1);
			AcadGeo.Vector3d vec2 = AcadFuncs.GetVec(pos, pos2);
			AcadGeo.Vector3d vec3 = AcadFuncs.GetVec(pos, pos3);

			if (vec1.IsParallelTo(vec2))
				return pos3;
			if (vec1.IsParallelTo(vec3))
				return pos2;
			return pos1;
		}

		private AcadGeo.Point3d GetBranchTeePos(AcadGeo.Point3d pos, AcadGeo.Point3d pos1, AcadGeo.Point3d pos2, AcadGeo.Point3d pos3)
		{
			AcadGeo.Vector3d vec1 = AcadFuncs.GetVec(pos, pos1);
			AcadGeo.Vector3d vec2 = AcadFuncs.GetVec(pos, pos2);
			AcadGeo.Vector3d vec3 = AcadFuncs.GetVec(pos, pos3);

			if (vec1.IsParallelTo(vec2))
				return pos1;
			else if (vec1.IsParallelTo(vec3))
				return pos1;
			else
				return pos2;
		}

		private Solid.Profile CalculateProfile(AcadGeo.Point3d pnt)
		{
			Solid.Profile ret_prol = new Solid.Profile(profile);

			Node sn = FindNode(ret_prol.BasePoint);
			Node dn = FindNode(pnt);

			if (sn == dn)
				return ret_prol;

			if (null == sn || null == dn)
				return null;

			IEnumerable<Connection> routes = null;
			TryFunc<Node, IEnumerable<Connection>> func = routers.ShortestPathsDijkstra(e => 1, sn);
			if(func(dn, out routes))
			{
				if (null == routes || 0 == routes.Count())
					return null;

				List<AcadGeo.Point3d> route_pnts = new List<AcadGeo.Point3d>();
				route_pnts.Add(sn.Position);
				for(int i = 0; i < routes.Count(); i++)
				{
					if (routes.ElementAt(i).Source.Position.IsEqualTo(route_pnts.Last()))
						route_pnts.Add(routes.ElementAt(i).Target.Position);
					else
						route_pnts.Add(routes.ElementAt(i).Source.Position);
				}

				for(int i = 0; i < route_pnts.Count() - 1; i++)
				{
					AcadGeo.Point3d sc = route_pnts.ElementAt(i);
					AcadGeo.Point3d dc = route_pnts.ElementAt(i + 1);

					ret_prol.Translate(AcadFuncs.GetVec(dc, sc) * sc.DistanceTo(dc));
					if (i > 0)
					{
						ret_prol.Rotate(AcadFuncs.GetVec(dc, sc));
					}
				}

				return ret_prol;
			}

			return null;
		}

		private AcadGeo.Vector3d ConnVec(Connection conn)
		{
			return AcadFuncs.GetVec(conn.Target.Position, conn.Source.Position);
		}

		public void BuildProfile()
		{
			//double width = 0.0;
			if (!AcadFuncs.GetDouble(ref width, "Nhập chiều rộng:"))
				throw new Exception("Cancel process");

			/*
			double height = 0.0;
			if (!AcadFuncs.GetDouble(ref height, "Nhập chiều cao:"))
				throw new Exception("Cancel process");
			*/

			height = width;

			if (!AcadFuncs.GetDouble(ref thickness, "Nhập độ dày:"))
				throw new Exception("Cancel process");

			//AcadGeo.Point3d bp = new AcadGeo.Point3d();
			//while (true)
			//{
			//	if (!AcadFuncs.GetPoint(ref bp, "Chọn điểm bắt đầu:"))
			//		throw new Exception("Cancel process");

			//	if (null != FindNode(bp))
			//		break;
			//}

			//AcadGeo.Point3d vp = new AcadGeo.Point3d();
			//if (!AcadFuncs.GetPoint(ref vp, "Chọn hướng:", bp))
			//	throw new Exception("Cancel process");

			//AcadGeo.Point3d up = new AcadGeo.Point3d();
			//if (!AcadFuncs.GetPoint(ref up, "Chọn up:", bp))
			//	throw new Exception("Cancel process");

			AcadGeo.Point3d bp = new AcadGeo.Point3d();
			AcadGeo.Vector3d up_vec = new AcadGeo.Vector3d();
			AcadGeo.Vector3d norm = new AcadGeo.Vector3d();
			AutoDetectParams(ref bp, ref up_vec, ref norm);

			profile.Width = width;
			profile.Height = height;
			profile.Thickness = thickness;
			profile.BasePoint = bp;
			profile.NormalVector = norm;
			profile.UpVector = up_vec;
		}

		private void AutoDetectParams(ref AcadGeo.Point3d bp, ref AcadGeo.Vector3d up_vec, ref AcadGeo.Vector3d norm)
		{
			foreach(var conn in routers.Edges)
			{
				if(ConnVec(conn).DotProduct(AcadGeo.Vector3d.ZAxis) < 0.0001)
				{
					bp = conn.Source.Position;
					up_vec = AcadGeo.Vector3d.ZAxis;
					norm = ConnVec(conn);
				}
			}
		}

		public void BuildModel()
		{
			List<AcadDB.ObjectId> obj_ids = AcadFuncs.PickEnts();
			FilterEnts(ref obj_ids);

			foreach(var id in obj_ids)
			{
				List<AcadGeo.Point3d> pnts = ParseLine(id, obj_ids);
				for(int i = 0; i < pnts.Count - 1; i++)
				{
					Connect(pnts[i], pnts[i + 1]);
				}
			}
		}

		private void Connect(AcadGeo.Point3d fp, AcadGeo.Point3d sp)
		{
			Node fn = FindNode(fp);
			if (null == fn)
			{
				fn = new Node(fp);
				routers.AddVertex(fn);
			}

			Node sn = FindNode(sp);
			if (null == sn)
			{
				sn = new Node(sp);
				routers.AddVertex(sn);
			}

			if (routers.ContainsEdge(fn, sn))
				return;

			Connection conn = new Connection(fn, sn);
			routers.AddEdge(conn);
			fn.Connections.Add(conn);
			sn.Connections.Add(conn);
		}

		private static List<AcadGeo.Point3d> ParseLine(AcadDB.ObjectId id, List<AcadDB.ObjectId> ids)
		{
			List<AcadGeo.Point3d> pnts = new List<AcadGeo.Point3d>();
			using (AcadDB.Transaction tr = AcadFuncs.GetActiveDB().TransactionManager.StartTransaction())
			{
				AcadDB.Entity ent = tr.GetObject(id, AcadDB.OpenMode.ForRead) as AcadDB.Entity;
				if (!(ent is AcadDB.Line))
					return pnts;

				AcadDB.Line line = ent as AcadDB.Line;
				pnts.Add(line.StartPoint);
				pnts.Add(line.EndPoint);

				foreach(var tmp_id in ids)
				{
					if (id == tmp_id)
						continue;

					AcadDB.Entity tmp_ent = tr.GetObject(tmp_id, AcadDB.OpenMode.ForRead) as AcadDB.Entity;
					AcadGeo.Point3dCollection intersected_pnts = new AcadGeo.Point3dCollection();
					line.IntersectWith(tmp_ent, AcadDB.Intersect.OnBothOperands, intersected_pnts, (IntPtr)0, (IntPtr)0);

					for(int i = 0; i < intersected_pnts.Count; i++)
					{
						if (intersected_pnts[i].IsEqualTo(line.StartPoint) || intersected_pnts[i].IsEqualTo(line.EndPoint))
							continue;

						pnts.Add(intersected_pnts[i]);
					}
				}
			}

			pnts.Sort((x, y) => x.DistanceTo(pnts.First()).CompareTo(y.DistanceTo(pnts.First())));
			pnts = pnts.Distinct().ToList();
			return pnts;
		}

		private void FilterEnts(ref List<AcadDB.ObjectId> ids)
		{
			using (AcadDB.Transaction tr = AcadFuncs.GetActiveDB().TransactionManager.StartTransaction())
			{
				foreach (var id in ids.ToList())
				{
					AcadDB.Entity ent = tr.GetObject(id, AcadDB.OpenMode.ForRead) as AcadDB.Entity;
					if (!(ent is AcadDB.Line))
					{
						ids.Remove(id);
					}
				}
			}
		}

		private Node FindNode(AcadGeo.Point3d pnt)
		{
			List<Node> nodes = routers.Vertices.ToList<Node>();

			foreach (Node node in nodes)
			{
				if (pnt.Equals(node.Position))
					return node;
			}
			return null;
		}

	}
}
