using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SBaier.Master
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private RectTransform _panel;
        [SerializeField]
        private CanvasGroup _canvasGroup;
		[SerializeField]
		private Vector2 _positionOffset = new Vector2(10, 10);

		protected virtual void Start()
		{
			ShowPanel(false);
			_canvasGroup.blocksRaycasts = false;
			_canvasGroup.interactable = false;
		}
        
        public void Show(string textToDisplay, Vector2 position)
		{
			_text.text = textToDisplay;
			UpdatePosition(position);
			ShowPanel(true);
		}

		public void Hide()
		{
			ShowPanel(false);
		}

		public void UpdatePosition(Vector2 position)
		{
			_panel.position = position + _positionOffset;
		}

		private void ShowPanel(bool shallShow)
		{
			_canvasGroup.alpha = shallShow ? 1 : 0;
		}
    }
}