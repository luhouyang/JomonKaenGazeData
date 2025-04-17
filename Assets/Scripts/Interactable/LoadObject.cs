using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.JomonKaenGazeData
{
    /// <summary>
    /// Switch game object.
    /// </summary>
    [AddComponentMenu("Scripts/Utils/LoadObject")]
    public class LoadObject : MonoBehaviour
    {
        static public GameObject selectedObject = null;
        
        #region Serialized variables
        [SerializeField]
        private GameObject objectToLoad = null;
        #endregion

        void Start()
        {
            
        }

        void Update()
        {

        }

        public void LoadThis()
        {
            if (selectedObject != null)
            {
                selectedObject.gameObject.SetActive(false);
            }
            selectedObject = objectToLoad;
            selectedObject.gameObject.SetActive(true);
            Debug.Log(selectedObject.name);
        }
    }
}
