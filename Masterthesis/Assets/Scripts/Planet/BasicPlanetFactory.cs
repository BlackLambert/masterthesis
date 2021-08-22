using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class BasicPlanetFactory
    {
        private IcosahedronGenerator _icosahedronGenerator;
        private MeshFaceSeparator _faceSeparator;
        private SphereMeshFormer _meshFormer;
        private MeshSubdivider _subdivider;
        private Planet.Factory _planetFactory;

        public BasicPlanetFactory(IcosahedronGenerator meshGenerator,
            MeshFaceSeparator faceSeparator,
            SphereMeshFormer meshFormer,
            MeshSubdivider subdivider,
            Planet.Factory planetFactory)
		{
            _faceSeparator = faceSeparator;
            _meshFormer = meshFormer;
            _subdivider = subdivider;
            _planetFactory = planetFactory;
            _icosahedronGenerator = meshGenerator;
        }

        public Planet Create(Parameter parameter)
		{
			Mesh baseMesh = new Mesh();
            _icosahedronGenerator.GenerateMeshFor(baseMesh, parameter.Dimensions.KernelRadius);
            MeshFaceSeparatorTarget[] targets = _faceSeparator.Separate(baseMesh);
            PlanetFace[] faces = GetFaces(targets);
			SubdivideFaces(faces, parameter.Subdivisions);
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
            face.Init(data, planetData);
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
            List<PlanetLayerData> layerData = new List<PlanetLayerData>(5) { };
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
            return new PlanetData(parameter.Dimensions, parameter.TemperatureSpectrum, parameter.AxisData, parameter.Seed);
        }

        public class Parameter
        {

            public Parameter(PlanetDimensions dimensions,
                TemperatureSpectrum temperatureSpectrum,
                PlanetAxisData axisData,
                int subdivisions, 
                Seed seed)
			{
				Dimensions = dimensions;
				TemperatureSpectrum = temperatureSpectrum;
				AxisData = axisData;
				Subdivisions = subdivisions;
				Seed = seed;
			}

			public PlanetDimensions Dimensions { get; }
			public TemperatureSpectrum TemperatureSpectrum { get; }
			public PlanetAxisData AxisData { get; }
			public int Subdivisions { get; }
			public Seed Seed { get; }
		}
    }
}