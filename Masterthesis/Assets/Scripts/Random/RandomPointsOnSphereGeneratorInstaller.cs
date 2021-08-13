using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class RandomPointsOnSphereGeneratorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {

            Container.Bind<RandomPointsOnSphereGenerator>().AsTransient();
        }
	}
}