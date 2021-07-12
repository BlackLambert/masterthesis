using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3SorterBase
    {
		protected static void ValidateIndexRange(IList<Vector3> points, Vector2Int valueRange)
		{
			if (valueRange.x > valueRange.y || valueRange.x < 0 || valueRange.y >= points.Count)
				throw new ArgumentOutOfRangeException();
		}

		protected static void ValidateCompareValueIndex(int compareValueIndex)
		{
			if (compareValueIndex < 0 || compareValueIndex > 2)
				throw new ArgumentOutOfRangeException();
		}
	}
}