using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TransitionScene;
using Audio;

namespace UI
{
    /// <summary>
    /// The start screen manager, manages how the start screen behaves. 
    /// It is responsible for starting, loading and quitting the game.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB), Jacky Huynh (JH)
    /// 
    /// Private Variables:
    /// currentIndex        The index of the button.
    /// upInput             Input for the up key code.
    /// downInput           Down input key code.
    /// confirmInput        Keycode for confirm.
    /// selectBlock         The selected color of a button.
    /// unselectBlock       unselected color for a button.
    /// transition          scene manager to determine how to load the game on switching scenes
    /// saveManager         manager to load a scene from its save file.
    /// state               state of the menu.
    /// 
    /// Public Variables:
    /// buttons     The buttons that are on the start screen.
    /// controlObj  The object with the controls description of the game.
    /// title       Title of the game.
    /// 
    public class StartScreenManager : MonoBehaviour
    {
        /// <summary>
        /// The states that the menu can enter.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial Documentation.
        /// 
        private enum MenuState
        {
            Main, Controls
        }
        
        public Button[] buttons;
        //private AudioSource[] audioSources = new AudioSource[3];
        private int currentIndex = 0;

        private KeyCode upInput = KeyCode.W;
        private KeyCode downInput = KeyCode.S;
        private KeyCode confirmInput = KeyCode.Space;

        //private Color unselectedColor;
        //private Color selectColor;
        private ColorBlock selectBlock;
        private ColorBlock unselectBlock;

        private TransitionSceneManager transition;
        private SaveDataManager saveManager;
        private bool loading;

        private MenuState state = MenuState.Main;
        public GameObject controlObj;
        public GameObject title;

        /// <summary>
        /// Initialize values
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 2021-07-13 JH Add transition and scene managers.
        /// 
        private void Start()
        {
            //unselectedColor = buttons[0].colors.normalColor;
            Color selectColor = buttons[0].colors.highlightedColor;
            selectBlock = buttons[0].colors;
            unselectBlock = buttons[0].colors;
            selectBlock.normalColor = selectColor;
            buttons[0].colors = selectBlock;

            GameObject sceneManager = GameObject.FindGameObjectWithTag("Scene Manager");
            transition = sceneManager.GetComponent<TransitionSceneManager>();
            saveManager = sceneManager.GetComponent<SaveDataManager>();
            loading = false;

            //for(int i = 0; i < audioSources.Length; i++)
            //{
            //    audioSources[i] = buttons[i].gameObject.GetComponent<AudioSource>();
            //}

        }

        /// <summary>
        /// Handles the behavior of the start screen, such as new game, load and quit.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 2021-07-31 RB Overhauled functionality to handle states.
        /// 
        private void Update()
        {
            switch (state)
            {
                case MenuState.Main:
                    HandleMain();
                    break;
                case MenuState.Controls:
                    HandleControls();
                    break;
            }
        }

        /// <summary>
        /// Handles when the menu is in the main state.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial Documentation.
        /// 
        private void HandleMain()
        {
            if (Input.GetKeyDown(upInput))
            {
                buttons[currentIndex].colors = unselectBlock;
                currentIndex = (currentIndex - 1) < 0 ? buttons.Length - 1 : currentIndex - 1;
                buttons[currentIndex].colors = selectBlock;
            }
            else if (Input.GetKeyDown(downInput))
            {
                buttons[currentIndex].colors = unselectBlock;
                currentIndex = (currentIndex + 1) >= buttons.Length ? 0 : currentIndex + 1;
                buttons[currentIndex].colors = selectBlock;
            }
            else if (Input.GetKeyDown(confirmInput))
            {
                switch (currentIndex)
                {
                    case (0):
                        // start new game
                        buttons[0].onClick.Invoke();
                        break;
                    case (1):
                        // load game
                        buttons[1].onClick.Invoke();
                        break;
                    case (2):
                        // controls
                        buttons[2].onClick.Invoke();
                        break;
                    case (3):
                        // exit game
                        buttons[3].onClick.Invoke();
                        break;
                    default:
                        Debug.Log("Button was pressed.");
                        break;
                }
            }
        }

        /// <summary>
        /// Handles controls when the menu is in the controls state.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial Documentation.
        /// 
        private void HandleControls()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                state = MenuState.Main;
                foreach (Button button in buttons)
                {
                    button.gameObject.SetActive(true);
                }
                SoundManager.PlaySound(SoundManager.Sounds.StartGame);
                controlObj.SetActive(false);
                title.SetActive(true);
            }
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 2021-07-13 JH Add LoadType, FirstLevel, to give starting items
        /// 
        public void StartGame()
        {
            //if (!loading)
            //{
                
            //    loading = true;
            //    transition.loadType = TransitionSceneManager.LoadType.NoData;
            //    SceneManager.LoadScene(1);
            //    loading = false;
            //}
            SoundManager.PlaySound(SoundManager.Sounds.StartGame);
            MenuManagers.StartGame(ref loading, ref transition);
        }

        /// <summary>
        /// Loads a game file.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 2021-07-13 JH Add LoadType, AllData, to load previous save data.
        /// 
        public void LoadGame()
        {
            //if (!loading)
            //{
            //    loading = true;
            //    transition.loadType = TransitionSceneManager.LoadType.AllData;
            //    saveManager.LoadScene();
            //    loading = false;
            //}
            SoundManager.PlaySound(SoundManager.Sounds.StartGame);
            MenuManagers.LoadGame(ref loading, ref transition, saveManager);
        }

        /// <summary>
        /// The method that is called when the controls button is pressed.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial Documentation.
        /// 
        public void Controls()
        {
            SoundManager.PlaySound(SoundManager.Sounds.StartGame);
            foreach(Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }
            title.SetActive(false);
            state = MenuState.Controls;
            controlObj.SetActive(true);
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        /// 
        /// 2021-07-09 RB Intial Documentation.
        /// 
        public void QuitGame()
        {
            MenuManagers.QuitGame();
        }
    }
}

