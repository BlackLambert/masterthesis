using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetFace : MonoBehaviour, MeshFaceSeparatorTarget
    {
        [SerializeField]
        private MeshFilter _meshFilter;
		public MeshFilter MeshFilter => _meshFilter;
        private Mesh SharedMesh => _meshFilter.sharedMesh;
		public int VertexCount => _meshFilter.sharedMesh.vertexCount;
		public Vector3[] Vertices => _meshFilter.sharedMesh.vertices;
		public Vector3[] Normals => _meshFilter.sharedMesh.normals;
		public Vector3[] WarpedVertices { get; private set; }
		public KDTree<Vector3> WarpedVertexTree { get; private set; }


		public Transform Base => transform;

        public PlanetFaceData Data { get; private set; }
		public PlanetData PlanetData { get; private set; }

		private Vector3[] _vertexNormalized;
		private KDTree<Vector3> _vertexTree;

		public void Init(PlanetFaceData data, PlanetData planetData, KDTree<Vector3> vertexTree)
		{
            Data = data;
            PlanetData = planetData;
			_vertexTree = vertexTree;
			UpdateNormalizedVertices();
		}

		public void Destruct()
		{
			GameObject.Destroy(SharedMesh);
			Destroy(gameObject);
		}

		public void SetWarpedVertices(Vector3[] vertices, KDTree<Vector3> warpedVertexTree)
		{
			if (vertices.Length != VertexCount)
				throw new ArgumentException();
			WarpedVertices = vertices;
			WarpedVertexTree = warpedVertexTree;
		}

		private void UpdateNormalizedVertices()
		{
			Vector3[] vertices = SharedMesh.vertices;
			_vertexNormalized = new Vector3[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
				_vertexNormalized[i] = vertices[i].normalized;
		}

        public void UpdateVertexPositions()
        {
            Vector3[] vertices = SharedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
				UpdatePosition(vertices, i);
			SharedMesh.vertices = vertices;
		}

		private void UpdatePosition(Vector3[] vertices, int vertexIndex)
		{
			EvaluationPointData data = Data.EvaluationPoints[vertexIndex];
			int layersCount = data.Layers.Count;
			float relativeHeight = 0;
			for (int i = 0; i < layersCount; i++)
				relativeHeight += GetLayerHeight(data.Layers[i]);
			PlanetDimensions dimensions = PlanetData.Dimensions;
			float height = dimensions.KernelRadius + dimensions.MaxHullThickness * relativeHeight;
			vertices[vertexIndex] = _vertexNormalized[vertexIndex].FastMultiply(height);
		}

		private float GetLayerHeight(PlanetMaterialLayerData layer)
		{
			if (!PlanetData.IsLayerActive(layer.MaterialType))
				return 0;
			bool layerIsAir = layer.State == PlanetMaterialState.Gas;
			if (layerIsAir)
				return 0;
			return layer.Height;
		}

		public int GetNearestTo(Vector3 point)
		{
			return _vertexTree.GetNearestTo(point);
		}
	}
}