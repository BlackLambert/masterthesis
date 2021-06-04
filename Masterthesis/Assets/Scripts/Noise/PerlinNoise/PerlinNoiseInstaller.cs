using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PerlinNoiseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(Noise3D), typeof(Noise2D)).To<PerlinNoise>().AsSingle().NonLazy();
        }
    }
}