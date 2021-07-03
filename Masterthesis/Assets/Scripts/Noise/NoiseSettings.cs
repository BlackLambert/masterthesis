using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class NoiseSettings : ScriptableObject
    {
        public abstract NoiseType GetNoiseType();
        [SerializeField]
        private float _weight = 1;
        public float Weight => _weight;
        [SerializeField]
        private float _frequencyFactor = 1;
        public float FrequencyFactor => _frequencyFactor;
    }
}