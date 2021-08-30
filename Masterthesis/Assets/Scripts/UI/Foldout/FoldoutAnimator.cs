using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class FoldoutAnimator : MonoBehaviour
    {
        [SerializeField]
        private Foldout _foldout;
        [SerializeField]
        private Animator _animator;

        protected virtual void Start()
		{
            SetState();
		}

        protected virtual void Update()
		{
			SetState();
		}

		private void SetState()
		{
			_animator.SetBool("Hidden", !_foldout.FoldOut);
		}
	}
}