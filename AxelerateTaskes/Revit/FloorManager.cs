using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AxelerateTaskes.Revit
{
	public class FloorManager
	{
		public static void CreateFloor(List<Curve> curves)
		{
			// Create curve loop
			CurveLoop curveLoop = CurveLoop.Create(curves);

			CurveArray curveArray = new CurveArray();

			foreach (Curve curve in curveLoop) 
			{
				curveArray.Append(curve);
			}
		}
	}
}
