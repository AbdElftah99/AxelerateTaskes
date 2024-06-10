using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Task2.Revit
{
	public class RooomManager
	{
		public static List<Room> GetRoomBesideWall(Document doc, Wall wall)
		{
			List<Room> besidesRooms = new List<Room>();

			SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();

			FilteredElementCollector collector = new FilteredElementCollector(doc)
													.OfClass(typeof(SpatialElement));

			foreach (SpatialElement element in collector)
			{
				if (element is Room room)
				{
					IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(options);

					foreach (IList<BoundarySegment> boundary in boundaries)
					{
						foreach (BoundarySegment segment in boundary)
						{
							ElementId boundaryElementId = segment.ElementId;

							if (boundaryElementId == wall.Id)
							{
								besidesRooms.Add(room);
								break;
							}
						}
					}
				}
			}

			return besidesRooms;
		}

		public static Room GetRoom(List<Room> besidesRooms , string roomName)
		{
			foreach (Room room in besidesRooms)
			{
				if (room.Name == roomName)
				{
					return room;
				}
			}
			return null;
		}

		public static List<FamilyInstance> GetDoorsInRoom(Document doc, Room room)
		{
			List<FamilyInstance> doorsInRoom = new List<FamilyInstance>();

			SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
			IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(options);

			if (boundaries == null || boundaries.Count == 0)
				return doorsInRoom;

			foreach (IList<BoundarySegment> boundarySegmentList in boundaries)
			{
				foreach (BoundarySegment boundSegment in boundarySegmentList)
				{
					if (boundSegment == null)
						continue;

					Wall wallInRoom = doc.GetElement(boundSegment.ElementId) as Wall;

					if (wallInRoom == null || wallInRoom is not HostObject)
						continue;

					IList<ElementId> hostedElementsOnWall = wallInRoom.FindInserts(true, true, true, true);

					if (hostedElementsOnWall != null && hostedElementsOnWall.Count > 0)
					{
						List<FamilyInstance> doorsCollector = new FilteredElementCollector(doc, hostedElementsOnWall)
																	.OfCategory(BuiltInCategory.OST_Doors)
																	.WhereElementIsNotElementType()
																	.Cast<FamilyInstance>()
																	.ToList();

						List<FamilyInstance> doorsInSpecifiedRoom = doorsCollector
																			.Where(door => door.Room.Name == room.Name)
																			.ToList();

						doorsInRoom.AddRange(doorsInSpecifiedRoom);
					}
				}
			}

			return doorsInRoom;
		}
	}
}
