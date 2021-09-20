using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [Serializable]
    public class Preset
    {
        public string Name { get; }
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