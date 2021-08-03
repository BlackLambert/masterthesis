using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlate : Polygon
    {
        public Vector3 Site { get; }
        public ContinentalPlateSegment[] Segments { get; }
        public float MovementAngle { get; }
		public float MovementStrength { get; }
		public Vector2Int[] Borders { get; }

        private int[] _borderCorners;
		public override IList<int> VertexIndices => _borderCorners;

		public ContinentalPlate(
            Vector3 site,
            ContinentalPlateSegment[] segments,
            float movementAngle,
            float movementStrength,
            Vector2Int[] borders)
		{
            ValidateSegments(segments);
            ValidateMovementAngle(movementAngle);

            Site = site;
            Segments = segments;
            MovementAngle = movementAngle;
			MovementStrength = movementStrength;
            Borders = borders;
            _borderCorners = ExtractCorners();
        }

		private int[] ExtractCorners()
		{
            List<int> borderCorners = new List<int>();
            Vector2Int current = new Vector2Int(0, 0);
			for (int i = 0; i < Borders.Length; i++)
			{
                borderCorners.Add(Borders[current.x][current.y]);
                int other = (current.y + 1) % 2;
                current = FindNext(Borders[current.x][other], current.x);
            }
            return borderCorners.ToArray();
		}

		private Vector2Int FindNext(int borderStart, int currentIndex)
		{
			for (int i = 0; i < Borders.Length; i++)
			{
                if (i == currentIndex)
                    continue;
                if (Borders[i][0] == borderStart)
                    return new Vector2Int(i, 0);
                if (Borders[i][1] == borderStart)
                    return new Vector2Int(i, 1);
			}
            throw new ArgumentException("The borders are not convex!");
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