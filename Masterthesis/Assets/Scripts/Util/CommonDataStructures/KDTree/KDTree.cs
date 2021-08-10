using System.Collections.Generic;

namespace SBaier.Master
{
    public interface KDTree<T>
    {
        public int Count { get; }
        int GetNearestTo(T sample);
        int GetNearestTo(int sampleIndex);
        int[] GetNearestToWithin(T sample, float maxDistance);
        int[] GetNearestToWithin(int sampleIndex, float maxDistance);
        void BuildTree(T[] nodes, QuickSelector<T> quickSelector);
    }
}