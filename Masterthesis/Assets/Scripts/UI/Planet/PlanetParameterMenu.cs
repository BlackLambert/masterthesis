using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class PlanetParameterMenu : MonoBehaviour
    {
		private const int _maximalSampleEliminationFactor = 3;
		[SerializeField]
        private TextInputPanel _seedInput;

		[SerializeField]
		private Toggle _randomizeToggle;

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
		private SliderPanel _warpChaosPanel;
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
		private SliderPanel _mountainHeightFactorPanel;
		[SerializeField]
		private SliderPanel _canyonsBreadthPanel;
		[SerializeField]
		private SliderPanel _canyonBlendFactorPanel;
		[SerializeField]
		private SliderPanel _canyonMinPanel;
		[SerializeField]
		private SliderPanel _canyonMinBreadthPanel;
		[SerializeField]
		private SliderPanel _canyonDepthFactorPanel;

		[Header("Temperature")]
		[SerializeField]
		private SliderPanel _minTempPanel;
		[SerializeField]
		private SliderPanel _maxTempPanel;

		public PlanetGenerator.Parameter CreateParameters()
		{
			if (_randomizeToggle.isOn)
				RandomizeSeed();
			Seed seed = CreateSeed();
			if (_randomizeToggle.isOn)
				Randomize(seed);
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

		public void Randomize(Seed seed)
		{
			_atmosphereSliderPanel.Randomize(seed);
			_maxHullSliderPanel.Randomize(seed);
			_bedrockSliderPanel.Randomize(seed);
			_seaLevelSliderPanel.Randomize(seed);

			_axisAnglePanel.Randomize(seed);
			_secondsPerRevolutionPanel.Randomize(seed);

			_plateSegmentsPanel.Randomize(seed);
			_continentsPanel.Randomize(seed);
			_oceansPanel.Randomize(seed);
			_platesPanel.Randomize(seed);
			_platesMinForcePanel.Randomize(seed);
			_warpPanel.Randomize(seed);
			_warpChaosPanel.Randomize(seed);
			_blendPanel.Randomize(seed);
			_sampleEliminationPanel.Randomize(seed);

			_mountainBreadthPanel.Randomize(seed);
			_mountainBlendFactorPanel.Randomize(seed);
			_mountainMinPanel.Randomize(seed);
			_mountainMinBreadthPanel.Randomize(seed);
			_mountainHeightFactorPanel.Randomize(seed);

			_canyonsBreadthPanel.Randomize(seed);
			_canyonBlendFactorPanel.Randomize(seed);
			_canyonMinPanel.Randomize(seed);
			_canyonMinBreadthPanel.Randomize(seed);
			_canyonDepthFactorPanel.Randomize(seed);

			_minTempPanel.Randomize(seed);
			_maxTempPanel.Randomize(seed);
		}

		public void Load(PlanetGenerator.Parameter parameter)
		{
			PlanetDimensions dimensions = parameter.Dimensions;
			_atmosphereSliderPanel.Slider.value = dimensions.AtmosphereRadius;
			_maxHullSliderPanel.Slider.value = dimensions.HullMaxRadius;
			_bedrockSliderPanel.Slider.value = dimensions.KernelRadius;
			_seaLevelSliderPanel.Slider.value = dimensions.RelativeSeaLevel;

			PlanetAxisData axis = parameter.AxisData;
			_axisAnglePanel.Slider.value = axis.Angle;
			_secondsPerRevolutionPanel.Slider.value = axis.SecondsPerRevolution;

			ContinentalPlatesParameter plates = parameter.ContinentalPlatesParameter;
			_plateSegmentsPanel.Slider.value = plates.SegmentsAmount;
			_continentsPanel.Slider.value = plates.ContinentsAmount;
			_oceansPanel.Slider.value = plates.OceansAmount;
			_platesPanel.Slider.value = plates.PlatesAmount;
			_platesMinForcePanel.Slider.value = plates.PlatesMinForce;
			_warpPanel.Slider.value = plates.WarpFactor;
			_warpChaosPanel.Slider.value = plates.WarpChaosFactor;
			_blendPanel.Slider.value = plates.BlendFactor;
			_sampleEliminationPanel.Slider.value = plates.SampleEliminationFactor;

			PlatesShapingParameter shaping = parameter.Shaping.Plates;
			_mountainBreadthPanel.Slider.value = shaping.MountainsBreadthFactor;
			_mountainBlendFactorPanel.Slider.value = shaping.MountainsBlendDistanceFactor;
			_mountainMinPanel.Slider.value = shaping.MountainMin;
			_mountainMinBreadthPanel.Slider.value = shaping.MountainMinBreadth;
			_mountainHeightFactorPanel.Slider.value = shaping.MountainHeightFactor;
			_canyonsBreadthPanel.Slider.value = shaping.CanyonsBreadthFactor;
			_canyonBlendFactorPanel.Slider.value = shaping.CanyonsBlendDistanceFactor;
			_canyonMinPanel.Slider.value = shaping.CanyonMin;
			_canyonMinBreadthPanel.Slider.value = shaping.CanyonMinBreadth;
			_canyonDepthFactorPanel.Slider.value = shaping.CanyonDepthFactor;

			TemperatureSpectrum tempSpectrum = parameter.TemperatureSpectrum;
			_minTempPanel.Slider.value = tempSpectrum.Min;
			_maxTempPanel.Slider.value = tempSpectrum.Max;
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
			float warpChaosFactor = _warpChaosPanel.Slider.value;
			float blendFactor = _blendPanel.Slider.value;
			float sampleEliminationFactor = _sampleEliminationPanel.Slider.value;
			sampleEliminationFactor = 1 + (1 - sampleEliminationFactor) * _maximalSampleEliminationFactor;
			float platesMinFoce = _platesMinForcePanel.Slider.value;
			ContinentalPlatesParameter continentalPlatesParameter =
				new ContinentalPlatesParameter(plateSegments, contients, oceans, plates, warpFactor, warpChaosFactor, blendFactor, sampleEliminationFactor, platesMinFoce);
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
			float mountainHeightFactor = _mountainHeightFactorPanel.Slider.value;
			float canyonBreadthFactor = _canyonsBreadthPanel.Slider.value;
			float canyonBlendFactor = _canyonBlendFactorPanel.Slider.value;
			float canyonMin = _canyonMinPanel.Slider.value;
			float canyonMinBreadth = _canyonMinBreadthPanel.Slider.value;
			float canyonDepthFactor = _canyonDepthFactorPanel.Slider.value;
			return new PlatesShapingParameter(
				mountainBreadthFactor, 
				mountainBlendFactor, 
				mountainMin,
				mountainMinBreadth,
				mountainHeightFactor,
				canyonBreadthFactor, 
				canyonBlendFactor, 
				canyonMin,
				canyonMinBreadth,
				canyonDepthFactor);
		}

		private Seed CreateSeed()
		{
			string seedText = _seedInput.InputField.text;
			int seedValue = int.Parse(seedText);
			return new Seed(seedValue);
		}

		private void RandomizeSeed()
		{
			int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			_seedInput.InputField.text = seed.ToString();
		}
	}
}