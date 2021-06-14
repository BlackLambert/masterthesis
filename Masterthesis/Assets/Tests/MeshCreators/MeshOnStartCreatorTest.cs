using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using Moq;
using UnityEngine;
using System;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    public class MeshOnStartCreatorTest : ZenjectIntegrationTestFixture
    {
		private const string _creatorPrefabPath = "MeshCreators/TestMeshOnStartCreator";
		private const float _testSize = 2.6f;
		private Mock<MeshGeneratorFactory> _meshGeneratorFactoryMoq;
		private Mock<MeshGenerator> _meshGeneratorMoq;
		private MeshOnStartCreator _creator;

        [TearDown]
        public void Destruct()
		{
            if(_creator == null)
                GameObject.Destroy(_creator.gameObject);
        }

		[UnityTest]
        public IEnumerator GenerateMeshCalledWithExpectedArguments()
        {
            GivenADefaultSetup();
            GivenAGeneratorTestSetup();
            yield return 0;
            ThenGenerateMeshIsCalledWithExpectedArguments();
        }

		private void GivenADefaultSetup()
		{
            PreInstall();
            _meshGeneratorFactoryMoq = new Mock<MeshGeneratorFactory>();
            _meshGeneratorMoq = new Mock<MeshGenerator>();
            Container.Bind<MeshGeneratorFactory>().FromInstance(_meshGeneratorFactoryMoq.Object).AsSingle();
            Container.Bind<MeshOnStartCreator>().FromComponentInNewPrefabResource(_creatorPrefabPath).AsSingle();
            PostInstall();

            _creator = Container.Resolve<MeshOnStartCreator>();
            _creator.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }

        private void GivenAGeneratorTestSetup()
        {
            _meshGeneratorMoq.Setup(generator => generator.GenerateMeshFor(It.IsAny<Mesh>(), _testSize));
            _meshGeneratorFactoryMoq.Setup(f => f.Create(It.IsAny<MeshGeneratorSettings>())).Returns(_meshGeneratorMoq.Object);
        }

        private void ThenGenerateMeshIsCalledWithExpectedArguments()
        {
            Mesh mesh = _creator.GetComponent<MeshFilter>().sharedMesh;
            _meshGeneratorMoq.Verify(generator => generator.GenerateMeshFor(mesh, _testSize));
        }
    }
}