using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBaier.Master
{
    [RequireComponent(typeof(RectTransform))]
    public class Foldout : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject _target;
        [SerializeField]
        private bool _foldOutOnStart = true;
        private bool _foldOut = true;
        public bool FoldOut => _foldOut;
        
        protected virtual void Start()
		{
            _foldOut = _foldOutOnStart;
            Fold();
        }

        public void OnPointerClick(PointerEventData eventData)
		{
            _foldOut = !_foldOut;
            Fold();
		}

		private void Fold()
		{
            _target.SetActive(_foldOut);
		}
	}
}