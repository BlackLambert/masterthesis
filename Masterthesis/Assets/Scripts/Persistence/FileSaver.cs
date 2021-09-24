using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Zenject;

namespace SBaier.Master
{
    public abstract class FileSaver<T> : FileSaver
    {
        [SerializeField]
        private string _fileName = "Presets";
		private JsonSerializerSettings _settings;

		protected abstract object GetSerializableObject();



        [Inject]
        public void Construct(JsonSerializerSettings settings)
        {
            _settings = settings;

        }

        public override void Save()
        {
            object serializableObject = GetSerializableObject();
            if (serializableObject.Equals(default(T)))
                return;
            string json = JsonConvert.SerializeObject(serializableObject, _settings);
            File.WriteAllText($"{Application.persistentDataPath}/{_fileName}.json", json);
        }
    }

    public abstract class FileSaver : MonoBehaviour 
    {
        public abstract void Save();
    }
}