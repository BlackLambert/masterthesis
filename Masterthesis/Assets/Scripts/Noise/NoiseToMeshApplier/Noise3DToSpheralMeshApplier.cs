using System;
using UnityEngine;

namespace SBaier.Master
{
    public class Noise3DToSpheralMeshApplier
    {
		private Vector2 _range = new Vector2(0, 1);
        public Vector2 Range 
        {
            get => _range;
			set
			{
                if (value.x > value.y || value.x < 0 || value.y < 0)
                    throw new ArgumentOutOfRangeException();
                _range = value;
            }
        }

        private Vector3[] _formerVertices;
        private Vector3[] _newVertices;

        private SphereMeshFormer _meshFormer;

        public Noise3DToSpheralMeshApplier(SphereMeshFormer meshFormer)
		{
            _meshFormer = meshFormer;
        }

		public void Apply(Mesh mesh, Noise3D noise)
		{
			_meshFormer.Form(mesh, Range.x);
			_formerVertices = mesh.vertices;
			InitNewVertices(mesh);
			ApplyEvaluatedDataToVertices(noise);
			mesh.vertices = _newVertices;
		}

		private void InitNewVertices(Mesh mesh)
		{
			if (_newVertices == null || _newVertices.Length != mesh.vertexCount)
				_newVertices = new Vector3[mesh.vertexCount];
		}

		private void ApplyEvaluatedDataToVertices(Noise3D noise)
		{
			float delta = Range.y - Range.x;

			for (int i = 0; i < _formerVertices.Length; i++)
			{
				Vector3 vertex = _formerVertices[i];
				double evaluatedData = noise.Evaluate(vertex.x, vertex.y, vertex.z);
				_newVertices[i] = _formerVertices[i].normalized * (float)(Range.x + delta * evaluatedData);
			}
		}
	}
}