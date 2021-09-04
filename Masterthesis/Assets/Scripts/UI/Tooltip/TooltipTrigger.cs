using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace SBaier.Master
{
    [RequireComponent(typeof(RectTransform))]
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const float _secondsTillShow = 1f;
        [SerializeField]
        private string _textToDisplay = "Tooltip";

        private bool _isOver = false;
        private float _timeTillShow = 0;
        private Tooltip _tooltip;
		private bool _shown;

		[Inject]
        public void Construct(Tooltip tooltip)
		{
            _tooltip = tooltip;
        }

        protected virtual void Start()
		{
			ResetValues();
		}

        protected virtual void Update()
        {
            if(_isOver && !_shown)
                _timeTillShow -= Time.deltaTime;
			if (_timeTillShow <= 0 && !_shown)
				Show();
			if (_shown)
				_tooltip.UpdatePosition(Input.mousePosition);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_isOver = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ResetValues();
			Hide();
		}

		private void Show()
		{
            _shown = true;
			_tooltip.Show(_textToDisplay, Input.mousePosition);
		}

		private void ResetValues()
		{
			_isOver = false;
			_timeTillShow = _secondsTillShow;
		}

		private void Hide()
		{
            _shown = false;
			_tooltip.Hide();
		}
	}
}