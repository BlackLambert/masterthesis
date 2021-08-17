using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class AllOverShapingPrimitive : ShapingPrimitive
	{
		public AllOverShapingPrimitive(Vector3 position, float blendArea, float weight) : 
			base(position, blendArea, weight)
		{

		}

		public override float MaxAreaOfEffect => float.MaxValue;

		protected override float GetBlendedValue(Vector3 point)
		{
			return 1;
		}

		protected override void InitEvaluation(Vector3 point)
		{
			
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			return true;
		}

		protected override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			return false;
		}
	}
}