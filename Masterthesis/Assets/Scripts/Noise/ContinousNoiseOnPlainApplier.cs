using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class ContinousNoiseOnPlainApplier : MonoBehaviour
    {
        [SerializeField]
        private NoiseSettings _noiseSettings;
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private float _maxHeight = 5f;
        [SerializeField]
        private float _noiseFactor = 0.01f;
        [SerializeField]
        private Vector3 _startPositionDelta = new Vector3(0.02f, 0, 0.03f);
        [SerializeField]
        private Vector3 _startPosition = Vector3.zero;

        private NoiseFactory _noiseFactory;
        private Seed _seed;

        private Noise3D _noise;
        private Vector3 _currentStartPositionDelta;

        [Inject]
        private void Construct(NoiseFactory noiseFactory,
            Seed seed)
		{
            _noiseFactory = noiseFactory;
            _seed = seed;
        }

        protected virtual void Start()
		{
            CreateNoise();
            _currentStartPositionDelta = _startPosition;
        }

        protected virtual void Update()
		{
            ApplyNoise();
            _currentStartPositionDelta += _startPositionDelta;
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
                Vector3 evaluationVector = former + _currentStartPositionDelta;
                float noiseValue = (float) _noise.Evaluate(evaluationVector.x * _noiseFactor, evaluationVector.z * _noiseFactor);
                newVertices[i] = new Vector3(former.x, noiseValue * _maxHeight, former.z);
            }

            mesh.vertices = newVertices;
        }
    }
}