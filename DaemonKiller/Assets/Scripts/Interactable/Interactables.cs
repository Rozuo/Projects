using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// Status of the Interactable
    /// </summary>
    /// 
    /// Open:   The object can be interacted with
    /// Close:  The object cannot be interacted with, and emits a message stating so in-game (default)
    /// Locked: The object needs to be unlocked with an item before it can be interacted with
    /// 
    public enum State
    {
        Open, Close, Locked
    }

    /// <summary>
    /// The interactable interface is solely for defining what an interactable object must have.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB)
    /// 
    /// Protected Variables: 
    /// state           States of an interactable object.
    /// gm              The game manager.
    /// gui             UI of the game.
    /// inventory       The inventory of the player.
    /// 
    public abstract class Interactables : MonoBehaviour
    {
        protected State state = State.Open;

        protected GameManager gm;
        protected GameUI gui;
        protected Inventory inventory;

        /// <summary>
        /// How an object will interacted with.
        /// </summary>
        /// <param name="gO">The game object that will be affected by the interaction.</param>
        /// 
        /// 2021-05-06 RB Initial Documentation
        /// 
        public abstract void Interact(GameObject gO);

        /// <summary>
        /// Get or set the states of an interactable.
        /// </summary>
        /// 
        /// 2021-07-09 RB Initial Documentation.
        /// 
        public State InteractableState
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
    }
}

