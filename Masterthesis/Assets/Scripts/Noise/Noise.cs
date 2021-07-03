using System;

namespace SBaier.Master
{
    public interface Noise
    {
        public float FrequencyFactor { get; }
        public float Weight { get; }
        NoiseType NoiseType { get; }
    }
}