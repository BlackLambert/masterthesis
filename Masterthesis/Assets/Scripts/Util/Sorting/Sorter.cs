using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public interface Sorter<T>
    {
        void Sort(T[] points, int compareValueIndex = 0);
        void Sort(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex = 0);
        void Sort(T[] points, Vector2Int indexRange, int compareValueIndex = 0);
        void Sort(T[] points, int[] indexPermutations, int compareValueIndex = 0);
    }
}