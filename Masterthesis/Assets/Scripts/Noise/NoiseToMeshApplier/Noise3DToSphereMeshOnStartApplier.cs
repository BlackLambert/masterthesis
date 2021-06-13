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
        private NoiseSettings _noiseSettings;
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private Vector2 _valueRange = new Vector2(2, 3.5f);

        private NoiseFactory _noiseFactory;
        private Noise3DToSpheralMeshApplier _noiseApplier;
        private Seed _seed;
        private Noise3D _noise;

        [Inject]
        public void Construct(NoiseFactory noiseFactory,
            Noise3DToSpheralMeshApplier noiseApplier,
            Seed seed)
		{
            _noiseFactory = noiseFactory;
            _noiseApplier = noiseApplier;
            _seed = seed;
        }

        protected virtual void Start()
		{
            _noise = _noiseFactory.Create(_noiseSettings, _seed);
            _noiseApplier.Range = _valueRange;
            ApplyNoise();
        }

		private void ApplyNoise()
		{
            _noiseApplier.Apply(_meshFilter.sharedMesh, _noise);
        }
	}
}