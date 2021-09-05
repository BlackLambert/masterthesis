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


		public Transform Base => transform;

        public PlanetFaceData Data { get; private set; }
		public PlanetData PlanetData => _planetData;

		private PlanetData _planetData;
		private Vector3[] _vertexNormalized;
		private KDTree<Vector3> _vertexTree;
		private Vector3BinaryKDTreeFactory _treeFactory;

		[Inject]
		public void Construct(Vector3BinaryKDTreeFactory treeFactory)
		{
			_treeFactory = treeFactory;
		}

		public void Init(PlanetFaceData data, PlanetData planetData)
		{
            Data = data;
            _planetData = planetData;
			UpdateNormalizedVertices();

		}

		public void UpdateMesh(Vector3[] vertices, int[] faces, KDTree<Vector3> vertexTree)
		{
			SharedMesh.vertices = vertices;
			SharedMesh.triangles = faces;
			_vertexTree = vertexTree;
			UpdateNormalizedVertices();
		}

		internal void Destruct()
		{
			GameObject.Destroy(SharedMesh);
			Destroy(gameObject);
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
			_vertexTree = _treeFactory.Create(vertices);
		}

		private void UpdatePosition(Vector3[] vertices, int vertexIndex)
		{
			Vector3 evaluationPoint = _vertexNormalized[vertexIndex].FastMultiply(_planetData.Dimensions.KernelRadius);
			vertices[vertexIndex] = evaluationPoint;
			EvaluationPointData data = Data.EvaluationPoints[vertexIndex];
			int layersCount = data.Layers.Count;
			for (int i = 0; i < layersCount; i++)
				AddLayerHeight(vertices, vertexIndex, data.Layers[i]);
		}

		private void AddLayerHeight(Vector3[] vertices, int vertexIndex, PlanetMaterialLayerData layer)
		{
			bool layerIsAir = layer.State == PlanetMaterialState.Gas;
			if (layerIsAir)
				return;
			if (!IsLayerActive(layer.MaterialType))
				return;
			float layerHeight = layer.Height * _planetData.Dimensions.MaxHullThickness;
			vertices[vertexIndex] = vertices[vertexIndex].FastAdd(_vertexNormalized[vertexIndex].FastMultiply(layerHeight));
		}

		private bool IsLayerActive(PlanetMaterialType materialType)
		{
			uint maskValue = (uint)materialType;
			return (PlanetData.LayerBitMask & maskValue) > 0;
		}

		public int GetNearestTo(Vector3 point)
		{
			return _vertexTree.GetNearestTo(point);
		}
	}
}