using Zenject;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class MeshSubdividerTest : Zenject.ZenjectUnitTestFixture
    {
        protected readonly Vector3[] _testVertices = new Vector3[]
        {
            new Vector3(-2.6f, -5.2f, 1.5f),
            new Vector3(7.8f, -0.2f, 5.4f),
            new Vector3(1.2f, 4.3f, -0.5f),
            new Vector3(0, -6.6f, -2.5f),
            new Vector3(-4.1f, 10.3f, 3.9f),
        };

        protected readonly int[] _testVertexIndices = new int[]
        {
            0, 1, 2,
            2, 1, 3,
            2, 3, 4,
        };

        

        private readonly int[] _invalidAmountValues = new int[]
        {
            -3, -10, -12, -21, -1
        };

        private readonly int[] _validAmountValues = new int[]
        {
            1, 2, 3, 4
        };

        private Mesh _testMesh;
        private Mesh _secondTestMesh;
        private MeshSubdivider _subdivider;

		[TearDown]
        public void Destruct()
        {
            if (_testMesh != null)
                UnityEngine.Object.Destroy(_testMesh);
            if (_secondTestMesh != null)
                UnityEngine.Object.Destroy(_testMesh);
        }

        [Test(Description = "The resulting mesh from Subdivide equals the result from Subdivide with amount one")]
        public void Subdivide_EqualsSubdivideWithValueOne()
        {
            GivenADefaultSetup();
            GivenASecondTestMesh();
            WhenSubdivideIsCalledOnTestMesh(_testMesh);
            WhenSubdivideIsCalledOnTestMeshWithAmount(_secondTestMesh, 1);
            ThenMeshesAreEqual(_testMesh, _secondTestMesh);
        }

        [Test(Description = "The Subdivide Method throws an ArgumentOutOfRangeException if recursionDepth is smaller than one.")]
        public void Subdivide_WithAmount_ThrowsExceptionOnInvalidValue()
        {
            GivenADefaultSetup();
            _subdivider = Container.Resolve<MeshSubdivider>();
            Mesh mesh = Container.Resolve<Mesh>();
            foreach (int amount in _invalidAmountValues)
            {
                TestDelegate test = () => WhenSubdivideIsCalledOnTestMeshWithAmount(mesh, amount);
                ThenThrowsArgumentOutOfRangeException(test);
            }
        }

        [Test(Description = "The result from Subdivide called with test amount has expected triangle count")]
        public void Subdivide_WithAmount_ResultHasExpectedTrianglesCount()
        {
            foreach(int amount in _validAmountValues)
			{
                GivenADefaultSetup();
                WhenSubdivideIsCalledOnTestMeshWithAmount(_testMesh, amount);
                int expected = GetExpectedTrianglesAmountFor(amount);
                ThenTestMeshHasExpectedTrianglesCount(expected);
                Teardown();
                Setup();
            }
        }

        [Test(Description = "The result from Subdivide called with test amount has expected vertex count")]
        public void Subdivide_WithAmount_ResultHasExpectedVertexCount()
        {
            foreach (int amount in _validAmountValues)
            {
                GivenADefaultSetup();
                WhenSubdivideIsCalledOnTestMeshWithAmount(_testMesh, amount);
                int expected = GetExpectedVertexAmountFor(amount);
                ThenTestMeshHasExpectedVertexCount(expected);
                Teardown();
                Setup();
            }
        }

		protected abstract int GetExpectedTrianglesAmountFor(int amount);
        protected abstract int GetVerticesPerEdgeFor(int amount);

		private void GivenADefaultSetup()
		{
            GivenAMeshSubdivider();
            Container.Bind<Mesh>().FromMethod(CreateMesh).AsTransient();

            _subdivider = Container.Resolve<MeshSubdivider>();
            _testMesh = Container.Resolve<Mesh>();
        }

		protected abstract void GivenAMeshSubdivider();

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
            Assert.AreEqual(expectedTrianglesCount, _testMesh.triangles.Length / 3);
        }

        private void ThenTestMeshHasExpectedVertexCount(int expectedVertexCount)
        {
            Assert.AreEqual(expectedVertexCount, _testMesh.vertexCount);
        }

        private Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = _testVertices;
            mesh.triangles = _testVertexIndices;
            return mesh;
        }

        private int GetExpectedVertexAmountFor(int amount)
        {
            int sharedTriangleEdges = GetSharedTriangleEdges(_testVertexIndices);
            int verticesPerEdge = GetVerticesPerEdgeFor(amount);
            int triangles = _testVertexIndices.Length / 3;
            int maxVertices = ((verticesPerEdge * verticesPerEdge + verticesPerEdge) / 2) * triangles;
            int sharedVertices = sharedTriangleEdges * verticesPerEdge;
            return maxVertices - sharedVertices;
        }

        private int GetSharedTriangleEdges(int[] testVertexIndices)
        {
            HashSet<Vector2Int> edges = new HashSet<Vector2Int>();
            int sharedCount = 0;

            for (int i = 0; i < testVertexIndices.Length / 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int i1 = i * 3 + j;
                    int i2 = i * 3 + (j + 1) % 3;
                    Vector2Int edge = new Vector2Int(testVertexIndices[i1], testVertexIndices[i2]);
                    Vector2Int edgeReversed = new Vector2Int(testVertexIndices[i2], testVertexIndices[i1]);
                    if (edges.Contains(edge))
                        sharedCount++;
                    else
                    {
                        edges.Add(edge);
                        edges.Add(edgeReversed);
                    }
                }
            }

            return sharedCount;
        }
    }
}