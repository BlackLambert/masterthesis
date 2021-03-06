using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetAxisGizmoDrawer : MonoBehaviour
    {
		private const int _additionalAxisLength = 10;
		private const int _totalAngles = 360;
		private PlanetAxisData _axis;
        private PlanetDimensions _dimensions;

        [Inject]
        public void Construct(PlanetAxisData axis,
            PlanetDimensions dimensions)
		{
            _axis = axis;
            _dimensions = dimensions;
        }

        public void OnDrawGizmosSelected()
		{
            if (_axis == null || _dimensions.Equals(default(PlanetDimensions)))
                return;
            Gizmos.color = Color.red;
            Vector3 axis = Vector3.up * _dimensions.AtmosphereRadius + Vector3.up * _additionalAxisLength;
            axis = Quaternion.AngleAxis(_axis.Angle, Vector3.forward) * axis;
            Gizmos.DrawLine(transform.position - axis, transform.position + axis);
		}
    }
}