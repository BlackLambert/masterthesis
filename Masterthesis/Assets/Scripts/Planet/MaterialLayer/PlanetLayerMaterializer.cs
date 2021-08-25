using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterializer
    {
		private PlanetAirLayerAdder _airAdder;
		private PlanetRockLayerAdder _rockAdder;
		private PlanetGroundLayerAdder _groundAdder;
		private readonly PlanetLiquidLayerAdder _liquidAdder;
		private readonly PlanetGroundVegetationLayerAdder _vegetationAdder;

        public PlanetLayerMaterializer(
            PlanetAirLayerAdder airAdder,
            PlanetRockLayerAdder rockAdder,
            PlanetGroundLayerAdder groundAdder,
            PlanetLiquidLayerAdder liquidAdder,
            PlanetGroundVegetationLayerAdder vegetationAdder)
		{
            _airAdder = airAdder;
            _rockAdder = rockAdder;
            _groundAdder = groundAdder;
			_liquidAdder = liquidAdder;
            _vegetationAdder = vegetationAdder;

        }

        public void CreateRockLayer(Parameter parameter)
        {
            PlanetRockLayerAdder.Parameter p = CreateRockParameter(parameter);
            _rockAdder.AddLayer(p);
        }

        public void CreateGroundLayer(Parameter parameter)
        {
            PlanetLayerAdder.Parameter p = CreateParameter(parameter);
            _groundAdder.AddLayer(p);
        }

        public void CreateLiquidLayer(Parameter parameter)
        {
            PlanetLayerAdder.Parameter p = CreateParameter(parameter);
            _liquidAdder.AddLayer(p);
        }

        public void CreateGroundVegetationLayer(Parameter parameter)
        {
            PlanetLayerAdder.Parameter p = CreateParameter(parameter);
            _vegetationAdder.AddLayer(p);
        }

		public void CreateAirLayer(Parameter parameter)
        {
            PlanetLayerAdder.Parameter p = CreateParameter(parameter);
            _airAdder.AddLayer(p);
        }

		private PlanetLayerAdder.Parameter CreateParameter(Parameter parameter)
		{
            return new PlanetLayerAdder.Parameter(parameter.Biomes, parameter.Planet, parameter.BlendFactor);
        }

		private PlanetRockLayerAdder.Parameter CreateRockParameter(Parameter parameter)
		{
            return new PlanetRockLayerAdder.Parameter(parameter.Biomes, parameter.Planet, parameter.BlendFactor, parameter.ShapingLayers);
        }

        public class Parameter
		{
            public Parameter(Planet planet,
                Biome[] biomes,
                ShapingLayer[] shapingLayers,
                float blendFactor)
			{
				Planet = planet;
				Biomes = biomes;
				ShapingLayers = shapingLayers;
				BlendFactor = blendFactor;
			}

			public Planet Planet { get; }
			public Biome[] Biomes { get; }
			public ShapingLayer[] ShapingLayers { get; }
			public float BlendFactor { get; }
		}
    }
}