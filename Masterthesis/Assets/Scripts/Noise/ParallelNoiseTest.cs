using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBaier.Master
{
    public class ParallelNoiseTest : MonoBehaviour
    {
        private const int _threadSleepTime = 0;
        private const int _defaultEvaluationAmount = 100000;

        [SerializeField]
        private NoiseSettings _noiseSettings;
        [SerializeField]
        private Text _sequencialText;
        [SerializeField]
        private Text _parallelText;
        [SerializeField]
        private InputField _iterationsInput;
        [SerializeField]
        private Button _performTestButton;

		private NoiseFactory _factory;
		private Seed _seed;
		private Noise3D _noise;

        private Vector3[] _evalPoints;

		[Inject]
        public void Construct(NoiseFactory factory, Seed seed)
		{
            _factory = factory;
            _seed = seed;
		}
        
        protected virtual void Start()
		{
            _noise = _factory.Create(_noiseSettings, _seed);
            _iterationsInput.text = _defaultEvaluationAmount.ToString();
            _performTestButton.onClick.AddListener(DoTests);
        }

        private void DoTests()
		{
            int iterations = int.Parse(_iterationsInput.text);
            FillEvalPoints(iterations);
            DoTest(DoSequencialTest, "Sequencial Test", _sequencialText, iterations);
            DoTest(DoParallelTest, "Parallel Test", _parallelText, iterations);
        }

		private void DoTest(Action<int> actionToTest, string testName, Text textField, int iterations)
		{
            float startTime = Time.realtimeSinceStartup;
            actionToTest(iterations);
            float duration = Time.realtimeSinceStartup - startTime;
            textField.text = $"The {testName} with {iterations} iterations of {_noise.NoiseType} took {duration} seconds";
        }

        private void DoSequencialTest(int iterations)
		{
            for (int i = 0; i < iterations; i++)
            {
                Thread.Sleep(_threadSleepTime);
                Vector3 point = _evalPoints[i];
                _noise.Evaluate(point.x, point.y, point.z);
            }
        }

        private void DoParallelTest(int iterations)
		{
            _noise.Evaluate(_evalPoints);
        }

        private void FillEvalPoints(int iterations)
        {
            _evalPoints = new Vector3[iterations];
            Vector3 startVector = Vector3.zero;
            Vector3 delta = new Vector3(-0.01f, 0.005f, 0.002f);
            for (int i = 0; i < _evalPoints.Length; i++)
                _evalPoints[i] = startVector + delta * i;
        }
    }
}