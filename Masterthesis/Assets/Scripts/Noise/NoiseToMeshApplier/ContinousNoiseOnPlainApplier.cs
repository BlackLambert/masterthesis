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
        private Vector3 _startPositionDelta = new Vector3(0.02f, 0, 0.03f);
        [SerializeField]
        private Vector3 _startPosition = Vector3.zero;

        private NoiseFactory _noiseFactory;
        private Seed _seed;

        private Noise3D _noise;
        private Vector3 _currentStartPositionDelta;
        private Vector3[] _formerVertices;
        private Vector3[] _newVertices;
        private Vector2[] _evaluationPoints;
        private float[] _evaluatedValues;

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
            _currentStartPositionDelta += _startPositionDelta * Time.deltaTime;
        }

		private void CreateNoise()
		{
            _noise = _noiseFactory.Create(_noiseSettings, _seed);
        }

        private void ApplyNoise()
		{
			Mesh mesh = _meshFilter.sharedMesh;
			InitFormerVertices(mesh);
			InitNewVertices(mesh);
			InitEvaluationPoints(mesh);
			UpdateEvaluationPoints();
			SetNewVertices();
			UpdateMesh(mesh);
		}

		private void InitFormerVertices(Mesh mesh)
		{
			_formerVertices = mesh.vertices;
		}

		private void InitNewVertices(Mesh mesh)
		{
			if (_newVertices == null || _newVertices.Length != mesh.vertexCount)
				_newVertices = new Vector3[mesh.vertexCount];
		}

		private void InitEvaluationPoints(Mesh mesh)
		{
			if (_evaluationPoints == null || _evaluationPoints.Length != mesh.vertexCount)
				_evaluationPoints = new Vector2[mesh.vertexCount];
		}

		private void UpdateEvaluationPoints()
		{
			for (int i = 0; i < _formerVertices.Length; i++)
			{
				Vector3 former = _formerVertices[i];
				_evaluationPoints[i] = new Vector2(former.x + _currentStartPositionDelta.x, former.z + _currentStartPositionDelta.z);
			}
			_evaluatedValues = _noise.Evaluate2D(_evaluationPoints);
		}

		private void SetNewVertices()
		{
			for (int i = 0; i < _newVertices.Length; i++)
			{
				Vector3 formerVertex = _formerVertices[i];
				_newVertices[i] = new Vector3(formerVertex.x, _evaluatedValues[i] * _maxHeight, formerVertex.z);
			}
		}

		private void UpdateMesh(Mesh mesh)
		{
			mesh.vertices = _newVertices;
			mesh.RecalculateNormals();
		}
	}
}