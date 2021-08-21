using System.Collections.Generic;

namespace SBaier.Master
{
    public interface KDTree<T>
    {
        public int Count { get; }
        public int Depth { get; }

        int GetNearestTo(T sample);
        int[] GetNearestTo(T[] samples);
        int GetNearestTo(int sampleIndex);
        int[] GetNearestTo(int[] sampleIndices);

        int[] GetNearestToWithin(T sample, float maxDistance);
        int[] GetNearestToWithin(int sampleIndex, float maxDistance);
        int[][] GetNearestToWithin(T[] samples, float maxDistance);
        int[][] GetNearestToWithin(int[] sampleIndices, float maxDistance);
    }
}