﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Task2.Revit
{
	public class WallFilter : ISelectionFilter
	{
		public bool AllowElement(Element elem)
		{
			return elem is Wall;
		}

		public bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}
}
