using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class MeshGeneratorTest : ZenjectUnitTestFixture
    {
        private MeshGenerator _generator;
        private Mesh _testMesh;
        private Mesh _testMeshTwo;

        private float[] _invalidSizes =
        {
            0, -0.23f, -4f, -3.3f, float.MinValue
        };

		public override void Teardown()
		{
			base.Teardown();
            if(_testMesh != null)
                GameObject.Destroy(_testMesh);
            if(_testMeshTwo != null)
                GameObject.Destroy(_testMeshTwo);
        }


		[Test]
        public void AMeshGeneratedWithSizeOneEqualsMeshGeneratedWithNoSize()
		{
            GivenADefaultSetup();
            GivenASecondMesh();
            WhenGenerateIsCalledFor(_testMesh);
            WhenGenerateIsCalledFor(_testMeshTwo, 1f);
            ThenMeshesAreEqual(_testMesh, _testMeshTwo);
        }

        [Test]
        public void GenerateMesh_WithSize_ThrowsExceptionOnInvalidSize()
		{
            for (int i = 0; i < _invalidSizes.Length; i++)
            {
                GivenADefaultSetup();
                TestDelegate test = () => WhenGenerateIsCalledFor(_testMesh, _invalidSizes[i]);
                ThenThrowsArgumentOutOfRangeExcpetion(test);
                Teardown();
                Setup();
            }
        }

		protected abstract MeshGenerator GivenANewMeshGenerator();

        private void GivenADefaultSetup()
		{
            _generator = GivenANewMeshGenerator();
            _testMesh = new Mesh();
        }

        private void GivenASecondMesh()
        {
            _testMeshTwo = new Mesh();
        }

        private void WhenGenerateIsCalledFor(Mesh mesh)
        {
            _generator.GenerateMeshFor(mesh);
        }

        private void WhenGenerateIsCalledFor(Mesh mesh, float size)
        {
            _generator.GenerateMeshFor(mesh, size);
        }

        private void ThenMeshesAreEqual(Mesh meshOne, Mesh meshTwo)
        {
            Assert.AreEqual(meshOne.vertices, meshTwo.vertices);
            Assert.AreEqual(meshOne.triangles, meshTwo.triangles);
        }

        private void ThenThrowsArgumentOutOfRangeExcpetion(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }
    }
}