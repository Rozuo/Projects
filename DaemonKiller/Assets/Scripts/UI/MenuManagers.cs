using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionScene;

namespace UI
{
    public static class MenuManagers
    {
        /// <summary>
        /// Starts a new game.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 2021-07-13 JH Add LoadType, NoData, to give starting items
        /// 
        public static void StartGame(ref bool loading, ref TransitionSceneManager transition)
        {
            if (!loading)
            {
                loading = true;
                transition.loadType = TransitionSceneManager.LoadType.NoData;
                SceneManager.LoadScene(1);
                loading = false;
            }
        }

        /// <summary>
        /// Loads a game file.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 2021-07-13 JH Add LoadType, AllData, to load previous save data.
        /// 
        public static void LoadGame(ref bool loading, ref TransitionSceneManager transition, SaveDataManager saveManager)
        {
            if (!loading)
            {
                loading = true;
                transition.loadType = TransitionSceneManager.LoadType.AllData;
                saveManager.LoadScene();
                loading = false;
            }
        }

        /// <summary>
        /// Returns the player to the main menu.
        /// </summary>
        /// <param name="loading"></param>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 
        public static void ReturnToMainMenu(ref bool loading)
        {
            if (!loading)
            {
                loading = true;
                SceneManager.LoadScene(0);
                loading = false;
            }
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        /// 
        /// 2021-07-09 RB Intial Documentation.
        /// 
        public static void QuitGame()
        {
            Debug.Log("Quit game");
            Application.Quit();
        }

    }
}
