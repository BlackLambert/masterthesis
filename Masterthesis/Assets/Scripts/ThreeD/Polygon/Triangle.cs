using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Triangle : Polygon
    {
		private int[] _vertexIndices;
		public Vector3 Normal { get; }
        public Vector3 CircumcenterCenter { get; }

		public override int[] VertexIndices => _vertexIndices;

		public Triangle(int[] vertexIndices, Vector3 normal, Vector3 circumcenterCenter)
		{
            _vertexIndices = vertexIndices;
            Normal = normal;
            CircumcenterCenter = circumcenterCenter;
        }
	}
}