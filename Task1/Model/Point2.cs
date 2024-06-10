namespace Task1.Model
{
	public class Point2
	{
		public int x;
		public int y;

		public Point2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj is Point2 point)
			{
				return x == point.x && y == point.y;
			}
			return false;
		}
	}
}