using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class FileOnDestroySaver : MonoBehaviour
    {
        [SerializeField]
        private FileSaver _saver;

        protected virtual void OnDestroy()
		{
            _saver.Save();
        }
    }
}