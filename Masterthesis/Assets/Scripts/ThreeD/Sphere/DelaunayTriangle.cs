using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public struct DelaunayTriangle
    {
        public Vector3Int VertexIndices { get; }
        public Vector3 Normal { get; }

        public DelaunayTriangle(Vector3Int vertexIndices, Vector3 normal)
		{
            VertexIndices = vertexIndices;
            Normal = normal;
        }

        public bool ContainsEdge(Vector2Int edge)
		{
            bool b0 = CompareEdge(edge, 0, 1);
            bool b1 = CompareEdge(edge, 1, 2);
            bool b2 = CompareEdge(edge, 2, 0);
            return b0 || b1 || b2;
		}

		private bool CompareEdge(Vector2Int edge, int i0, int i1)
		{
            return edge[0] == VertexIndices[i0] && edge[1] == VertexIndices[i1] ||
                edge[1] == VertexIndices[i0] && edge[0] == VertexIndices[i1];
		}
	}
}