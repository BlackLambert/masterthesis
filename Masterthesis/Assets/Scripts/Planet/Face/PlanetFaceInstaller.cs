using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetFaceInstaller : MonoInstaller
    {
        [SerializeField]
        private PlanetFace _planetFace;

        public override void InstallBindings()
        {
            Container.Bind<PlanetFace>().FromInstance(_planetFace).AsSingle();
        }
    }
}