using UnityEngine;
using Zenject;


namespace SBaier.Master
{
    public class BillowNoiseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PerlinNoise>().To<PerlinNoise>().AsTransient();
            Container.Bind(typeof(Noise3D), typeof(Noise2D)).To<BillowNoise>().AsSingle();
        }
    }
}