using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable
{
    /// <summary>
    /// Interact script that uses UnityEvents when activated
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var          desc
    /// lockKey             InventoryItem required to activate this GameObject
    /// consumeOnUse        If lockKey will be removed from inventory when this GameObject is opened
    /// singleUse           Only allow the player to interact with this GameObject once in its lifetime
    /// isActivated         true if event has been activated, false otherwise
    /// examineCompLocked   The dialogue to play when the GameObject is Locked
    /// examineCompUnlocked The dialogue to play when the GameObject has just been Unlocked
    /// OnActivationEvent   UnityEvent system specifying what happens when this GameObject is interacted with
    /// 
    public class EventInteract: Interactables
    {
        [Header("Key Item Settings")]
        [Tooltip("The item required to use this GameObject.")]
        public InventoryItem lockKey;
        [Tooltip("If enabled, the Lock Key will be consumed when used.")]
        public bool consumeOnUse;
        [Tooltip("If enabled, this switch can only be activated once.")]
        public bool singleUse;
        [HideInInspector]
        public bool isActivated = false;

        [Header("Interactable Object Settings")]
        public ExamineInteract examineCompLocked;       // the dialogue to play when locked
        public ExamineInteract examineCompUnlocked;     // the dialogue to play when unlocked

        public UnityEvent OnActivationEvent;     

        /// <summary>
        /// Get components, set state
        /// </summary>
        /// 
        /// 2021-06-18  TH  Initial Implementation
        /// 2021-07-06  RB  Intent to improve performance
        /// 
        void Awake()
        {
            // Set state based on if lockKey is defined
            if (lockKey)
                state = State.Locked;
            else
                state = State.Open;
                 
            // Fetch components from the Game Manager
            GameObject gameObj = GameObject.FindGameObjectWithTag("Game Manager");         
            if (gameObj)
            {
                gui = gameObj.transform.Find("GameHUD").GetComponent<GameUI>();
                inventory = gameObj.GetComponent<Inventory>();
            }
            else
                Debug.LogWarning(gameObject.name + " failed to find the GameDirector.");
        }

        /// <summary>
        /// Invokes the event if already activated
        /// </summary>
        /// 
        /// 2021-07-20  JH  Initial Implementation.
        /// 
        void OnEnable()
        {
            if (isActivated)
            {
                OnActivationEvent.Invoke();
            }
        }

        /// <summary>
        /// Invokes UnityEvent or ExamineInteract dialogue depending on state
        /// </summary>
        /// <param name="gO">Not used</param>
        /// 
        /// 2021-06-18  TH  Initial Implementation
        /// 
        public override void Interact(GameObject gO)
        {        
            if (state == State.Locked) // An item must be used before the GameObject can be activated
            {
                // Unlock the Interactable if the player has the key
                if (inventory.ItemExists(lockKey))
                {
                    // set Interactable's state to Open
                    state = State.Open;

                    // activate the ExamineInteract component when unlocked, also activate UnityEvent
                    if (examineCompUnlocked)
                        examineCompUnlocked.Interact(gO);
                    OnActivationEvent.Invoke();

                    // remove the item from the inventory (if enabled)
                    if (consumeOnUse)
                        inventory.Use(1, lockKey);
                }
                else
                {
                    // activate ExamineInteract component when locked (if exists), else send UI text
                    if (examineCompLocked)
                        examineCompLocked.Interact(gO);
                    else
                        gui.ChangeTextTimed("I need the <color=lime>" + lockKey.itemName + "</color>.");
                }
            }
            else if (state == State.Open)
            {
                // activate UnityEvent
                OnActivationEvent.Invoke();
            }
            
        }

        /// <summary>
        /// Sets the Animator's activated flag to true/false. Used for EventInteract.
        /// </summary>
        /// <param name="newSetting">New boolean setting</param>
        /// 
        /// 2021-06-18  TH  Initial Implementation
        /// 2021-07-13  JH  Added isActivated bool
        /// 
        public void SetActivated(bool newSetting)
        {
            isActivated = newSetting;

            Animator anim = GetComponent<Animator>();
            if (anim)
                anim.SetBool("activated", newSetting);
            else
                Debug.LogWarning(gameObject.name + " attempted to set Animator when it didn't have one.");
        }
    }
}