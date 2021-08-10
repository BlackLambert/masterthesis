using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlate : Polygon
    {
        public Vector3 Site { get; }
        public IList<int> Regions { get; }
        public Vector3 MovementTangent { get; }
		public float MovementStrength { get; }
		public Vector2Int[] Borders { get; }

        private int[] _borderCorners;
		public override IList<int> VertexIndices => _borderCorners;

		public ContinentalPlate(
            Vector3 site,
            IList<int> regions,
            Vector3 movementTangent,
            float movementStrength,
            Vector2Int[] borders)
		{
            Site = site;
            Regions = regions;
            MovementTangent = movementTangent;
			MovementStrength = movementStrength;
            Borders = borders;
            _borderCorners = ExtractCorners();
        }

		private int[] ExtractCorners()
		{
            return new ConvexAreaCornersExtractor(Borders).ExtractCorners();
		}
    }
}