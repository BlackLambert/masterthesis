using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RecursiveMeshSubdividerTest : ZenjectUnitTestFixture
    {
		private const int _expectedVertexAmount = 12;
		private const int _testRecursionDepth = 3;
		private const int _minAmount = 1;
		private const int _additionalTrianglesPerSubdivision = 4;
		private Mesh _testMesh;
        private Mesh _secondTestMesh;
        private MeshSubdivider _subdivider;

        private readonly Vector3[] _testVertices = new Vector3[]
        {
            new Vector3(-2.6f, -5.2f, 1.5f),
            new Vector3(7.8f, -0.2f, 5.4f),
            new Vector3(1.2f, 4.3f, -0.5f),
            new Vector3(0, -6.6f, -2.5f),
            new Vector3(-4.1f, 10.3f, 3.9f),
        };

        private readonly int[] _testTriangleVertexIndices = new int[]
        {
            0, 1, 2,
            2, 1, 3,
            2, 3, 4,
        };

        private readonly List<int[]> _expectedTriangles = new List<int[]>()
        {
            new int[]{0, 5, 7},
            new int[]{7, 5, 6},
            new int[]{5, 1, 6},
            new int[]{7, 6, 2},
            new int[]{2, 6, 9},
            new int[]{6, 1, 8},
            new int[]{9, 8, 3},
            new int[]{6, 8, 9},
            new int[]{2, 9, 11},
            new int[]{9, 3, 10},
            new int[]{11, 10, 4},
            new int[]{11, 9, 10},
        };


        [TearDown]
        public void Destruct()
		{
            if (_testMesh != null)
                UnityEngine.Object.Destroy(_testMesh);
            if (_secondTestMesh != null)
                UnityEngine.Object.Destroy(_testMesh);
        }


        [Test(Description = "Subdivided Mesh has expected vertex count")]
        public void Subdivide_ResultHasExpectedVerticesCount()
        {
            GivenADefaultSetup();
            WhenSubdivideIsCalledOnTestMesh(_testMesh);
            ThenTestMeshHasExpectedVerticesCount();
        }
        
        [Test(Description = "Subdivided Mesh has vertices at expected positions")]
        public void Subdivide_ResultHasVerticesAtExpectedPositions()
        {
            GivenADefaultSetup();
            WhenSubdivideIsCalledOnTestMesh(_testMesh);
            ThenTestMeshHasVerticesAtExpectedPositions();
        }

        [Test(Description = "The Subdivided Mesh has expected triangles")]
        public void Subdivide_ResultHasExpectedTriangles()
        {
            GivenADefaultSetup();
            WhenSubdivideIsCalledOnTestMesh(_testMesh);
            ThenTestMeshHasExpectedTriangles();
        }

        [Test(Description = "The Subdivide Method throws an ArgumentOutOfRangeException if recursionDepth is smaller than one.")]
        public void Subdivide_WithAmount_ThrowsExceptionOnInvalidValue()
        {
            GivenADefaultSetup();
            TestDelegate test = () => WhenSubdivideIsCalledOnTestMeshWithAmount(_testMesh, 0);
            ThenThrowsArgumentOutOfRangeException(test);
            test = () => WhenSubdivideIsCalledOnTestMeshWithAmount(_testMesh, -3);
            ThenThrowsArgumentOutOfRangeException(test);
        }

        [Test(Description = "The resulting mesh from Subdivide equals the result from Subdivide with amount one")]
        public void Subdivide_EqualsSubdivideWithValueOne()
        {
            GivenADefaultSetup();
            GivenASecondTestMesh();
            WhenSubdivideIsCalledOnTestMesh(_testMesh);
            WhenSubdivideIsCalledOnTestMeshWithAmount(_secondTestMesh, _minAmount);
            ThenMeshesAreEqual(_testMesh, _secondTestMesh);
        }

        [Test(Description = "The result from Subdivide called with test amount has expected triangle count")]
        public void Subdivide_WithAmount_ResultHasExpectedTrianglesCount()
        {
            GivenADefaultSetup();
            WhenSubdivideIsCalledOnTestMeshWithAmount(_testMesh, _testRecursionDepth);
            int testTrianglesIndicesCount = _testTriangleVertexIndices.Length;
            int expectedTrianglesCount = (int) (Math.Pow(_additionalTrianglesPerSubdivision, _testRecursionDepth) * testTrianglesIndicesCount);
            ThenTestMeshHasExpectedTrianglesCount(expectedTrianglesCount);
        }

		private void GivenADefaultSetup()
		{
            Container.Bind<Mesh>().FromMethod(CreateMesh).AsTransient();
            Container.Bind<MeshSubdivider>().To<RecursiveMeshSubdivider>().AsTransient();

            _testMesh = Container.Resolve<Mesh>();
            _subdivider = Container.Resolve<MeshSubdivider>();
        }

        private void GivenASecondTestMesh()
        {
            _secondTestMesh = Container.Resolve<Mesh>();
        }

        private void WhenSubdivideIsCalledOnTestMesh(Mesh _testMesh)
        {
            _subdivider.Subdivide(_testMesh);
        }

        private void WhenSubdivideIsCalledOnTestMeshWithAmount(Mesh _testMesh, int recursionDepth)
        {
            _subdivider.Subdivide(_testMesh, recursionDepth);
        }

        private void ThenTestMeshHasExpectedVerticesCount()
        {
            Assert.AreEqual(_expectedVertexAmount, _testMesh.vertices.Length);
        }

        private void ThenTestMeshHasVerticesAtExpectedPositions()
        {
            foreach (Vector3 vertex in _testVertices)
                Assert.True(_testMesh.vertices.Contains(vertex));
            for(int i = 0; i < _testTriangleVertexIndices.Length / 3; i++)
			{
                Vector3 vertex0 = _testVertices[_testTriangleVertexIndices[i * 3]];
                Vector3 vertex1 = _testVertices[_testTriangleVertexIndices[i * 3 + 1]];
                Vector3 vertex2 = _testVertices[_testTriangleVertexIndices[i * 3 + 2]];
                Vector3 vertex3 = vertex0 + (vertex1 - vertex0) * 0.5f;
                Vector3 vertex4 = vertex1 + (vertex2 - vertex1) * 0.5f;
                Vector3 vertex5 = vertex2 + (vertex0 - vertex2) * 0.5f;
                Assert.True(_testMesh.vertices.Contains(vertex3, new Vector3EqualityComparer()));
                Assert.True(_testMesh.vertices.Contains(vertex4, new Vector3EqualityComparer()));
                Assert.True(_testMesh.vertices.Contains(vertex5, new Vector3EqualityComparer()));
            }
        }

        private void ThenTestMeshHasExpectedTriangles()
        {
            Assert.AreEqual(_expectedTriangles.Count, _testMesh.triangles.Length / 3);
            int[] triangles = _testMesh.triangles;
            for (int i = 0; i < triangles.Length / 3; i++)
			{
                int[] triangle1 = new int[] { triangles[i * 3], triangles[i * 3 + 1], triangles[i * 3 + 2] };
                int[] triangle2 = new int[] { triangles[i * 3 + 2], triangles[i * 3], triangles[i * 3 + 1] };
                int[] triangle3 = new int[] { triangles[i * 3 + 1], triangles[i * 3 + 2], triangles[i * 3] };

                bool contains = false;
                foreach (int[] triangle in _expectedTriangles)
                {
                    if (triangle.SequenceEqual(triangle1) || triangle.SequenceEqual(triangle2) || triangle.SequenceEqual(triangle3))
                    {
                        contains = true;
                        break;
                    }
                }

                Assert.True(contains);
            }
        }

        private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenMeshesAreEqual(Mesh testMesh, Mesh secondTestMesh)
        {
            Assert.True(testMesh.vertices.SequenceEqual(secondTestMesh.vertices));
            Assert.True(testMesh.triangles.SequenceEqual(secondTestMesh.triangles));
        }

        private void ThenTestMeshHasExpectedTrianglesCount(int expectedTrianglesCount)
        {
            Assert.AreEqual(expectedTrianglesCount, _testMesh.triangles.Length);
        }

        private Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = _testVertices;
            mesh.triangles = _testTriangleVertexIndices;
            return mesh;
        }
    }
}