using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField]
        private CameraFocalDistanceController _focalDistanceController;
        [SerializeField]
        private Camera _camera;

        public override void InstallBindings()
        {
            Container.Bind<CameraFocalDistanceController>().FromInstance(_focalDistanceController).AsSingle();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle();
        }
    }
}