using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetGenerator : MonoBehaviour
    {
        [Header("Planet Settings")]
        [Range(0, 350)]
        [SerializeField]
        private int _subdivisions = 256;
        [SerializeField]
        private float _bedrockRadius = 4;
        [SerializeField]
        private float _atmosphereRadius = 10;
        [SerializeField]
        private float _continentalPlateMax = 0.1f;

        [Header("Temperature Spectrum")]
        [SerializeField]
        private float _teperatureMin = -20f;
        [SerializeField]
        private float _teperatureMax = 50f;

        [Header("Axis")]
        [Range(0, 90)]
        [SerializeField]
        private float _axisAngle = 20f;
        [SerializeField]
        private float _secondsPerRevolution = 60f;

        [Header("Planet Body")]
        [SerializeField]
        private IcosahedronGeneratorSettings _meshGeneratorSettings;

        [Header("Noise")]
        [SerializeField]
        private NoiseSettings _continentalPlatesNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentsNoiseSettings;


        private MeshGenerator _icosahedronGenerator;
        private MeshFaceSeparator _faceSeparator;
        private SphereMeshFormer _meshFormer;
        private Seed _seed;
        private MeshSubdivider _subdivider;
        private Planet.Factory _planetFactory;

        private Noise3D _continentsNoise;

        [Inject]
        public void Construct(MeshGeneratorFactory meshGeneratorFactory,
            MeshFaceSeparator faceSeparator,
            SphereMeshFormer meshFormer,
            NoiseFactory noiseFactory,
            Seed seed,
            MeshSubdivider subdivider,
            Planet.Factory planetFactory)
		{
            _faceSeparator = faceSeparator;
            _meshFormer = meshFormer;
            _seed = seed;
            _subdivider = subdivider;
            _planetFactory = planetFactory;

            _continentsNoise = noiseFactory.Create(_continentsNoiseSettings, _seed);
            _icosahedronGenerator = meshGeneratorFactory.Create(_meshGeneratorSettings);
        }

        protected virtual void Start()
		{
			Mesh baseMesh = new Mesh();
			GenerateMesh(baseMesh);
			MeshFaceSeparatorTarget[] targets = SeparateMeshFaces(baseMesh);
			PlanetFace[] faces = GetFaces(targets);
			SubdivideFaces(faces);
			FormSphere(faces);
			PlanetData data = CreatePlanetData();
			Init(faces, data);
			Planet planet = CreatePlanet(data);
			planet.Init(faces);
			InitContinentLayer(planet);
			planet.UpdateMesh();
		}

		private Planet CreatePlanet(PlanetData data)
		{
            Planet result = _planetFactory.Create(data);
            result.transform.SetParent(null);
            return result;
		}

		private PlanetData CreatePlanetData()
		{
            PlanetDimensions dimensions = new PlanetDimensions(_bedrockRadius, _atmosphereRadius);
            TemperatureSpectrum temperature = new TemperatureSpectrum(_teperatureMin, _teperatureMax);
            PlanetAxisData axis = new PlanetAxisData(_axisAngle, _secondsPerRevolution);
            return new PlanetData(dimensions, temperature, axis, _seed);
		}

		private void GenerateMesh(Mesh mesh)
		{
            _icosahedronGenerator.GenerateMeshFor(mesh, _bedrockRadius);
        }

        private MeshFaceSeparatorTarget[] SeparateMeshFaces(Mesh mesh)
        {
            return _faceSeparator.Separate(mesh);
		}

        private PlanetFace[] GetFaces(MeshFaceSeparatorTarget[] targets)
		{
            PlanetFace[] result = new PlanetFace[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                result[i] = targets[i].Base.GetComponentInChildren<PlanetFace>();
            return result;
        }

        private void SubdivideFaces(PlanetFace[] faces)
        {
            foreach (PlanetFace face in faces)
                _subdivider.Subdivide(face.MeshFilter.sharedMesh, _subdivisions);
        }

        private void FormSphere(PlanetFace[] faces)
        {
            foreach (PlanetFace face in faces)
                _meshFormer.Form(face.MeshFilter.sharedMesh, _bedrockRadius);
        }

        private void Init(PlanetFace[] faces, PlanetData planetData)
        {
            foreach(PlanetFace face in faces)
				Init(face, planetData);
		}

		private void Init(PlanetFace face, PlanetData planetData)
		{
			EvaluationPointData[] evaluationPointsData = CreateEvaluationPointsData(face);
			PlanetFaceData data = new PlanetFaceData(evaluationPointsData);
			face.Init(data, planetData);
		}

		private EvaluationPointData[] CreateEvaluationPointsData(PlanetFace face)
		{
            int evaluationPointCount = face.SharedMesh.vertexCount;
            return CreateEvaluationPointsData(evaluationPointCount);
        }

        private EvaluationPointData[] CreateEvaluationPointsData(int amount)
		{
            EvaluationPointData[] result = new EvaluationPointData[amount];
            for (int i = 0; i < amount; i++)
                result[i] = CreateEvaluationPointData();
            return result;
        }


        private EvaluationPointData CreateEvaluationPointData()
		{
			List<PlanetLayerData> layerData = new List<PlanetLayerData>();
			layerData.Add(new PlanetLayerData(0, 1));
			return new EvaluationPointData(layerData, new float[1]);
		}


		private void InitContinentLayer(Planet planet)
        {
            for(int i = 0; i < planet.Faces.Count; i++)
				InitContinentPlate(planet.Faces[i]);
		}

		private void InitContinentPlate(PlanetFace face)
		{
			NativeArray<float> result = Evaluate(_continentsNoise, face.SharedMesh.vertices);
			for (int i = 0; i < result.Length; i++)
				InitContinentPlate(face.Data.EvaluationPoints[i], result[i]);
			result.Dispose();
		}

		private void InitContinentPlate(EvaluationPointData data, float value)
		{
			float continentalPlateHeight = value * _continentalPlateMax;
			data.Layers[0].Height = data.Layers[0].Height - continentalPlateHeight;
			data.Layers.Insert(0, new PlanetLayerData(1, continentalPlateHeight));
		}

		private NativeArray<float> Evaluate(Noise3D noise, Vector3[] points)
		{
            for (int i = 0; i < points.Length; i++)
                points[i] = points[i].normalized * _atmosphereRadius;
            NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(points, Allocator.TempJob);
            NativeArray<float> result = noise.Evaluate3D(verticesNative);
            verticesNative.Dispose();
            return result;
        }
    }
}