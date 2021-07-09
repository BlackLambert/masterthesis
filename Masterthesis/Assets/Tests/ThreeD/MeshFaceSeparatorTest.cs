using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;
using Moq;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class MeshFaceSeparatorTest : Zenject.ZenjectUnitTestFixture
    {
        private readonly Vector3[] _testVertices = new Vector3[]
        {
            Vector3.zero,
            Vector3.up,
            Vector3.left,
            new Vector3(5.1f, 1.3f, 0.2f),
            new Vector3(0.3f, -1.2f, 3.4f)
        };

        private readonly int[] _testFaces = new int[]
        {
            0, 1, 2, 
            2, 1, 3, 
            3, 2, 4,
            4, 3, 0
        };
        private int ExpectedTargetsAmount => _testFaces.Length / 3;
        private int[] ExpectedTargets => new int[] { 0, 1, 2 };

        private Mesh _testMesh;
        private MeshFaceSeparator _separator;
        private MeshFaceSeparatorTarget[] _result;


        [Test(Description = "The amount of created targets is as expected")]
        public void Separate_ReturnsExpectedAmountOfCreatedTargets()
        {
            GivenADefaultSetup();
            WhenSeparateIsCallesOn(_testMesh);
            ThenResultHasExpectedLength();
        }

        [Test(Description = "The created targets have meshes with expected triangles")]
        public void Separate_ReturnsTargetMeshesWithExpectedTriangles()
        {
            GivenADefaultSetup();
            WhenSeparateIsCallesOn(_testMesh);
            ThenResultHasExpectedTriangles();
        }

        [Test(Description = "The created targets have meshes with expected vertices")]
        public void Separate_ReturnsTargetMeshesWithExpectedVertices()
        {
            GivenADefaultSetup();
            WhenSeparateIsCallesOn(_testMesh);
            ThenResultHasExpectedVertices();
        }

		public override void Teardown()
		{
			base.Teardown();

		}

		private void GivenADefaultSetup()
		{
            Container.Bind<IFactory<MeshFaceSeparatorTarget>>().FromMethod(CreateTargetFactoryMock).AsTransient();
            Container.Bind<MeshFaceSeparator>().AsTransient();

            _testMesh = CreateTestMesh();
            _separator = Container.Resolve<MeshFaceSeparator>();
        }

		private void WhenSeparateIsCallesOn(Mesh mesh)
		{
            _result = _separator.Separate(mesh);
        }

        private void ThenResultHasExpectedLength()
        {
            Assert.AreEqual(ExpectedTargetsAmount, _result.Length);
        }

        private void ThenResultHasExpectedTriangles()
        {
            foreach (MeshFaceSeparatorTarget target in _result)
                Assert.AreEqual(ExpectedTargets, target.MeshFilter.sharedMesh.triangles);
        }

        private void ThenResultHasExpectedVertices()
        {
            for (int i = 0; i < ExpectedTargetsAmount; i++)
            {
                Vector3 v0 = _testMesh.vertices[_testMesh.triangles[i * 3]];
                Vector3 v1 = _testMesh.vertices[_testMesh.triangles[i * 3 + 1]];
                Vector3 v2 = _testMesh.vertices[_testMesh.triangles[i * 3 + 2]];
                Vector3[] vertices = new Vector3[] {v0, v1, v2 };
                Assert.AreEqual(vertices, _result[i].MeshFilter.sharedMesh.vertices);
			}
        }

        private Mesh CreateTestMesh()
		{
            Mesh result = new Mesh();
            result.vertices = _testVertices;
            result.triangles = _testFaces;
            return result;
		}

        private IFactory<MeshFaceSeparatorTarget> CreateTargetFactoryMock()
        {
            Mock<IFactory<MeshFaceSeparatorTarget>> mock = new Mock<IFactory<MeshFaceSeparatorTarget>>();
            mock.Setup(m => m.Create()).Returns(() => CreateTargetMock());
            return mock.Object;
        }

		private MeshFaceSeparatorTarget CreateTargetMock()
		{
            MeshFilter meshFilter = CreateDummyMeshFilter();
            Mock<MeshFaceSeparatorTarget> targetMock = new Mock<MeshFaceSeparatorTarget>();
            targetMock.SetupGet(m => m.MeshFilter).Returns(() => meshFilter);
            return targetMock.Object;
        }

		private MeshFilter CreateDummyMeshFilter()
		{
            return new GameObject().AddComponent<MeshFilter>();
		}
	}
}