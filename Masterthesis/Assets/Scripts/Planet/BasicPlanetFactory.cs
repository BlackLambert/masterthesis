using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class BasicPlanetFactory
    {
		private const int _maxSubdivisions = 350;
		private IcosahedronGenerator _icosahedronGenerator;
        private MeshFaceSeparator _faceSeparator;
        private SphereMeshFormer _meshFormer;
        private MeshSubdivider _subdivider;
        private Planet.Factory _planetFactory;
        private Vector3BinaryKDTreeFactory _treeFactory;

        public BasicPlanetFactory(IcosahedronGenerator meshGenerator,
            MeshFaceSeparator faceSeparator,
            SphereMeshFormer meshFormer,
            MeshSubdivider subdivider,
            Planet.Factory planetFactory,
            Vector3BinaryKDTreeFactory treeFactory
            )
		{
            _faceSeparator = faceSeparator;
            _meshFormer = meshFormer;
            _subdivider = subdivider;
            _planetFactory = planetFactory;
            _icosahedronGenerator = meshGenerator;
            _treeFactory = treeFactory;
        }

        public Planet Create(Parameter parameter)
		{
			Mesh baseMesh = new Mesh();
            _icosahedronGenerator.GenerateMeshFor(baseMesh, parameter.Dimensions.KernelRadius);
            MeshFaceSeparatorTarget[] targets = _faceSeparator.Separate(baseMesh);
            PlanetFace[] faces = GetFaces(targets);
			SubdivideFaces(faces, (int) (parameter.Subdivisions * _maxSubdivisions));
			FormSphere(faces, parameter.Dimensions.KernelRadius);
			PlanetData data = CreatePlanetData(parameter);
			Init(faces, data);
			Planet planet = CreatePlanet(data);
			planet.Init(faces);
			return planet;
		}

        private PlanetFace[] GetFaces(MeshFaceSeparatorTarget[] targets)
        {
            PlanetFace[] result = new PlanetFace[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                result[i] = targets[i].Base.GetComponentInChildren<PlanetFace>();
            return result;
        }

        private void SubdivideFaces(PlanetFace[] faces, int subdivisions)
        {
            foreach (PlanetFace face in faces)
                _subdivider.Subdivide(face.MeshFilter.sharedMesh, subdivisions);
        }

        private void FormSphere(PlanetFace[] faces, float kernelRadius)
        {
            foreach (PlanetFace face in faces)
                _meshFormer.Form(face.MeshFilter.sharedMesh, kernelRadius);
        }

        private void Init(PlanetFace[] faces, PlanetData planetData)
        {
            foreach (PlanetFace face in faces)
                Init(face, planetData);
        }

        private void Init(PlanetFace face, PlanetData planetData)
        {
            EvaluationPointData[] evaluationPointsData = CreateEvaluationPointsData(face);
            PlanetFaceData data = new PlanetFaceData(evaluationPointsData);
            KDTree<Vector3> tree = _treeFactory.Create(face.Vertices);
            face.Init(data, planetData, tree);
        }

        private EvaluationPointData[] CreateEvaluationPointsData(PlanetFace face)
        {
            int evaluationPointCount = face.VertexCount;
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
            List<PlanetMaterialLayerData> layerData = new List<PlanetMaterialLayerData>(5) { };
            return new EvaluationPointData(layerData);
        }

        private Planet CreatePlanet(PlanetData data)
        {
            Planet result = _planetFactory.Create(data);
            result.transform.SetParent(null);
            return result;
        }

        private PlanetData CreatePlanetData(Parameter parameter)
        {
            PlanetData result = new PlanetData(
                parameter.Dimensions,
                parameter.TemperatureSpectrum,
                parameter.AxisData, 
                parameter.Seed, 
                CreateIDToMaterial(parameter.Materials), 
                parameter.GradientNoise);
            result.SetLayerBitMask(CreateLayerBitMask());
            return result;
        }

		private uint CreateLayerBitMask()
		{
            uint result = 0;
            foreach (PlanetMaterialType type in Enum.GetValues(typeof(PlanetMaterialType)))
                result |= (uint)type;
            return result;
        }

		private Dictionary<short, PlanetLayerMaterialSettings> CreateIDToMaterial(PlanetLayerMaterialSettings[] materials)
		{
            Dictionary<short, PlanetLayerMaterialSettings> result = new Dictionary<short, PlanetLayerMaterialSettings>();
            foreach (PlanetLayerMaterialSettings material in materials)
                result.Add(material.ID, material);
            return result;
        }

		public class Parameter
        {

            public Parameter(PlanetDimensions dimensions,
                TemperatureSpectrum temperatureSpectrum,
                PlanetAxisData axisData,
                float subdivisions, 
                Seed seed,
                PlanetLayerMaterialSettings[] materials,
                Noise3D gradientNoise)
			{
				Dimensions = dimensions;
				TemperatureSpectrum = temperatureSpectrum;
				AxisData = axisData;
				Subdivisions = subdivisions;
				Seed = seed;
				Materials = materials;
				GradientNoise = gradientNoise;
			}

			public PlanetDimensions Dimensions { get; }
			public TemperatureSpectrum TemperatureSpectrum { get; }
			public PlanetAxisData AxisData { get; }
			public float Subdivisions { get; }
			public Seed Seed { get; }
			public PlanetLayerMaterialSettings[] Materials { get; }
			public Noise3D GradientNoise { get; }
		}
    }
}