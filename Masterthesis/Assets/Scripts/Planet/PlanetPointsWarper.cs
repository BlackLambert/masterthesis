using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetPointsWarper
    {
		private const float _maxWarpAngle = 360;
		public Noise3D _warpNoise;

        private Vector3[] _vertices;
        private float _warpFactor;
		private float _warpChaosFactor;
		private float _evalRadius;


        public PlanetPointsWarper(Noise3D warpNoise)
		{
            _warpNoise = warpNoise;
		}

        public Vector3[] Warp(Vector3[] vertices, float warpFactor, float warpChaosFactor, float evalRadius)
		{
            Init(vertices, warpFactor, warpChaosFactor, evalRadius);
            float[] warpValues = WarpContientenalPlateEvaluationPoint();
            return WarpSpherePoints(warpValues);
        }

		private void Init(Vector3[] vertices, float warpFactor, float warpChaosFactor, float evalRadius)
		{
            _vertices = vertices;
            _warpFactor = warpFactor;
            _warpChaosFactor = warpChaosFactor;
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
            Vector3 normalized = vertex.normalized;
            float dot = Vector3.Dot(normalized, Vector3.forward);
            Vector3 crossVector = dot == 1 ? Vector3.right : Vector3.forward;
            Vector3 tangential = Vector3.Cross(normalized, crossVector);
            float deltaLength = warpValue * _warpFactor * _evalRadius;
            Vector3 deltaVector = tangential.FastMultiply(deltaLength);
            Vector3 result = normalized.FastMultiply(_evalRadius);
            float angle = (warpValue * _maxWarpAngle * _warpChaosFactor)%_maxWarpAngle;
            Quaternion rot = Quaternion.AngleAxis(angle, result);
            result = result.FastAdd(rot * deltaVector);
            return result;
        }
    }
}