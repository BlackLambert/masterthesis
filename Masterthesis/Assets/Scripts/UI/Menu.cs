using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Menu : MonoBehaviour
    {
        private bool _shown = true;
        public bool Shown
		{
            get => _shown;
			set
			{
                _shown = value;
                OnShownChanged?.Invoke();
			}
		}
        public event Action OnShownChanged;
    }
}