using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "BiomeSettings", menuName = "Biome/ContinentBiomeSetting")]
    public class ContinentBiomeSettings : BiomeSettings
    {
        public override ContinentalRegion.Type RegionType => ContinentalRegion.Type.ContinentalPlate;

        [SerializeField]
        private Color _mountainSlopeColor;
        public Color MountainSlopeColor => _mountainSlopeColor;
    }
}