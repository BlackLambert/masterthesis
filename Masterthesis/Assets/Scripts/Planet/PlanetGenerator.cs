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
		private const float _oceanLevel = 0.5f;

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
        [Range(0f, 1f)]
        [SerializeField]
        private float _warpFactor = 0.05f;
        [Range(0f, 1f)]
        [SerializeField]
        private float _segmentsBlendFactor = 0.05f;
        [Range(1f, 5f)]
        [SerializeField]
        private float _sampleEliminationFactor = 1;
        [SerializeField]
        private int _oceans = 10;
        [SerializeField]
        private int _continents = 10;
        [SerializeField]
        private int _plates = 10;

        [Header("Biome")]
        [SerializeField]
        private BiomeSettings[] _biomes;

        [Header("Noise")]
        [SerializeField]
        private NoiseSettings _continentalPlatesNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentsNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentalPlatesWarpingNoiseSettings;
        [SerializeField]
        private NoiseSettings _mountainsNoiseSettings;
        [SerializeField]
        private NoiseSettings _canyonsNoiseSettings;

        [SerializeField]
        private MeshFilter _delaunay;
        [SerializeField]
        private MeshFilter _voronoi;


        private MeshGenerator _icosahedronGenerator;
        private MeshFaceSeparator _faceSeparator;
        private SphereMeshFormer _meshFormer;
        private Seed _seed;
        private MeshSubdivider _subdivider;
        private Planet.Factory _planetFactory;
        private Vector3BinaryKDTreeFactory _treeFactory;
        private ContinentalPlatesFactory _continentalPlatesFactory;
        private ShapingFactory _shapingFactory;


        private Noise3D _continentsNoise;
        private Noise3D _continentalPlatesWarpingNoise;
        private Noise3D _mountainsNoise;
        private Noise3D _canyonsNoise;

        [Inject]
        public void Construct(MeshGeneratorFactory meshGeneratorFactory,
            MeshFaceSeparator faceSeparator,
            SphereMeshFormer meshFormer,
            NoiseFactory noiseFactory,
            Seed seed,
            MeshSubdivider subdivider,
            Planet.Factory planetFactory,
            ContinentalPlatesFactory continentalPlatesFactory,
            ShapingFactory shapingFactory,
            Vector3BinaryKDTreeFactory treeFactory)
		{
            _faceSeparator = faceSeparator;
            _meshFormer = meshFormer;
            _seed = seed;
            _subdivider = subdivider;
            _planetFactory = planetFactory;
            _continentalPlatesFactory = continentalPlatesFactory;
            _shapingFactory = shapingFactory;
            _treeFactory = treeFactory;

            _continentsNoise = noiseFactory.Create(_continentsNoiseSettings, _seed);
            _continentalPlatesWarpingNoise = noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, _seed);
            _mountainsNoise = noiseFactory.Create(_mountainsNoiseSettings, _seed);
            _canyonsNoise = noiseFactory.Create(_canyonsNoiseSettings, _seed);
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
			ContinentalPlatesFactory.Parameters continentalPlatesFactoryParameter = CreatePlatesFactoryParameters(planet);
			ContinentalPlates plates = _continentalPlatesFactory.Create(continentalPlatesFactoryParameter);
            ShapingFactory.Parameter shapingFactoryParameter = new ShapingFactory.Parameter(plates, data, _mountainsNoise, _canyonsNoise);
            ShapingLayer[] shapingLayers = _shapingFactory.Create(shapingFactoryParameter);
            
			InitSegmentsDelaunayMesh(plates);
			InitSegmentsVoronoiView(plates);
			Biome[] biomes = CreateBiomes();
			AddRockLayer(planet, plates, biomes, shapingLayers);
			//InitContinentLayer(planet);
			planet.UpdateMesh();
		}

		private ContinentalPlatesFactory.Parameters CreatePlatesFactoryParameters(Planet planet)
		{
			return new ContinentalPlatesFactory.Parameters(
				planet,
				_seed,
				_plateSegments,
				_plates,
				_biomes,
				_sampleEliminationFactor,
				_continents,
				_oceans);
		}

		private Biome[] CreateBiomes()
		{
            Biome[] result = new Biome[_biomes.Length];
			for (int i = 0; i < _biomes.Length; i++)
			{
                BiomeSettings settings = _biomes[i];
                result[i] = new Biome(settings.BaseColor, settings.RegionType);
			}
            return result;
		}

		private void InitSegmentsVoronoiView(ContinentalPlates plates)
		{
            _voronoi.sharedMesh = new Mesh();
            _voronoi.sharedMesh.vertices = plates.SegmentsVoronoi.Vertices;
            Draw(plates.SegmentsVoronoi);
        }

        private void Draw(VoronoiDiagram diagram)
        {
            foreach (VoronoiRegion region in diagram.Regions)
            {
                for (int i = 0; i < region.VertexIndices.Count; i++)
                {
                    Vector3 c0 = diagram.Vertices[region.VertexIndices[i]];
                    Vector3 c1 = diagram.Vertices[region.VertexIndices[(i + 1) % region.VertexIndices.Count]];
                    Debug.DrawLine(c0, c1, Color.green, 60);
                }
            }
        }

        private void InitSegmentsDelaunayMesh(ContinentalPlates plates)
        {
            _delaunay.sharedMesh = new Mesh();
            _delaunay.sharedMesh.vertices = plates.SegmentSites;
            _delaunay.sharedMesh.triangles = CreateTriangles(plates.SegmentsDelaunayTriangles);
            _delaunay.sharedMesh.RecalculateNormals();
        }

        private int[] CreateTriangles(Triangle[] triangles)
        {
            int[] result = new int[triangles.Length * 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                result[i * 3] = triangles[i].VertexIndices[0];
                result[i * 3 + 1] = triangles[i].VertexIndices[1];
                result[i * 3 + 2] = triangles[i].VertexIndices[2];
            }

            return result;
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


		private void AddRockLayer(Planet planet, ContinentalPlates plates, Biome[] biomes, ShapingLayer[] shapingLayers)
		{
            KDTree<Vector3> segmentsKDTree = _treeFactory.Create(plates.SegmentSites);
            KDTree<Vector3> regionsKDTree = _treeFactory.Create(plates.RegionSites);
            for (int i = 0; i < planet.Faces.Count; i++)
            {
                PlanetFace face = planet.Faces[i];
                Vector3[] vertices = face.Vertices;
                float[] warpValues = WarpContientenalPlateEvaluationPoint(vertices);
                vertices = WarpSpherePoints(vertices, warpValues);
                PlanetShaper shaper = new PlanetShaper(shapingLayers, _oceanLevel);
                float[] shapeValues = shaper.Shape(vertices);
                Color[] vertexColors = new Color[face.MeshFilter.sharedMesh.vertexCount];

                for (int j = 0; j < vertices.Length; j++)
				{
					Vector3 vertex = vertices[j];
					int segmentIndex = segmentsKDTree.GetNearestTo(vertex);
                    ContinentalPlateSegment segment = plates.Segments[segmentIndex];
                    Biome biome = biomes[segment.BiomeID];
					float delta = CalculateDelta(planet, shapeValues[j], biome);
					Color vertexColor = GetVertexColor(planet, vertex, plates, biomes, segmentIndex);
					InitContinentPlate(face.Data.EvaluationPoints[j], delta);
					vertexColors[j] = vertexColor;
				}

				face.MeshFilter.sharedMesh.colors = vertexColors;
            }
        }

		private Color GetVertexColor(Planet planet, Vector3 vertex, ContinentalPlates plates, Biome[] biomes, int segmentIndex)
        {
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
            Biome biome = biomes[segment.BiomeID];
            Color color = biome.BaseColor;
            float blendDistance = _segmentsBlendFactor * planet.AtmosphereRadius;
            Vector3 pointOnBorder = plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, segmentIndex);
            float distanceToInnerBorder = GetDistanceOnPlanet(planet, vertex, pointOnBorder);
            if (distanceToInnerBorder >= blendDistance)
                return color;
            for (int i = 0; i < segment.Neighbors.Length; i++)
			{
                int neighborSegmentIndex = segment.Neighbors[i];
                ContinentalPlateSegment neighborSegment = plates.Segments[neighborSegmentIndex];
                Biome neighborBiome = biomes[neighborSegment.BiomeID];
                if (neighborBiome.RegionType != biome.RegionType)
                    continue;
                Vector3 pointNeighborOnBorder = plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, neighborSegmentIndex);
                float distanceToSegment = GetDistanceOnPlanet(planet, vertex, pointNeighborOnBorder);
                if (distanceToSegment > blendDistance)
                    continue;
                float portion = 0.5f - (distanceToSegment / blendDistance) / 2;
                color = Lerp(color, neighborBiome.BaseColor, portion);
            }
            return color;
        }

        private float GetDistanceOnPlanet(Planet planet, Vector3 vertex, Vector3 point)
		{
            point = point.normalized * planet.AtmosphereRadius;
            return vertex.FastSubstract(point).magnitude;
        }

		private Color Lerp(Color color, Color otherColor, float portion)
		{
            return Color.Lerp(color, otherColor, portion);
		}

		private float CalculateDelta(Planet planet, float shapeValue, Biome biome)
		{
            bool isOceanic = biome.RegionType == ContinentalRegion.Type.Oceanic;
            if (isOceanic)
				return CreateOceanDelta(planet);
			else
				return CreateLandDelta(planet, shapeValue);
		}

		private static float CreateOceanDelta(Planet planet)
		{
			return _oceanLevel * planet.Data.Dimensions.VariableAreaThickness;
		}

		private static float CreateLandDelta(Planet planet, float shapeValue)
		{
			return shapeValue * planet.Data.Dimensions.VariableAreaThickness;
		}

		private Vector3[] WarpSpherePoints(Vector3[] vertices, float[] warpValues)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
                Vector3 vertex = vertices[i].normalized * _atmosphereRadius;
                vertex = WarpSpherePoint(vertex, warpValues[i]);
                vertices[i] = vertex.normalized * _atmosphereRadius;
            }
            return vertices;
		}

        private void InitContinentPlate(EvaluationPointData data, float value)
        {
            float continentalPlateHeight = value * _continentalPlateMax;
            data.Layers[0].Height = data.Layers[0].Height - continentalPlateHeight;
            data.Layers.Insert(0, new PlanetLayerData(1, continentalPlateHeight));
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
            List<PlanetLayerData> layerData = new List<PlanetLayerData>(1) { new PlanetLayerData(0, 1) };
			return new EvaluationPointData(layerData, new float[1]);
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
            Vector3 crossVector = vertex.normalized == Vector3.forward ? Vector3.right : Vector3.forward;
            Vector3 tangential = Vector3.Cross(vertex, crossVector);
            Vector3 deltaVector = tangential.normalized * warpValue * _atmosphereRadius * _warpFactor;
            float sinWarpValueHalf = Mathf.Sin(Mathf.PI * warpValue);
            float cosWarpValueHalf = Mathf.Cos(Mathf.PI * warpValue);
            deltaVector = new Quaternion(sinWarpValueHalf * vertex.x,
                sinWarpValueHalf * vertex.y,
                sinWarpValueHalf * vertex.z,
                cosWarpValueHalf).normalized * deltaVector;
            return vertex + deltaVector;
        }

        
    }
}