using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBaier.Master
{
    public class SeedDisplay : MonoBehaviour
    {
        private const string _seedText = "Seed: {0}";

        [SerializeField]
        private Text _seedDisplayText;
        private Seed _seed;

        [Inject]
        public void Construct(Seed seed)
		{
            _seed = seed;
        }


        protected virtual void Reset()
		{
            _seedDisplayText = GetComponent<Text>();
        }

        protected virtual void Start()
		{
            _seedDisplayText.text = string.Format(_seedText, _seed.SeedNumber.ToString());

        }
    }
}