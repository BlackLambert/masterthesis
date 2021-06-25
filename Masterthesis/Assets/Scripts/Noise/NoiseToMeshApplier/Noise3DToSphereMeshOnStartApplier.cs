using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Noise3DToSphereMeshOnStartApplier : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private Vector2 _valueRange = new Vector2(2, 3.5f);

        private Noise3DToSpheralMeshApplier _noiseApplier;
        private Noise3D _noise;

        [Inject]
        public void Construct(Noise3D noise,
            Noise3DToSpheralMeshApplier noiseApplier)
		{
            _noiseApplier = noiseApplier;
            _noise = noise;
        }

        protected virtual void Start()
		{
            _noiseApplier.Range = _valueRange;
            ApplyNoise();
        }

		private void ApplyNoise()
		{
            _noiseApplier.Apply(_meshFilter.sharedMesh, _noise);
        }
	}
}