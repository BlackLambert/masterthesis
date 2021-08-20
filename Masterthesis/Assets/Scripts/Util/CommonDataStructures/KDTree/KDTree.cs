using System.Collections.Generic;

namespace SBaier.Master
{
    public interface KDTree<T>
    {
        public int Count { get; }
        public int Depth { get; }
        int GetNearestTo(T sample);
        int GetNearestTo(int sampleIndex);
        int[] GetNearestToWithin(T sample, float maxDistance);
        int[] GetNearestToWithin(int sampleIndex, float maxDistance);
    }
}