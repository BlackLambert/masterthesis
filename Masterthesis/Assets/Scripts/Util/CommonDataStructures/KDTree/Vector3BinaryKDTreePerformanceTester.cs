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
        [SerializeField]
        private bool _doNaiveSearch = true;

        private Vector3BinaryKDTree _tree;

        protected virtual void Start()
		{
			Vector3[] points = CreateSamples(_pointsCount);

			Vector2 kDTreeTestTimes = DoKDTreeSearch(points);
			if (_doNaiveSearch)
			{
				Vector2 naiveTestTimes = DoNaiveSearch(points);
				Debug.Log($"The relative duration difference is {naiveTestTimes.x / kDTreeTestTimes.x} and {naiveTestTimes.y / kDTreeTestTimes.y}");
			}
		}

		private Vector2 DoKDTreeSearch(Vector3[] points)
		{
			float timeStamp = Time.realtimeSinceStartup;
			QuickSorter<Vector3, float> sorter = new QuickSorter<Vector3, float>((p, i) => p[i]);
			_tree = new Vector3BinaryKDTree(points, sorter);
			Debug.Log($"The creation of the tree took {Time.realtimeSinceStartup - timeStamp} seconds");
			int depth = _tree.Depth;
			Debug.Log($"The created tree has a depth of {depth}");
			timeStamp = Time.realtimeSinceStartup;
			int nearest = _tree.GetNearestTo(_sample);
			float singleTime = Time.realtimeSinceStartup - timeStamp;
			Debug.Log($"KDSEARCH: The nearest Point to {_sample} is at index {nearest} ({points[nearest]}). The search took {singleTime} seconds.");
			timeStamp = Time.realtimeSinceStartup;
			IList<int> nearestList = _tree.GetNearestToWithin(_sample, _findNearestRadius);
			float multiTime = Time.realtimeSinceStartup - timeStamp;
			Debug.Log($"KDSEARCH: {nearestList.Count} nearest points to {_sample} found within radius {_findNearestRadius}. The search took {multiTime} seconds.");
			return new Vector2(singleTime, multiTime);
		}

		private Vector2 DoNaiveSearch(Vector3[] points)
		{
			float timeStamp = Time.realtimeSinceStartup;
			int nearest = GetNearestNaive(points);
			float singleTime = Time.realtimeSinceStartup - timeStamp;
			Debug.Log($"NAIVE: The nearest Point to {_sample} is at index {nearest} ({points[nearest]}). The search took {singleTime} seconds.");
			timeStamp = Time.realtimeSinceStartup;
			IList<int> nearestList = GetNearestNaiveWithin(points, _findNearestRadius);
			float multiTime = Time.realtimeSinceStartup - timeStamp;
			Debug.Log($"NAIVE: {nearestList.Count} nearest points to {_sample} found within radius {_findNearestRadius}. The search took {multiTime} seconds.");
			return new Vector2(singleTime, multiTime);
		}

		private IList<int> GetNearestNaiveWithin(Vector3[] points, float findNearestRadius)
		{
			List<int> result = new List<int>();
			for (int i = 0; i < points.Length; i++)
				if ((points[i] - _sample).magnitude <= findNearestRadius)
					result.Add(i);
			return result;
		}

		private int GetNearestNaive(Vector3[] points)
		{
			float nearestDistance = float.PositiveInfinity;
			int result = -1;
			for (int i = 0; i < points.Length; i++)
			{
				float mag = (points[i] - _sample).magnitude;
				if (mag < nearestDistance)
				{
					nearestDistance = mag;
					result = i;
				}
			}
			return result;
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