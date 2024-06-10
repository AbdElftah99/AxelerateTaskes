using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.Model;

namespace Task1.Data
{
	public class LinesData
	{
		public static List<LineModel> CreateLines()
		{
			return new List<LineModel>
		    {
			    new LineModel(new Point2(0, 0), new Point2(79, 0)),    
                new LineModel(new Point2(44, 25), new Point2(13, 25)), 
                new LineModel(new Point2(13, 40), new Point2(-8, 40)),
                new LineModel(new Point2(55, 34), new Point2(55, 10)), 
                new LineModel(new Point2(79, 34), new Point2(55, 34)), 
                new LineModel(new Point2(0, 20), new Point2(0, 0)),    
                new LineModel(new Point2(55, 10), new Point2(44, 12)),
                new LineModel(new Point2(-8, 40), new Point2(-8, 20)), 
                new LineModel(new Point2(79, 0), new Point2(79, 34)),  
                new LineModel(new Point2(44, 12), new Point2(44, 25)), 
                new LineModel(new Point2(-8, 20), new Point2(0, 20)),  
                new LineModel(new Point2(13, 25), new Point2(13, 40))  
            };
		}
	}
}
