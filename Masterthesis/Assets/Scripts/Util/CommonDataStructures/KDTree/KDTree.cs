using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public interface KDTree<T>
    {
        int GetNearestTo(T sample);
        int GetNearestTo(int sampleIndex);
        IList<int> GetNearestToWithin(T sample, float maxDistance);
        IList<int> GetNearestToWithin(int sampleIndex, float maxDistance);
        void BuildTree(IList<T> nodes, QuickSelector<T> quickSelector);
    }
}