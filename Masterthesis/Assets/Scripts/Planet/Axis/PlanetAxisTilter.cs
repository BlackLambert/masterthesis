
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetAxisTilter : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        private PlanetAxisData _axis;

        [Inject]
        public void Construct(PlanetAxisData axis)
		{
            _axis = axis;
        }

        protected virtual void Start()
		{
            _target.Rotate(Vector3.forward, _axis.Angle, Space.World);

        }
    }
}