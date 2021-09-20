using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class FileLoader<T> : FileLoader
    {
        [SerializeField]
        private string _fileName;

        public T Load()
		{
            string path = $"{Application.persistentDataPath}/{_fileName}.json";
            if (!File.Exists(path))
                return default(T);
            string json = File.ReadAllText(path);
            T result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
    }

    public abstract class FileLoader : MonoBehaviour
	{

	}
}