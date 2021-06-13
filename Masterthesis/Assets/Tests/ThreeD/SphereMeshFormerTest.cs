using NUnit.Framework;
using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master.Test
{
    public class SphereMeshFormerTest : ZenjectUnitTestFixture
    {
		private const double _epsilon = 0.0001;
		private const float _testSize = 3.63f;
		private readonly Vector3[] _testVertices = new Vector3[]
        {
            Vector3.zero,
            new Vector3(1,4,-1),
            new Vector3(-2,5.5f, 3),
            new Vector3(-3.1f, -2.4f, 1),
            new Vector3(4.5f, -0.2f, -7.1f),
            new Vector3(0.1f, 0.75f, 0.5f)
        };

        private Mesh _testMesh;
        private SphereMeshFormer _meshFormer;

        [TearDown]
        public void Destruct()
		{
            if (_testMesh != null)
                UnityEngine.Object.Destroy(_testMesh);

        }

        [Test(Description = "All Vertices of the Mesh form a unit sphere")]
        public void Form_CreatesAnUnitSphere()
        {
            GivenADefaultSetup();
            WhenFormIsCalledOnTestMesh();
            ThenAllVerticesHaveMagnitude(1);
        }

        [Test(Description = "The Vertices of the Mesh form a sphere with given radius")]
        public void Form_CreatesAnSphereWithTestRadius()
        {
            GivenADefaultSetup();
            WhenFormIsCalledOnTestMeshWithSize(_testSize);
            ThenAllVerticesHaveMagnitude(_testSize);
        }

		private void GivenADefaultSetup()
		{
            Mesh mesh = new Mesh();
            mesh.vertices = _testVertices;
            Container.Bind<Mesh>().To<Mesh>().FromInstance(mesh).AsSingle();
            Container.Bind<SphereMeshFormer>().AsTransient();

            _testMesh = Container.Resolve<Mesh>();
            _meshFormer = Container.Resolve<SphereMeshFormer>();
        }

        private void WhenFormIsCalledOnTestMesh()
        {
            _meshFormer.Form(_testMesh);
        }

        private void WhenFormIsCalledOnTestMeshWithSize(float testSize)
        {
            _meshFormer.Form(_testMesh, testSize);
        }

        private void ThenAllVerticesHaveMagnitude(float expectedMagnitude)
        {
            foreach (Vector3 vertex in _testMesh.vertices)
                Assert.AreEqual(expectedMagnitude, vertex.magnitude, _epsilon);
        }
    }
}