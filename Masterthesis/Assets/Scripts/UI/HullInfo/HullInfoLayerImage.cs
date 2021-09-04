using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class HullInfoLayerImage : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private LayoutElement _layoutElement;
        [SerializeField]
        private Transform _base;
        [SerializeField]
        private float _minImageHeight = 3f;

        public void Init(Color color, float portion, Transform parent)
		{
            _base.SetParent(parent, false);
            _image.color = color;
            _layoutElement.flexibleHeight = Mathf.Max(Mathf.Clamp01(portion) * 100, _minImageHeight);
            _base.localScale = Vector3.one;
        }

		internal void Destruct()
		{
            Destroy(_base.gameObject);
		}
	}
}