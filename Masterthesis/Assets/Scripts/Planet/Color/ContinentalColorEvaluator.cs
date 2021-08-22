using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class ContinentalColorEvaluator : PlanetColorEvaluator
	{
		private ContinentalBiome _biome;
		private Vector3 _vertex;
		private Vector3 _vertexNormal;

		public ContinentalColorEvaluator (
			float blendDistance):
			base(blendDistance)
		{
			
		}

		public void Init(ContinentalBiome biome,
			float distanceToBorder)
		{
			_biome = biome;
			Init(distanceToBorder);
		}

		public void InitVertexData(
			Vector3 vertex,
			Vector3 vertexNormal)
		{
			_vertex = vertex;
			_vertexNormal = vertexNormal;
		}

		public override Result Evaluate()
		{
			float blendWeight = GetBlendWeight();
			float angle = Vector3.Angle(_vertex.normalized, _vertexNormal.normalized);
			float weight = angle / 90;
			//weight = Mathf.Clamp01(weight / _biome.SlopeThreshold);
			//Color color = Color.Lerp(_biome.BaseColor, _biome.SlopeColor, weight);
			Color color = new Color(1, 1, 1);
			return new Result(blendWeight, color);
		}
	}
}