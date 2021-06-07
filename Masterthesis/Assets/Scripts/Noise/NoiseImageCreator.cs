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
        private Vector2 _bottomLeft = Vector2.zero;
        [SerializeField]
        private Vector2 _delta = new Vector2(0.1f, 0.1f);
        [SerializeField]
        private Vector2Int _imageSize = new Vector2Int(512, 512);
        [SerializeField]
        private string _fileName = "Noise";
        [SerializeField]
        private string _filePath = "Noise";

        private Noise2D _noise;

        public string DirectoryPath => Path.Combine(Application.persistentDataPath, _filePath);
        public string FileName => $"{_fileName}_{_imageSize.x}x{_imageSize.y}.png";
        public string FilePath => Path.Combine(DirectoryPath, FileName);
        public Vector2Int ImageSize => _imageSize;

        [Inject]
        private void Construct(Noise2D noise)
		{
            _noise = noise;
        }

        protected virtual void Start()
		{
            CreateImage();
        }

        private void CreateImage()
		{
			Texture2D texture = new Texture2D(_imageSize.x, _imageSize.y, TextureFormat.RGB24, false);
			for (int y = 0; y < _imageSize.y; y++)
				SetImageRowPixels(ref texture, y);
			texture.Apply();
			CreatePNG(texture);
			Object.Destroy(texture);
		}

		private void SetImageRowPixels(ref Texture2D texture, int y)
		{
			for (int x = 0; x < _imageSize.x; x++)
				SetPixel(ref texture, y, x);
		}

		private void SetPixel(ref Texture2D texture, int y, int x)
		{
			Vector2 evaluationValue = _bottomLeft + new Vector2(_delta.x * y, _delta.y * x);
			float noiseValue = (float)_noise.Evaluate(evaluationValue.x, evaluationValue.y);
			texture.SetPixel(x, y, new Color(noiseValue, noiseValue, noiseValue));
		}

		private void CreatePNG(Texture2D texture)
		{
			byte[] bytes = texture.EncodeToPNG();
			Directory.CreateDirectory(DirectoryPath);
			File.WriteAllBytes(FilePath, bytes);
		}
	}
}