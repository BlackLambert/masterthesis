using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTreeFactory
    {
        private QuickSelector<Vector3> _quickSelector;

        public Vector3BinaryKDTreeFactory(QuickSelector<Vector3> quickSelector)
		{
            _quickSelector = quickSelector;
        }

        public KDTree<Vector3> Create(Vector3[] nodes)
		{
            return new Vector3BinaryKDTree(nodes, _quickSelector);
		}
    }
}
