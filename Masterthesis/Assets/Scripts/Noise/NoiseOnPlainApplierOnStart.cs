using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class NoiseOnPlainApplierOnStart : MonoBehaviour
    {
        [SerializeField]
        private NoiseSettings _noiseSettings;
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private float _maxHeight = 5f;

        private NoiseFactory _noiseFactory;
        private Seed _seed;

        private Noise3D _noise;

        [Inject]
        private void Construct(NoiseFactory noiseFactory,
            Seed seed)
		{
            _noiseFactory = noiseFactory;
            _seed = seed;
        }

        protected virtual IEnumerator Start()
		{
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            CreateNoise();
            ApplyNoise();
		}

		private void CreateNoise()
		{
            _noise = _noiseFactory.Create(_noiseSettings, _seed);
        }

        private void ApplyNoise()
        {
            Mesh mesh = _meshFilter.sharedMesh;
            Vector3[] newVertices = new Vector3[mesh.vertices.Length];

            for(int i = 0; i< newVertices.Length; i++)
			{
                Vector3 former = mesh.vertices[i];
                float noiseValue = (float) _noise.Evaluate(former.x, former.z);
                newVertices[i] = new Vector3(former.x, noiseValue * _maxHeight, former.z);
            }

            mesh.vertices = newVertices;
        }
    }
}