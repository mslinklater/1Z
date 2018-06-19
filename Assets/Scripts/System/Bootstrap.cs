using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// This component bootstraps the game into the start scene.
// As long as an instance of this component is in a level, when you
// hit 'play' the bootstrap will redirect to the correct entry scene.

namespace MSL
{
    public class Bootstrap : MonoBehaviour
    {
#region Instance Management
        public static Bootstrap instance { get{ return _instance;}}
        private static Bootstrap _instance = null;
#endregion

        public string entrySceneName;

        void Awake()
        {
            try {
                if(_instance != null)
                {
                    // An instance already exists, so destroy my game object
                    DestroyImmediate(gameObject);
                    return;
                }
                else
                {
                    // This is the first Bootstrap, so lets set some stuff up

                    // Set singleton reference
                    _instance = this;

                    // Make my game object immune to level load destruction
                    DontDestroyOnLoad(gameObject);

                    // Instantiate subsystems

                    // Initialise subsystems

                    // Go to the entry scene
                    if(entrySceneName != "")
                    {
                        // TODO: Need check for valid scene name here
                        SceneManager.LoadScene(entrySceneName);
                    }
                }
            }
            catch(System.Exception ex) {
                // TODO: Create a proper exception handling scene
                Application.Quit();
            }
        }

        void OnEnable()
        {

        }

        void Start()
        {

        }

        void OnDisable()
        {

        }

        void OnDestroy()
        {

        }        
    }
}