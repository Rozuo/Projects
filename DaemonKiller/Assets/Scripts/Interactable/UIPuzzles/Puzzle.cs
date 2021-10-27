using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterMovement.Player;

namespace Interactable.UIPuzzles{
    /// <summary>
    /// Puzzle class is the basis for all other puzzles.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Protected Variables:
    /// state                   The state of the puzzle.
    /// inventory               inventory of the player.
    /// ui                      The puzzle ui.
    /// playerController        The player controller.
    /// gmObj                   The game manager object.
    /// gm                      The game manager.
    /// notSolvedText           Text that will show when the puzzle is not solved.
    /// background              The background of the puzzle.
    /// solution                Solution of the puzzle.
    /// currentAnswer           The answer of the player.
    /// correct                 That is used to determine if the player is correct.
    /// 
    public class Puzzle : Interactables
    {   
        public enum PuzzleStates { Solved, Interacting, NotSolved }

        public Interactables unlockableObject;

        [SerializeField]
        protected PuzzleStates puzzState = PuzzleStates.NotSolved;
        //protected Inventory inventory;
        protected GameUI ui;
        protected PlayerController playerController;
        protected GameObject gmObj;
        //protected GameManager gm;

        [Header("Puzzle Requirements")]
        [SerializeField]
        protected Text notSolvedText;
        [SerializeField]
        protected GameObject background;
        [SerializeField]
        protected int[] solution;
        [SerializeField]
        protected int[] currentAnswer;

        [SerializeField]
        protected Text helpText;
        protected bool correct = false;

        /// <summary>
        /// Initialize variables.
        /// </summary>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 2021-07-14 JH Moved activating background to Start. 
        /// 
        protected void Awake()
        {
            gmObj = GameObject.FindGameObjectWithTag("Game Manager");
            gm = gmObj.GetComponent<GameManager>();
            ui = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<GameUI>();
            inventory = gmObj.GetComponent<Inventory>();
            unlockableObject.InteractableState = State.Close;
            background = gm.GetDialPuzzleBackground();
            helpText = GetHelpText(background.gameObject);
        }

        /// <summary>
        /// Initialize variables.
        /// </summary>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 2021-07
        /// 
        protected virtual void Start()
        {
            playerController = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().GetPlayer();
            background.SetActive(true);
        }

        /// <summary>
        /// The behavior when the game object is interacted with.
        /// </summary>
        /// <param name="gO"></param>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 
        public override void Interact(GameObject gO)
        {
            return;
        }

        /// <summary>
        /// Get the state of the puzzle.
        /// </summary>
        /// <returns>puzzle state</returns>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 2021-07-14 JH Fixed return from State to PuzzleStates
        /// 
        public PuzzleStates GetState()
        {
            return puzzState;
        }

        /// <summary>
        /// Set the state of the puzzle.
        /// </summary>
        /// <param name="state">state of the puzzle to set as</param>
        /// 
        /// 2021-07-14 JH Initial Documentation.
        /// 
        public void SetState(PuzzleStates state)
        {
            puzzState = state;
        }

        /// <summary>
        /// Get the value that determines if the player is correct
        /// </summary>
        /// <returns>puzzle solved or not</returns>
        /// 
        /// 2021-07-14 JH Initial Documentation
        /// 
        public bool GetCorrect()
        {
            return correct;
        }

        /// <summary>
        /// Set the correct value if the player is correct
        /// </summary>
        /// 
        /// 2021-07-14 JH Initial Documentation
        /// 
        public void SetCorrect(bool state)
        {
            correct = state;
        }

        /// <summary>
        /// Displays the help text if it exists on the current puzzle.
        /// </summary>
        /// <param name="text"></param>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 
        protected virtual void InitHelpText(string text)
        {
            if (helpText == null)
            {
                return;
            }
            Debug.Log(helpText.name);
            helpText.text = text;
        }

        /// <summary>
        /// Gets the help text from a child object.
        /// </summary>
        /// <param name="parent">Object we want to get the text from.</param>
        /// <returns></returns>
        /// 
        /// 2021-07-29 RB Initial Documentation.
        /// 
        protected Text GetHelpText(GameObject parent)
        {
            Transform transform;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                transform = parent.transform.GetChild(i);
                if(transform.tag == "HelpText")
                {
                    return transform.gameObject.GetComponent<Text>();
                }
            }
            return null;
        }
    }

}
