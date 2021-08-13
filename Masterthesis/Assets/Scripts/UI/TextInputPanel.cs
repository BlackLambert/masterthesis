using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SBaier.Master
{
    public class TextInputPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _inputField;
        public TMP_InputField InputField => _inputField;
    }
}