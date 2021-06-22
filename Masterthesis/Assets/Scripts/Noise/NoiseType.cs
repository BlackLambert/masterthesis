using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public enum NoiseType
    {
        Unset = 0,
        Perlin = 1,
        Billow = 2,
        Ridged = 4,
        Layered = 8,
        Octave = 16,
        Simplex = 32,
        NoiseValueLimiter = 64,
        WhiteNoise = 128
    }
}