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
        public float MovementAngle { get; }
		public float MovementStrength { get; }
		public Vector2Int[] Borders { get; }

        private int[] _borderCorners;
		public override IList<int> VertexIndices => _borderCorners;

		public ContinentalPlate(
            Vector3 site,
            IList<int> regions,
            float movementAngle,
            float movementStrength,
            Vector2Int[] borders)
		{
            ValidateMovementAngle(movementAngle);

            Site = site;
            Regions = regions;
            MovementAngle = movementAngle;
			MovementStrength = movementStrength;
            Borders = borders;
            _borderCorners = ExtractCorners();
        }

		private int[] ExtractCorners()
		{
            return new ConvexAreaCornersExtractor(Borders).ExtractCorners();
		}

        private void ValidateMovementAngle(float movementAngle)
        {
            if (movementAngle < 0 || movementAngle >= 360)
                throw new ArgumentOutOfRangeException();
        }
    }
}