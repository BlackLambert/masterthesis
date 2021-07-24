using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class EvaluationPointData
    {
        public List<PlanetLayerData> Layers { get; }
        public float[] NoiseAmplifiers { get; }
        public int BiomeIndex { get; set; } = -1;
        public int ContinentalPlateIndex { get; set; } = -1;
        public float Temperature { get; set; } = 0;

        public EvaluationPointData(float[] noiseAmplifiers)
		{
            Layers = new List<PlanetLayerData>();
            NoiseAmplifiers = noiseAmplifiers;
        }

        public EvaluationPointData(List<PlanetLayerData> layerData, float[] noiseAmplifiers)
		{
            Layers = layerData;
            NoiseAmplifiers = noiseAmplifiers;
        }
    }
}