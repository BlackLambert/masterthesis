using UnityEngine;

namespace SBaier.Master
{
    public class PlanetFace : MonoBehaviour, MeshFaceSeparatorTarget
    {
        [SerializeField]
        private MeshFilter _meshFilter;
		public MeshFilter MeshFilter => _meshFilter;
        public Mesh SharedMesh => _meshFilter.sharedMesh;

        public Transform Base => transform;

        public PlanetFaceData Data { get; private set; }
        private PlanetData _planetData;

        public void Init(PlanetFaceData data, PlanetData planetData)
		{
            Data = data;
            _planetData = planetData;
        }

        public void UpdateVertexPositions()
        {
            Vector3[] vertices = SharedMesh.vertices;
            for (int j = 0; j < vertices.Length; j++)
				UpdatePosition(vertices, j);
			SharedMesh.vertices = vertices;
        }

		private void UpdatePosition(Vector3[] vertices, int vertexIndex)
		{
			Vector3 evaluationPoint = vertices[vertexIndex].normalized * _planetData.AtmosphereRadius;
			vertices[vertexIndex] = evaluationPoint;
			EvaluationPointData data = Data.EvaluationPoints[vertexIndex];
			for (int i = 0; i < data.Layers.Count; i++)
				AddLayerHeight(vertices, vertexIndex, data.Layers[i]);
		}

		private void AddLayerHeight(Vector3[] vertices, int vertexIndex, PlanetLayerData layer)
		{
			bool layerIsAir = layer.MaterialIndex == 0;
			if (layerIsAir)
				return;
			float layerHeight = layer.Height * _planetData.HullThickness;
			vertices[vertexIndex] += vertices[vertexIndex].normalized * layerHeight;
		}
	}
}