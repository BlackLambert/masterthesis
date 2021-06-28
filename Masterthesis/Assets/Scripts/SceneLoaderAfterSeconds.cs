using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SBaier.Master
{
    public class SceneLoaderAfterSeconds : MonoBehaviour
    {
        [SerializeField]
        private float _seconds = 5;
        [SerializeField]
		private string _sceneName = "Scene";

		// Start is called before the first frame update
		IEnumerator Start()
        {
			yield return new WaitForSeconds(_seconds);
            SceneManager.LoadScene(_sceneName);
        }
    }
}