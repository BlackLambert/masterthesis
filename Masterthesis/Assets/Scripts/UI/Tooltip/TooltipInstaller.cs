using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class TooltipInstaller : MonoInstaller
    {
        [SerializeField]
        private Tooltip _tooltip;

        public override void InstallBindings()
        {
            Container.Bind<Tooltip>().FromInstance(_tooltip);
        }
    }
}