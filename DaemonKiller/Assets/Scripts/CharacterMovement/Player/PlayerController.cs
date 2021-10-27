using System.Collections;
using System.Collections.Generic;
using Unit.Info.Player;
using UnityEngine;
using Unit.Info;
using Unit.Player;

namespace CharacterMovement.Player
{
    /// <summary>
    /// The player controller is responsible for telling the PlayerMotor how to move and where to move. 
    /// As well as tells the motor if to interact an object in the world.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB)
    ///         Tyson Hoang (TH)
    ///         Jacky Huynh (JH)
    /// 
    /// 
    /// Public Variables:
    /// stepRayLower            The minimum height for an object to be climbed over.
    /// stepRayUpper            The maximum height for an object to be climbed over.
    /// interactPoint           A point where the character can interact with an object.
    /// 
    /// Private Variables:
    /// SWAP_COOLDOWN           Cooldown between swapping guns
    /// RELOAD_COOLDOWN         Cooldown between reloading
    /// nextSwapTime            Time until next swap available
    /// nextReloadTime          Time until next reload available
    /// state                   The state of the player.
    /// prevState               The previous state of the player. (Exploring/Combat only)
    /// motor                   The PlayerMotor.
    /// xInput                  The x input of the player.
    /// zInput                  The z input of the player.
    /// invInput                The inventory input of the player.
    /// lastInvInput            The last inventory input value of the player.
    /// interactInput           Input of the interact key.
    /// lastInteractInput       The last input of the interact key.
    /// reloadInput             Input of the reload key
    /// lastReloadInput         The last input of the reload key
    /// swapInput               Input of the swap key
    /// lastSwapInput           The last input of the swap key
    /// maxSpeedX               The max X speed.
    /// maxSpeedZ               The max Z speed.
    /// maxOverallSpeed         Max overall speed of the player.
    /// stepHeight              How high the character can step up.
    /// stepSmooth              The step height of the character.
    /// interactPoint           The point that the character can interact with.
    /// reloadAnimationName     string of the reload animation name
    /// pauseUI                 The pause UI parent GameObject.
    /// playerInfo              PlayerInfo of the player, holding player information
    /// 
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// States the player you could enter while playing the game.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public enum State
        {
            Inventory, Exploring, Combat, Cutscene, Dead, Pause
        }

        private const float SWAP_COOLDOWN = 0.2f;
        private const float RELOAD_COOLDOWN = 1f;
        private float nextSwapTime = 0f;
        private float nextReloadTime = 0f;

        [SerializeField]
        private State state = State.Exploring;
        private State prevState = State.Exploring;
        private PlayerMotor motor;
        private bool block = false;

        [Header("Inputs")]
        private float xInput = 0f;
        private float zInput = 0f;
        private float invInput = 0f;
        private float lastInvInput = 0f;
        private float interactInput = 0f;
        private float lastInteractInput = 0f;
        private float reloadInput = 0f;
        private float lastReloadInput = 0f;
        private float swapInput = 0f;
        private float lastSwapInput = 0f;
        private float sprintInput = 0f;
        private float blockInput = 0f;
        private float pauseInput = 0f;
        private float lastPauseInput = 0f;

        [Header("Movement")]
        private float maxSpeedX = 5f;
        private float maxSpeedZ = 5f;
        private float maxOverallSpeed = 5f;
        private float maxCombatSpeed = 2.5f;
        private float maxBlockSpeed = 1.5f;
        private bool exhaust = false;
        private IEnumerator lastExhaust;

        [Header("Stairs")]
        private float stepHeight = 0.25f;
        private float stepSmooth = 0.13f;
        public GameObject stepRayLower;
        public GameObject stepRayUpper;

        [Header("Interactions")]
        public Transform interactPoint;

        [Header("Reload")]
        [SerializeField]
        private string reloadAnimationName;

        // Probably want to remove health, maxhealth, and energy + related functions
        // since they've been moved to PlayerInfo.cs - TH
        [Header("Stats and UI")]
        private int health = 100;
        //private int maxHealth = 100;
        private int energy = 100;
        private float blockDecreaseRate = 1.0f;
        private float sprintDecreaseRate = 30f;
        public GameObject invUI;
        private GameObject actionUI;
        private GameObject hudUI;
        private GameObject pauseUI;
        private PlayerInfo playerInfo;

        [SerializeField]
        private string deathAnimationName;

        /// <summary>
        /// Finds the inventory UI and action UI.
        /// </summary>
        /// 
        /// 2021-05-21  RB  Initial Documentation.
        /// 2021-06-10  JH  Now initializes player info and adds 
        ///                 reload animation name to motor
        /// 
        private void Awake()
        {
            //    Debug.Log(interactPoint.bounds.center);
            hudUI = GameObject.FindGameObjectWithTag("Game HUD");
            invUI = GameObject.FindGameObjectWithTag("Inventory UI");
            actionUI = GameObject.FindGameObjectWithTag("Action UI");
            pauseUI = GameObject.FindGameObjectWithTag("Pause UI");
            playerInfo = GetComponent<PlayerInfo>();
            motor = new PlayerMotor(GetComponent<Transform>(), GetComponent<Rigidbody>(), GetComponent<Animator>(), 
                maxSpeedX, maxSpeedZ, maxOverallSpeed, deathAnimationName, reloadAnimationName, stepSmooth, 
                stepHeight, stepRayLower, stepRayUpper, interactPoint, gameObject);
        }

        /// <summary>
        /// Initilizes all necessary variables for the character controller.
        /// </summary>
        /// 
        /// 2021-05-06 RB Initial Documentation
        /// 2021-07-25 JH Add pauseUI
        /// 
        void Start()
        {
            invUI.SetActive(false);
            actionUI.SetActive(false);
            pauseUI.SetActive(false);
        }

        /// <summary>
        /// Handles inputs for states that need input during a time scale of 0f.
        /// </summary>
        /// 
        /// 2021-07-25 JH Initial Documentation
        /// 
        private void Update()
        {
            //if (Time.timeScale == 1f && Input.GetKeyDown(KeyCode.X))
            //    Time.timeScale = 0f;
            //else if (Time.timeScale != 1f && Input.GetKeyDown(KeyCode.C))
            //    Time.timeScale = 1f;
            if (state == State.Pause && Time.timeScale == 0f)
                HandlePauseState();
        }

        /// <summary>
        /// Handles when to use a different player state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        void FixedUpdate()
        {
            switch (state)
            {
                case State.Exploring:
                    ExploringState();
                    break;
                case State.Inventory:
                    InventoryState();
                    break;
                case State.Combat:
                    HandleCombatState();
                    break;
            }
        }

        /// <summary>
        /// Handles the idle state of the player.
        /// </summary>
        /// 
        /// 2021-05-06 RB Initial Documentation
        /// 
        private void ExploringState()
        {
            HandleExploringInput();
            motor.StairClimb();
            
        }

        /// <summary>
        /// Handles the play input.
        /// </summary>
        /// 
        /// 2021-05-06 RB Initial Documentation
        /// 2021-06-08 JH Add reload and swap inputs - reference: https://docs.unity3d.com/ScriptReference/Time-time.html
        /// 2021-07-25 JH Add pause inputs.
        /// 2021-08-04 JH Add time check when pausing
        /// 
        private void HandleExploringInput()
        {
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");
            interactInput = Input.GetAxis("Interact");
            motor.Move(xInput, zInput);

            if(interactInput == 1f && lastInteractInput == 0f)
                motor.Interact(interactInput);
            lastInteractInput = interactInput;

            invInput = Input.GetAxis("Inventory");
            if (invInput == 1f && lastInvInput == 0f)
            {
                state = State.Inventory;
                motor.Move(0f, 0f);
                invUI.SetActive(true);
            }
            lastInvInput = invInput;

            reloadInput = Input.GetAxis("Reload");
            if (reloadInput == 1f && lastReloadInput == 0f)
            {
                if (playerInfo.IsReloadAvailable() && !motor.GetReloadAnimatorState() && Time.time > nextReloadTime)
                {
                    nextReloadTime = Time.time + RELOAD_COOLDOWN;
                    motor.Move(0f, 0f);
                    motor.Reload();
                    playerInfo.ReloadOnce();
                }
            }
            lastReloadInput = reloadInput;

            swapInput = Input.GetAxis("Swap");
            if (swapInput != 0f && lastSwapInput == 0f && Time.time > nextSwapTime)
            {
                nextSwapTime = Time.time + SWAP_COOLDOWN;
                playerInfo.NextGun(swapInput);
            }
            lastSwapInput = swapInput;

            pauseInput = Input.GetAxis("Pause");
            if (pauseInput == 1f && lastPauseInput == 0f && Time.timeScale == 1f)
            {
                state = State.Pause;
                pauseUI.SetActive(true);
                Time.timeScale = 0f;
                motor.SetAnimatorUpdateMode(AnimatorUpdateMode.Normal);
                prevState = State.Exploring;
            }
            lastPauseInput = pauseInput;

            hudUI.GetComponent<GameUI>().ChangeInteractText(motor.GetInteractObject());
        }

        /// <summary>
        /// Handles the inventory state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        private void InventoryState()
        {
            HandleInventoryInput();
        }

        /// <summary>
        /// Handles the input within the inventory.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        private void HandleInventoryInput()
        {
            invInput = Input.GetAxis("Inventory");
            if (invInput == 1f && lastInvInput == 0f)
            {
                state = State.Exploring;
                invUI.SetActive(false);
            }
            lastInvInput = invInput;
        }

        /// <summary>
        /// Handles the input while paused.
        /// </summary>
        /// 
        /// 2021-07-25 JH Initial Documentation
        /// 
        private void HandlePauseState()
        {
            pauseInput = Input.GetAxisRaw("Pause");
            if (pauseInput == 1f && lastPauseInput == 0f)
            {
                state = prevState;
                pauseUI.SetActive(false);
                Time.timeScale = 1f;
                motor.SetAnimatorUpdateMode(AnimatorUpdateMode.UnscaledTime);
            }
            lastPauseInput = pauseInput;
        }

        /// <summary>
        /// Handles the combat state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documenation.
        /// 2021-06-10 JH Add reload and swap inputs - reference: https://docs.unity3d.com/ScriptReference/Time-time.html
        /// 2021-06-30 RB Add sprinting and blocking.
        /// 2021-07-06 JH Blocking now exhausts the player if they hit 0 energy while blocking.
        ///               Sprinting only drains energy when moving.
        /// 2021-07-25 JH Add pause input
        /// 2021-08-04 JH Add time check when pausing
        /// 
        private void HandleCombatState()
        {
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");
            sprintInput = Input.GetAxis("Sprint");
            blockInput = Input.GetAxis("Block");

            //Debug.Log("Player controller val: " + blockInput);

            // Sprinting, Blocking and moving
            if(!exhaust && blockInput > 0f)
            {
                motor.SetOverallMaxMove(maxBlockSpeed);
                motor.SetBlockingAnimation(true);
                if (playerInfo.currentEnergy <= 0)
                {
                    if (lastExhaust != null)
                    {
                        StopCoroutine(lastExhaust);
                    }
                    lastExhaust = Exhaustion();
                    StartCoroutine(lastExhaust);
                }
            }
            else if (!exhaust && playerInfo.currentEnergy > 0 && sprintInput == 1f && (xInput != 0 || zInput != 0))
            {
                motor.SetBlockingAnimation(false);
                motor.SetOverallMaxMove(maxOverallSpeed);
                motor.SetCombatAnimation(false);
                playerInfo.AddEnergy(-sprintDecreaseRate * Time.deltaTime);
                if (playerInfo.currentEnergy <= 0)
                {
                    if (lastExhaust != null)
                    {
                        StopCoroutine(lastExhaust);
                    }
                    lastExhaust = Exhaustion();
                    StartCoroutine(lastExhaust);
                }
            }
            else
            {
                motor.SetBlockingAnimation(false);
                motor.SetCombatAnimation(true);
                motor.SetOverallMaxMove(maxCombatSpeed);
            }
            motor.Move(xInput, zInput);

            // Reloading
            reloadInput = Input.GetAxis("Reload");
            if (reloadInput == 1f && lastReloadInput == 0f && !motor.GetReloadAnimatorState())
            {
                if (playerInfo.IsReloadAvailable())
                {
                    motor.Move(0f, 0f);
                    motor.Reload();
                    playerInfo.ReloadOnce();
                }
            }
            lastReloadInput = reloadInput;

            // Weapon Swap
            swapInput = Input.GetAxis("Swap");
            if (swapInput != 0f && lastSwapInput == 0f && Time.time > nextSwapTime)
            {
                nextSwapTime += SWAP_COOLDOWN;
                playerInfo.NextGun(swapInput);
            }
            lastSwapInput = swapInput;

            pauseInput = Input.GetAxis("Pause");
            if (pauseInput == 1f && lastPauseInput == 0f && Time.timeScale == 1f)
            {
                state = State.Pause;
                pauseUI.SetActive(true);
                Time.timeScale = 0f;
                motor.SetAnimatorUpdateMode(AnimatorUpdateMode.Normal);
                prevState = State.Combat;
            }
            lastPauseInput = pauseInput;
        }

        /// <summary>
        /// Exhausts the player for a set amount of time.
        /// </summary>
        /// <returns>yield to wait a few seconds before recovering from exhaust</returns>
        /// 
        /// 2021-06-30 RB Initial Documentation.
        /// 2021-07-06 JH Add HUD color changes upon exhausting and recovering from exhaust.
        /// 
        private IEnumerator Exhaustion()
        {
            exhaust = true;
            hudUI.GetComponent<PlayerHUD>().SetEnergyExhaust(true);
            yield return new WaitForSeconds(5f);
            exhaust = false;
            hudUI.GetComponent<PlayerHUD>().SetEnergyExhaust(false);
        }

        // THIS IS PURELY FOR DEBUGGING THE INTERACTIONS BOX CAST.
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = interactPoint.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 1.8f, 0.5f));
            //Gizmos.DrawWireSphere(interactPoint.position, 1f);
        }

        /// <summary>
        /// Sets the player into the combat state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public void SetCombatState()
        {
            state = State.Combat;
            motor.SetCombatAnimation(true);
            motor.SetOverallMaxMove(maxCombatSpeed);
            actionUI.SetActive(true);
        }

        /// <summary>
        /// Sets the player into the exploring state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public void SetExploringState()
        {
            state = State.Exploring;
            motor.SetCombatAnimation(false);
            motor.SetOverallMaxMove(maxOverallSpeed);
            actionUI.SetActive(false);
        }

        /// <summary>
        /// Gets and sets the player state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public State PlayerState
        {
            set
            {
                state = value;
            }

            get
            {
                return state;
            }
        }

        /// <summary>
        /// Sets the current state to the previous state.
        /// Previous state limited to Exploring and Combat States.
        /// </summary>
        /// 
        /// 2021-07-25 JH Initial Documentation
        /// 
        public void SetToPreviousState()
        {
            state = prevState;
        }

        /// <summary>
        /// Gets the CharacterMotor.
        /// </summary>
        /// <returns>The motor.</returns>
        /// 
        /// 2021-06-04 RB Initial Documentation
        /// 
        public CharacterMotor GetMotor()
        {
            return motor;
        }

        /// <summary>
        /// Gets the PlayerMotor
        /// </summary>
        /// <returns>PlayerMotor</returns>
        /// 
        /// 2021-07-29 JH Initial Documentation
        /// 
        public PlayerMotor GetPlayerMotor()
        {
            return motor;
        }

        /// <summary>
        /// Gets the boolean for exhaustion
        /// </summary>
        /// <returns>true if exhausted, false otherwise</returns>
        /// 
        /// 2021-07-29 JH Initial Documentation
        /// 
        public bool GetExhaustion()
        {
            return exhaust;
        }

        /// <summary>
        /// Toggles the player between full control and no control
        /// </summary>
        /// 
        /// 2021-05-21  TH  Initial Implementation
        /// 2021-06-02  TH  Character now goes idle when toggling the Cutscene state
        /// 
        public void ToggleState()
        {
            if (state != State.Cutscene)
            {
                state = State.Cutscene;
                GetComponent<Animator>().SetFloat("Speed", 0.0f);
            }
            else
                state = State.Exploring;           
        }

        /// <summary>
        /// Gets the input of the block input.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-06-30 RB Initial Documentation
        /// 
        public float GetBlockInput()
        {
            return blockInput;
        }

        /// <summary>
        /// Gets the blocking animation bool.
        /// </summary>
        /// <returns>true if blocking animation bool is on, false otherwise.</returns>
        /// 
        /// 2021-07-06 JH Initial Documentation.
        /// 
        public bool GetBlockingAnimation()
        {
            return motor.animator.GetBool("Block");
        }

        /// <summary>
        /// Gets health.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public int GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Gets Energy.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public int GetEnergy()
        {
            return energy;
        }
    }
}

