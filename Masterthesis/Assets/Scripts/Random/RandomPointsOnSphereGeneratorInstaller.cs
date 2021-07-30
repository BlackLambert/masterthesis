using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class RandomPointsOnSphereGeneratorInstaller : MonoInstaller
    {
        [Inject]
        private Seed _seed;

        public override void InstallBindings()
        {

            Container.Bind<RandomPointsOnSphereGenerator>().AsTransient().WithArguments(CreateSeed());
        }

		private Seed CreateSeed()
		{
            return new Seed(_seed.Random.Next());
		}
	}
}