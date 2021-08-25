using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetAirLayerAdder : PlanetLayerAdder
    {
        protected override PlanetMaterialState LayerState => PlanetMaterialState.Gas;
        protected override int MaterialIndex => 3;

        public PlanetAirLayerAdder(PlanetLayerMaterialSerializer serializer) : base(serializer)
		{
        }

		protected override void AddLayer(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                AddLayer(face, i, CalculateHeight(face.Data.EvaluationPoints[i]));
        }

		protected override void InitConcrete(Parameter parameter)
		{
            
        }

        private float CalculateHeight(EvaluationPointData data)
        {
            float heightSum = data.Layers.Sum(l => l.Height);
            return Mathf.Clamp01(1 - heightSum);
        }
	}
}