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
		public void Create_ThrowsNotImplementedExceptionOnUndefinedSettings()
		{
			GivenADefaultSetup();
			Mock<MeshGeneratorSettings> mock = new Mock<MeshGeneratorSettings>();
			mock.SetupGet(m => m.Type).Returns(MeshGeneratorType.Undefined);
			TestDelegate test = () => WhenCreateCalledWithSettings(mock.Object);
			ThenANotImplementedExceptionIsThrown(test);
		}

		private void GivenADefaultSetup()
		{
			Container.Bind<MeshGeneratorFactory>().To<MeshGeneratorFactoryImpl>().AsTransient();

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
	}
}