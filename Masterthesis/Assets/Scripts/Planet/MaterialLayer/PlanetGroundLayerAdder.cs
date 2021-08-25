using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetGroundLayerAdder : PlanetLayerAdder
    {
        private const float _maxSolidTopLayerThickness = 0.02f;
        private const float _maxSlopeAngle = 90;

        private Biome[] _biomes;

		protected override PlanetMaterialState LayerState => PlanetMaterialState.Solid;
		protected override int MaterialIndex => 1;

		public PlanetGroundLayerAdder(PlanetLayerMaterialSerializer serializer) : base(serializer)
		{
        }

        protected override void InitConcrete(Parameter parameter)
        {
            _biomes = parameter.Biomes;
        }

        protected override void AddLayer(PlanetFace face)
        {
            Vector3[] normals = face.Normals;
            for (int i = 0; i < normals.Length; i++)
            {
                EvaluationPointData data = face.Data.EvaluationPoints[i];
                AddLayer(face, i, CalculateHeight(data.WarpedPoint, normals[i], _biomes[data.BiomeID].GroundMaterial));
            }
        }

        private float CalculateHeight(Vector3 warpedPosition, Vector3 normal, SolidPlanetLayerMaterialSettings topSolidMaterial)
        {
            float angle = Vector3.Angle(warpedPosition, normal);
            float portion = angle / _maxSlopeAngle;
            float density = topSolidMaterial.Density;
            if (portion > density)
                return 0;
            float halfDensity = density / 2;
            if (portion < halfDensity)
                return _maxSolidTopLayerThickness;
            float heightPortion = 1 - (portion - halfDensity) / (topSolidMaterial.Density - halfDensity);
            return heightPortion * _maxSolidTopLayerThickness;
        }
	}
}