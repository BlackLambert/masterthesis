using UnityEngine;
using Zenject;

namespace SBaier.Master
{
	public class MeshOnStartCreator : MonoBehaviour
	{
		private MeshGenerator _generator;
		[SerializeField]
		private MeshFilter _meshFilter;
		[SerializeField]
		private float _size = 1f;

		[Inject]
		public void Construct(MeshGenerator generator)
		{
			_generator = generator;
		}

		protected virtual void Start()
		{
			_generator.GenerateMeshFor(_meshFilter.mesh, _size);
		}
	}
}