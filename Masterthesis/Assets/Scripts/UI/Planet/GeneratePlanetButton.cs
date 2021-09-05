using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class GeneratePlanetButton : MonoBehaviour
    {
        [SerializeField]
        private PlanetGenerator _planetGenerator;
        [SerializeField]
        private PlanetParameterMenu _parameterMenu;
        [SerializeField]
        private Button _button;
        
        protected virtual void Start()
		{
            _button.onClick.AddListener(GeneratePlanet);
        }
        
        protected virtual void OnDestroy()
		{
            _button.onClick.RemoveListener(GeneratePlanet);
        }

		private void GeneratePlanet()
		{
            PlanetGenerator.Parameter parameter = _parameterMenu.CreateParameters();
            _planetGenerator.Generate(parameter);
        }
	}
}
