using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class NoiseSettings : ScriptableObject
    {
        public abstract NoiseType GetNoiseType();
    }
}