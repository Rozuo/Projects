using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Interactable
{
    /// <summary>
    /// Manages the switch for animators
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    ///         Rozario (Ross) Beaudin (RB)
    /// 
    /// public var      desc
    /// singleUse       If this interaction can only be used once
    /// animObj         Animator Component
    /// lockKey         InventoryItem required to activate this GameObject
    /// consumeOnUse    If lockKey will be removed from inventory when this interaction is activated
    /// isActivated     true if switch has been activated, false otherwise
    /// theType         Switch's interaction type (see Type enum for details)
    /// 
    [RequireComponent(typeof(Animator))]
    public class SwitchInteract : Interactables
    {

        /// <summary>
        /// The type of interaction this switch will do
        /// </summary>
        /// 
        /// slideDoor: Interaction is for sliding doors
        /// 
        public enum Type
        {
            slideDoor,
        }

        [Tooltip("If enabled, this switch can only be activated once.")]
        public bool singleUse;
        private Animator animObj;

        [Header("Object Key (if applicable)")]
        public InventoryItem lockKey;
        [Tooltip("If enabled, the Lock Key will be consumed when used.")] 
        public bool consumeOnUse;
        [HideInInspector]
        public bool isActivated = false;

        public Type theType = Type.slideDoor;

        /// <summary>
        /// Set state of switch and get its animator
        /// </summary>
        /// 
        /// 2021-07-14  JH  Initial Implementation
        /// 
        void Awake()
        {
            if (lockKey) // automatically "lock" the interaction if a key is defined
                state = State.Locked;

            // fetch the Animator component
            animObj = GetComponent<Animator>();
            if (!animObj)
                Debug.LogError(gameObject.name + " is missing an Animator Component! Please add one!");
        }

        /// <summary>
        /// Check components
        /// </summary>
        /// 
        /// 2021-06-01  TH  Initial Implementation
        /// 2021-06-02  TH  Added lockKey check
        /// 2021-07-06  RB  Add initialization for inventory and gui.
        /// 2021-07-14  JH  Moved state and animator to Awake
        /// 
        void Start()
        {
            inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
            gui = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<GameUI>();
        }

        /// <summary>
        /// Plays animation for the object on enable. This prevents the switches from closing
        /// when saving or moving between rooms.
        /// </summary>
        /// 
        /// 2021-07-14  JH  Initial Implementation.
        /// 
        void OnEnable()
        {
            if (animObj != null) SetActivated(isActivated);
        }

        /// <summary>
        /// Activate/Deactivate animation
        /// </summary>
        /// <param name="gO">not used</param>
        /// 
        /// 2021-06-01  TH  Initial Implementation
        /// 2021-06-02  TH  Now uses the lock system like DoorInteract.cs
        /// 2021-07-06  RB  Utilizes gui and inventory variables with the intent of performance improvements.
        /// 2021-07-21  RB  Sounds added.
        /// 
        public override void Interact(GameObject gO)
        {
            if (singleUse && animObj.GetBool("activated")) 
                return; //do not proceed if switch is already activated

            if(state == State.Locked)
            {
                // An item must be used before the GameObject can be activated.
                if (inventory.ItemExists(lockKey)) // player has the lock key, "unlock" the switch
                {
                    // unlock interaction
                    SetActivated(!animObj.GetBool("activated"));
                    state = State.Open;

                    if (consumeOnUse) inventory.Use(1, lockKey); // consume key (if enabled)

                    gui.ChangeTextTimed("Used the <color=lime>" + lockKey.itemName + "</color>.");

                    switch (theType)
                    {
                        case (Type.slideDoor):
                            SoundManager.PlaySound(SoundManager.Sounds.SlideGate);
                            break;
                    }
                }
                else // notify player they need the lock key
                    gui.ChangeTextTimed("Need the <color=lime>" + lockKey.itemName + "</color>.");
            }
            else if (state == State.Open)
            {
                SetActivated(!animObj.GetBool("activated"));
            }
            else
                Debug.LogError(gameObject.name + " has reached a weird outside case!");
        }

        /// <summary>
        /// Sets the Animator's activated flag to true/false. Used for EventInteract.
        /// </summary>
        /// <param name="newSetting">New boolean setting</param>
        /// 
        /// 2021-06-18  TH  Initial Implementation
        /// 2021-07-14  JH  Added isActivated bool for saving data purposes
        /// 
        public void SetActivated(bool newSetting)
        {
            isActivated = newSetting;

            animObj.SetBool("activated", newSetting);
        }
    }
}
