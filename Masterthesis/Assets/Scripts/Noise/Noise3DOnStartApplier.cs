using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Noise3DOnStartApplier : MonoBehaviour
    {
        [SerializeField]
        private NoiseSettings _noiseSettings;
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private Vector2 _valueRange = new Vector2(2, 3.5f);

        private NoiseFactory _noiseFactory;
        private Seed _seed;
        private Noise3D _noise;

        [Inject]
        public void Construct(NoiseFactory noiseFactory,
            Seed seed)
		{
            _noiseFactory = noiseFactory;
            _seed = seed;
        }

        protected virtual void Start()
		{
            _noise = _noiseFactory.Create(_noiseSettings, _seed);
            ApplyNoise();
        }

		private void ApplyNoise()
		{
            Mesh mesh = _meshFilter.sharedMesh;
            Vector3[] formerVertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[mesh.vertexCount];
            Vector3 vertex;
            float delta = _valueRange.y - _valueRange.x;

            for(int i = 0; i < formerVertices.Length; i++)
			{
                vertex = formerVertices[i];
                double evaluationValue = _noise.Evaluate(vertex.x, vertex.y, vertex.z);
                newVertices[i] = formerVertices[i].normalized * (float) (_valueRange.x + delta * evaluationValue);
			}
            mesh.vertices = newVertices;
        }
	}
}