using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class VertexDistanceRangeCalculatorOnStart : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private bool _ignoreDiagonals = true;

        // Start is called before the first frame update
        void Start()
        {
            Vector3[] vertices = _meshFilter.sharedMesh.vertices;
            int[] indices = _meshFilter.sharedMesh.triangles;

            float max = -1;
            float min = -1;

            for(int i = 0; i < indices.Length / 3; i++)
			{
                Vector3 v0 = vertices[indices[i * 3]];
                Vector3 v1 = vertices[indices[i * 3 + 1]];
                Vector3 v2 = vertices[indices[i * 3 + 2]];

                Vector3 v10 = v1 - v0;
                Vector3 v21 = v2 - v1;
                Vector3 v02 = v0 - v2;

                float d0 = v10.magnitude;
                float d1 = v21.magnitude;
                float d2 = v02.magnitude;

                float cMax = (d0 > d1 && d0 > d2) ? d0 : (d1 > d2) ? d1 : d2;
                float cMin = (d0 < d1 && d0 < d2) ? d0 : (d1 < d2) ? d1 : d2;
                float cMid = (cMax == d0 && cMin == d1) || (cMax == d1 && cMin == d0) ? d2 : 
                    (cMax == d1 && cMin == d2) || (cMax == d2 && cMin == d1) ? d0 : d1;

                if (cMin < min || min < 0)
                    min = cMin;
                if (_ignoreDiagonals && (cMid > max || max < 0))
                    max = cMid;
                else if (!_ignoreDiagonals && (cMax > max || max < 0))
                    max = cMax;
            }

            Debug.Log($"Max vertex distance of the mesh on {_meshFilter.gameObject.name}:{max}");
            Debug.Log($"Min vertex distance of the mesh on {_meshFilter.gameObject.name}:{min}");
            Debug.Log($"Relative distance (max / min) of the mesh on {_meshFilter.gameObject.name}:{max / min}");
        }
    }
}