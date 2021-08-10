using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using Moq;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class MeshGeneratorFactoryTest : Zenject.ZenjectUnitTestFixture
    {
		private const string _icosahedronSettingsPath = "MeshGenerators/TestIcosahedronGeneratorSettings";
		private const string _plainSettingsPath = "MeshGenerators/TestPlainGeneratorSettings";
		private const string _cubeSettingsPath = "MeshGenerators/TestCubeGeneratorSettings";
		private const string _uVSphereSettingsPath = "MeshGenerators/TestUVSphereGeneratorSettings";
		private const string _sampleEliminationSphereSettingsPath = "MeshGenerators/TestSampleEliminationSphereGeneratorSettings";
		private const int _baseSeed = 1234;
		private MeshGeneratorFactory _factory;
		private MeshGenerator _meshGenerator;

		[Test]
        public void Create_ReturnsIcosahedronGeneratorOnCalledWithSettings()
        {
            GivenADefaultSetup();
			IcosahedronGeneratorSettings settings = Resources.Load<IcosahedronGeneratorSettings>(_icosahedronSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenCreatedMeshGeneratorIs(typeof(IcosahedronGenerator));
		}

        [Test]
        public void Create_ReturnsPlainGeneratorOnCalledWithSettings()
        {
            GivenADefaultSetup();
			PlainGeneratorSettings settings = Resources.Load<PlainGeneratorSettings>(_plainSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenCreatedMeshGeneratorIs(typeof(PlainGenerator));
		}

        [Test]
        public void Create_ReturnsCubeGeneratorOnCalledWithSettings()
        {
            GivenADefaultSetup();
			CubeGeneratorSettings settings = Resources.Load<CubeGeneratorSettings>(_cubeSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenCreatedMeshGeneratorIs(typeof(CubeMeshGenerator));
		}

        [Test]
        public void Create_ReturnsUVSphereGeneratorOnCalledWithSettings()
        {
            GivenADefaultSetup();
			UVSphereGeneratorSettings settings = Resources.Load<UVSphereGeneratorSettings>(_uVSphereSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenCreatedMeshGeneratorIs(typeof(UVSphereMeshGenerator));
		}

        [Test]
        public void Create_CreatedUVSphereGeneratorHasValuesBasedOnTheSettings()
        {
            GivenADefaultSetup();
			UVSphereGeneratorSettings settings = Resources.Load<UVSphereGeneratorSettings>(_uVSphereSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenTheUVSphereGeneratorHasExpectedValues(settings);
		}

		[Test]
		public void Create_ThrowsNotImplementedExceptionOnUndefinedSettings()
		{
			GivenADefaultSetup();
			Mock<MeshGeneratorSettings> mock = new Mock<MeshGeneratorSettings>();
			mock.SetupGet(m => m.Type).Returns(MeshGeneratorType.Undefined);
			TestDelegate test = () => WhenCreateCalledWithSettings(mock.Object);
			ThenANotImplementedExceptionIsThrown(test);
		}

		[Test]
		public void Create_CreatesSampleEliminationSphereGeneratorOnCalledWithSettings()
		{
			GivenADefaultSetup();
			SampleEliminationSphereGeneratorSettings settings = Resources.Load<SampleEliminationSphereGeneratorSettings>(_sampleEliminationSphereSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenCreatedMeshGeneratorIs(typeof(SampleEliminationSphereGenerator));
		}

		[Test]
		public void Create_CreatedSampleEliminationSphereGeneratorHasExpectedValues()
		{
			GivenADefaultSetup();
			SampleEliminationSphereGeneratorSettings settings = Resources.Load<SampleEliminationSphereGeneratorSettings>(_sampleEliminationSphereSettingsPath);
			WhenCreateCalledWithSettings(settings);
			ThenCreatedSampleEliminationSphereGeneratorHasExpectedValues(settings);
		}

		private void GivenADefaultSetup()
		{
			Func<Vector3, int, float> compareValueSelector = (p, i) => p[i];
			Container.Bind<QuickSelector<Vector3>>().To<QuickSorter<Vector3, float>>().AsTransient().WithArguments(compareValueSelector);
			Container.Bind<Vector3BinaryKDTreeFactory>().AsTransient();
			Container.Bind<Seed>().AsSingle().WithArguments(_baseSeed);
			Container.Bind<MeshGeneratorFactory>().To<MeshGeneratorFactoryImpl>().AsTransient();
			Container.Bind<SampleElimination3D>().AsTransient();
			Container.Bind<RandomPointsOnSphereGenerator>().AsTransient();

			_factory = Container.Resolve<MeshGeneratorFactory>();
		}

		private void WhenCreateCalledWithSettings(MeshGeneratorSettings settings)
		{
			_meshGenerator = _factory.Create(settings);
		}

		private void ThenCreatedMeshGeneratorIs(Type type)
		{
			Assert.AreEqual(_meshGenerator.GetType(), type);
		}

		private void ThenANotImplementedExceptionIsThrown(TestDelegate test)
		{
			Assert.Throws<NotImplementedException>(test);
		}

		private void ThenTheUVSphereGeneratorHasExpectedValues(UVSphereGeneratorSettings settings)
		{
			UVSphereMeshGenerator generator = _meshGenerator as UVSphereMeshGenerator;
			Assert.AreEqual(settings.RingsAmount, generator.RingsAmount);
			Assert.AreEqual(settings.SegmentsAmount, generator.SegmentsAmount);
		}

		private void ThenCreatedSampleEliminationSphereGeneratorHasExpectedValues(SampleEliminationSphereGeneratorSettings settings)
		{
			SampleEliminationSphereGenerator generator = _meshGenerator as SampleEliminationSphereGenerator;
			Assert.AreEqual(settings.TargetSampleCount, generator.TargetSampleCount);
			Assert.AreEqual(settings.BaseSamplesFactor, generator.BaseSamplesFactor);
			Seed baseSeed = Container.Resolve<Seed>();
		}
	}
}