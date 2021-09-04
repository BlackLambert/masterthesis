using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SBaier.Master
{
    public class HullInfoLayerText : MonoBehaviour
    {
        private const string _textTemplate = "{0}. Layer ({1}, {2}%):{3}";
        private const string _materialTextTemplate = " {0} ({1}%)";

        [SerializeField]
        private Transform _base;
        [SerializeField]
        private TextMeshProUGUI _textField;

		public void Init(Transform parent, 
            int layerIndex, 
            float layerPortion, 
            PlanetMaterialState state, 
            PlanetData planetData,
            List<PlanetLayerMaterial> materials)
		{
            _base.SetParent(parent);
            _textField.text = string.Format(_textTemplate, layerIndex, state.ToName(), CreatePortionString(layerPortion), CreateMaterialInfo(planetData, materials));
            _base.localScale = Vector3.one;
        }

		private string CreateMaterialInfo(PlanetData planetData, List<PlanetLayerMaterial> materials)
		{
            string result = string.Empty;
            float sum = materials.Sum(m => m.Portion);
            Dictionary<PlanetLayerMaterialSettings, float> matToSum = CreateMaterialToPortion(planetData, materials);
            PlanetLayerMaterialSettings[] materialSettings = matToSum.Keys.ToArray();
            for (int i = 0; i < matToSum.Count; i++)
                result += AddMaterialInfo(materialSettings[i], matToSum[materialSettings[i]], sum, i);
            return result;
		}

		private Dictionary<PlanetLayerMaterialSettings, float> CreateMaterialToPortion(PlanetData planetData, List<PlanetLayerMaterial> materials)
		{
            Dictionary<PlanetLayerMaterialSettings, float> result = new Dictionary<PlanetLayerMaterialSettings, float>();
			for (int i = 0; i < materials.Count; i++)
			{
                PlanetLayerMaterialSettings m = planetData.Materials[materials[i].MaterialID];
                float portion = result.ContainsKey(m) ? result[m] : 0;
                result[m] = portion + materials[i].Portion;
            }
            return result;
        }

        private string AddMaterialInfo(PlanetLayerMaterialSettings material, float portion, float portionSum, int index)
        {
            string result = string.Empty;
            if (index > 0)
                result += ",";
            result += string.Format(_materialTextTemplate, material.Name, CreatePortionString(portion / portionSum));
            return result;
        }

        private string CreatePortionString(float portion)
		{
            return (((int)(Mathf.Clamp01(portion) * 1000)) / 10.0f).ToString();
        }

		public void Destruct()
		{
            Destroy(_base.gameObject);
        }
	}
}