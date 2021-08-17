using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class BaseShapingLayerFactory : ShapingLayerFactory
    {
		public BaseShapingLayerFactory(Vector3BinaryKDTreeFactory kdTreeFactory) : base(kdTreeFactory)
		{
		}

		public ShapingLayer Create(Noise3D noise, ShapingLayer.Mode mode)
		{
            ShapingPrimitive[] primitive = new ShapingPrimitive[] { new AllOverShapingPrimitive(Vector3.zero, 0, 1) };
            return CreateShapingLayer(primitive, noise, mode);
        }
    }
}