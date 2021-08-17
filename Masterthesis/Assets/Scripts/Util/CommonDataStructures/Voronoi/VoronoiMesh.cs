using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class VoronoiMesh : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private float _lineDisplayTime = 60;
        [SerializeField]
        private Color _lineColor = Color.green;

        private VoronoiDiagram _diagram;


        public void UpdateView(VoronoiDiagram diagram)
		{
            _diagram = diagram;
            _meshFilter.sharedMesh = new Mesh();
            _meshFilter.sharedMesh.vertices = _diagram.Vertices;
            Draw();
        }

        private void Draw()
        {
            foreach (VoronoiRegion region in _diagram.Regions)
				Draw(region);
		}

		private void Draw(VoronoiRegion region)
		{
			for (int i = 0; i < region.VertexIndices.Length; i++)
				Draw(region, i);
		}

		private void Draw(VoronoiRegion region, int index)
		{
			Vector3 c0 = _diagram.Vertices[region.VertexIndices[index]];
			Vector3 c1 = _diagram.Vertices[region.VertexIndices[(index + 1) % region.VertexIndices.Length]];
			Debug.DrawLine(c0, c1, _lineColor, _lineDisplayTime);
		}
	}
}