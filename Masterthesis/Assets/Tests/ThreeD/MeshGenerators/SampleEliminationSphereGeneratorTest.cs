using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Linq;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class SampleEliminationSphereGeneratorTest : MeshGeneratorTest
	{
		private readonly int[] _testTargetSamples = new int[]
		{
			10, 20, 30, 50
		};

		private readonly int[] _invalidTargetSamples = new int[]
		{
			3, 0, -12, -1000
		};

		private readonly float[] _testBaseSamplesFactor = new float[]
		{
			2.0f, 3.0f, 1.5f, 5.0f
		};

		private readonly float[] _invalidBaseSamplesFactor = new float[]
		{
			1.0f, 0.0f, -2.0f, -5f
		};

		private readonly float[] _testRadius = new float[]
		{
			2.0f, 3.0f, 1.5f, 5.0f, 0.21f
		};

		private readonly int[] _seeds = new int[]
		{
			1234, 10, 42
		};

		private const float _epsilon = 0.0001f;

		private SampleEliminationSphereGenerator _generator;
		private Mesh _mesh;

		protected override MeshGenerator GivenANewMeshGenerator()
		{
			BindSeed(_seeds[0]);
			BindSampleEliminationSphereGenerator(_testTargetSamples[0], _testBaseSamplesFactor[0]);
			return Container.Resolve<MeshGenerator>();
		}

		[TearDown]
		public override void Teardown()
		{
			base.Teardown();
			if (_mesh != null)
				GameObject.Destroy(_mesh);
		}

		[Test]
		public void Constructor_ThrowsExceptionOnInvalidTargetSamples()
		{
			for (int i = 0; i < _invalidTargetSamples.Length; i++)
			{
				TestDelegate test = () => GivenASampleEliminationSphereGenerator(_invalidTargetSamples[i], _testBaseSamplesFactor[0], _seeds[0]);
				ThenThrowsArgumentOutOfRangeException(test);
				Teardown();
				Setup();
			}
		}

		[Test]
		public void Constructor_ThrowsExceptionOnInvalidBaseSamplesFactor()
		{
			for (int i = 0; i < _invalidBaseSamplesFactor.Length; i++)
			{
				TestDelegate test = () => GivenASampleEliminationSphereGenerator(_testTargetSamples[0], _invalidBaseSamplesFactor[i], _seeds[0]);
				ThenThrowsArgumentOutOfRangeException(test);
				Teardown();
				Setup();
			}
		}

		[Test]
		public void GenerateMeshFor_VerticesOfMeshLayOnUnitSphere()
		{
			for (int i = 0; i < _testTargetSamples.Length; i++)
			{
				for (int j = 0; j < _testBaseSamplesFactor.Length; j++)
				{
					GivenASampleEliminationSphereGenerator(_testTargetSamples[i], _testBaseSamplesFactor[j], _seeds[0]);
					GivenASampleMesh();
					WhenGenerateMeshForIsCalledOn(_mesh);
					ThenVerticesLayOnSphere(_mesh, 1);
					Teardown();
					Setup();
				}
			}
		}

		[Test]
		public void GenerateMeshFor_MeshHasExpectedVertexCount()
		{
			for (int i = 0; i < _testTargetSamples.Length; i++)
			{
				for (int j = 0; j < _testBaseSamplesFactor.Length; j++)
				{
					GivenASampleEliminationSphereGenerator(_testTargetSamples[i], _testBaseSamplesFactor[j], _seeds[0]);
					GivenASampleMesh();
					WhenGenerateMeshForIsCalledOn(_mesh);
					ThenMeshHasExpectedVertexCount(_mesh, _testTargetSamples[i]);
					Teardown();
					Setup();
				}
			}
		}

		[Test]
		public void GenerateMeshFor_WithSize_MeshHasVerticesOnSphereWithProvidedRadius()
		{
			for (int i = 0; i < _testTargetSamples.Length; i++)
			{
				for (int j = 0; j < _testBaseSamplesFactor.Length; j++)
				{
					for (int k = 0; k < _testRadius.Length; k++)
					{
						GivenASampleEliminationSphereGenerator(_testTargetSamples[i], _testBaseSamplesFactor[j], _seeds[0]);
						GivenASampleMesh();
						WhenGenerateMeshForIsCalledOn(_mesh, _testRadius[k]);
						ThenVerticesLayOnSphere(_mesh, _testRadius[k]);
						Teardown();
						Setup();
					}
				}
			}
		}

		[Test]
		public void GenerateMeshFor_GeneratesDifferentVerticesOnDifferentSeeds()
		{
			for (int i = 0; i < _testTargetSamples.Length; i++)
			{
				for (int j = 0; j < _testBaseSamplesFactor.Length; j++)
				{
					Vector3[] vertices = new Vector3[0];
					for (int k = 0; k < _seeds.Length; k++)
					{
						GivenASampleEliminationSphereGenerator(_testTargetSamples[i], _testBaseSamplesFactor[j], _seeds[k]);
						GivenASampleMesh();
						WhenGenerateMeshForIsCalledOn(_mesh);
						ThenMeshVerticesDifferFrom(_mesh, vertices);
						vertices = _mesh.vertices;
						Teardown();
						Setup();
					}
				}
			}
		}

		private void GivenASampleMesh()
		{
			Container.Bind<Mesh>().AsTransient();
			_mesh = Container.Resolve<Mesh>();
		}

		private void GivenASampleEliminationSphereGenerator(int targetSamples, float baseSamplesFactor, int seed)
		{
			BindSeed(seed);
			BindSampleEliminationSphereGenerator(targetSamples, baseSamplesFactor);
			_generator = Container.Resolve<SampleEliminationSphereGenerator>();
		}

		private void BindSampleEliminationSphereGenerator(int targetSamples, float baseSamplesFactor)
		{
			Seed seed = Container.Resolve<Seed>();
			Func<Vector3, int, float> compareValueSelect = (p, i) => p[i];
			QuickSelector<Vector3> selector = new QuickSorter<Vector3, float>(compareValueSelect);
			Vector3BinaryKDTreeFactory treeFactory = new Vector3BinaryKDTreeFactory(selector);
			RandomPointsOnSphereGenerator randomPointsGenerator = new RandomPointsOnSphereGenerator(seed);
			SampleElimination3D sampleElimination = new SampleElimination3D(treeFactory);
			SampleEliminationSphereGenerator generator = new SampleEliminationSphereGenerator(targetSamples, baseSamplesFactor, sampleElimination, randomPointsGenerator, selector);
			Container.Bind(typeof(MeshGenerator), typeof(SampleEliminationSphereGenerator)).To<SampleEliminationSphereGenerator>().FromInstance(generator).AsTransient();
		}

		private void BindSeed(int seed)
		{
			Container.Bind<Seed>().AsTransient().WithArguments(seed);
		}

		private void WhenGenerateMeshForIsCalledOn(Mesh mesh, float radius = 1)
		{
			if (radius == 1)
				_generator.GenerateMeshFor(mesh);
			else
				_generator.GenerateMeshFor(mesh, radius);
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}

		private void ThenVerticesLayOnSphere(Mesh mesh, float radius)
		{
			Vector3[] vertices = mesh.vertices;
			foreach (Vector3 vertex in vertices)
				Assert.AreEqual(radius, vertex.magnitude, _epsilon);
		}

		private void ThenMeshHasExpectedVertexCount(Mesh mesh, int targetSamples)
		{
			Assert.AreEqual(targetSamples, mesh.vertices.Length);
		}

		private void ThenMeshVerticesDifferFrom(Mesh mesh, Vector3[] vertices)
		{
			if (mesh.vertexCount == 0)
				return;
			Assert.AreNotEqual(vertices, mesh.vertices);
		}
	}
}