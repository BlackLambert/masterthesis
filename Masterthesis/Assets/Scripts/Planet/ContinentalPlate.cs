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

        public ContinentalPlate(ContinentalPlateSegment[] segments,
            float movementAngle)
		{
            ValidateSegments(segments);
            ValidateMovementAngle(movementAngle);

            Segments = segments;
            MovementAngle = movementAngle;
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