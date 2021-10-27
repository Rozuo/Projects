using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Interactable
{
    /// <summary>
    /// The door interact class is responsible for controlling the behavior of the door when it is interacted with.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB)
    ///         Tyson Hoang (TH)
    /// 
    /// public var      desc
    /// destination     The Room this door will set active when activated.
    ///                 If this value is NULL, this door is permanently unusable in-game.
    /// doorKey         An Inventory item that must be used to unlock this door.
    ///                 If this value is NULL, the door can be used if a destination exists.
    /// consumeOnUse    If the doorKey is used upon unlocking the door.
    /// doorStuckText   The text to display if the door cannot be used ever.
    /// doorLockedText  The text to display if a door is currently locked.
    /// frontPosi       The front position of the door.
    /// backPosi        The back position of the door.
    /// 
    /// private var     desc
    /// roomManager     reference to RoomManager component
    /// camTarget       reference to CameraTarget component
    /// lastDoorPlayer  (unused)
    /// 
    [RequireComponent(typeof(Collider))]
    public class DoorInteract : Interactables
    {
        [Tooltip("If this is NULL, this door is unusable in-game.")]
        public GameObject destination;

        [Header("Door Key (if applicable)")]
        [Tooltip("If this is NULL, door is always open in-game (if destination exists).")]
        public InventoryItem doorKey;
        [Tooltip("If enabled, the Lock Key will be consumed when used.")]
        public bool consumeOnUse = true;

        [Header("Text Messages")]
        public string doorStuckText;
        public string doorLockedText;

        [Header("System Values (don't touch)")]
        public Transform frontPosi;
        public Transform backPosi;

        private RoomManager roomManager;
        private Camera.CameraTarget camTarget;
        private IEnumerator lastDoorPlayed;

        /// <summary>
        /// Initialize door state (moved from Start)
        /// </summary>
        /// 
        /// 2021-07-13  JH  Initial Implementation
        /// 2021-07-21  JH  changed from private to public virtual
        /// 
        public virtual void Awake()
        {
            // set the door's state
            if (doorKey)
                state = State.Locked;
            else if (!destination)
                state = State.Close;
        }

        /// <summary>
        /// Find references
        /// </summary>
        /// 
        /// 2021-07-13  JH  First Documentation. Setting door's state moved
        ///                 to awake.
        /// 2021-07-21  JH  changed from private to public virtual
        /// 
        public virtual void Start()
        {
            gui = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<GameUI>();
            inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
            roomManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<RoomManager>();
            camTarget = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera.CameraTarget>();
        }

        /// <summary>
        /// The interaction that the door will performed when called.
        /// </summary>
        /// <param name="gO">The game object that will be affected by the interaction.</param>
        /// 
        /// 2021-05-06  RB  Initial Documentation.
        /// 2021-05-21  TH  Moved original function to a Coroutine.
        /// 2021-05-25  TH  Added checks based on door state.
        /// 
        public override void Interact(GameObject gO)
        {          
            if(state == State.Close)
            {
                // Door will output a Permanently Closed message.
                gui.ChangeTextTimed(doorStuckText);
                return;
            }

            if(state == State.Locked)
            {
                // Check if the player current has the key.
                if (inventory.ItemExists(doorKey))
                {
                    state = State.Open;
                    if (consumeOnUse) inventory.Use(1, doorKey);
                    gui.ChangeTextTimed(
                        "Unlocked the door with the <color=lime>" + doorKey.itemName + "</color>.");
                    //SoundManager.PlaySound(SoundManager.Sounds.DoorOpen);
                    //if(lastDoorPlayed != null)
                    //{
                    //    StopCoroutine(lastDoorPlayed);
                    //}
                    //lastDoorPlayed = SoundManager.DelayPlaySound(2f, SoundManager.Sounds.DoorClose);
                    //StartCoroutine(lastDoorPlayed);
                }
                else
                {
                    // Door is currently locked.
                    gui.ChangeTextTimed(
                        doorLockedText + " I need the <color=lime>" + doorKey.itemName + "</color>.");
                    //SoundManager.PlaySound(SoundManager.Sounds.DoorLock);
                }
            }
            else
            {
                // Door is unlocked, and can be used.
                StartCoroutine(DurationTimer(gO));
            }          
        }

        /// <summary>
        /// Beings a Door Transition using Unity Timeline
        /// </summary>
        /// <param name="gO">The GameObject that will be affected by the interaction</param>
        /// <returns>Time duration before the interaction is performed</returns>
        /// 
        /// 2021-05-21  TH  Initial Implementation
        /// 
        private IEnumerator DurationTimer(GameObject gO)
        {           
            // fade the screen out temporarily
            gui.InvokeFade();    
            yield return new WaitForSeconds(1.0f);

            // activate the destination room
            roomManager.SetRoomActive(destination, true);
            
            // TODO: method only needs the frontPosi, remove backPosi
            // Do the Ross code
            Vector3 diff = Vector3.forward + Vector3.right;
            if (Vector3.Distance(gO.transform.position, frontPosi.position) > Vector3.Distance(gO.transform.position, backPosi.position))
            {
                diff = Vector3.Scale(diff, frontPosi.position);
                diff.y = gO.transform.position.y;
                gO.transform.position = diff;
            }
            else if (Vector3.Distance(gO.transform.position, frontPosi.position) < Vector3.Distance(gO.transform.position, backPosi.position))
            {
                diff = Vector3.Scale(diff, backPosi.position);
                diff.y = gO.transform.position.y;
                gO.transform.position = diff;
            }

            camTarget.InstantMove(); // instantly focus the camera to the player
            roomManager.SetRoomActive(roomManager.CurrentRoom, false); // deactivate the previous room
            roomManager.CurrentRoom = destination; // update the current room the player is in
        }
    }
}
