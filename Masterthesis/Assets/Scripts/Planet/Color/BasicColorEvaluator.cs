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
			Color color = new Color(0,0,0);
			float weight = GetBlendWeight();
			return new Result(weight, color);
		}
	}
}