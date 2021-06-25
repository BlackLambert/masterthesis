using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public interface MeshFaceSeparatorTarget
    {
        public MeshFilter MeshFilter { get; }
        public Transform Base { get; }
    }
}