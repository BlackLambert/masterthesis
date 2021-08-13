using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MenuAnimator : MonoBehaviour
    {
		private const string _animatorPropertyName = "Shown";
		[SerializeField]
        private Menu _menu;
        [SerializeField]
        private Animator _animator;

        protected virtual void Start()
		{
            _menu.OnShownChanged += UpdateAnimator;
        }

        protected virtual void OnDestroy()
        {
            _menu.OnShownChanged -= UpdateAnimator;
        }

        private void UpdateAnimator()
		{
            _animator.SetBool(_animatorPropertyName, _menu.Shown);
        }
	}
}