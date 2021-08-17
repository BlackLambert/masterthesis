using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class ShapingLayerFactory
    {
		private readonly Vector3BinaryKDTreeFactory _kdTreeFactory;

		public ShapingLayerFactory(Vector3BinaryKDTreeFactory kdTreeFactory)
		{
            _kdTreeFactory = kdTreeFactory;
		}

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
            KDTree<Vector3> tree = nodes.Length > 0 ? _kdTreeFactory.Create(nodes) : null;
            return new ShapingLayer(primitives, tree, noise, mode);
        }
    }
}