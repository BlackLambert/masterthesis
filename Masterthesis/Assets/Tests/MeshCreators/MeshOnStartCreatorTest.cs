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
		private Mock<MeshGenerator> _meshGeneratorMoq;
		private MeshOnStartCreator _creator;

		[UnityTest]
        public IEnumerator GenerateMeshCalledWithExpectedArguments()
        {
            GivenADefaultSetup();
            _creator = Container.Resolve<MeshOnStartCreator>();
            _creator.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            GivenAGeneratorTestSetup();
            yield return 0;
            ThenGenerateMeshIsCalledWithExpectedArguments();
            GameObject.Destroy(_creator.gameObject);
        }

		private void GivenADefaultSetup()
		{
            PreInstall();
            _meshGeneratorMoq = new Mock<MeshGenerator>();
            Container.Bind<MeshGenerator>().FromInstance(_meshGeneratorMoq.Object).AsSingle();
            Container.Bind<MeshOnStartCreator>().FromComponentInNewPrefabResource(_creatorPrefabPath).AsSingle();
            PostInstall();
        }

        private void GivenAGeneratorTestSetup()
        {
            _meshGeneratorMoq.Setup(generator => generator.GenerateMeshFor(It.IsAny<Mesh>(), _testSize));
        }

        private void ThenGenerateMeshIsCalledWithExpectedArguments()
        {
            Mesh mesh = _creator.GetComponent<MeshFilter>().sharedMesh;
            _meshGeneratorMoq.Verify(generator => generator.GenerateMeshFor(mesh, _testSize));
        }
    }
}