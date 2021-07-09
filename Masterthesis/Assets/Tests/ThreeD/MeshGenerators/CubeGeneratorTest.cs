using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class CubeGeneratorTest : MeshGeneratorTest
    {
		private const float _epsilon = 0.001f;
		private Mesh _mesh;
		private MeshGenerator _meshGenerator;

		private float[] _testSizes = new float[]
		{
			1.23f,
			4.94f,
			1f,
			7.14f,
			0.23f
		};

		private float[] _invalidSizes = new float[]
		{
			0,
			-0.24f,
			-4.76f,
			-12.56f
		};

		public override void Teardown()
		{
			base.Teardown();
			if (_mesh != null)
				GameObject.Destroy(_mesh);
		}

		[Test (Description = "Created mesh has expected vertex positions ")]
        public void GenerateMeshFor_CubeHasExpectedVertices()
        {
            GivenADefaultSetup();
            WhenGenerateIsCalled();
            ThenTheMeshHasExpectedVertices();
		}

		[Test(Description = "Created mesh has expected vertex indices")]
		public void GenerateMeshFor_CubeHasExpectedVertexIndices()
		{
			GivenADefaultSetup();
			WhenGenerateIsCalled();
			ThenTheMeshHasExpectedVertexIndices();
		}

		[Test(Description = "Created mesh has expected side size")]
		public void GenerateMeshFor_CubeHasExpectedSideLength()
		{
			GivenADefaultSetup();
			WhenGenerateIsCalled();
			ThenTheCubeHasExpectedSideLength(1f);
		}

		[Test(Description = "Created mesh has expected side size")]
		public void GenerateMeshFor_WithSize_CubeHasExpectedSideLength()
		{
			for (int i = 0; i < _testSizes.Length; i++)
			{
				GivenADefaultSetup();
				WhenGenerateIsCalled(_testSizes[i]);
				ThenTheCubeHasExpectedSideLength(_testSizes[i]);
				Teardown();
				Setup();
			}
		}

		[Test(Description = "Created mesh has expected side size")]
		public void GenerateMeshFor_WithSize_InvalidSizeThrowsException()
		{
			for (int i = 0; i < _invalidSizes.Length; i++)
			{
				GivenADefaultSetup();
				TestDelegate test = () => WhenGenerateIsCalled(_invalidSizes[i]);
				ThenThrowsArgumentOutOfRangeException(test);
				Teardown();
				Setup();
			}
		}

		protected override MeshGenerator GivenANewMeshGenerator()
		{
			GivenADefaultSetup();
			return Container.Resolve<MeshGenerator>();
		}

		private void GivenADefaultSetup()
		{
			Container.Bind<MeshGenerator>().To<CubeMeshGenerator>().AsTransient();
			Container.Bind<Mesh>().AsTransient();

			_mesh = Container.Resolve<Mesh>();
			_meshGenerator = Container.Resolve<MeshGenerator>();
		}

		private void WhenGenerateIsCalled()
		{
			_meshGenerator.GenerateMeshFor(_mesh);
		}

		private void WhenGenerateIsCalled(float size)
		{
			_meshGenerator.GenerateMeshFor(_mesh, size);
		}

		private void ThenTheMeshHasExpectedVertices()
		{
			Vector3[] vertices = _mesh.vertices;
			Vector3[] expected = Cube.Vertices;

			for(int i = 0; i < expected.Length; i++)
				Assert.AreEqual(expected[i].normalized, vertices[i].normalized);
		}

		private void ThenTheMeshHasExpectedVertexIndices()
		{
			int[] expected = Cube.VertexIndices;
			int[] actual = _mesh.triangles;
			Assert.AreEqual(expected, actual);
		}

		private void ThenTheCubeHasExpectedSideLength(float sideLength)
		{
			Vector3[] vertices = _mesh.vertices;
			int[] triangles = _mesh.triangles;
			float diagonalLength = Mathf.Sqrt(sideLength * sideLength + sideLength * sideLength);

			for(int i = 0; i < triangles.Length / 3; i++)
			{
				Vector3 v0 = vertices[triangles[i * 3]];
				Vector3 v1 = vertices[triangles[i * 3 + 1]];
				Vector3 v2 = vertices[triangles[i * 3 + 2]];

				Vector3 v10 = v1 - v0;
				Vector3 v21 = v2 - v1;
				Vector3 v02 = v0 - v2;

				if (v10.magnitude > sideLength + _epsilon)
					Assert.AreEqual(diagonalLength, v10.magnitude, _epsilon);
				else
					Assert.AreEqual(sideLength, v10.magnitude, _epsilon);

				if (v21.magnitude > sideLength + _epsilon)
					Assert.AreEqual(diagonalLength, v21.magnitude, _epsilon);
				else
					Assert.AreEqual(sideLength, v21.magnitude, _epsilon);

				if (v02.magnitude > sideLength + _epsilon)
					Assert.AreEqual(diagonalLength, v02.magnitude, _epsilon);
				else
					Assert.AreEqual(sideLength, v02.magnitude, _epsilon);
			}
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}
	}
}