using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "UVSphereGeneratorSettings", menuName = "Mesh/UVSphereGeneratorSettings")]
    public class UVSphereGeneratorSettings : MeshGeneratorSettings
    {
        [SerializeField]
        private int _ringsAmount = 5;
        [SerializeField]
        private int _segmentsAmount = 10;

        public int RingsAmount => _ringsAmount;
        public int SegmentsAmount => _segmentsAmount;

		public override MeshGeneratorType Type => MeshGeneratorType.UVSphere;
	}
}