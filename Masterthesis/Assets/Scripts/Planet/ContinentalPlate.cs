using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlate
    {
        public ContinentalPlateSegment[] Segments { get; }
        public float MovementAngle { get; }
		public float MovementStrength { get; }
		public Vector2Int[] Borders { get; }

        public ContinentalPlate(ContinentalPlateSegment[] segments,
            float movementAngle,
            float movementStrength,
            Vector2Int[] borders)
		{
            ValidateSegments(segments);
            ValidateMovementAngle(movementAngle);

            Segments = segments;
            MovementAngle = movementAngle;
			MovementStrength = movementStrength;
			Borders = borders;
        }

		private void ValidateSegments(ContinentalPlateSegment[] segments)
		{
            if (segments.Length == 0)
                throw new ArgumentNullException();
        }

        private void ValidateMovementAngle(float movementAngle)
        {
            if (movementAngle < 0 || movementAngle >= 360)
                throw new ArgumentOutOfRangeException();
        }
    }
}