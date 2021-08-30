using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class Polygon 
    {
        public abstract int[] VertexIndices { get; }


        public bool HasEdge(Vector2Int edge)
        {
            int count = VertexIndices.Length;
            for (int i = 0; i < count; i++)
            {
                if (IsSameEdge(edge, i, (i + 1) % count))
                    return true;
            }
            return false;
        }

        public bool HasCorner(int vertexIndex)
        {
            int[] vertices = VertexIndices;
            int count = vertices.Length;;
            for (int i = 0; i < count; i++)
            {
                if (vertices[i] == vertexIndex)
                    return true;
            }
            return false;
        }

        private bool IsSameEdge(Vector2Int edge, int i0, int i1)
        {
            int[] vertices = VertexIndices;
            int edge0 = edge.x;
            int edge1 = edge.y;
            int vI0 = vertices[i0];
            int vI1 = vertices[i1];
            return edge0 == vI0 && edge1 == vI1 ||
                edge1 == vI0 && edge0 == vI1;
        }
    }
}