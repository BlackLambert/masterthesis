using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class BasicColorEvaluator : PlanetColorEvaluator
	{
		private Biome _biome;

		public BasicColorEvaluator(
			float blendDistance):
			base(blendDistance)
		{
			 
		}

		public void Init(
			Biome biome,
			float distanceToBorder)
		{
			_biome = biome;
			Init(distanceToBorder);
		}

		public override Result Evaluate()
		{
			Color color = _biome.BaseColor;
			float weight = GetBlendWeight();
			return new Result(weight, color);
		}
	}
}