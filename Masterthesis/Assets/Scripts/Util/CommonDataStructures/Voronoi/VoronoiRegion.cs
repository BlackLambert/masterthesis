using UnityEngine;

namespace SBaier.Master
{
    public class VoronoiRegion : Polygon
    {
        public Vector3 Site { get; }
        public int[] NeighborIndices { get; }
        public Vector3[] DistanceVectorToNeighbors { get; }
        public float[] DistanceToNeighbors { get; }

        public override int[] VertexIndices { get; }

		public VoronoiRegion(Vector3 site, 
            int[] corners, 
            int[] neighborIndices,
            Vector3[] neighborSites)
		{
            Site = site;
            VertexIndices = corners;
            NeighborIndices = neighborIndices;
            DistanceVectorToNeighbors = CalculateDistanceVectorToNeighbors(neighborSites);
            DistanceToNeighbors = CalculateDistanceToNeighbors();
        }

		private Vector3[] CalculateDistanceVectorToNeighbors(Vector3[] neighborSites)
		{
            Vector3[] result = new Vector3[NeighborIndices.Length];
			for (int i = 0; i < neighborSites.Length; i++)
                result[i] = neighborSites[i].FastSubstract(Site) / 2;
            return result;
        }

		private float[] CalculateDistanceToNeighbors()
		{
            float[] result = new float[DistanceVectorToNeighbors.Length];
            for (int i = 0; i < DistanceVectorToNeighbors.Length; i++)
                result[i] = DistanceVectorToNeighbors[i].magnitude;
            return result;
        }
	}
}