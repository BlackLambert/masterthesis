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
        private MeshFilter _delaunay;
        [SerializeField]
        private MeshFilter _voronoi;


        private MeshGenerator _icosahedronGenerator;
        private MeshFaceSeparator _faceSeparator;
        private SphereMeshFormer _meshFormer;
        private Seed _seed;
        private MeshSubdivider _subdivider;
        private Planet.Factory _planetFactory;
        private QuickSelector<Vector3> _quickSelector;
        private ContinentalPlatesFactory _continentalPlatesFactory;


        private Noise3D _continentsNoise;
        private Noise3D _continentalPlatesWarpingNoise;
        private Noise3D _mountainsNoise;

        [Inject]
        public void Construct(MeshGeneratorFactory meshGeneratorFactory,
            MeshFaceSeparator faceSeparator,
            SphereMeshFormer meshFormer,
            NoiseFactory noiseFactory,
            Seed seed,
            MeshSubdivider subdivider,
            Planet.Factory planetFactory,
            QuickSelector<Vector3> quickSelector,
            ContinentalPlatesFactory continentalPlatesFactory)
		{
            _faceSeparator = faceSeparator;
            _meshFormer = meshFormer;
            _seed = seed;
            _subdivider = subdivider;
            _planetFactory = planetFactory;
            _quickSelector = quickSelector;
            _continentalPlatesFactory = continentalPlatesFactory;

            _continentsNoise = noiseFactory.Create(_continentsNoiseSettings, _seed);
            _continentalPlatesWarpingNoise = noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, _seed);
            _mountainsNoise = noiseFactory.Create(_mountainsNoiseSettings, _seed);
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
			ContinentalPlatesFactory.Parameters parameter = CreatePlatesFactoryParameters(planet);
			ContinentalPlates plates = _continentalPlatesFactory.Create(parameter);
            ShapingLayer[] shapingLayers = CreateShapingLayers(plates);
            
			InitSegmentsDelaunayMesh(plates);
			InitSegmentsVoronoiView(plates);
			Biome[] biomes = CreateBiomes();
			AddRockLayer(planet, plates, biomes, shapingLayers);
			//InitContinentLayer(planet);
			planet.UpdateMesh();
		}

		private ShapingLayer[] CreateShapingLayers(ContinentalPlates plates)
		{
            ShapingPrimitive[] mountainPrimitives = CreateMountainShaping(plates);
            ShapingLayer mountainsLayer = CreateShapingLayer(mountainPrimitives);
            return new ShapingLayer[] { mountainsLayer };
        }

		private ShapingLayer CreateShapingLayer(ShapingPrimitive[] primitives)
		{
            Vector3[] nodes = primitives.Select(p => p.Position).ToArray();
            KDTree<Vector3> tree = new Vector3BinaryKDTree(nodes, _quickSelector);
            return new ShapingLayer(primitives, tree, _mountainsNoise);
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
                result[i] = new Biome(_biomes[i].BaseColor);
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

        private ShapingPrimitive[] CreateMountainShaping(ContinentalPlates plates)
        {
            List<ShapingPrimitive> primitives = new List<ShapingPrimitive>();
            HashSet<Vector2Int> handledBorders = new HashSet<Vector2Int>();
            float mountainThickness = _atmosphereRadius / 18;
            float mountainLengthAddition = _atmosphereRadius / 25;
            float bledDistance = _atmosphereRadius / 10;
            for (int i = 0; i < plates.PlateNeighbors.Length; i++)
			{
                Vector2Int neighbors = plates.PlateNeighbors[i];
                Vector2Int[] borders = plates.PlateBorders[neighbors];
				for (int j = 0; j < borders.Length; j++)
				{
                    Vector2Int border = borders[j];
                    if (handledBorders.Contains(border))
                        continue;
                    Vector3 corner0 = plates.SegmentCorners[border[0]];
                    Vector3 corner1 = plates.SegmentCorners[border[1]];
                    Vector3 distanceVector = corner1 - corner0;
                    float distance = distanceVector.magnitude;
                    if (distance == 0)
                        continue;
                    Vector3 pos = (corner0 + distanceVector / 2).normalized * corner0.magnitude;
                    float mountainLength = distance + mountainLengthAddition;
                    float max = Mathf.Max(mountainLength, mountainThickness);
                    float min = Mathf.Min(mountainLength, mountainThickness);
                    //float bledDistance = 0;
                    primitives.Add(new ElipsoidShapePrimitive(pos, distanceVector, min, max, bledDistance));
                    handledBorders.Add(border);
                }
            }
            return primitives.ToArray();
        }

        private void AddRockLayer(Planet planet, ContinentalPlates plates, Biome[] biomes, ShapingLayer[] shapingLayers)
		{
            KDTree<Vector3> segmentsKDTree = new Vector3BinaryKDTree(plates.SegmentSites, _quickSelector);
            KDTree<Vector3> regionsKDTree = new Vector3BinaryKDTree(plates.RegionSites, _quickSelector);
            for (int i = 0; i < planet.Faces.Count; i++)
            {
                PlanetFace face = planet.Faces[i];
                Vector3[] vertices = face.Vertices;
                float[] warpValues = WarpContientenalPlateEvaluationPoint(vertices);
                vertices = WarpSpherePoints(vertices, warpValues);
                float[] shapeValues = ShapeSurface(shapingLayers, face.Vertices);
                Color[] vertexColors = new Color[face.MeshFilter.sharedMesh.vertexCount];

                for (int j = 0; j < vertices.Length; j++)
                {
                    Vector3 vertex = vertices[j];
                    int segmentIndex = segmentsKDTree.GetNearestTo(vertex);
                    int regionIndex = regionsKDTree.GetNearestTo(vertex);
                    ContinentalPlateSegment segment = plates.Segments[segmentIndex];
                    ContinentalRegion region = plates.Regions[regionIndex];
                    bool isOceanic = region.RegionType == ContinentalRegion.Type.Oceanic;
                    float delta = isOceanic ? 0f : 0.1f;
                    delta = (delta + 0.9f * shapeValues[j]) * planet.Data.Dimensions.VariableAreaThickness;
                    Color vertexColor = isOceanic ? Color.blue : biomes[segment.BiomeID].BaseColor;
                    InitContinentPlate(face.Data.EvaluationPoints[j], delta);
                    vertexColors[j] = vertexColor;
                }

                face.MeshFilter.sharedMesh.colors = vertexColors;
            }
        }

		private float[] ShapeSurface(ShapingLayer[] layers, Vector3[] vertices)
		{
            float[] result = new float[vertices.Length];
            int[] blendValues = new int[vertices.Length];
            for (int i = 0; i < layers.Length; i++)
			{
                ShapingLayer layer = layers[i];
                float [] evalValues = layer.Evaluate(vertices);
				for (int j = 0; j < evalValues.Length; j++)
				{
                    if (evalValues[j] == 0)
                        continue;
                    result[j] += evalValues[j];
                    blendValues[j]++;
                }
            }

			for (int i = 0; i < result.Length; i++)
			{
                if (blendValues[i] == 0)
                    continue;
                result[i] /= blendValues[i];
            }

            return result;
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

        
    }
}