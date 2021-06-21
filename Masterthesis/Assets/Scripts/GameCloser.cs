using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balloon
{
    public class GameCloser : MonoBehaviour
    {
        [SerializeField]
        private List<KeyCode> m_liCloseGameKeys = new List<KeyCode>();

        protected virtual void Update()
		{
			if (!Input.anyKeyDown)
				return;
			checkQuit();
		}

		private void checkQuit()
		{
			foreach (KeyCode keyCode in m_liCloseGameKeys)
				checkQuitBy(keyCode);
		}

		private void checkQuitBy(KeyCode _keyCode)
		{
			if (Input.GetKeyDown(_keyCode))
				quit();
		}

		private void quit()
		{
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}