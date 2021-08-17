using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class PlanetColorEvaluator
    {
		private float _blendDistance;
		private float _distanceToBorder;

		public PlanetColorEvaluator(
			float blendDistance)
		{
			_blendDistance = blendDistance;
		}

		protected void Init(float distanceToBorder)
		{
			_distanceToBorder = distanceToBorder;
		}

		public abstract Result Evaluate();



		protected float GetBlendWeight()
		{
			if (_distanceToBorder <= -_blendDistance)
				return 1;
			else if (_distanceToBorder >= _blendDistance)
				return 0;
			return 1 - (_distanceToBorder + _blendDistance) / (_blendDistance * 2);
		}

		public class Result
		{
            public Result(float weight,
                Color color)
			{
				Weight = weight;
				Color = color;
			}

			public float Weight { get; }
			public Color Color { get; }
		}
    }
}