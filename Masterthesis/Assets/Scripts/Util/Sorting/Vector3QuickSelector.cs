using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface Vector3QuickSelector
	{
		void QuickSelect(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex, int selectedIndex);
	}
}