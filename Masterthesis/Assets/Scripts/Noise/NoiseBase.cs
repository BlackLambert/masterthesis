
using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class NoiseBase
    {
        private float _frequencyFactor = 1;
        public float FrequencyFactor
		{
            get => _frequencyFactor;
            set
            {
                CheckStartFrequencyOutOfRange(value);
                _frequencyFactor = value;
            }
        }

        private float _weight = 1;
        public float Weight 
        {
            get => _weight;
            set
			{
                CheckStartWeightOutOfRange(value);
                _weight = value;
            }
        }



        private void CheckStartWeightOutOfRange(float startWeight)
        {
            if (startWeight < 0)
                throw new ArgumentOutOfRangeException();
        }

        private void CheckStartFrequencyOutOfRange(float startFrequency)
        {
            if (startFrequency <= 0)
                throw new ArgumentOutOfRangeException();
        }

        protected void ApplyFrequencyFactor(NativeArray<Vector3> points)
		{
            if (FrequencyFactor == 1)
                return;
            for (int i = 0; i < points.Length; i++)
                points[i] = ApplyFrequencyFactor3D(points[i]);
        }

        protected void ApplyFrequencyFactor(NativeArray<Vector2> points)
        {
            if (FrequencyFactor == 1)
                return;
            for (int i = 0; i < points.Length; i++)
                points[i] = ApplyFrequencyFactor2D(points[i]);
        }

        protected Vector2 ApplyFrequencyFactor2D(Vector2 point)
        {
            return point.FastMultiply(FrequencyFactor);
        }

        protected Vector3 ApplyFrequencyFactor3D(Vector3 point)
        {
            return point.FastMultiply(FrequencyFactor);
        }
    }
}