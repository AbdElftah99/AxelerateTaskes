using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AxelerateTaskes.Revit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Task1.RApplication
{
	[TransactionAttribute(TransactionMode.Manual)]
	public class CreateFloorCommand : IExternalCommand
	{
		#region Properties
		public static Document Document { get; set; }
		public static UIDocument UIDocument { get; set; }
		public static UIApplication UIApplication { get; set; }
		#endregion
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication = commandData.Application;
			UIDocument = UIApplication.ActiveUIDocument;
			Document = UIDocument.Document;

			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(Document);

			CurveLoop curveLoop = FloorManager.CreateCurveLoop();

			if (curveLoop != null ) 
			{
				List<CurveLoop> curveLoopList = [curveLoop];

				// Get First floor type
				var floorType = filteredElementCollector
								.OfClass(typeof(FloorType))
								.WhereElementIsElementType()
								.FirstOrDefault();

				// Get First Level
				Level firstLevel = Document.ActiveView.GenLevel;


				using (Transaction tsn = new Transaction(Document, "Floor Creation"))
				{
					tsn.Start();
					
					Floor floor = Floor.Create(Document, curveLoopList, floorType.Id, firstLevel.Id);

					tsn.Commit();
				}


				return Result.Succeeded;
			}
			else
			{
				return Result.Succeeded;
			}
		}
	}
}
