using Zenject;
using NUnit.Framework;
using SBaier.Master;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class IcosahedronGeneratorTest : MeshGeneratorTest
	{
		private IcosahedronGenerator _generator;
		private Mesh _mesh;
		private const float _testSize = 2.36f;

		public override void Teardown()
		{
			base.Teardown();
			if (_mesh != null)
				GameObject.Destroy(_mesh);
		}

		[Test]
        public void GeneratedMeshHasExpectedVertexCount()
		{
			GivenADefaultSetup();
			WhenMeshOfUnitIcosahedronIsGenerated();
			ThenVertexCountIsAsExpected();
		}

		[Test]
		public void AllVerticesHaveDistanceOneToCenter()
		{
			GivenADefaultSetup();
			WhenMeshOfUnitIcosahedronIsGenerated();
			ThenTheVertexDistanceToTheBodyCenterIs(1.0f);
		}

		[Test]
		public void AllVerticesHaveProvidedDistanceToCenter()
		{
			GivenADefaultSetup();
			WhenMeshOfCustomIcosahedronIsGenerated();
			ThenTheVertexDistanceToTheBodyCenterIs(_testSize);
		}

		[Test]
		public void AllVerticesHaveExpectedPositions()
		{
			GivenADefaultSetup();
			WhenMeshOfUnitIcosahedronIsGenerated();
			ThenTheNormalizedVertexPositionsAreAsExpected();
		}

		[Test]
		public void TriangleIndexCountIsAsExpected()
		{
			GivenADefaultSetup();
			WhenMeshOfUnitIcosahedronIsGenerated();
			ThenTheTriangleIndexCountIsAsExpected();
		}

		[Test]
		public void AllTriangleIndicesAreAsExpected()
		{
			GivenADefaultSetup();
			WhenMeshOfUnitIcosahedronIsGenerated();
			ThenTheTriangleIndicesAreAsExpected();
		}

		private void GivenADefaultSetup()
		{
			GivenAGenerator();
			GivenATestMesh();
		}

		protected override MeshGenerator GivenANewMeshGenerator()
		{
			return new IcosahedronGenerator();
		}

		private void GivenAGenerator()
		{
			_generator = new IcosahedronGenerator();
		}

		private void GivenATestMesh()
		{
			_mesh = new Mesh();
		}

		private void WhenMeshOfUnitIcosahedronIsGenerated()
		{
			_generator.GenerateMeshFor(_mesh);
		}

		private void WhenMeshOfCustomIcosahedronIsGenerated()
		{
			_generator.GenerateMeshFor(_mesh, _testSize);
		}

		private void ThenVertexCountIsAsExpected()
		{
			Assert.AreEqual(Icosahedron.Vertices.Length, _mesh.vertices.Length);
		}

		private void ThenTheVertexDistanceToTheBodyCenterIs(float distanceToCenter)
		{
			foreach (Vector3 vertex in _mesh.vertices)
				Assert.AreEqual(distanceToCenter, vertex.magnitude);
		}

		private void ThenTheNormalizedVertexPositionsAreAsExpected()
		{
			for(int i = 0; i < _mesh.vertices.Length; i++)
			{
				Vector3 normalizedPosition = _mesh.vertices[i].normalized;
				Assert.AreEqual(Icosahedron.Vertices[i].normalized, normalizedPosition);
			}
		}

		private void ThenTheTriangleIndexCountIsAsExpected()
		{
			Assert.AreEqual(Icosahedron.Triangles.Length, _mesh.triangles.Length);
		}

		private void ThenTheTriangleIndicesAreAsExpected()
		{
			for (int i = 0; i < _mesh.triangles.Length; i++)
				Assert.AreEqual(Icosahedron.Triangles[i], _mesh.triangles[i]);
		}
	}
}