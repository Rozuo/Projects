using System.Collections;
using Interactable;
using TransitionScene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable
{
    /// <summary>
    /// Interact that allows the player to transition to the next scene when interacted with.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Var      Description
    /// sceneName       name of the scene to transition to
    /// 
    /// Private Var     Description
    /// loading         bool to determine if scene is loading
    /// saveMan         save manager to save with
    /// 
    public class SceneInteract : Interactables
    {
        [Header("Scene Name to Load")]
        public string sceneName;
        private bool loading = false;
        private SaveDataManager saveMan;

        /// <summary>
        /// Initialize save manager
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        void Start()
        {
            // remove after adding Scene Manager to each scene
            if (GameObject.FindGameObjectWithTag("Scene Manager"))
                saveMan = GameObject.FindGameObjectWithTag("Scene Manager").GetComponent<SaveDataManager>();
        }

        /// <summary>
        /// Transitions the player to the scene name provided
        /// </summary>
        /// <param name="gO">player that interacted with the interactable</param>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        public override void Interact(GameObject gO)
        {
            // Prepare to transfer current player data
            if (saveMan)
                saveMan.SavePlayerData();
            if (TransitionSceneManager.instance)
                TransitionSceneManager.instance.loadType = TransitionSceneManager.LoadType.PlayerData;
            // load next scene
            if (sceneName != null && !loading)
            {
                loading = true; // to ensure only one load is occuring
                StartCoroutine(LoadSceneByName());
            }
        }

        /// <summary>
        /// Loads the scene by name and waits until it is done
        /// </summary>
        /// <returns>IEnumerator for the coroutine</returns>
        /// 
        /// 2021-07-12  JH  Initial Implementation
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