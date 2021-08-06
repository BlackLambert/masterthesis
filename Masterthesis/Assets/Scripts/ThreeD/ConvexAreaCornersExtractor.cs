using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ConvexAreaCornersExtractor 
    {
        public ConvexAreaCornersExtractor(Vector2Int[] borders)
		{
			Borders = borders;
		}

		public Vector2Int[] Borders { get; }

		public int[] ExtractCorners()
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
            throw new BorderNotConvexException();
        }

        public class BorderNotConvexException : ArgumentException { }
    }
}