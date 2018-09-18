using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(RoutingSolid.AcadCmds))]
namespace RoutingSolid
{
	public class AcadCmds
	{
		[CommandMethod("RoutingSolid")]
		public void RoutingSolid()
		{
			try
			{
				AcadFuncs.GetEditor().WriteMessage("dev by [gdt.anv@gmail.com]");
				Model model = new Model();
				model.BuildModel();
				model.BuildProfile();
				model.BuildRoutingSolid();
				model.RoutingSolid();
			}
			catch(Autodesk.AutoCAD.Runtime.Exception ex)
			{

			}
			finally
			{

			}
		}
	}
}
