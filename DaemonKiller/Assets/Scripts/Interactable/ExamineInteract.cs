using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// Manages any interactions that involve printing text
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var      desc    
    /// examineText[]   lines of text to draw when interacted with
    ///
    /// private var     desc
    /// txtInd          index of current text to draw
    /// activated       has this GameObject been interacted with by the player?
    /// targetObj       Targeting GameObject
    /// playerAnim      Player's Animator component
    /// 
    public class ExamineInteract : Interactables
    {       
        public string[] examineText;
        private int txtInd = 0;
        private bool activated = false;       
        private GameObject targetObj;
        private Animator playerAnim;

        /// <summary>
        /// Set up Components and references
        /// </summary>
        /// 
        /// 2021-06-21  TH  Initial Implementation
        /// 
        void Awake()
        {
            GameObject manObj = GameObject.FindGameObjectWithTag("Game Manager");
            if (manObj)
            {
                gui = manObj.transform.Find("GameHUD").GetComponent<GameUI>(); // defined in Interactables interface script
                targetObj = manObj.transform.Find("Targeting").gameObject;
            }

            playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        }

        /// <summary>
        /// Wait for player input, then advance text
        /// </summary>
        /// 
        /// 2021-06-01  TH  Initial Implementation
        /// 
        void Update()
        {
            if (!activated) return;

            if (Input.GetButtonDown("Submit"))
            {              
                if (++txtInd > examineText.Length-1)
                {                   
                    playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;    // set player's animator to default (unscaled time)
                    gui.ClearText();                                            // remove any text from the UI
                    gui.examineObj.gameObject.SetActive(true);
                    txtInd = 0;                                                 // reset text index so it can be reused when interacted with again
                    activated = false;
                    Time.timeScale = 1.0f;
                    targetObj.SetActive(true);
                }
                else
                {
                    gui.SetText(examineText[txtInd]);
                }
            }
        }

        /// <summary>
        /// Send the message to the GameUI to show
        /// </summary>
        /// <param name="gO">not used</param>
        /// 
        /// 2021-05-31  TH  Initial Implementation
        /// 2021-07-26  TH  Added a safety measure for playerAnim
        /// 
        public override void Interact(GameObject gO)
        {        
            if(!playerAnim)
            {
                // Attempt to fetch the player's Animator again
                playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
            }

            playerAnim.updateMode = AnimatorUpdateMode.Normal;  // "freeze" the player (animation is normally unscaled time)
            gui.SetText(examineText[0]);                        // print the first text value to the UI
            gui.examineObj.gameObject.SetActive(false);         // disable text that notifies an interaction (ex. "<E> Examine Item")
            activated = true;                                   // set this object as currently being examined
            Time.timeScale = 0;                                 // "pause" the game by stopping time
            targetObj.SetActive(false);                         // disable the Targeting GameObject to prevent activating it
        }
    }
}

