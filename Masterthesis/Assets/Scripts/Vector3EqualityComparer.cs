using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SBaier.Master
{
    // Source: Unity forum; User "Kiou-23"; https://forum.unity.com/threads/vector3-as-key-in-dictionary-does-not-work.509145/
    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {
        private float _epsilon;

        public Vector3EqualityComparer(float epsilon = 0.0001f)
		{
            _epsilon = epsilon;
        }

        public bool Equals(Vector3 vec1, Vector3 vec2)
        {
            return Equals(vec1.x, vec2.x) && Equals(vec1.y, vec2.y) && Equals(vec1.z, vec2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Equals(float f1, float f2)
		{
            return f1 == f2 || ((f1 - _epsilon) < f2) && ((f1 + _epsilon) > f2);
        }

        public int GetHashCode(Vector3 vec)
        {
            return Mathf.FloorToInt(vec.x) ^ Mathf.FloorToInt(vec.y) << 2 ^ Mathf.FloorToInt(vec.z) >> 2;
        }
    }
}