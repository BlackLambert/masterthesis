using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Continental Plates")]
        [SerializeField]
        private int _plateSegments = 150;
        [SerializeField]
        private float _warpFactor = 0.05f;
        [SerializeField]
        private int _plates = 10;

        [Header("Noise")]
        [SerializeField]
        private NoiseSettings _continentalPlatesNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentsNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentalPlatesWarpingNoiseSettings;

        [SerializeField]
        private MeshFilter _delaunay;


        private MeshGenerator _icosahedronGenerator;
        private MeshFaceSeparator _faceSeparator;
        private SphereMeshFormer _meshFormer;
        private Seed _seed;
        private MeshSubdivider _subdivider;
        private Planet.Factory _planetFactory;
		private RandomPointsOnSphereGenerator _randomPointsGenerator;
		private SampleElimination3D _sampleElimination;
        private QuickSelector<Vector3> _quickSelector;

		private Noise3D _continentsNoise;
        private Noise3D _continentalPlatesWarpingNoise;

        [Inject]
        public void Construct(MeshGeneratorFactory meshGeneratorFactory,
            MeshFaceSeparator faceSeparator,
            SphereMeshFormer meshFormer,
            NoiseFactory noiseFactory,
            Seed seed,
            MeshSubdivider subdivider,
            Planet.Factory planetFactory,
            RandomPointsOnSphereGenerator randomPointsGenerator,
            SampleElimination3D sampleElimination,
            QuickSelector<Vector3> quickSelector)
		{
            _faceSeparator = faceSeparator;
            _meshFormer = meshFormer;
            _seed = seed;
            _subdivider = subdivider;
            _planetFactory = planetFactory;
            _randomPointsGenerator = randomPointsGenerator;
            _sampleElimination = sampleElimination;
            _quickSelector = quickSelector;

            _continentsNoise = noiseFactory.Create(_continentsNoiseSettings, _seed);
            _continentalPlatesWarpingNoise = noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, _seed);
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
            ContinentalPlate[] plates = InitContinentalPlates(planet);
            //InitContinentLayer(planet);
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
			List<PlanetLayerData> layerData = new List<PlanetLayerData>();
			layerData.Add(new PlanetLayerData(0, 1));
			return new EvaluationPointData(layerData, new float[1]);
		}

        private ContinentalPlate[] InitContinentalPlates(Planet planet)
		{
			Vector3[] points = _randomPointsGenerator.Generate(_plateSegments * 3, _atmosphereRadius);
			float sphereSurface = GetSphereSurface();
			Vector3[] plateSegmentSites = _sampleElimination.Eliminate(points, _plateSegments, sphereSurface).ToArray();
            DelaunayTriangle[] triangles = new SphericalDelaunayTriangulation().Create(plateSegmentSites);
            _delaunay.sharedMesh = new Mesh();
            _delaunay.sharedMesh.vertices = plateSegmentSites;
            _delaunay.sharedMesh.triangles = CreateTriangles(triangles);
            _delaunay.sharedMesh.RecalculateNormals();
            ContinentalPlateSegment[] segments = CreateContinentalPlateSegments(plateSegmentSites);
			KDTree<Vector3> sitesKDTree = new Vector3BinaryKDTree(plateSegmentSites, _quickSelector);

			for (int i = 0; i < planet.Faces.Count; i++)
			{
				PlanetFace face = planet.Faces[i];
				Vector3[] vertices = face.Vertices;
				float[] warpValues = WarpContientenalPlateEvaluationPoint(vertices);

				for (int j = 0; j < vertices.Length; j++)
				{
					Vector3 vertex = vertices[j].normalized * _atmosphereRadius;
					vertex = WarpSpherePoint(vertex, warpValues[j]);
					vertex = vertex.normalized * _atmosphereRadius;
					int index = sitesKDTree.GetNearestTo(vertex);
					float delta = segments[index].Oceanic ? 0.1f : 0.4f;
					InitContinentPlate(face.Data.EvaluationPoints[j], delta);
				}
			}

			ContinentalPlate[] result = CreatePlates(segments);
			return result;
		}

		private int[] CreateTriangles(DelaunayTriangle[] triangles)
		{
            int[] result = new int[triangles.Length * 3];
			for (int i = 0; i < triangles.Length; i++)
			{
                result[i * 3] = triangles[i].VertexIndices.x;
                result[i * 3 + 1] = triangles[i].VertexIndices.y;
                result[i * 3 + 2] = triangles[i].VertexIndices.z;
            }

            return result;
		}

		private float GetSphereSurface()
		{
			return 4 * Mathf.PI * _atmosphereRadius;
		}

		private ContinentalPlate[] CreatePlates(ContinentalPlateSegment[] segments)
		{
            float sphereSurface = GetSphereSurface();
            Vector3[] continentalPoints = _randomPointsGenerator.Generate(_plates * 3, _atmosphereRadius);
			Vector3[] plateSites = _sampleElimination.Eliminate(continentalPoints, _plates, sphereSurface).ToArray();
			KDTree<Vector3> platesKDTree = new Vector3BinaryKDTree(plateSites, _quickSelector);
			List<ContinentalPlateSegment>[] plateToSegments = new List<ContinentalPlateSegment>[_plates];
			for (int i = 0; i < plateToSegments.Length; i++)
				plateToSegments[i] = new List<ContinentalPlateSegment>();

			for (int i = 0; i < segments.Length; i++)
			{
				int index = platesKDTree.GetNearestTo(segments[i].Site);
				plateToSegments[index].Add(segments[i]);
			}

			ContinentalPlate[] result = new ContinentalPlate[_plates];
			Seed seed = new Seed(_seed.Random.Next());
			for (int i = 0; i < plateToSegments.Length; i++)
				result[i] = new ContinentalPlate(plateToSegments[i].ToArray(), (float)seed.Random.NextDouble() * 360);

			return result;
		}

		private float[] WarpContientenalPlateEvaluationPoint(Vector3[] vertices)
		{
            NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(vertices, Allocator.TempJob);
            NativeArray<float> resultNative = _continentalPlatesWarpingNoise.Evaluate3D(verticesNative);
            float[] result = resultNative.ToArray();
            verticesNative.Dispose();
            resultNative.Dispose();
            return result;
        }

        private Vector3 WarpSpherePoint(Vector3 vertex, float warpValue)
        {
            Vector3 tangential = Vector3.Cross(vertex, Vector3.forward);
            Vector3 deltaVector = tangential.normalized * warpValue * _atmosphereRadius * _warpFactor;
            float sinWarpValueHalf = Mathf.Sin(Mathf.PI * warpValue);
            float cosWarpValueHalf = Mathf.Cos(Mathf.PI * warpValue);
            deltaVector = new Quaternion(sinWarpValueHalf * vertex.x,
                sinWarpValueHalf * vertex.y,
                sinWarpValueHalf * vertex.z,
                cosWarpValueHalf).normalized * deltaVector;
            return vertex + deltaVector;
        }

        private ContinentalPlateSegment[] CreateContinentalPlateSegments(Vector3[] plateSites)
		{
            ContinentalPlateSegment[] result = new ContinentalPlateSegment[plateSites.Length];

			for (int i = 0; i < plateSites.Length; i++)
			{
                bool oceanic = UnityEngine.Random.Range(0, 2) == 0;
                result[i] = new ContinentalPlateSegment(plateSites[i], oceanic);
            }

            return result;
		}

		private void InitContinentLayer(Planet planet)
        {
            for(int i = 0; i < planet.Faces.Count; i++)
				InitContinentPlate(planet.Faces[i]);
		}

		private void InitContinentPlate(PlanetFace face)
		{
			NativeArray<float> result = Evaluate(_continentsNoise, face.Vertices);
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

        private Vector3 WarpContientenalPlateEvaluationPoint(Vector3 vertex)
        {
            return new Vector3(_continentalPlatesWarpingNoise.Evaluate3D(vertex + Vector3.left),
                _continentalPlatesWarpingNoise.Evaluate3D(vertex + Vector3.up),
                _continentalPlatesWarpingNoise.Evaluate3D(vertex + Vector3.down));
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