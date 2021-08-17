using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetPointsWarper
    {
        public Noise3D _warpNoise;

        private Vector3[] _vertices;
        private float _warpFactor;
        private float _evalRadius;


        public PlanetPointsWarper(Noise3D warpNoise)
		{
            _warpNoise = warpNoise;
		}

        public Vector3[] Warp(Vector3[] vertices, float warpFactor, float evalRadius)
		{
            Init(vertices, warpFactor, evalRadius);
            float[] warpValues = WarpContientenalPlateEvaluationPoint();
            return WarpSpherePoints(warpValues);
        }

		private void Init(Vector3[] vertices, float warpFactor, float evalRadius)
		{
            _vertices = vertices;
            _warpFactor = warpFactor;
            _evalRadius = evalRadius;
        }

		private float[] WarpContientenalPlateEvaluationPoint()
        {
            NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(_vertices, Allocator.TempJob);
            NativeArray<float> resultNative = _warpNoise.Evaluate3D(verticesNative);
            float[] result = resultNative.ToArray();
            verticesNative.Dispose();
            resultNative.Dispose();
            return result;
        }

        private Vector3[] WarpSpherePoints(float[] warpValues)
        {
            Vector3[] result = new Vector3[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
                result[i] = WarpSpherePoint(_vertices[i], warpValues[i]);
            return result;
        }

        private Vector3 WarpSpherePoint(Vector3 vertex, float warpValue)
        {
            vertex = vertex.normalized * _evalRadius;
            float dot = Vector3.Dot(vertex.normalized, Vector3.forward);
            Vector3 crossVector = dot > Mathf.PI / 2 ? Vector3.right : Vector3.forward;
            Vector3 tangential = Vector3.Cross(vertex, crossVector);
            Vector3 deltaVector = tangential.normalized * warpValue * _evalRadius * _warpFactor;
            Quaternion rot = Quaternion.AngleAxis(warpValue * 360, vertex);
            vertex += rot * deltaVector;
            return vertex;
        }
    }
}