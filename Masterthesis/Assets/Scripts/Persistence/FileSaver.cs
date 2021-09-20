using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace SBaier.Master
{
    public abstract class FileSaver<T> : FileSaver
    {
        [SerializeField]
        private string _fileName = "Presets";

        protected abstract object GetSerializableObject();

        public override void Save()
        {
            object serializableObject = GetSerializableObject();
            if (serializableObject.Equals(default(T)))
                return;
            string json = JsonConvert.SerializeObject(serializableObject);
            File.WriteAllText($"{Application.persistentDataPath}/{_fileName}.json", json);
        }
    }

    public abstract class FileSaver : MonoBehaviour 
    {
        public abstract void Save();
    }
}