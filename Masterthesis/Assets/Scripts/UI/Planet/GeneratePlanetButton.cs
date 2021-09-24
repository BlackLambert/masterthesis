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
        private GameObject _loadingScreen;
        [SerializeField]
        private Button _button;
        
        protected virtual void Start()
		{
            _button.onClick.AddListener(StartGeneratePlanet);
        }
        
        protected virtual void OnDestroy()
		{
            _button.onClick.RemoveListener(StartGeneratePlanet);
        }

		private void StartGeneratePlanet()
		{
            StartCoroutine(GeneratePlanet());
        }

        private IEnumerator GeneratePlanet()
		{
            _loadingScreen.SetActive(true);
            yield return 0;
            PlanetGenerator.Parameter parameter = _parameterMenu.CreateParameters();
            _planetGenerator.Generate(parameter);
            yield return 0;
            _loadingScreen.SetActive(false);
        }
	}
}
