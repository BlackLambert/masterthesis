using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ShapingParameter
    {
        public ShapingParameter(
            PlatesShapingParameter plates)
		{
			Plates = plates;
		}

		public PlatesShapingParameter Plates { get; }
	}
}