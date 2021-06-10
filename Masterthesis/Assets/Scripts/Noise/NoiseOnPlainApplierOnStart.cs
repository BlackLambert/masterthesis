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
        [SerializeField]
        private float _noiseFactor = 0.01f;

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
            Vector3[] formerVertices = mesh.vertices;

            for(int i = 0; i< newVertices.Length; i++)
			{
                Vector3 former = formerVertices[i];
                float noiseValue = (float) _noise.Evaluate(former.x * _noiseFactor, former.z * _noiseFactor);
                newVertices[i] = new Vector3(former.x, noiseValue * _maxHeight, former.z);
            }

            mesh.vertices = newVertices;
        }
    }
}