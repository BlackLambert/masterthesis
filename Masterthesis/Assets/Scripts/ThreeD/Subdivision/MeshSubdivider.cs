using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface MeshSubdivider
	{
		void Subdivide(Mesh mesh);
		void Subdivide(Mesh mesh, int amount);

		public class VertexLimitReachedException : Exception { }
	}
}