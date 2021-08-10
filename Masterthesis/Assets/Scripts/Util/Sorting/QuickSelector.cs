using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface QuickSelector<T>
	{
		void QuickSelect(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex, int selectedIndex);
	}
}