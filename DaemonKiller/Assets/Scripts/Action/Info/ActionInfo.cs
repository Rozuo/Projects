using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action.Target;
using Unit.Info;
using Unit.Player;
using Action.HUDListView;
using Action.Gauge;

/// <summary>
/// Specifies the type for the action.
/// Attack : Damages a target
/// Item   : Consumable item that may heal or damage a target
/// Ability: Ability that varies, may damage, heal, or apply effects onto targets
/// Imbue  : Imbues a bullet with various effects
/// Menu   : Not exactly an action, but for the action HUD to open sub menus
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
public enum ActionType { Attack, Item, Ability, Imbue, Menu }

namespace Action.Info
{
    /// <summary>
    /// Contains all information about an action such as 
    /// costs, name, and description.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars         Description
    /// actionName          name of action
    /// actionDescription   description of action
    /// range               range of action in radius
    /// targetType          type of target for action
    /// actionType          type of action, attack, item, or ability
    /// energyCost          energy cost of action
    /// actionCost          action cost of action
    /// particle            particle to spawn on target (optional)
    /// actionValue         attack/heal amount of action
    /// 
    /// Protected Vars      
    /// playerHUD           player HUD to update
    /// actionGauge         action UI to reference
    /// inventory           player's inventory
    /// 
    public abstract class ActionInfo : MonoBehaviour
    {
        [Header("Action Description")]
        public string actionName;
        public string actionDescription;
        public float range;
        public UnitType targetType;
        public ActionType actionType; // Setup in SetUpAction in inherited class
        [Header("Cost of Action")]
        public float energyCost = 0;
        public float actionCost = 0;
        [Header("Particle to spawn on target (if applicable)")]
        public GameObject particle;
        [Header("Attack/Heal Amount")]
        public int actionValue = 0;
        protected PlayerHUD playerHUD;
        protected ActionGauge actionGauge;
        protected Inventory inventory;

        /// <summary>
        /// Initializes references to UI and HUD
        /// </summary>
        /// 
        /// 2021-05-21  JH  Initial Work
        /// 
        public virtual void SetUpAction()
        {
            playerHUD = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<PlayerHUD>();
            actionGauge = GameObject.FindGameObjectWithTag("Action UI").GetComponent<ActionGauge>();
            inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
        }

        /// <summary>
        /// Setter method for the action's name
        /// </summary>
        /// <param name="name">string to name the action</param>
        /// 
        /// 2021-05-26  JH  Initial Work
        /// 
        protected void SetActionName(string name)
        {
            actionName = name;
        }

        /// <summary>
        /// Getter method for target type of action
        /// </summary>
        /// <returns>UnitType of the action</returns>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 
        public UnitType GetTargetType()
        {
            return targetType;
        }

        /// <summary>
        /// Getter method for action description
        /// </summary>
        /// <returns>description of the action</returns>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 
        public string GetDescription()
        {
            return actionDescription;
        }

        /// <summary>
        /// Returns the range of the action
        /// </summary>
        /// <returns>range of the action</returns>
        /// 
        /// 2021-05-25  JH  Initial Work
        /// 
        public float GetRange()
        {
            return range;
        }

        /// <summary>
        /// Activates the behaviour of the action.
        /// Needs a target to do action on.
        /// </summary>
        /// <param name="target">target to commence action on</param>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 2021-05-27  JH  Changed to abstract
        /// 2021-05-28  JH  Changed from abstract to virtual. Common updates implemented
        /// 2021-06-22  JH  Particle application added
        /// 2021-06-29  JH  Add target checking
        /// 2021-06-30  JH  Particle application removed
        /// 2021-07-08  JH  Removed target checking
        /// 
        public virtual bool CommenceAction(UnitInfo target)
        {
            actionGauge.AddAction(-actionCost);
            playerHUD.player.AddEnergy(-energyCost);
            playerHUD.UpdateEnergy();
            return true;
        }
        /// <summary>
        /// Method to determine if this action is available to use in combat
        /// </summary>
        /// <returns>true if action can be used in combat, false otherwise</returns>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public abstract bool ActionAvailable();

        /// <summary>
        /// Checks the energy cost comparing to
        /// the player's current energy
        /// </summary>
        /// <returns>true if player has enough energy, false otherwise</returns>
        /// 
        /// 2021-06-04  JH  Initial Work
        /// 
        public bool CheckEnergyCost()
        {
            if (playerHUD.player.currentEnergy >= energyCost)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Instantiates particle on the target.
        /// </summary>
        /// <param name="target">target to instantiate particle on</param>
        /// 
        /// 2021-06-22  JH  Initial Work
        /// 
        protected void ApplyParticle(UnitInfo target)
        {
            if (particle != null && target != null)
                Instantiate(particle, target.transform.position, Quaternion.identity);
        }
    }
}
