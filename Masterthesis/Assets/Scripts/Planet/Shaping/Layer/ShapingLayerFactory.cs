using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class ShapingLayerFactory
    {

        protected ShapingLayer[] CreateLayers(ShapingPrimitive[][] primitives, Noise3D[] noise, ShapingLayer.Mode[] modes)
        {
            List<ShapingLayer> layers = new List<ShapingLayer>();
            for (int i = 0; i < primitives.Length; i++)
                layers.Add(CreateShapingLayer(primitives[i], noise[i], modes[i]));
            return layers.ToArray();
        }

        protected ShapingLayer CreateShapingLayer(ShapingPrimitive[] primitives, Noise3D noise, ShapingLayer.Mode mode)
        {
            Vector3[] nodes = primitives.Select(p => p.Position).ToArray();
            return new ShapingLayer(primitives, noise, mode);
        }
    }
}