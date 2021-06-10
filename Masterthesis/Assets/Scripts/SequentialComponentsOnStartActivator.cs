using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class SequentialComponentsOnStartActivator : MonoBehaviour
    {
        [SerializeField]
        private List<MonoBehaviour> _components = new List<MonoBehaviour>();

        protected virtual void Awake()
		{
            foreach (MonoBehaviour component in _components)
                component.enabled = false;
        }

        protected virtual void Start()
		{
            foreach (MonoBehaviour component in _components)
                component.enabled = true;
        }
    }
}