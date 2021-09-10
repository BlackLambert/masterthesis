using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetRebuilder : MonoBehaviour
    {
		private PlanetLayerTogglePanel _layerTogglePanel;
		private Planet _planet;
		private PlanetColorizer _planetColorizer;

		[Inject]
        public void Construct(PlanetLayerTogglePanel layerTogglePanel,
			Planet planet,
			PlanetColorizer planetColorizer)
		{
            _layerTogglePanel = layerTogglePanel;
			_planet = planet;
			_planetColorizer = planetColorizer;
		}

        protected virtual void Start()
		{
            _layerTogglePanel.OnToggleChanged += OnLayerMaskChanged;

        }

		protected virtual void OnDestroy()
		{
			_layerTogglePanel.OnToggleChanged -= OnLayerMaskChanged;
		}

		private void OnLayerMaskChanged()
		{
			_planet.Data.SetLayerBitMask(_layerTogglePanel.GetLayerMask());
			Rebuild();
		}

		private void Rebuild()
		{
			_planet.UpdateMesh();
			_planetColorizer.Compute(new PlanetColorizer.Parameter(_planet));
		}
	}
}
