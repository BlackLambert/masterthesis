using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class HullInfo : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _panel;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Transform _layerImageHook;
        [SerializeField]
        private Transform _layerTextHook;
        [SerializeField]
        private HullInfoLayerImage _imagePrefab;
        [SerializeField]
        private HullInfoLayerText _textPrefab;

        private List<HullInfoLayerImage> _images = new List<HullInfoLayerImage>();
        private List<HullInfoLayerText> _texts = new List<HullInfoLayerText>();
		private PlanetLayerMaterialSerializer _serializer;

		[Inject]
        public void Construct(PlanetLayerMaterialSerializer serializer)
		{
            _serializer = serializer;
        }

        protected virtual void Start()
		{
            Hide();
        }

        public void Show(Vector2 position, EvaluationPointData pointData, PlanetData planetData)
		{
            Clear();
            _panel.position = position;
            Init(pointData, planetData);
            _canvasGroup.alpha = 1;
        }

		public void Hide()
		{
            _canvasGroup.alpha = 0;
        }

        private void Init(EvaluationPointData pointData, PlanetData planetData)
        {
            List<PlanetMaterialLayerData> layers = pointData.Layers;
            int count = layers.Count;
            for (int i = count - 1; i >= 0; i--)
                AddLayer(layers[i], planetData, i);
                
        }

		private void AddLayer(PlanetMaterialLayerData layer, PlanetData planetData, int layerIndex)
		{
            List<PlanetLayerMaterial> materials = CreateMaterials(layer.Materials);
            Color color = GetColor(materials, planetData);
            HullInfoLayerImage image = Instantiate(_imagePrefab);
            image.Init(color, layer.Height, _layerImageHook);
            HullInfoLayerText text = Instantiate(_textPrefab);
            text.Init(_layerTextHook, layerIndex, layer.Height, layer.State, planetData, materials);
            _images.Add(image);
            _texts.Add(text);
        }

		private Color GetColor(List<PlanetLayerMaterial> materials, PlanetData planetData)
		{
            Color result = Color.black;
            foreach (PlanetLayerMaterial material in materials)
                result += GetColor(material, planetData);
            return result / materials.Sum(m => m.Portion);
        }

		private Color GetColor(PlanetLayerMaterial material, PlanetData planetData)
		{
            PlanetLayerMaterialSettings m = planetData.Materials[material.MaterialID];
            return m.ImageColor * material.Portion;
        }

		private List<PlanetLayerMaterial> CreateMaterials(List<short> materials)
		{
            List<PlanetLayerMaterial> result = new List<PlanetLayerMaterial>();
            for (int i = 0; i < materials.Count; i++)
                result.Add(_serializer.Deserialize(materials[i]));
            return result;
        }

		private void Clear()
        {
            foreach (HullInfoLayerImage image in _images)
                image.Destruct();
            foreach (HullInfoLayerText text in _texts)
                text.Destruct();
            _images.Clear();
            _texts.Clear();
        }
    }
}