using System.Collections;
using TransitionScene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable
{
    /// <summary>
    /// Transitions to another scene if a specific object collides with the trigger.
    /// NOTE: Is mainly intended for LV0_Intro
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    ///         Jacky Huynh (JH)
    /// NOTE:   Code taken from JH's DoorSceneInteract script and modified by TH.
    /// 
    /// public var      desc 
    /// sceneName       Exact name of the scene to go to
    /// 
    /// private var     desc
    /// loading         If the scene is already being loaded
    /// 
    public class DoorSceneTrigger : MonoBehaviour
    {
        [Tooltip("The exact name of the level to go to.")]
        public string sceneName;
        private bool loading;

        /// <summary>
        /// If the Main Camera collides with this trigger, being loading the next scene.
        /// </summary>
        /// <param name="other">The object colliding with this trigger</param>
        /// 
        /// 2021-07-23  TH  Initial Implementation
        ///        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("MainCamera"))
            {
                // set load type to First Level, to give the player their items
                if (TransitionSceneManager.instance)
                    TransitionSceneManager.instance.loadType = TransitionSceneManager.LoadType.FirstLevel;
                
                // load next scene
                if (sceneName != null && !loading)
                {
                    loading = true; // to ensure only one load is occuring
                    StartCoroutine(LoadSceneByName());
                }
            }
            else
                Debug.LogWarning("Collided object isn't the Main Camera");
        }

        /// <summary>
        /// Loads the scene by name and waits until it is done
        /// </summary>
        /// <returns>IEnumerator for the coroutine</returns>
        /// 
        /// 2021-07-20  JH  Initial Implementation
        /// 
        private IEnumerator LoadSceneByName()
        {
            AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            // wait until load is done
            while (!asyncSceneLoad.isDone && asyncSceneLoad != null)
            {
                yield return null;
            }
            loading = false; // doesnt occur due to object destroyed by scene loading in Single mode
        }
    }
}

