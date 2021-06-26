using Unity.Jobs;
using System;
using Unity.Collections;

namespace SBaier.Master
{
    public interface NoiseEvaluationJob : IDisposable
    {
        JobHandle Schedule();
        JobHandle Schedule(JobHandle dependency);
        NativeArray<float> Result { get; }
    }
}