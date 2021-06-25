using UnityEngine;

namespace SBaier.Master
{
    public class PlanetFace : MonoBehaviour, MeshFaceSeparatorTarget
    {
        [SerializeField]
        private MeshFilter _meshFilter;
		public MeshFilter MeshFilter => _meshFilter;

        public Transform Base => transform;
	}
}