using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Noise3DInstaller : MonoInstaller
    {
        [SerializeField]
        private NoiseSettings _noise;
        [Inject]
        private NoiseFactory _noiseFactory;
        [Inject]
        private Seed _seed;

        public override void InstallBindings()
        {
            Container.Bind<Noise3D>().FromMethod(CreateNoise).AsSingle();
        }

        private Noise3D CreateNoise()
		{
            return _noiseFactory.Create(_noise, _seed);
		}
    }
}