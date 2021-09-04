using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField]
        private CameraFocalDistanceController _focalDistanceController;

        public override void InstallBindings()
        {
            Container.Bind<CameraFocalDistanceController>().FromInstance(_focalDistanceController).AsSingle();
        }
    }
}