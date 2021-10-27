using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TransitionScene;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// Manages the process of running a Game Over
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH), Rozario (Ross) Beaudin
    /// 
    /// private var     desc
    /// director        reference to the Game Director GameObject
    /// transition      reference to TransitionSceneManager component
    /// saveManager     reference to SaveDataManager component
    /// loading         if the game is currently loading another scene
    /// 
    [RequireComponent(typeof(AudioSource))]
    public class GameOverManager : MonoBehaviour
    {
        private GameObject director;       
        private TransitionSceneManager transition;
        private SaveDataManager saveManager;
        private bool loading = false;

        /// <summary>
        /// If GameDirector exists, disable its many game elements.
        /// </summary>
        /// 
        /// 2020-06-01  TH  Initial Implementation
        /// 
        void Awake()
        {
            // get component references
            director = GameObject.FindGameObjectWithTag("Game Manager");
            GameObject sceneManager = GameObject.FindGameObjectWithTag("Scene Manager");
            transition = sceneManager.GetComponent<TransitionSceneManager>();
            saveManager = sceneManager.GetComponent<SaveDataManager>();

            if (director)
            {
                // Disable HUD elements
                director.transform.Find("GameHUD").gameObject.SetActive(false);

                // Alter Background Music
            }
        }

        /// <summary>
        /// Send the player back to the main menu.
        /// </summary>
        /// 
        /// 2021-07-01 RB Initial Documentation.
        /// 
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MainMenuButton();
            }
        }

        /// <summary>
        /// Sends the player back to the main menu.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 
        public void MainMenuButton()
        {
            MenuManagers.ReturnToMainMenu(ref loading);
        }

        /// <summary>
        /// Sends the player back to the main menu.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 
        public void RetryButton()
        {
            MenuManagers.LoadGame(ref loading, ref transition, saveManager);
        }
    }
}
