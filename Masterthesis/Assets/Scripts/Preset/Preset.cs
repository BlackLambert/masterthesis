using Newtonsoft.Json;
using System;

namespace SBaier.Master
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Preset
    {
        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("generatorParameters")]
        public PlanetGenerator.Parameter Parameters { get; private set; }

        public Preset(string name,
            PlanetGenerator.Parameter parameters)
		{
            Name = name;
            Parameters = parameters;
        }

        public void Update(PlanetGenerator.Parameter parameters)
		{
            Parameters = parameters;
        }
    }
}