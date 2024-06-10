using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Document = Autodesk.Revit.DB.Document;

namespace Task2.Revit
{
	public class FamilyInstanceManager
	{
		public static FamilyInstance InsertFamilyInstance(Autodesk.Revit.DB.Document doc , string familyName , XYZ point , XYZ direction , Element hostWall ,double angle)
		{
			FamilySymbol familySymbol = new FilteredElementCollector(doc)
											 .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
											 .WhereElementIsElementType()
											 .Cast<FamilySymbol>()
											 .FirstOrDefault(a => a.Name == familyName);

			FamilyInstance familyInstance = null;

			if (angle>100) 
			{
				XYZ newDirection = new XYZ();

				if (direction.X == -1 || (direction.X == 1))
				{
					newDirection = new XYZ(direction.X * -1, 0, 0);
				}
				else if (direction.Y == -1 || direction.Y == 1)
				{
					newDirection = new XYZ(0, direction.Y * -1, 0);
				}


				familyInstance = doc.Create.NewFamilyInstance(point, familySymbol, newDirection, hostWall, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

				if (newDirection.X == 1)
				{
					XYZ translationVector = new XYZ(-0.3 / 0.3048, -0.1 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
				else if (newDirection.X == -1)
				{
					XYZ translationVector = new XYZ(0.3 / 0.3048, 0.1 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
				else if (newDirection.Y == 1)
				{
					XYZ translationVector = new XYZ(0.1 / 0.3048, 0.3 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
				else if (newDirection.Y == -1)
				{
					XYZ translationVector = new XYZ(-0.1 / 0.3048, -0.3 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
			}
			else
			{
				familyInstance = doc.Create.NewFamilyInstance(point, familySymbol, direction, hostWall, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

				if (direction.X == 1)
				{
					XYZ translationVector = new XYZ(-0.3 / 0.3048, -0.1 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
				else if(direction.X == -1)
				{
					XYZ translationVector = new XYZ( 0.3 / 0.3048, 0.1 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
				else if (direction.Y == 1)
				{
					XYZ translationVector = new XYZ(0.1 / 0.3048, 0.3 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
				else if (direction.Y == -1)
				{
					XYZ translationVector = new XYZ(- 0.1 / 0.3048, -0.3 / 0.3048, 0);

					ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
				}
			}


			return familyInstance;
		}

		public static XYZ GetFamilyInstanceDirection(XYZ familyLocation, Wall colinearWall)
		{
			LocationCurve locationCurve = colinearWall.Location as LocationCurve;

			XYZ startPoint = locationCurve.Curve.GetEndPoint(0);
			XYZ endPoint = locationCurve.Curve.GetEndPoint(1);

			XYZ direction = endPoint - startPoint;

			direction = direction.Normalize();

			return direction;
		}

		public static void MoveFamilyInstanceToRoomBoundary(Document doc, FamilyInstance familyInstance, Curve roomBoundaryCurve)
		{
			BoundingBoxXYZ familyBoundingBox = familyInstance.get_BoundingBox(doc.ActiveView);

			XYZ intersectionPoint = GetIntersectionPoint(familyBoundingBox, roomBoundaryCurve);

			LocationPoint currentLocation = familyInstance.Location as LocationPoint;
			XYZ translationVector = intersectionPoint - currentLocation.Point;

			ElementTransformUtils.MoveElement(doc, familyInstance.Id, translationVector);
		}


		public static XYZ GetIntersectionPoint(BoundingBoxXYZ boundingBox, Curve curve)
		{
			XYZ midPoint = (boundingBox.Min + boundingBox.Max) / 2.0;

			IntersectionResult intersection = curve.Project(midPoint);

			if (intersection != null && intersection.XYZPoint != null)
			{
				return intersection.XYZPoint;
			}
			
			return null;
		}



		public static Curve GetRoomBoundaryCurve(Document doc, Room room)
		{
			SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();

			IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(options);

			if (boundarySegments == null || boundarySegments.Count == 0)
			{
				return null;
			}

			List<Curve> curves = new List<Curve>();
			foreach (IList<BoundarySegment> segmentList in boundarySegments)
			{
				foreach (BoundarySegment segment in segmentList)
				{
					Curve curve = segment.GetCurve();
					if (curve != null)
					{
						curves.Add(curve);
					}
				}
			}

			CurveLoop curveLoop = CurveLoop.Create(curves);

			if (curveLoop.IsOpen())
			{
				curveLoop = CurveLoop.CreateViaOffset(curveLoop, 0.01, XYZ.BasisZ);
			}

			IList<XYZ> polygonPoints = curveLoop.Select(c => c.GetEndPoint(0)).ToList();

			polygonPoints.Add(polygonPoints[0]);

			return Line.CreateBound(polygonPoints.First(), polygonPoints.Last());
		}

	}
}
