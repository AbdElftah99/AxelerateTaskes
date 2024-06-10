using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Task1.Data;
using Task1.Model;
using LineModel = Task1.Model.LineModel;


namespace AxelerateTaskes.Revit
{
	public class FloorManager
	{
		public static CurveLoop CreateCurveLoop()
		{
			// First get the sorted lines
			List<LineModel> lines = LinesData.CreateLines();
			HashSet<LineModel> sortedLines = SortLines(lines);

			StringBuilder sb = new StringBuilder();

			foreach (var item in sortedLines)
			{
				sb.AppendLine(item.ToString());
			}

			TaskDialog.Show("Sorted Lines" , sb.ToString());

			// Create curve loop
			CurveLoop curveLoop = new();

			if (IsClosedShape(sortedLines.ToList())) 
			{
				List<Line> revitLines = CreateRevitLines(sortedLines);

				foreach (Line line in revitLines) 
				{
					curveLoop.Append(line);
				}
			}
			else
			{
				TaskDialog.Show("Exception","Can't Create floor from this lines");

				return null;
			}

			return curveLoop;
		}

		public static List<Line> CreateRevitLines(HashSet<LineModel> sortedLines)
		{
			List<Line> revitLines = new List<Line>();

			foreach (LineModel line in sortedLines) 
			{
				Line revitLine = Line.CreateBound(new XYZ(line.StartPoint.x, line.StartPoint.y, 0), new XYZ(line.EndPoint.x, line.EndPoint.y, 0));

				revitLines.Add(revitLine);
			}

			return revitLines;
		}



		public static HashSet<LineModel> SortLines(List<LineModel> lines)
		{
			HashSet<LineModel> sortedLines = [lines[0]];

			LineModel flag = lines[0];

			for (int i = 0; i < lines.Count -1; i++)
			{
				Point2 endPoint = flag.EndPoint;

				for (int j = 0; j < lines.Count; j++)
				{
					if (j != i)
					{
						if (lines[j].StartPoint.Equals(endPoint))
						{
							flag = lines[j];
							sortedLines.Add(lines[j]);
							break;
						}
					}
				}
			}

			return sortedLines;
		}



		public static bool IsClosedShape(List<LineModel> sortedLines)
		{
			LineModel firstLine = sortedLines[0];
			LineModel lastLine = sortedLines[sortedLines.Count - 1];

			return firstLine.StartPoint.Equals(lastLine.EndPoint);
		}
	}
}
