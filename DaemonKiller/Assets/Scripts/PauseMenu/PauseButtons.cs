using CharacterMovement.Player;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace PauseMenu
{
    /// <summary>
    /// Handles the buttons on the pause menu.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// buttonType      enum for the type of button
    /// 
    /// Private Vars    Description
    /// loading         bool to allow only one action of a button at a time.
    /// pC              player controller that handles the player inputs.
    /// pauseUI         GameObject UI that holds the pause buttons
    /// 
    public class PauseButtons : MonoBehaviour
    {
        /// <summary>
        /// Specifies the type of button
        /// Continue:   Resumes the game 
        /// Menu:       Returns the player to the main menu
        /// Exit:       Exits the application
        /// </summary>
        public enum ButtonType { Continue, Menu, Exit }
        public ButtonType buttonType;

        private bool loading;
        private PlayerController pC;
        private GameObject pauseUI;

        /// <summary>
        /// Initializes the pause UI
        /// </summary>
        /// 
        /// 2021-07-26  JH  Initial Implementation.
        /// 
        void Awake()
        {
            pauseUI = GameObject.FindGameObjectWithTag("Pause UI");
        }

        /// <summary>
        /// Reinitialize values and references on reenable of the pause UI
        /// </summary>
        /// 
        /// 2021-07-26  JH  Initial Implementation.
        /// 
        void OnEnable()
        {
            loading = false;
            if (!pC)
                pC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        /// <summary>
        /// Click behaviour of the button.
        /// Behaviour depends on the button type.
        /// </summary>
        /// 
        /// 2021-07-26  JH  Initial Implementation.
        /// 
        public void OnClick()
        {
            if (!loading)
                switch (buttonType)
                {
                    case ButtonType.Continue:
                        pC.SetToPreviousState();
                        pauseUI.SetActive(false);
                        Time.timeScale = 1f;
                        pC.GetPlayerMotor().SetAnimatorUpdateMode(AnimatorUpdateMode.UnscaledTime);
                        break;
                    case ButtonType.Menu:
                        Time.timeScale = 1f;
                        MenuManagers.ReturnToMainMenu(ref loading);
                        break;
                    case ButtonType.Exit:
                        MenuManagers.QuitGame();
                        break;
                }
        }
    }
}
