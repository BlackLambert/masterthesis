using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class NoiseImageCreator : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _upperLeft = Vector2.zero;
        [SerializeField]
        private Vector2 _delta = new Vector2(0.1f, 0.1f);
        [SerializeField]
        private Vector2Int _size = new Vector2Int(124, 124);
        [SerializeField]
        private string _fileName = "Noise";

        private Noise2D _noise;

        [Inject]
        private void Construct(Noise2D noise)
		{
            _noise = noise;
        }

        protected virtual void Start()
		{
            Create();

        }

        private void Create()
		{
            Texture2D texture = new Texture2D(_size.x, _size.y, TextureFormat.RGB24, false);
            for(int i = 0; i < _size.x; i++)
			{
				for (int j = 0; j < _size.y; j++)
				{
                    Vector2 evaluationValue = _upperLeft - new Vector2(_delta.x * i, _delta.y * j);
                    float noiseValue = (float) _noise.Evaluate(evaluationValue.x, evaluationValue.y);
                    texture.SetPixel(i, j, new Color(noiseValue, noiseValue, noiseValue));

                }
			}
            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();
            Object.Destroy(texture);
            File.WriteAllBytes(Application.dataPath + $"/../{_fileName}_{_size.x}x{_size.y}_.png", bytes);
        }
    }
}