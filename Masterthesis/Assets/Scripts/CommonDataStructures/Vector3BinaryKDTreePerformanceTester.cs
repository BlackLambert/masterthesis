using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTreePerformanceTester : MonoBehaviour
    {
        [SerializeField]
        private int _pointsCount = 1000;
        [SerializeField]
        private float _findNearestRadius = 0.1f;
        [SerializeField]
        private Vector3 _sample = Vector3.zero;

        private Vector3BinaryKDTree _tree;

        protected virtual void Start()
		{
            Vector3[] points = CreateSamples(_pointsCount);
            float timeStamp = Time.realtimeSinceStartup;
            _tree = new Vector3BinaryKDTree(points, new Vector3SelectionSorter());
            Debug.Log($"The creation of the tree took {Time.realtimeSinceStartup - timeStamp} seconds");
            int depth = _tree.Depth;
            Debug.Log($"The created tree has a depth of {depth}");
            timeStamp = Time.realtimeSinceStartup;
            int nearest = _tree.GetNearestTo(_sample);
            Debug.Log($"The nearest Point to {_sample} is at index {nearest} ({points[nearest]}). The search took {Time.realtimeSinceStartup - timeStamp} seconds.");
            timeStamp = Time.realtimeSinceStartup;
            IList<int> nearestList = _tree.GetNearestToWithin(_sample, _findNearestRadius);
            Debug.Log($"{nearestList.Count} nearest points to {_sample} found within radius {_findNearestRadius}. The search took {Time.realtimeSinceStartup - timeStamp} seconds.");
        }

		private Vector3[] CreateSamples(int samplesCount)
		{
            Vector3[] result = new Vector3[samplesCount];
            for (int i = 0; i < samplesCount; i++)
                result[i] = UnityEngine.Random.insideUnitSphere;
            return result;
        }
	}
}