using Action.Info;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using Unit.Info;
using Unit.Info.Player;
using UnityEngine;

namespace Action.Info.Imbues
{
    /// <summary>
    /// Specifies the type of imbuement
    /// Fire    : Burns the enemy and deals damage over time
    /// Ice     : Slows the enemy's movement and freezes them at a certain stack
    /// Electric: Stun the target, chaining onto other targets affected
    /// Force   : Pushes a target away from the user
    /// Menu    : Not an actual type, but for sub menus
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    public enum ImbueType { Fire, Ice, Electric, Force, Menu}

    public class ImbueActionInfo : ActionInfo
    {
        /// <summary>
        /// Contains all information about an imbuement.
        /// Effects vary depending on the imbue effect
        /// </summary>
        /// 
        /// Author: Jacky Huynh (JH)
        /// 
        /// Private Vars        Description
        /// imbuement           type of imbuement for bullet 
        /// imbueData           data for the imbue effect
        /// ImbueEffect         effect's of the imbuement to apply to target
        /// 
        [Header ("Imbue Information")]
        public ImbueType imbuement;
        public ImbueData imbueData;
        private ImbueEffect imbueEffect;

        /// <summary>
        /// Initializes action info data
        /// Sets up references and description for the imbuement
        /// </summary>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 2021-06-02  JH  Creates imbue effect based on imbue selected
        /// 2021-06-03  JH  Add force and electric creation
        /// 
        public override void SetUpAction()
        {
            base.SetUpAction();
            switch (imbuement)
            {
                case (ImbueType.Fire):
                    actionDescription = "Imbue the next bullet with fire, " +
                        "allowing the bullet to inflict <color=orange>burn</color> to the enemy";
                    imbueEffect = new ImbueFireEffect(null, (ImbueFireData)imbueData);
                    break;
                case (ImbueType.Ice):
                    actionDescription = "Imbue the next bullet with ice, " +
                        "allowing the next bullet to inflict <color=aqua>slow</color> to the enemy. " +
                        "Enough stacks <color=lightblue>freeze</color> them.";
                    imbueEffect = new ImbueIceEffect(null, (ImbueIceData)imbueData);
                    break;
                case (ImbueType.Electric):
                    actionDescription = "Imbue the next bullet with electric, " +
                        "allowing the next bullet to <color=magenta>shock</color> nearby enemies after a brief delay.";
                    imbueEffect = new ImbueElectricEffect(null, (ImbueElectricData)imbueData);
                    break;
                case (ImbueType.Force):
                    actionDescription = "Imbue the next bullet with force, " +
                        "allowing the next bullet to <color=yellow>push</color> the enemy away.";
                    imbueEffect = new ImbueForceEffect(null, (ImbueForceData)imbueData);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Setter method for the imbuement type
        /// </summary>
        /// <param name="imbueType">element to imbue a bullet</param>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public void SetImbuementType(ImbueType imbueType)
        {
            imbuement = imbueType;
        }

        /// <summary>
        /// Getter method for the imbuement type
        /// </summary>
        /// <returns>imbuement type to use</returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public ImbueType GetImbuementType()
        {
            return imbuement;
        }

        /// <summary>
        /// Getter for imbue effect
        /// </summary>
        /// <returns>imbue effect to apply to target</returns>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public ImbueEffect GetImbueEffect()
        {
            return imbueEffect;
        }

        /// <summary>
        /// Activates the behaviour of the action.
        /// Adds imbuement to a queue on the player that will be used
        /// for the next bullet.
        /// Also updates the HUDs 
        /// </summary>
        /// <param name="target">the target's unit information</param>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Adds imbuement to a queue
        /// 2021-06-02  JH  Changed to add imbue effect rather than action info
        /// 2021-06-03  JH  Add energy check
        /// 2021-06-10  JH  Adds shallow copy instead of the original imbue effect
        /// 2021-06-29  JH  Add target checking
        /// 
        public override bool CommenceAction(UnitInfo target)
        {
            if (target == null || target.currentHealth <= 0)
            {
                Debug.Log("Target is null or dead for action: " + actionName);
                return false;
            }
            PlayerInfo player = playerHUD.player;
            if (player.currentEnergy >= energyCost)
            {
                // Add imbue effect to a list of imbuements (on player);
                player.AddImbuement(imbueEffect.ShallowCopy());
                // Update HUDs
                base.CommenceAction(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether imbuing a bullet is available.
        /// For imbue MENU, not the actual imbuement
        /// Available if equipped gun has ammo
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Availability is for imbuement menu now
        ///                 Assumes imbues are all unlocked 
        /// 2021-06-05  JH  Checks ammo in clip rather than the inventory
        /// 
        public override bool ActionAvailable()
        {
            PlayerInfo player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
            InventoryItem equippedGun = player.GetGun();
            if (actionType == ActionType.Menu && equippedGun)
            {
                if (player.GetCurrentClip() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
