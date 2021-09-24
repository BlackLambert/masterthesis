using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public abstract class FileLoader<T> : FileLoader
    {
        [SerializeField]
        private string _fileName;
		private JsonSerializerSettings _settings;

		[Inject]
        public void Construct(JsonSerializerSettings settings)
		{
            _settings = settings;

        }

        public T Load()
		{
            string path = $"{Application.persistentDataPath}/{_fileName}.json";
            if (!File.Exists(path))
                return default(T);
            string json = File.ReadAllText(path);
            T result = JsonConvert.DeserializeObject<T>(json, _settings);
            //CanyonSettings c = JsonConvert.DeserializeObject<CanyonSettings>(@"{'canyonMinBreadth':0.2,'canyonMaxBreadth':0.1,'canyonMinDepth':0.1,'canyonMaxDepth':1.0,'canyonBlendvalue':0.15}", _settings);
            //MountainSettings m = JsonConvert.DeserializeObject<MountainSettings>(@"{'mountainMinBreadth':0.4,'mountainMaxBreadth':0.1,'mountainMinHeight':0.3,'mountainMaxHeight':1.0,'mountainBlendvalue':0.15}", _settings);
            //PlanetDimensions d = JsonConvert.DeserializeObject<PlanetDimensions>(@"{'kernelRadius':4.2,'hullMaxRadius':5.0,'atmosphereRadius':8.0,'relativeSeaLevel':0.5,'maxHullThickness':0.8000002}", _settings);
            //TemperatureSpectrum t = JsonConvert.DeserializeObject<TemperatureSpectrum>(@"{'tempMinimal':-30.0,'tempMaximal':50.0}", _settings);
            //ShapingParameter s = JsonConvert.DeserializeObject<ShapingParameter>(@"{'plates':{'mountain':{ 'mountainMinBreadth':0.4,'mountainMaxBreadth':0.1,'mountainMinHeight':0.3,'mountainMaxHeight':1.0,'mountainBlendvalue':0.25},'canyon':{ 'canyonMinBreadth':0.2,'canyonMaxBreadth':0.1,'canyonMinDepth':0.1,'canyonMaxDepth':1.0,'canyonBlendvalue':0.15}}}", _settings);
            return result;
        }
    }

    public abstract class FileLoader : MonoBehaviour
	{

	}
}