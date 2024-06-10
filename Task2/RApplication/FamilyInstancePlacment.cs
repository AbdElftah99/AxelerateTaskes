using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Revit;

namespace Task2.RApplication
{
	[TransactionAttribute(TransactionMode.Manual)]
	public class FamilyInstancePlacment : IExternalCommand
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
			try
			{
				// Step 1: Let the user select a wall
				Reference pickedRef = UIDocument.Selection.PickObject(ObjectType.Element, new WallFilter(), "Select a wall");
				if (pickedRef == null)
					return Result.Cancelled;

				Wall selectedWall = Document.GetElement(pickedRef) as Wall;

				if (selectedWall == null)
				{
					message = "Selected element is not a wall.";
					return Result.Failed;
				}

				List<Wall> intersectedWalls = WallManager.GetAllIntersectedWalls(Document, selectedWall);
				

				// Step 2: Identify rooms adjacent to the selected wall
				List<Room> besidesRooms = RooomManager.GetRoomBesideWall(Document, selectedWall);

				// Step 3: Rename rooms named "Bathroom" to "WC"
				using (Transaction tsn = new Transaction(Document))
				{
					tsn.Start("Rename Bathroom to WC");
					List<Wall> wallsInBathroom = new();
					List<FamilyInstance> doors = new();

					// Create a list of element Ids to select
					List<ElementId> elementIdsToSelect = new List<ElementId>();

					foreach (Room room in besidesRooms)
					{
						if (room.Name.Contains("Bathroom"))
						{
							doors = RooomManager.GetDoorsInRoom(Document , room);

							FamilyInstance doorInstance = doors.FirstOrDefault();

							LocationPoint LocationOfDoor = doorInstance.Location as LocationPoint;

							Wall ColinearWall = WallManager.GetWallHostDoor(Document , doorInstance , room);

							XYZ intersectionPoint = WallManager.FindWallsIntersectionPoint(selectedWall , ColinearWall);

							XYZ familyDirection = FamilyInstanceManager.GetFamilyInstanceDirection(intersectionPoint , selectedWall);

							double angle = familyDirection.AngleTo(LocationOfDoor.Point) * (180 / 3.14);

							FamilyInstance familyInstance = FamilyInstanceManager.InsertFamilyInstance(Document , "Trial", intersectionPoint , familyDirection , selectedWall , angle);
						}
					}

					tsn.Commit();
				}

				return Result.Succeeded;
			}
			catch (Exception ex)
			{
				message = ex.Message;

				TaskDialog.Show("Error", message.ToString());

				return Result.Failed;
			}
		}
	}
}
