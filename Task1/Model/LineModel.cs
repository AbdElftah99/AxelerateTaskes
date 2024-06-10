using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.Model
{
	public class LineModel
	{
		public Point2 StartPoint { get; set; }
		public Point2 EndPoint { get; set; }

		public LineModel(Point2 start, Point2 end)
		{
			StartPoint = start;
			EndPoint = end;
		}

		public override string ToString()
		{
			return $"Start: ({StartPoint.x}, {StartPoint.y}), End: ({EndPoint.x}, {EndPoint.y})";
		}
	}
}
