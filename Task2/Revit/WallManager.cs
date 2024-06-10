using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Revit
{
	public class WallManager
	{
		public static bool IsWallInRoom(Wall wall, Room room)
		{
			SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();

			IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(options);

			foreach (IList<BoundarySegment> boundary in boundaries)
			{
				foreach (BoundarySegment boundarySegment in boundary)
				{
					if (boundarySegment.ElementId == wall.Id)
					{
						return true;
					}
				}
			}

			return false;
		}

		public static bool AreWallIntersected(Wall wall1 , Wall wall2)
		{
			LocationCurve locationCurveForWall1 = wall1.Location as LocationCurve;
			LocationCurve locationCurveForWall2 = wall2.Location as LocationCurve;

			if (locationCurveForWall1 != null && locationCurveForWall2 != null)
			{
				Curve curve1 = locationCurveForWall1.Curve;
				Curve curve2 = locationCurveForWall2.Curve;

				IntersectionResultArray results;
				SetComparisonResult result = curve1.Intersect(curve2, out results);

				return result == SetComparisonResult.Overlap || result == SetComparisonResult.Subset;
			}

			return false;
		}

		public static List<Wall> GetAllIntersectedWalls(Document doc , Wall selectedWall)
		{
			List<Wall> intersectingWalls = new();

			BoundingBoxXYZ boundingBox = selectedWall.get_BoundingBox(doc.ActiveView);

			Outline outline = new(boundingBox.Min, boundingBox.Max);

			BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new(outline);

			FilteredElementCollector wallCollector = new FilteredElementCollector(doc)
												  .OfClass(typeof(Wall))
												  .WherePasses(boundingBoxIntersectsFilter);

            foreach (Wall wall in wallCollector)
            {
				if (wall.Id != selectedWall.Id && AreWallIntersected(selectedWall, wall))
				{
					intersectingWalls.Add(wall);
				}
			}

			return intersectingWalls;
        }

		public static Wall GetWallHostDoor(Document doc, FamilyInstance bathroomDoor , Room room)
		{
			Wall hostWall = bathroomDoor.Host as Wall;
			List<Wall> wallsInBathroom = new();

			List<Wall> wallsIntersectedWithHostWall = GetAllIntersectedWalls(doc, hostWall);

			foreach (Wall wall in wallsIntersectedWithHostWall)
			{
				if (IsWallInRoom(wall, room))
				{
					wallsInBathroom.Add(wall);
				}
			}

			Wall closestWall = null;
			double maxDistance = double.MinValue;

			LocationPoint doorLocationPoint = bathroomDoor.Location as LocationPoint;

			XYZ doorPoint = doorLocationPoint.Point;

			foreach (Wall wall in wallsInBathroom)
            {
				LocationCurve wallLocationCurve = wall.Location as LocationCurve;
				Curve wallCurve = wallLocationCurve?.Curve;

				XYZ start = wallCurve.GetEndPoint(0);
				XYZ end = wallCurve.GetEndPoint(1);
				XYZ midpoint = (start + end) / 2.0;

				double distance = midpoint.DistanceTo(doorPoint);

				if (distance > maxDistance)
				{
					maxDistance = distance;
					closestWall = wall;
				}
			}

			return closestWall;
		}


		public static XYZ FindWallsIntersectionPoint(Wall wall1, Wall wall2)
		{
			// Get the location curves of the walls
			LocationCurve locationCurve1 = wall1.Location as LocationCurve;
			LocationCurve locationCurve2 = wall2.Location as LocationCurve;

			if (locationCurve1 == null || locationCurve2 == null)
			{
				// One or both walls don't have a valid location curve
				return null;
			}

			Curve curve1 = locationCurve1.Curve;
			Curve curve2 = locationCurve2.Curve;

			// Check if the curves intersect
			IntersectionResultArray intersectionResultArray;

			if (curve1.Intersect(curve2, out intersectionResultArray) == SetComparisonResult.Overlap)
			{
				IntersectionResult intersectionResult = intersectionResultArray.get_Item(0);

				return intersectionResult.XYZPoint;
			}

			// The walls do not intersect
			return null;
		}
	}
}
