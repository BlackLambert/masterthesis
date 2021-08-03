using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "BiomeSettings", menuName = "Biome/BiomeSettings")]
    public class BiomeSettings : ScriptableObject
    {
        [SerializeField]
        private Color _baseColor = Color.white;
        public Color BaseColor => _baseColor;

        [SerializeField]
        private int _frequency = 1;
        public int Frequency => _frequency;
    }
}