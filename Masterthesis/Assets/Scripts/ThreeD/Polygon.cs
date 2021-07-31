using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class Polygon 
    {
        public abstract IList<int> VertexIndices { get; }



        public bool ContainsEdge(Vector2Int edge)
        {
            int count = VertexIndices.Count;
            bool result = false;
			for (int i = 0; i < count; i++)
                result = result || CompareEdge(edge, i, (i + 1) % count);
            return result;
        }

        public bool ContainsCorner(int vertexIndex)
        {
            int count = VertexIndices.Count;
            bool result = false;
            for (int i = 0; i < count; i++)
                result = result || VertexIndices[i] == vertexIndex;
            return result;
        }

        private bool CompareEdge(Vector2Int edge, int i0, int i1)
        {
            return edge[0] == VertexIndices[i0] && edge[1] == VertexIndices[i1] ||
                edge[1] == VertexIndices[i0] && edge[0] == VertexIndices[i1];
        }
    }
}