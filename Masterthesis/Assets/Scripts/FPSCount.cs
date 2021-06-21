using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class FPSCount : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        // Update is called once per frame
        void Update()
        {
            _text.text = (1.0f / Time.deltaTime).ToString();
        }
    }
}