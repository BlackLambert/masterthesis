using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class LayoutElementWidthLimiter : MonoBehaviour
    {
        [SerializeField]
        private int _maxCharacterPerLine = 50;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private LayoutElement _layoutElement;

        public virtual void Update()
		{
            Adjust();
        }

        public void Adjust()
		{
            bool isOverMax = _text.text.Length > _maxCharacterPerLine;
            _layoutElement.enabled = isOverMax;
            
        }
    }
}