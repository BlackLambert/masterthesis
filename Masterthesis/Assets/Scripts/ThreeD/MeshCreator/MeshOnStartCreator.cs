using UnityEngine;
using Zenject;

namespace SBaier.Master
{
	public class MeshOnStartCreator : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter _meshFilter;
		[SerializeField]
		private MeshGeneratorSettings _generatorSettings;
		[SerializeField]
		private float _size = 1f;

		private MeshGeneratorFactory _factory;
		private MeshGenerator _generator;

		[Inject]
		public void Construct(MeshGeneratorFactory factory)
		{
			_factory = factory;
		}

		protected virtual void Start()
		{
			_generator = _factory.Create(_generatorSettings);
			_generator.GenerateMeshFor(_meshFilter.mesh, _size);
		}
	}
}