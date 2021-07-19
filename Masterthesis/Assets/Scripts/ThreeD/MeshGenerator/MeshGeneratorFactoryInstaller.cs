using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Func<Vector3, int, float> compareValueSelect = (p, i) => p[i];
            Container.Bind<QuickSelector<Vector3>>().To<QuickSorter<Vector3, float>>().AsTransient().WithArguments(compareValueSelect);
            Container.Bind<MeshGeneratorFactory>().To<MeshGeneratorFactoryImpl>().AsTransient();
        }
    }
}