using UnityEngine;
using UnityEditor;
using Zenject;
using Unity.Collections;
using System.Linq;

namespace SBaier.Master.Editor
{
    public class NoiseInspector : ZenjectEditorWindow
    {
        [MenuItem("Window/NoiseInspector")]
        public static NoiseInspector GetOrCreateWindow()
        {
            var window = EditorWindow.GetWindow<NoiseInspector>();
            window.titleContent = new GUIContent("NoiseInspector");
            return window;
        }

        public override void InstallBindings()
        {
            int rand = Random.Range(int.MinValue, int.MaxValue);
            Container.Bind<Seed>().AsTransient().WithArguments(rand);
            Container.Bind<NoiseFactory>().To<NoiseFactoryImpl>().AsTransient();

            Container.BindInterfacesTo<NoiseInspectorController>().AsSingle();
        }
    }

	class NoiseInspectorController : IGuiRenderable
	{
        private const int _distributionCalculationSteps = 100;

        private readonly NoiseFactory _factory;
        private NoiseSettings _noiseSettingsToInspect;
        private Vector3Int _samplesAmount = new Vector3Int(20, 20, 20);
        private float _delta = 0.24f;
        private Seed _seed;

        private float[] _result;
        private float _maxValue = 0;
        private float _minValue = 0;
        private float _calculationDuration = 0;
        private bool _resultFoldedOut = true;
        private int _samplesTaken = 0;
        private AnimationCurve _valueDistribution = AnimationCurve.Linear(0, 0, 1, 0);

        public NoiseInspectorController(NoiseFactory factory, Seed seed)
		{
            _factory = factory;
            _seed = seed;
        }


        public void GuiRender()
		{
            EditorGUILayout.LabelField("Use this window to inspect any NoiseSettings");
            _noiseSettingsToInspect = EditorGUILayout.ObjectField("Noise to inspect:", _noiseSettingsToInspect, typeof(NoiseSettings), false) as NoiseSettings;
            _samplesAmount = EditorGUILayout.Vector3IntField("Samples amount", _samplesAmount);
            _delta = EditorGUILayout.FloatField("Sample Delta", _delta);

            if (GUILayout.Button("Inspect!"))
			{
                if (_noiseSettingsToInspect == null)
                    EditorGUILayout.LabelField("Please select a noise setting to inspect");
                else if (_samplesAmount.x <= 0 || _samplesAmount.y <= 0 || _samplesAmount.z <= 0)
                    EditorGUILayout.LabelField("Please select valid samples");
                else if (_delta == 0)
                    EditorGUILayout.LabelField("Please choose a delta, that is not zero");
                else
                    CalculateNoiseValues();
            }

            if(_result != null && _result.Length != 0)
            {
                EditorGUILayout.Space();
                _resultFoldedOut = EditorGUILayout.Foldout(_resultFoldedOut, "Result");
                if (_resultFoldedOut)
                {
                    EditorGUILayout.LabelField($"Seed {_seed.SeedNumber}");
                    EditorGUILayout.LabelField($"{_samplesTaken} sample points have been evaluated");
                    EditorGUILayout.LabelField($"The evaluation process took {_calculationDuration} seconds");
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"The minimal evaluated Value is: {_minValue}");
                    EditorGUILayout.LabelField($"The maximal evaluated Value is: {_maxValue}");
                    EditorGUILayout.CurveField("Value distribution", _valueDistribution);
                }
            }
        }

        private void CalculateNoiseValues()
		{
            Noise3D noise = _factory.Create(_noiseSettingsToInspect, _seed);
            int samples = _samplesAmount.x * _samplesAmount.y * _samplesAmount.z;
            NativeArray<Vector3> points = new NativeArray<Vector3>(samples, Allocator.TempJob);

            Vector3 _xDelta = new Vector3(_delta, 0, 0);
            Vector3 _yDelta = new Vector3(0, _delta, 0);
            Vector3 _zDelta = new Vector3(0, 0, _delta);
            int currentSample = 0;

            for(int x = 0; x < _samplesAmount.x; x++)
			{
                for (int y = 0; y < _samplesAmount.y; y++)
                {
                    for (int z = 0; z < _samplesAmount.z; z++)
                    {
                        Vector3 point = Vector3.zero;
                        point += x * _xDelta;
                        point += y * _yDelta;
                        point += z * _zDelta;
                        points[currentSample] = point;
                        currentSample++;
                    }
                }
            }

            float startTime = Time.realtimeSinceStartup;
            NativeArray<float> result = noise.Evaluate3D(points);
            _calculationDuration = Time.realtimeSinceStartup - startTime;
            points.Dispose();
            _result = result.ToArray();
            result.Dispose();
            _minValue = _result.Min();
            _maxValue = _result.Max();
            _samplesTaken = samples;

            CalculateValueDistribution();
        }

        private void CalculateValueDistribution()
        {
            float min = 0;
            float max = 1;
            _valueDistribution = AnimationCurve.Linear(min, 0, max, 0);
            float delta = (max - min) / _distributionCalculationSteps;
            float current = min;
            float currentMax = current + delta;

            for (int i = 0; i < _distributionCalculationSteps; i++)
            {
                float value = (float)_result.Count(r => r > current && r < currentMax) / _samplesTaken;
                _valueDistribution.AddKey(current, value);
                AnimationUtility.SetKeyLeftTangentMode(_valueDistribution, i, AnimationUtility.TangentMode.Constant);
                current += delta;
                currentMax = current + delta;
            }

            for (int i = 0; i < _distributionCalculationSteps; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(_valueDistribution, i, AnimationUtility.TangentMode.Constant);
                AnimationUtility.SetKeyRightTangentMode(_valueDistribution, i, AnimationUtility.TangentMode.Constant);
            }
        }
	}
}