using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class SeedInstaller : MonoInstaller
    {
        [SerializeField]
        private int _seed = 24234;
        [SerializeField]
        private bool _useRandomSeed = false;

        public override void InstallBindings()
        {
            int seed = _useRandomSeed ? new System.Random().Next(int.MinValue, int.MaxValue) : _seed;
            Container.Bind<Seed>().To<Seed>().FromNew().AsTransient().WithArguments(seed);
        }
    }
}