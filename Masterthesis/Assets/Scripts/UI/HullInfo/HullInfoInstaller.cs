using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class HullInfoInstaller : MonoInstaller
    {
        [SerializeField]
        private HullInfo _hullInfo;

        public override void InstallBindings()
        {
            Container.Bind<HullInfo>().FromInstance(_hullInfo).AsSingle();
        }
    }
}