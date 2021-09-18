using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetParameterMenuInstaller : MonoInstaller
    {
        [SerializeField]
        private PlanetParameterMenu _menu;

        public override void InstallBindings()
        {
            Container.Bind<PlanetParameterMenu>().FromInstance(_menu).AsSingle();
        }
    }
}