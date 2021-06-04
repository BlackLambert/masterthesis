using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class SeedInstaller : MonoInstaller
    {
        [SerializeField]
        private int _seed = 24234;

        public override void InstallBindings()
        {
            Container.Bind<Seed>().To<Seed>().FromNew().AsTransient().WithArguments(_seed);
        }
    }
}