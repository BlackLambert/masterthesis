using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalRegion : Polygon
    {
		public Vector3 Site { get; }
        public Type RegionType { get; }
		public IList<int> Segements { get; }
		public Vector2Int[] Borders { get; }

		private int[] _borderCorners;
		public override IList<int> VertexIndices => _borderCorners;

		public ContinentalRegion(Type regionType,
			IList<int> segements,
			Vector3 site,
			Vector2Int[] borders)
		{
			RegionType = regionType;
			Segements = segements;
			Site = site;
			Borders = borders;
			_borderCorners = ExtractCorners();
		}

		private int[] ExtractCorners()
		{
			return new ConvexAreaCornersExtractor(Borders).ExtractCorners();
		}

		public enum Type
		{
            Undefinded = -1,
            ContinentalPlate = 0,
            Oceanic = 1,
		}
    }
}
