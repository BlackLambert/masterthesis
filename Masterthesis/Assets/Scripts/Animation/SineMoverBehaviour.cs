using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class SineMoverBehaviour : MonoBehaviour
	{ 
        private const float _startValue = 0;

        [SerializeField]
        private float _distance = 1;
        [SerializeField]
        private float _frequency = 0.5f;

        private Movable _movable;
        private float _formerValue = 0;

        protected virtual void Awake()
		{
            _movable = GetComponent<Movable>();
            if (_movable == null)
                throw new MissingComponentException();
        }

        protected virtual void Update()
		{
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			float sinValue = Mathf.Sin((Time.time + _startValue) * _frequency);
			float posSinValue = sinValue + 1;
			float value = posSinValue * _distance;
			_movable.Move(value - _formerValue);
			_formerValue = value;
		}
	}
}