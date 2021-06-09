using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class PlainGeneratorTest : ZenjectUnitTestFixture
    {
		private const int _vertexCount = 4;
		private const int _expectedPlainHeight = 0;
		private const float _testSize = 3.16f;
		private const int _verticesPerTriangle = 3;
        private readonly int[] _expectedTriangleVertexIndices = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };
		private readonly Vector3 _expectedMeshOrigin = Vector3.zero;

		private Mesh _testMesh;
        private MeshGenerator _meshGenerator;

        [TearDown]
        public void Destruuct()
		{
            if (_testMesh != null)
                UnityEngine.Object.Destroy(_testMesh);
		}


        [Test(Description = "GenerateMeshFor creates a mesh with four vertices")]
        public void GenerateMeshFor_CreatesMeshWithFourVertices()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMesh();
            ThenTestMeshHasFourVertices();
        }

        [Test(Description = "GenerateMeshFor called with size one creates a mesh with four vertices")]
        public void GenerateMeshFor_WithSize_CreatesMeshWithFourVertices()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMeshWithSize(1.0f);
            ThenTestMeshHasFourVertices();
        }

        [Test(Description = "The generated vertices form a plain with side length one")]
        public void GenerateMeshFor_GeneratesPlainWithSideLengthOne()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMesh();
            ThenTestMeshIsAPlainWithSideLength(1.0f);
        }

        [Test(Description = "The generated vertices form a mesh on the x-z-plain")]
        public void GenerateMeshFor_GeneratesHorizontalPlain()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMesh();
            ThenTestMeshIsHorizontal();
        }

        [Test(Description = "The generated mesh has its center at Vector3.zero")]
        public void GenerateMeshFor_CenterOfTheMeshIsAtOrigin()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMesh();
            ThenTestMeshsCenterIsAtOrigin();
        }

        [Test(Description = "The mesh created by GenerateMeshFor with size has a side length of size.")]
        public void GenerateMeshFor_WithSize_CreatedMeshHasExpectedSideLength()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMeshWithSize(_testSize);
            ThenTestMeshIsAPlainWithSideLength(_testSize);
        }

        [Test(Description = "The generated mesh has two trangles.")]
        public void GenerateMeshFor_MeshHasTwoTriangles()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMesh();
            ThenTheGeneratedMeshHasTriangleCount(2);
        }

        [Test(Description = "The triangles of the generated mesh have expected vertex indices.")]
        public void GenerateMeshFor_TrianglesHaveExpectedVertexIndices()
        {
            GivenANewMeshGenerator();
            WhenGenerateMeshForIsCalledOnTestMesh();
            ThenTheTrianglesHaveExpectedVertexIndices();
        }

		private void GivenANewMeshGenerator()
		{
            Container.Bind<MeshGenerator>().To<PlainGenerator>().AsTransient();
            Container.Bind<Mesh>().AsTransient();
        }

        private void WhenGenerateMeshForIsCalledOnTestMesh()
        {
            _testMesh = Container.Resolve<Mesh>();
            _meshGenerator = Container.Resolve<MeshGenerator>();
            _meshGenerator.GenerateMeshFor(_testMesh);
        }

        private void WhenGenerateMeshForIsCalledOnTestMeshWithSize(float size)
        {
            _testMesh = Container.Resolve<Mesh>();
            _meshGenerator = Container.Resolve<MeshGenerator>();
            _meshGenerator.GenerateMeshFor(_testMesh, size);
        }

        private void ThenTestMeshHasFourVertices()
		{
            Assert.AreEqual(_vertexCount, _testMesh.vertices.Length);
        }

        private void ThenTestMeshIsAPlainWithSideLength(float expectedSideLength)
        {
            for(int i = 0; i < _testMesh.vertices.Length; i++)
			{
                Vector3 currentVertex = _testMesh.vertices[i];
                Vector3 nextVertex = _testMesh.vertices[(i+1)%_testMesh.vertices.Length];
                float sideLength = (currentVertex - nextVertex).magnitude;
                Assert.AreEqual(expectedSideLength, sideLength);
            }
        }

        private void ThenTestMeshIsHorizontal()
        {
            for (int i = 0; i < _testMesh.vertices.Length; i++)
            {
                Vector3 currentVertex = _testMesh.vertices[i];
                Vector3 nextVertex = _testMesh.vertices[(i + 1) % _testMesh.vertices.Length];
                Vector3 distanceVertex = currentVertex - nextVertex;
                Assert.AreEqual(_expectedPlainHeight, distanceVertex.y);
            }
        }

        private void ThenTestMeshsCenterIsAtOrigin()
        {
            Vector3 diaginalOne = _testMesh.vertices[2] - _testMesh.vertices[0];
            Vector3 diaginalTwo = _testMesh.vertices[3] - _testMesh.vertices[1];
            Assert.AreEqual(_expectedMeshOrigin, diaginalOne / 2 + _testMesh.vertices[0]);
            Assert.AreEqual(_expectedMeshOrigin, diaginalTwo / 2 + _testMesh.vertices[1]);
        }

        private void ThenTheGeneratedMeshHasTriangleCount(int expectedTriangleCount)
        {
            Assert.AreEqual(expectedTriangleCount * _verticesPerTriangle, _testMesh.triangles.Length);
        }

        private void ThenTheTrianglesHaveExpectedVertexIndices()
        {
            Assert.AreEqual(_expectedTriangleVertexIndices, _testMesh.triangles);
        }
    }
}