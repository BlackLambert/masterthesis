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
        [SerializeField]
        private Transform _atmoSphere;

        public PlanetFace[] Faces { get; private set; }
        public PlanetData Data { get; private set; }
        public float AtmosphereRadius { get; private set; }

        [Inject]
        public void Construct(PlanetData data)
		{
            Data = data;
            AtmosphereRadius = data.Dimensions.AtmosphereRadius;
            UpdateAtmosphere();
        }

		public void Init(PlanetFace[] faces)
		{
            Faces = faces;

            AttatchFaces();
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            UpdateVertexPositions();
            RecalculateFaceNormals();
        }

        public void Destruct()
		{
			foreach (PlanetFace face in Faces)
                face.Destruct();
            Destroy(gameObject);
            Destroy(_atmoSphere.GetComponent<MeshFilter>().mesh);
		}

        private void UpdateAtmosphere()
        {
            _atmoSphere.localScale = Vector3.one.FastMultiply(AtmosphereRadius * 2);
        }

        private void AttatchFaces()
		{
			foreach (PlanetFace face in Faces)
                face.Base.SetParent(_facesHook, false);
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

        public float GetDistanceOnSurface(Vector3 p0, Vector3 p1)
		{
            p0 = p0.normalized.FastMultiply(AtmosphereRadius);
            p1 = p1.normalized.FastMultiply(AtmosphereRadius);
            return p0.FastSubstract(p1).magnitude;
        }

        public class Factory : PlaceholderFactory<PlanetData, Planet> { }
    }
}