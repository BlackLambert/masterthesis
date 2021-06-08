using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using Moq;
using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace SBaier.Master.Test
{
    public class NoiseImageCreatorTest : ZenjectIntegrationTestFixture
    {
		private const string _prefabPath = "Noise/TestNoiseImageCreator";
        private const double _evaluationDelta = 0.1f;
		private const float _epsilon = 0.002f;
		private const int _testSeed = 1234;
		private Mock<NoiseFactory> _noiseFactoryMock;
        private Mock<Noise3D> _noiseMock;
        private NoiseImageCreator _creator;
        private double _evaluationValue = 0;


        [TearDown]
        public void Destruct()
		{
            if (_creator == null)
                return;

            CleanDirectory();
            GameObject.Destroy(_creator.gameObject);
		}
        
        [UnityTest]
        public IEnumerator CreatesFolderAtSpecifiedLocation()
        {
            GivenADefaultSetup();
            GivenNoiseEvaluateReturnsOne();
            GivenNoiseFactoryReturnsMockedNoise();
            _creator = Container.Resolve<NoiseImageCreator>();
            yield return 0;
            ThenTheNoiseImageFolderIsCreatedAt(_creator.DirectoryPath);
        }

		[UnityTest]
        public IEnumerator CreatesFileWithExpectedName()
		{
            GivenADefaultSetup();
            GivenNoiseEvaluateReturnsOne();
            GivenNoiseFactoryReturnsMockedNoise();
            _creator = Container.Resolve<NoiseImageCreator>();
            yield return 0;
            ThenTheImageFilePathContainsFileName(_creator.FilePath, _creator.FileName);
            ThenTheImageFileIsCreatedAt(_creator.FilePath);
        }

        [UnityTest]
        public IEnumerator CreatesFileAtExpectedLocation()
        {
            GivenADefaultSetup();
            GivenNoiseEvaluateReturnsOne();
            GivenNoiseFactoryReturnsMockedNoise();
            _creator = Container.Resolve<NoiseImageCreator>();
            yield return 0;
            ThenTheImageFilePathContainsTheDirectoryPath(_creator.FilePath, _creator.DirectoryPath);
            ThenTheImageFileIsCreatedAt(_creator.FilePath);
        }

        [UnityTest]
        public IEnumerator ImageHasExpectedPixels()
		{
            GivenADefaultSetup();
            GivenNoiseEvaluateReturnsIncreasingValue();
            GivenNoiseFactoryReturnsMockedNoise();
            _creator = Container.Resolve<NoiseImageCreator>();
            yield return 0;
            ThenImageHasPixelsWithIncreasingValue();
        }

        [UnityTest]
        public IEnumerator ImageHasExpectedSize()
        {
            GivenADefaultSetup();
            GivenNoiseEvaluateReturnsOne();
            GivenNoiseFactoryReturnsMockedNoise();
            _creator = Container.Resolve<NoiseImageCreator>();
            yield return 0;
            ThenTheCreatedImageHasExpectedSize();
        }

		private void GivenADefaultSetup()
		{
            PreInstall();
            _noiseFactoryMock = new Mock<NoiseFactory>();
            _noiseMock = new Mock<Noise3D>();
            Container.Bind<Seed>().FromMethod(CreateSeed).AsTransient();
            Container.Bind<NoiseFactory>().To<NoiseFactory>().FromInstance(_noiseFactoryMock.Object).AsSingle();
            Container.Bind<NoiseImageCreator>().To<NoiseImageCreator>().FromComponentInNewPrefabResource(_prefabPath).AsSingle();
            PostInstall();
        }

		private Seed CreateSeed()
		{
            return new Seed(_testSeed);
		}

		private void GivenNoiseEvaluateReturnsOne()
        {
            _noiseMock.Setup(n => n.Evaluate(It.IsAny<double>(), It.IsAny<double>())).Returns(1);
        }

        private void GivenNoiseEvaluateReturnsIncreasingValue()
        {
            _evaluationValue = 0;
            _noiseMock.Setup(n => n.Evaluate(It.IsAny<double>(), It.IsAny<double>())).Returns(GetIncreasingEvaluationValue);
        }

        private void GivenNoiseFactoryReturnsMockedNoise()
        {
            _noiseFactoryMock.Setup(f => f.Create(It.IsAny<NoiseSettings>(), It.IsAny<Seed>())).Returns(_noiseMock.Object);
        }

        private void ThenTheNoiseImageFolderIsCreatedAt(string directoryPath)
        {
            Assert.That(Directory.Exists(directoryPath));
        }

        private void ThenTheImageFilePathContainsFileName(string filePath, string fileName)
        {
            Assert.That(filePath.Contains(fileName));
        }

        private void ThenTheImageFileIsCreatedAt(string filePath)
        {
            Assert.That(File.Exists(filePath));
        }

        private void ThenTheImageFilePathContainsTheDirectoryPath(string filePath, string directoryPath)
        {
            Assert.That(filePath.Contains(directoryPath));
        }

        private void ThenImageHasPixelsWithIncreasingValue()
		{
			_evaluationValue = 0;
			Texture2D texture = LoadNoiseImage();
			Color[] pixels = texture.GetPixels();
			foreach (Color color in pixels)
				TestIncreasingColorValue(color);
			UnityEngine.Object.Destroy(texture);
        }

        private void ThenTheCreatedImageHasExpectedSize()
        {
            Texture2D texture = LoadNoiseImage();
            Assert.AreEqual(_creator.ImageSize.x, texture.width);
            Assert.AreEqual(_creator.ImageSize.y, texture.height);
        }

        private void TestIncreasingColorValue(Color color)
		{
			double evaluatedValue = GetIncreasingEvaluationValue();
			Assert.AreEqual((float)evaluatedValue, color.r, _epsilon);
			Assert.AreEqual((float)evaluatedValue, color.g, _epsilon);
			Assert.AreEqual((float)evaluatedValue, color.b, _epsilon);
		}

		private Texture2D LoadNoiseImage()
		{
			byte[] bytes = File.ReadAllBytes(_creator.FilePath);
			Texture2D texture = new Texture2D(_creator.ImageSize.x, _creator.ImageSize.y);
			texture.LoadImage(bytes);
			return texture;
		}

		private void CleanDirectory()
        {
            if (File.Exists(_creator.FilePath))
                File.Delete(_creator.FilePath);
            if (Directory.Exists(_creator.DirectoryPath))
                Directory.Delete(_creator.DirectoryPath);
        }

        private double GetIncreasingEvaluationValue()
		{
            double value = _evaluationValue;
            _evaluationValue = (_evaluationValue + _evaluationDelta) % 1.0;
            return value;
        }
    }
}