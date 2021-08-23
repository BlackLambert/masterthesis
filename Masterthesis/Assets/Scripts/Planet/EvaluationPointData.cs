using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class EvaluationPointData
    {
        public List<PlanetMaterialLayerData> Layers { get; }
        public int BiomeID { get; set; } = -1;
        public int ContinentalPlateSegmentIndex { get; set; } = -1;
        public Vector3 WarpedPoint { get; set; }

        public EvaluationPointData(List<PlanetMaterialLayerData> layerData)
		{
            Layers = layerData;
        }
    }
}