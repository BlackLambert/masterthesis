using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class RidgedNoiseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PerlinNoise>().To<PerlinNoise>().AsTransient();
            Noise3D baseNoise = Container.Resolve<PerlinNoise>();
            Container.Bind<BillowNoise>().To<BillowNoise>().AsTransient().WithArguments(baseNoise);
            Container.Bind(typeof(Noise3D), typeof(Noise2D), typeof(RidgedNoise)).To<RidgedNoise>().AsSingle();
        }
    }
}