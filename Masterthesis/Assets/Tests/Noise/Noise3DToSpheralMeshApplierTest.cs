using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class Noise3DToSpheralMeshApplierTest : ZenjectUnitTestFixture
    {
		private const int _testSeed = 1234;
		private const float _epsilon = 0.01f;
		private readonly Vector2 _testRange = new Vector2(1.2f, 2.6f);
        private readonly Vector2 _expectedDefaultRange = new Vector2(0, 1);

        private readonly Vector2[] _invalidRanges = new Vector2[]
        {
            new Vector2(-0.001f, -2.1f),
            new Vector2(-0.001f, 0.1f),
            new Vector2(3.6f, 2.7f),
            new Vector2(1.3f, -4.2f)
        };

        private readonly Vector3[] _testVertices = new Vector3[]
         {
            Vector3.zero,
            new Vector3(1,4,-1),
            new Vector3(-2,5.5f, 3),
            new Vector3(-3.1f, -2.4f, 1),
            new Vector3(4.5f, -0.2f, -7.1f),
            new Vector3(0.1f, 0.75f, 0.5f)
         };

        private Noise3DToSpheralMeshApplier _noiseApplier;
        private Mesh _testMesh;
		private Noise3D _noise;

		[TearDown]
        public void Destruct()
        {
            if (_testMesh != null)
                UnityEngine.Object.Destroy(_testMesh);
        }

        [Test (Description = "Range set to an value of invalid range causes an ArgumentOutOfRangeException")]
        public void Range_Set_InvalidRangeThrowsAnException()
        {
            GivenADefaultSetup();
            foreach(Vector2 range in _invalidRanges)
			{
                TestDelegate test = () => WhenRangeSetTo(range);
                ThenThrowsAnArgumentOutOfRangeException(test);
			}
        }

        [Test(Description = "The default range is as expected")]
        public void Range_Get_TheDefaultRangeIsAsExpected()
        {
            GivenADefaultSetup();
            ThenRangeIs(_expectedDefaultRange);
        }

        [Test(Description = "The range property returns set value")]
        public void Range_Get_ReturnsSetValue()
        {
            GivenADefaultSetup();
            WhenRangeSetTo(_testRange);
            ThenRangeIs(_testRange);
        }

        [Test(Description = "The range property returns set value")]
        public void Apply_ResultingVertexMagnitudesAreInRange()
        {
            GivenADefaultSetup();
            WhenRangeSetTo(_testRange);
            WhenApplyIsCalledOn(_testMesh);
            ThenValueIsWithingRange(_testRange);
        }

		[Test(Description = "Vertex Positions are the minimum Range Value plus the range delta times noise value")]
        public void Apply_ResultingValueIsAsExpected()
        {
            GivenADefaultSetup();
            WhenRangeSetTo(_testRange);
            WhenApplyIsCalledOn(_testMesh);
            ThenValuesAreAsExpected(_testRange);
        }

		private void GivenADefaultSetup()
		{
            Mesh mesh = new Mesh();
            mesh.vertices = _testVertices;
            Container.Bind<Noise3D>().To<PerlinNoise>().FromMethod(CreatePerlinNoise).AsTransient();
            Container.Bind<SphereMeshFormer>().AsTransient();
            Container.Bind<Mesh>().FromInstance(mesh).AsSingle();
            Container.Bind<Noise3DToSpheralMeshApplier>().AsTransient();

            _noiseApplier = Container.Resolve<Noise3DToSpheralMeshApplier>();
            _testMesh = Container.Resolve<Mesh>();
            _noise = Container.Resolve<Noise3D>();
        }

        private void WhenRangeSetTo(Vector2 range)
        {
            _noiseApplier.Range = range;
        }

        private void WhenApplyIsCalledOn(Mesh testMesh)
        {
            _noiseApplier.Apply(testMesh, _noise);
        }

        private void ThenThrowsAnArgumentOutOfRangeException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenRangeIs(Vector2 range)
        {
            Assert.AreEqual(range, _noiseApplier.Range);
        }

        private void ThenValueIsWithingRange(Vector2 range)
        {
            foreach(Vector3 vertex in _testMesh.vertices)
			{
                float magnitude = vertex.magnitude;
                Assert.True(magnitude > range.x);
                Assert.True(magnitude < range.y);
			}
        }

        private void ThenValuesAreAsExpected(Vector2 range)
        {
            float delta = range.y - range.x;
            Vector3[] actualVertices = _testMesh.vertices;
            for (int i = 0; i < _testVertices.Length; i++)
            {
                Vector3 testVertex = _testVertices[i].magnitude > 0 ? _testVertices[i] : Vector3.up;
                Vector3 actualVertex = actualVertices[i];
                testVertex = testVertex.normalized * range.x;
                float noiseValue = _noise.Evaluate3D(testVertex);
                Vector3 expected = testVertex.normalized * (float)(range.x + delta * noiseValue);
                Assert.AreEqual(expected.x, actualVertex.x, _epsilon);
                Assert.AreEqual(expected.y, actualVertex.y, _epsilon);
                Assert.AreEqual(expected.z, actualVertex.z, _epsilon);
            }
        }

        private PerlinNoise CreatePerlinNoise()
		{
            return new PerlinNoise(new Seed(_testSeed));
		}
    }
}