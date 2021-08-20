using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class EvaluationPointDatasInitializerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EvaluationPointDatasInitializer>().AsTransient();
        }
    }
}