using Action.Info;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.Info;
using Audio;
using Unit.Info.Player;

namespace Action.Info.Items
{
    /// <summary>
    /// Contains all information about an action for an item
    /// 
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars         Description
    /// actionValue2        action value to represent energy recovery 
    /// item                Scriptable object of the item
    /// 
    public class ItemActionInfo : ActionInfo
    {
        [Header("Energy Value (if applicable)")]
        public float actionValue2;
        [Header("Item Scriptable Object")]
        public InventoryItem item;

        /// <summary>
        /// Initializes action info data
        /// </summary>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Removed some hard coding, change values on game object as needed
        /// 
        public override void SetUpAction()
        {
            base.SetUpAction();
            actionType = ActionType.Item;
            SetActionName(item.itemName);
            actionValue = item.healAmount;
            actionValue2 = item.energyAmount;
            switch (item.itemID)
            {
                case (Item.sup_medkit):
                    actionDescription = "Heal <color=lime>" + 
                                            actionValue + " HP</color> immediately.";
                    break;
                case (Item.sup_grenade):
                    actionDescription = "Deals <color=red>" + actionValue + 
                                            " damage</color> immediately.";
                    break;
                case (Item.sup_energydrink):
                    actionDescription = "Recovers <color=yellow>" + actionValue2 +
                                            " ENRG</color> immediately.";
                    break;
            }
        }
        /// <summary>
        /// Activates the behaviour of the action.
        /// For items, they may heal the player, recover energy, or
        /// damage the enemy. Also updates the inventory and
        /// the player HUD
        /// </summary>
        /// <param name="target">the target's unit information</param>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Add virtual base code
        /// 2021-06-16  JH  Adjusted for enum flags
        /// 2021-06-29  JH  Add target checking
        /// 2021-06-30  JH  Now applies particle if it exists
        /// 2021-07-21  RB  Sound added.
        /// 
        public override bool CommenceAction(UnitInfo target)
        {
            if (target == null || target.currentHealth <= 0)
            {
                Debug.Log("Target is null or dead for action: " + actionName);
                return false;
            }
            if (particle != null)
                ApplyParticle(target);
            if (targetType.HasFlag(UnitType.Enemy | UnitType.Environment))
            {
                target.TakeDamage(actionValue);
            }
            else if (targetType.HasFlag(UnitType.Player))
            {
                PlayerInfo pl = target.GetComponent<PlayerInfo>();
                if (target.currentHealth >= target.maxHealth && actionValue2 == 0)
                {
                    // health recovery
                    Debug.Log("Health is at maximum, failed to use");
                    return false;
                }
                else if (pl && pl.currentEnergy >= pl.maxEnergy && actionValue == 0)
                {
                    // energy recovery
                    Debug.Log("Energy is at maximum, failed to use");
                    return false;
                }
                else if (target.currentHealth >= target.maxHealth && pl && pl.currentEnergy >= pl.maxEnergy)
                {
                    // recovers both health and energy
                    Debug.Log("Both resources at maximum, failed to use");
                    return false;
                }
                switch (item.itemID)
                {
                    case (Item.sup_medkit):
                        SoundManager.PlaySound(SoundManager.Sounds.MedKit);
                        break;
                    case (Item.sup_energydrink):
                        SoundManager.PlaySound(SoundManager.Sounds.MonsterEnergyDrink);
                        break;
                }
                target.HealHealth(actionValue);
                pl.AddEnergy(actionValue2);
                playerHUD.UpdateHealth();
                playerHUD.UpdateEnergy();
            }
            // Update inventory
            inventory.Use(1, item);

            // Update HUDs
            base.CommenceAction(target);
            return true;
        }
        /// <summary>
        /// Determines if an action is available for an item
        /// Item count must be greater than 0 to be used
        /// </summary>
        /// <returns>true if gun can be used, false otherwise</returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-06-08  JH  Add case for healing items
        /// 2021-06-16  JH  Adjusted for enum flags
        /// 
        public override bool ActionAvailable()
        {
            if (targetType.HasFlag(UnitType.Player))
            {
                if (actionValue2 == 0 && playerHUD.player.currentHealth >= playerHUD.player.maxHealth)
                {
                    // do not show healing items when max hp
                    return false;
                }
                else if (actionValue == 0 && playerHUD.player.currentEnergy >= playerHUD.player.maxEnergy)
                {
                    // do not show energy items when max energy
                    return false;
                }
                else if (playerHUD.player.currentHealth >= playerHUD.player.maxHealth && playerHUD.player.currentEnergy >= playerHUD.player.maxEnergy)
                {
                    // do not show items that recover both when both are full
                    return false;
                }
                else if (inventory.Count(item) > 0)
                {
                    // checks if item in inventory and has quantity
                    return true;
                }
            }
            else if (inventory.Count(item) > 0)
            {
                // checks if item in inventory and has quantity
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the string information for an item action info object
        /// </summary>
        /// <returns>string to print out</returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public override string ToString()
        {
            return "x" + inventory.Count(item).ToString() + " " + actionName;
        }
    }

}
