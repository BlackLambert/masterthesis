using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public interface Sorter<T>
    {
        void Sort(IList<T> points, int compareValueIndex = 0);
        void Sort(IList<T> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex = 0);
        void Sort(IList<T> points, Vector2Int indexRange, int compareValueIndex = 0);
        void Sort(IList<T> points, IList<int> indexPermutations, int compareValueIndex = 0);
    }
}