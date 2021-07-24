using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Planet : MonoBehaviour
    {
        [SerializeField]
        private Transform _facesHook;

        public IList<PlanetFace> Faces { get; private set; }
        public PlanetData Data { get; private set; }

		public void Init(PlanetData data, IList<PlanetFace> faces)
		{
            Data = data;
            Faces = faces;

            AttatchFaces();
            UpdateMesh();
        }

		private void AttatchFaces()
		{
			foreach (PlanetFace face in Faces)
                face.Base.SetParent(_facesHook, false);
		}

		public void UpdateMesh()
		{
            UpdateVertexPositions();
            RecalculateFaceNormals();
        }

        private void UpdateVertexPositions()
        {
			foreach (PlanetFace face in Faces)
                face.UpdateVertexPositions();
        }

        private void RecalculateFaceNormals()
        {
            foreach (PlanetFace face in Faces)
                face.MeshFilter.sharedMesh.RecalculateNormals();
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, Planet> { }
    }
}