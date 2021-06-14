using UnityEngine;

namespace SBaier.Master
{
	public class MeshNormalsOnStartRecalculator : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter _meshFilter;

		protected virtual void Start()
		{
			_meshFilter.sharedMesh.RecalculateNormals();
		}
	}
}