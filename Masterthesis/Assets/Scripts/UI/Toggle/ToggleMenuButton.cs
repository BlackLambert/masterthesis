using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class ToggleMenuButton : MonoBehaviour
    {
        [SerializeField]
        private Menu _menu;
        public Menu Menu => _menu;
        [SerializeField]
        private Button _button;
        
        protected virtual void Start()
		{
            _button.onClick.AddListener(ToggleMenu);
        }

		protected virtual void OnDestroy()
		{
            _button.onClick.RemoveListener(ToggleMenu);
        }

        private void ToggleMenu()
        {
            Menu.Shown = !Menu.Shown;
        }
    }
}