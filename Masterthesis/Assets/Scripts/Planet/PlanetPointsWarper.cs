using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetPointsWarper
    {
        public Noise3D _warpNoise;

		public PlanetPointsWarper(Noise3D warpNoise)
		{
            _warpNoise = warpNoise;
		}

        public Vector3[] Warp(Vector3[] vertices, float warpFactor, float evalRadius)
		{
            float[] warpValues = WarpContientenalPlateEvaluationPoint(vertices);
            vertices = WarpSpherePoints(vertices, warpValues, warpFactor, evalRadius);
            return vertices;
        }

        private float[] WarpContientenalPlateEvaluationPoint(Vector3[] vertices)
        {
            NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(vertices, Allocator.TempJob);
            NativeArray<float> resultNative = _warpNoise.Evaluate3D(verticesNative);
            float[] result = resultNative.ToArray();
            verticesNative.Dispose();
            resultNative.Dispose();
            return result;
        }

        private Vector3[] WarpSpherePoints(Vector3[] vertices, float[] warpValues, float warpFactor, float evalRadius)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i].normalized * evalRadius;
                vertex = WarpSpherePoint(vertex, warpValues[i], warpFactor, evalRadius);
                vertices[i] = vertex.normalized * evalRadius;
            }
            return vertices;
        }

        private Vector3 WarpSpherePoint(Vector3 vertex, float warpValue, float warpFactor, float evalRadius)
        {
            Vector3 crossVector = vertex.normalized == Vector3.forward ? Vector3.right : Vector3.forward;
            Vector3 tangential = Vector3.Cross(vertex, crossVector);
            Vector3 deltaVector = tangential.normalized * warpValue * evalRadius * warpFactor;
            float sinWarpValueHalf = Mathf.Sin(Mathf.PI * warpValue);
            float cosWarpValueHalf = Mathf.Cos(Mathf.PI * warpValue);
            deltaVector = new Quaternion(sinWarpValueHalf * vertex.x,
                sinWarpValueHalf * vertex.y,
                sinWarpValueHalf * vertex.z,
                cosWarpValueHalf).normalized * deltaVector;
            return vertex + deltaVector;
        }
    }
}