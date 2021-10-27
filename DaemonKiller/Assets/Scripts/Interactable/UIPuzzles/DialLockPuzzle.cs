using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterMovement.Player;

namespace Interactable.UIPuzzles
{
    /// <summary>
    /// Dial lock puzzle is an extension of puzzle when 
    /// the player matches the correct combination it will unlock.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB)
    /// 
    /// Private Variables:
    /// selectedDialIndex       The selected index of a dial
    /// lastInteractInput       Last input of the interact key. 
    /// horizontalInput         Input of the horizontal axis. DEPRECATED
    /// lastHorizontalInput     Last input of the horizontal axis. DEPRECATED
    /// verticalInput           Input of the vertical input. DEPRECATED
    /// lastVerticalInput       Last input of the vertical axis. DEPRECATED
    /// upInput                 Key code for up input.
    /// downInput               Down input key code.
    /// rightInput              Key code for right input.
    /// leftInput               Left input key code.
    /// dialTexts               Array of the dial texts.
    /// dialBackground          Background image array for the dials.
    /// selectColor             Color of the a dial that is selected.
    /// unSelectedColor         The color of a dial that is not selected.
    /// 
    public class DialLockPuzzle : Puzzle
    {

        private int selectedDialIndex = 0;

        private float interactInput = 0f;
        private float lastInteractInput = 0f;
        private KeyCode upInput = KeyCode.W;
        private KeyCode downInput = KeyCode.S;
        private KeyCode rightInput = KeyCode.D;
        private KeyCode leftInput = KeyCode.A;
        private float horizontalInput = 0f;
        private float lastHorizontalInput = 0f;
        private float verticalInput = 0f;
        private float lastVerticalInput = 0f;

        private Text[] dialTexts;
        private Image[] dialBackground;
        private Color selectColor = Color.green;
        private Color unSelectedColor;

        /// <summary>
        /// Initialize all variables and search for dials.
        /// </summary>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 2021-07-14 JH Background active set here instead of awake 
        ///               Dials no longer randomize if previously solved.
        /// 
        protected override void Start()
        {
            base.Start();
            InitHelpText("Press <color=lime>[E]</color> to check if the combination is correct.\n" +
                "Press <color=lime>[A]</color> or <color=lime>[D]</color> to cycle between dials.\n" +
                "Press <color=lime>[W]</color> or <color=lime>[S]</color> to change number within a dial.\n" +
                "Press <color=lime>[Q]</color> to leave the dial lock alone.");
            FindDials();
            if (solution.Length == 0)
            {
                solution = new int[] {4, 3, 2};
            }
            else if(solution.Length > 3)
            {
                solution = new int[] { solution[0], solution[1], solution[2] };
            }

            if (correct)
            {
                currentAnswer = solution;
            }
            else
            {
                currentAnswer = new int[] { Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10) };
            }

            for (int i = 0; i < dialTexts.Length; i++)
            {
                dialTexts[i].text = currentAnswer[i].ToString();
            }
            unSelectedColor = dialBackground[0].color;
            dialBackground[0].color = selectColor;
            background.SetActive(false);
            //Debug.Log("dial texts: " + dialTexts[0].text + dialTexts[1].text + dialTexts[2].text);
        }

        /// <summary>
        /// Handles the different states.
        /// </summary>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 
        private void Update()
        {
            switch (puzzState)
            {
                case PuzzleStates.Interacting:
                    HandleInteracting();
                    break;
            }
        }

        /// <summary>
        /// Handles the interacting state.
        /// </summary>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 2021-07-09 RB Bug fixes with inputs.
        /// 
        private void HandleInteracting()
        {
            interactInput = Input.GetAxis("Interact");
            if (interactInput == 1 && lastInteractInput == 0f)
            {
                for(int i = 0; i < currentAnswer.Length; i++)
                {
                    if(currentAnswer[i] != solution[i])
                    {
                        correct = false;
                        break;
                    }
                    correct = true;
                }

                if(correct)
                {
                    ui.ChangeTextTimed("That was the combination!");
                    puzzState = PuzzleStates.Solved;
                    background.SetActive(false);
                    playerController.PlayerState = PlayerController.State.Exploring;
                    unlockableObject.InteractableState = State.Open;
                }
                else
                {
                    ui.ChangeTextTimed("That's not the right combination...");
                }
            }

            //horizontalInput = Input.GetAxis("Horizontal");
            //verticalInput = Input.GetAxis("Vertical");
            ////Debug.Log("hori = " + horizontalInput);
            ////Debug.Log("vert = " + verticalInput);
            if (Input.GetKeyDown(upInput))
            {

                currentAnswer[selectedDialIndex] += 1;
                currentAnswer[selectedDialIndex] = currentAnswer[selectedDialIndex] > 9 ? 0 : currentAnswer[selectedDialIndex];
                dialTexts[selectedDialIndex].text = currentAnswer[selectedDialIndex].ToString();
            }
            else if(Input.GetKeyDown(downInput))
            {
                currentAnswer[selectedDialIndex] -= 1;
                currentAnswer[selectedDialIndex] = currentAnswer[selectedDialIndex] < 0 ? 9 : currentAnswer[selectedDialIndex];
                dialTexts[selectedDialIndex].text = currentAnswer[selectedDialIndex].ToString();
            }


            if (Input.GetKeyDown(rightInput))
            {
                dialBackground[selectedDialIndex].color = unSelectedColor;
                selectedDialIndex += 1;
                selectedDialIndex = selectedDialIndex >= solution.Length ? 0 : selectedDialIndex;
                dialBackground[selectedDialIndex].color = selectColor;
            }
            else if(Input.GetKeyDown(leftInput))
            {
                dialBackground[selectedDialIndex].color = unSelectedColor;
                selectedDialIndex -= 1;
                selectedDialIndex = selectedDialIndex < 0 ? solution.Length - 1 : selectedDialIndex;
                dialBackground[selectedDialIndex].color = selectColor;
            }
            

            if(Input.GetKeyDown(KeyCode.Q))
            {
                puzzState = PuzzleStates.NotSolved;
                playerController.PlayerState = PlayerController.State.Exploring;
                background.SetActive(false);
            }

            lastInteractInput = interactInput;
            lastHorizontalInput = horizontalInput;
            lastVerticalInput = verticalInput;
        }

        /// <summary>
        /// Determines what happens when the player interacts with the dial lock puzzle.
        /// </summary>
        /// <param name="gO"></param>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 
        public override void Interact(GameObject gO)
        {
            puzzState = PuzzleStates.Interacting;
            playerController.PlayerState = PlayerController.State.Cutscene;
            background.SetActive(true);
        }

        /// <summary>
        /// Finds all dials in a scene
        /// </summary>
        /// 
        /// 2021-07-02 RB Initial Documentation.
        /// 
        private void FindDials()
        {
            GameObject[] gOs = GameObject.FindGameObjectsWithTag("DialText");
            dialTexts = new Text[gOs.Length];
            dialBackground = new Image[gOs.Length];
            for(int i = 0; i < gOs.Length; i++)
            {
                dialTexts[i] = gOs[i].GetComponent<Text>();
                dialBackground[i] = gOs[i].GetComponentInParent<Image>();
            }
        }
    }
}

