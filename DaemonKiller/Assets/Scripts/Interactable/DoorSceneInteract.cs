using System.Collections;
using TransitionScene;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Interactable
{
    /// <summary>
    /// Modified door interact class, allowing the player to transfer to a new scene upon interacting
    /// with an unlocked door.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Var      Description
    /// sceneName       name of the scene to transfer to
    /// 
    /// Private Var
    /// loading         determines if scene is loading or not
    /// saveMan         save data manager to transfer player data upon transfering scenes.
    /// 
    public class DoorSceneInteract : DoorInteract
    {
        [Header("Scene Name to Load")]
        public string sceneName;
        private bool loading = false;
        private SaveDataManager saveMan;

        /// <summary>
        /// Reference save manager and other door references.
        /// </summary>
        /// 
        /// 2021-07-20  JH  Initial Implementation
        /// 
        public override void Start()
        {
            base.Start();
            if (GameObject.FindGameObjectWithTag("Scene Manager"))
                saveMan = GameObject.FindGameObjectWithTag("Scene Manager").GetComponent<SaveDataManager>();
        }

        /// <summary>
        /// Interaction that the door will perform when called.
        /// </summary>
        /// <param name="gO">Game object that will be affected by the interacftion</param>
        /// 
        /// 2021-07-20  JH  Initial Implementation
        /// 
        public override void Interact(GameObject gO)
        {
            if (state == State.Close)
            {
                // Door will output a Permanently Closed message.
                gui.ChangeTextTimed(doorStuckText);
                return;
            }

            if (state == State.Locked)
            {
                // Check if the player current has the key.
                if (inventory.ItemExists(doorKey))
                {
                    state = State.Open;
                    if (consumeOnUse) inventory.Use(1, doorKey);
                    gui.ChangeTextTimed(
                        "Unlocked the door with the <color=lime>" + doorKey.itemName + "</color>.");
                }
                else
                {
                    // Door is currently locked.
                    gui.ChangeTextTimed(
                        doorLockedText + " I need the <color=lime>" + doorKey.itemName + "</color>.");
                }
            }
            else
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