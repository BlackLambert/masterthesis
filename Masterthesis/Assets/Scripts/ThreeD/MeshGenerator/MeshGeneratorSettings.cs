using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class MeshGeneratorSettings : ScriptableObject
    {
        public abstract MeshGeneratorType Type { get; } 
    }
}