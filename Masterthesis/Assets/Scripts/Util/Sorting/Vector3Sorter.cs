using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public interface Vector3Sorter
    {
        void Sort(IList<Vector3> points, int compareValueIndex = 0);
        void Sort(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex = 0);
        void Sort(IList<Vector3> points, Vector2Int indexRange, int compareValueIndex = 0);
        void Sort(IList<Vector3> points, IList<int> indexPermutations, int compareValueIndex = 0);
    }
}