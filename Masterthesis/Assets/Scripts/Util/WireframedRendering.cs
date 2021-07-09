using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class WireframedRendering : MonoBehaviour
    {
        void OnPreRender()
        {
            GL.wireframe = true;
        }
        void OnPostRender()
        {
            GL.wireframe = false;
        }
    }
}