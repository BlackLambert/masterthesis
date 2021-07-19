using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface QuickSelector<T>
	{
		void QuickSelect(IList<T> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex, int selectedIndex);
	}
}