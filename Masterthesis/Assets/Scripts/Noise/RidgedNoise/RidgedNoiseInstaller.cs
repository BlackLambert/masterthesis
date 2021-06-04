using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class RidgedNoiseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PerlinNoise>().To<PerlinNoise>().AsTransient();
            Container.Bind<BillowNoise>().To<BillowNoise>().AsTransient();
            Container.Bind(typeof(Noise3D), typeof(Noise2D)).To<RidgedNoise>().AsSingle();
        }
    }
}