using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public static class Vector3Extensions
    {
        public static Vector3 FastMultiply(this Vector3 v1, int s)
		{
            Vector3 result;
            result.x = v1.x * s;
            result.y = v1.y * s;
            result.z = v1.z * s;
            return result;
        }

        public static Vector3 FastAdd(this Vector3 v1, Vector3 v2)
		{
            Vector3 result;
            result.x = v1.x + v2.x;
            result.y = v1.y + v2.y;
            result.z = v1.z + v2.z;
            return result;
        }
    }
}