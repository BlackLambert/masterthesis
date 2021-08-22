using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetParameterMenu : MonoBehaviour
    {
        [SerializeField]
        private TextInputPanel _seedInput;

		[Header("Dimensionen")]
		[SerializeField]
		private SliderPanel _subdivisionsSliderPanel;
		[SerializeField]
		private SliderPanel _bedrockSliderPanel;
		[SerializeField]
		private SliderPanel _seaLevelSliderPanel;
		[SerializeField]
		private SliderPanel _maxHullSliderPanel;
		[SerializeField]
		private SliderPanel _atmosphereSliderPanel;

		[Header("Axis")]
		[SerializeField]
		private SliderPanel _axisAnglePanel;
		[SerializeField]
		private SliderPanel _secondsPerRevolutionPanel;

		[Header("Plate")]
		[SerializeField]
		private SliderPanel _plateSegmentsPanel;
		[SerializeField]
		private SliderPanel _continentsPanel;
		[SerializeField]
		private SliderPanel _oceansPanel;
		[SerializeField]
		private SliderPanel _platesPanel;
		[SerializeField]
		private SliderPanel _platesMinForcePanel;
		[SerializeField]
		private SliderPanel _warpPanel;
		[SerializeField]
		private SliderPanel _blendPanel;
		[SerializeField]
		private SliderPanel _sampleEliminationPanel;

		[Header("Shaping")]
		[SerializeField]
		private SliderPanel _mountainBreadthPanel;
		[SerializeField]
		private SliderPanel _mountainBlendFactorPanel;
		[SerializeField]
		private SliderPanel _mountainMinPanel;
		[SerializeField]
		private SliderPanel _mountainMinBreadthPanel;
		[SerializeField]
		private SliderPanel _canyonsBreadthPanel;
		[SerializeField]
		private SliderPanel _canyonBlendFactorPanel;
		[SerializeField]
		private SliderPanel _canyonMinPanel;
		[SerializeField]
		private SliderPanel _canyonMinBreadthPanel;

		[Header("Temperature")]
		[SerializeField]
		private SliderPanel _minTempPanel;
		[SerializeField]
		private SliderPanel _maxTempPanel;

		public PlanetGenerator.Parameter CreateParameters()
		{
			Seed seed = CreateSeed();
			int subdivisions = (int)_subdivisionsSliderPanel.Slider.value;
			PlanetDimensions dimensions = CreatePlanetDimensions();
			PlanetAxisData axis = CreateAxisData();
			ContinentalPlatesParameter continentalPlatesParameter = CreateContinentalPlatesParameter();
			TemperatureSpectrum temperatureSpectrum = CreateTemperatureSpecturm();
			ShapingParameter shaping = CreateShapingParameter();

			return new PlanetGenerator.Parameter
			(
				seed: seed,
				subdivisions: subdivisions,
				dimensions: dimensions,
				axisData: axis,
				continentalPlatesParameter: continentalPlatesParameter,
				temperatureSpectrum: temperatureSpectrum,
				shaping: shaping
			);
		}

		private TemperatureSpectrum CreateTemperatureSpecturm()
		{
			float minTemp = _minTempPanel.Slider.value;
			float maxTemp = _maxTempPanel.Slider.value;
			return new TemperatureSpectrum(minTemp, maxTemp);
		}

		private PlanetDimensions CreatePlanetDimensions()
		{
			float bedrockRadius = _bedrockSliderPanel.Slider.value;
			float seaLevel = _seaLevelSliderPanel.Slider.value;
			float maxHullRadius = _maxHullSliderPanel.Slider.value;
			float armosphereRadius = _atmosphereSliderPanel.Slider.value;
			PlanetDimensions dimensions = new PlanetDimensions(bedrockRadius, maxHullRadius, seaLevel, armosphereRadius);
			return dimensions;
		}

		private PlanetAxisData CreateAxisData()
		{
			float axisAngle = _axisAnglePanel.Slider.value;
			float secondsPerRevolution = _secondsPerRevolutionPanel.Slider.value;
			PlanetAxisData axis = new PlanetAxisData(axisAngle, secondsPerRevolution);
			return axis;
		}

		private ContinentalPlatesParameter CreateContinentalPlatesParameter()
		{
			int plateSegments = (int)_plateSegmentsPanel.Slider.value;
			int contients = (int)_continentsPanel.Slider.value;
			int oceans = (int)_oceansPanel.Slider.value;
			int plates = (int)_platesPanel.Slider.value;
			float warpFactor = _warpPanel.Slider.value;
			float blendFactor = _blendPanel.Slider.value;
			float sampleEliminationFactor = _sampleEliminationPanel.Slider.value;
			float platesMinFoce = _platesMinForcePanel.Slider.value;
			ContinentalPlatesParameter continentalPlatesParameter =
				new ContinentalPlatesParameter(plateSegments, contients, oceans, plates, warpFactor, blendFactor, sampleEliminationFactor, platesMinFoce);
			return continentalPlatesParameter;
		}

		private ShapingParameter CreateShapingParameter()
		{
			PlatesShapingParameter plates = CreatePlatesShapingParameter();
			return new ShapingParameter(plates);
		}

		private PlatesShapingParameter CreatePlatesShapingParameter()
		{
			float mountainBreadthFactor = _mountainBreadthPanel.Slider.value;
			float mountainBlendFactor = _mountainBlendFactorPanel.Slider.value;
			float mountainMin = _mountainMinPanel.Slider.value;
			float mountainMinBreadth = _mountainMinBreadthPanel.Slider.value;
			float canyonBreadthFactor = _canyonsBreadthPanel.Slider.value;
			float canyonBlendFactor = _canyonBlendFactorPanel.Slider.value;
			float canyonMin = _canyonMinPanel.Slider.value;
			float canyonMinBreadth = _canyonMinBreadthPanel.Slider.value;
			return new PlatesShapingParameter(
				mountainBreadthFactor, 
				mountainBlendFactor, 
				mountainMin,
				mountainMinBreadth,
				canyonBreadthFactor, 
				canyonBlendFactor, 
				canyonMin,
				canyonMinBreadth);
		}

		private Seed CreateSeed()
		{
			string seedText = _seedInput.InputField.text;
			int seedValue = int.Parse(seedText);
			return new Seed(seedValue);
		}
	}
}