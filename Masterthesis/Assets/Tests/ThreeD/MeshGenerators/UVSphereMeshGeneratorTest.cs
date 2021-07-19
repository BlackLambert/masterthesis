using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Linq;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class UVSphereMeshGeneratorTest : MeshGeneratorTest
    {
		private const float _epsilon = 0.001f;
		private int[] _testRingsAmount =
        {
            5, 2, 12, 1, 15
        };
        private int[] _invalidRingsAmount =
        {
            0, -2, -10, -1, -8
        };

        private int[] _testSegmentsAmount =
        {
            10, 4, 3, 15, 23
        };
        private int[] _invalidSegmentsAmount =
        {
            2, 1, 0, -3, -10
        };

        private float[] _testSizes =
        {
            0.45f, 0.02f, 1f, 2.75f, 13.5f
        };


        [Test(Description = "The constructor throws an ArgumentOutOfRangeException if provided with an invalid rings amount")]
        public void Constructor_ThrowsExpectionOnInvalidRingsAmount()
        {
            for(int i = 0; i < _invalidRingsAmount.Length; i++)
			{
                TestDelegate test = () => GivenAGeneratorWith(_invalidRingsAmount[i], _testSegmentsAmount[0]);
                ThenThrowsArgumentOutOfRangeException(test);
			}
        }

        [Test(Description = "The constructor throws an ArgumentOutOfRangeException if provided with an invalid segments amount")]
        public void Constructor_ThrowsExpectionOnInvalidSegmentsAmount()
        {
            for(int i = 0; i < _invalidSegmentsAmount.Length; i++)
			{
                TestDelegate test = () => GivenAGeneratorWith(_testRingsAmount[0], _invalidSegmentsAmount[i]);
                ThenThrowsArgumentOutOfRangeException(test);
			}
        }

        [Test(Description = "The generated mesh has expected vertex amount")]
        public void GenerateMesh_GeneratedMeshHasExpectedVertexAmount()
        {
            for(int i = 0; i < _testRingsAmount.Length; i++)
			{
                GivenAGeneratorWith(_testRingsAmount[i], _testSegmentsAmount[i]);
                GivenATestMesh();
                Mesh mesh = Container.Resolve<Mesh>();
                WhenGenerateMeshIsCalledOn(mesh);
                ThenMeshHasExpectedVertexAmount(mesh, _testRingsAmount[i], _testSegmentsAmount[i]);
                GameObject.Destroy(mesh);
                Teardown();
                Setup();
			}
        }

        [Test(Description = "The vertices of the generated mesh have expected magnitude")]
        public void GenerateMesh_VerticesHaveExpectedMagnitude()
        {
            for(int i = 0; i < _testRingsAmount.Length; i++)
			{
                GivenAGeneratorWith(_testRingsAmount[i], _testSegmentsAmount[i]);
                GivenATestMesh();
                Mesh mesh = Container.Resolve<Mesh>();
                WhenGenerateMeshIsCalledOn(mesh, _testSizes[i]);
                ThenVerticesOfMeshHaveExpectedMagnitude(mesh, _testSizes[i]);
                GameObject.Destroy(mesh);
                Teardown();
                Setup();
			}
        }

        [Test(Description = "The triangles amount of the generated is as expected")]
        public void GenerateMesh_TrianglesAmountIsAsExpected()
        {
            for(int i = 0; i < _testRingsAmount.Length; i++)
			{
                GivenAGeneratorWith(_testRingsAmount[i], _testSegmentsAmount[i]);
                GivenATestMesh();
                Mesh mesh = Container.Resolve<Mesh>();
                WhenGenerateMeshIsCalledOn(mesh);
                ThenNumberOfTrianglesIsAsExpectedOf(mesh, _testRingsAmount[i], _testSegmentsAmount[i]);
                GameObject.Destroy(mesh);
                Teardown();
                Setup();
			}
        }

        [Test(Description = "The generated vertices are at expected positions")]
        public void GenerateMesh_VerticesHaveExpectedPositions()
        {
            for(int i = 0; i < _testRingsAmount.Length; i++)
			{
                GivenAGeneratorWith(_testRingsAmount[i], _testSegmentsAmount[i]);
                GivenATestMesh();
                Mesh mesh = Container.Resolve<Mesh>();
                WhenGenerateMeshIsCalledOn(mesh);
                ThenVerticesHaveExpectedPositions(mesh, _testRingsAmount[i], _testSegmentsAmount[i]);
                GameObject.Destroy(mesh);
                Teardown();
                Setup();
			}
        }

        [Test(Description = "The vertex normals equal the normalized vertex position after recalculate normals")]
        public void GenerateMesh_VertexNormalsEqualTheNormalizedVertexPosition()
        {
            for(int i = 0; i < _testRingsAmount.Length; i++)
			{
                GivenAGeneratorWith(_testRingsAmount[i], _testSegmentsAmount[i]);
                GivenATestMesh();
                Mesh mesh = Container.Resolve<Mesh>();
                WhenGenerateMeshIsCalledOn(mesh);
                ThenNormalsEqualNormalizedVertexPositions(mesh);
                GameObject.Destroy(mesh);
                Teardown();
                Setup();
			}
        }

        [Test(Description = "There should be no duplicate vertices within the mesh vertices array")]
        public void GenerateMesh_MeshContainsNoDoubleVertices()
        {
            for (int i = 0; i < _testRingsAmount.Length; i++)
            {
                GivenAGeneratorWith(_testRingsAmount[i], _testSegmentsAmount[i]);
                GivenATestMesh();
                Mesh mesh = Container.Resolve<Mesh>();
                WhenGenerateMeshIsCalledOn(mesh);
                ThenMeshContainsNoDoubleVertices(mesh);
                GameObject.Destroy(mesh);
                Teardown();
                Setup();
            }
        }

		private void GivenATestMesh()
		{
            Container.Bind<Mesh>().AsTransient();
		}

		protected override MeshGenerator GivenANewMeshGenerator()
		{
            GivenAGeneratorWith(_testRingsAmount[0], _testSegmentsAmount[0]);
            return Container.Resolve<MeshGenerator>();
        }

        private void WhenGenerateMeshIsCalledOn(Mesh mesh)
        {
            MeshGenerator generator = Container.Resolve<MeshGenerator>();
            generator.GenerateMeshFor(mesh);
        }

        private void WhenGenerateMeshIsCalledOn(Mesh mesh, float size)
        {
            MeshGenerator generator = Container.Resolve<MeshGenerator>();
            generator.GenerateMeshFor(mesh, size);
        }

        private void GivenAGeneratorWith(int ringsAmount, int segmentsAmount)
		{
            UVSphereMeshGenerator generator = new UVSphereMeshGenerator(ringsAmount, segmentsAmount);
            Container.Bind(typeof(MeshGenerator), typeof(UVSphereMeshGenerator)).To<UVSphereMeshGenerator>().FromInstance(generator).AsTransient();
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenVerticesOfMeshHaveExpectedMagnitude(Mesh mesh, float size)
        {
            Vector3[] vertices = mesh.vertices;
            foreach (Vector3 vertex in vertices)
                Assert.AreEqual(size, vertex.magnitude, _epsilon);
        }

        private void ThenMeshHasExpectedVertexAmount(Mesh mesh, int ringsAmount, int segmentsAmount)
        {
            int expected = GetExpectedVertexAmount(ringsAmount, segmentsAmount);
            Assert.AreEqual(expected, mesh.vertices.Length);
        }

        private void ThenNumberOfTrianglesIsAsExpectedOf(Mesh mesh, int ringsAmount, int segmentsAmount)
        {
            int expected = ringsAmount * segmentsAmount * 2;
            int actual = mesh.triangles.Length / 3;
            Assert.AreEqual(expected, actual);
        }

        private void ThenVerticesHaveExpectedPositions(Mesh mesh, int ringsAmount, int segmentsAmount)
        {
            Vector3[] actual = mesh.vertices;
            Vector3[] expected = GetExpectedVertices(ringsAmount, segmentsAmount);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].normalized.x, actual[i].normalized.x, _epsilon);
                Assert.AreEqual(expected[i].normalized.y, actual[i].normalized.y, _epsilon);
                Assert.AreEqual(expected[i].normalized.z, actual[i].normalized.z, _epsilon);
            }
        }

        private void ThenNormalsEqualNormalizedVertexPositions(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                Assert.AreEqual(vertices[i].normalized.x, normals[i].x, _epsilon);
                Assert.AreEqual(vertices[i].normalized.y, normals[i].y, _epsilon);
                Assert.AreEqual(vertices[i].normalized.z, normals[i].z, _epsilon);
            }
        }

        private void ThenMeshContainsNoDoubleVertices(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            foreach(Vector3 vertex in vertices)
			{
                int sum = vertices.Count(v => v == vertex);
                Assert.AreEqual(1, sum, $"There are {sum} instances of {vertex} in vertices where there should only be one.");
            }
        }

        private int GetExpectedVertexAmount(int ringsAmount, int segmentsAmount)
		{
            int poleVertices = 2;
            int otherVertices = ringsAmount * segmentsAmount;
            return poleVertices + otherVertices;
        }

        private Vector3[] GetExpectedVertices(int ringsAmount, int segmentsAmount)
		{
            Vector3[] result = new Vector3[GetExpectedVertexAmount(ringsAmount, segmentsAmount)];
            float segmentDetla = (1f / segmentsAmount * Mathf.PI) * 2;
            float ringsDetla = 1f / (ringsAmount + 1) * Mathf.PI;

            result[0] = Vector3.down;
            result[result.Length - 1] = Vector3.up;
            for (int ring = 1; ring <= ringsAmount; ring++)
            {
                float ringAngle = Mathf.PI - (ringsDetla * ring);
                float sinRingAngle = Mathf.Sin(ringAngle);
                float cosRingAngle = Mathf.Cos(ringAngle);

                for (int segment = 0; segment < segmentsAmount; segment++)
                {
                    float segmentAngle = segmentDetla * segment;
                    float sinSegmentAngle = Mathf.Sin(segmentAngle);
                    float cosSegmentAngle = Mathf.Cos(segmentAngle);

                    float x = sinRingAngle * cosSegmentAngle;
                    float z = sinRingAngle * sinSegmentAngle;
                    float y = cosRingAngle;

                    result[1 + (ring - 1) * segmentsAmount + segment] = new Vector3(x, y, z);
                }
            }

            return result;
        }
    }
}