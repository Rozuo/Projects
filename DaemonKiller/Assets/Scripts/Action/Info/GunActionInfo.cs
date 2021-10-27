using UnityEngine;
using Unit.Info;
using Unit.Info.Player;
using System.Collections;
using Audio;

namespace Action.Info.Guns
{
    /// <summary>
    /// Contains all information about an action for a gun
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Private Vars        Description
    /// gun                 Scriptable object of the gun
    /// 
    public class GunActionInfo : ActionInfo
    {
        [Header("Item Scriptable Object")]
        [SerializeField]
        private InventoryItem gun;

        /// <summary>
        /// Initializes action info data
        /// Sets up references and the description for the weapon
        /// </summary>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Damage value text changed to red. 
        ///                 Removed setting values, instead set in prefab
        /// 2021-07-30  JH  Change weapon description.
        /// 
        public override void SetUpAction()
        {
            base.SetUpAction();
            actionValue = gun.damage;
            switch (gun.itemID)
            {
                case (Item.wep_handgun):
                    actionDescription = "Attack using the currently equipped weapon." +
                        " Deals damage to the target";
                    break;
                case (Item.wep_shotgun):
                    actionDescription = "Attack using the current equipped weapon" +
                        " Deals damage to the target";
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Activates the behaviour of the action.
        /// For guns, they do damage to the enemy, update
        /// the inventory slot, HUDs and play their shooting
        /// animation
        /// </summary>
        /// <param name="target">the target's unit information</param>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Now considers imbuement of a bullet
        /// 2021-05-30  JH  Imbue effects now added to a buff list to the unit
        /// 2021-06-02  JH  Add subscribing target to a clock
        /// 2021-06-03  JH  Add applier of buffs and better referencing
        /// 2021-06-05  JH  Update to use ammo from clip rather than inventory
        /// 2021-06-08  JH  Add imbue indicator updates + total ammo
        /// 2021-06-16  JH  Adjusted for unit type enum being a flag
        /// 2021-06-18  RB  Add special afliction for the force imbuement.
        /// 2021-06-29  JH  Add target checking
        /// 2021-07-04  JH  Imbue effect now reapplies properly, in SetupEffect
        /// 2021-07-16  RB  Add sounds to guns.
        /// 
        public override bool CommenceAction(UnitInfo target)
        {
            if (target == null || target.currentHealth <= 0)
            {
                Debug.Log("Target is null or dead for action: " + actionName);
                return false;
            }
            PlayerInfo player = playerHUD.player;
            if (targetType.HasFlag(UnitType.Enemy) || targetType.HasFlag(UnitType.Environment))
            {
                if (player.NumberOfImbuements() > 0)
                {
                    // get imbue
                    ImbueEffect imbueEffect = player.GetImbuement();
                    switch (imbueEffect.imbueType)
                    {
                        case Imbues.ImbueType.Force:
                            target.ApplySpecialAffliction(0.5f);
                            SoundManager.PlaySound(SoundManager.Sounds.MagicForce, player.transform.position);
                            break;
                        case Imbues.ImbueType.Fire:
                            SoundManager.PlaySound(SoundManager.Sounds.MagicFire, player.transform.position);
                            break;
                        case Imbues.ImbueType.Ice:
                            SoundManager.PlaySound(SoundManager.Sounds.MagicIce, player.transform.position);
                            break;
                        case Imbues.ImbueType.Electric:
                            SoundManager.PlaySound(SoundManager.Sounds.MagicElectricity, player.transform.position);
                            break;
                    }
                    // apply effects of imbue
                    imbueEffect.SetupEffect(target, player);
                    // subscribe to clock
                    target.SubscribeToClock();
                }
                else if (player.NumberOfImbuements() == 0)
                {
                    SoundManager.PlaySound(SoundManager.Sounds.PlayerAttack, player.transform.position);
                }
                // Damage target
                target.TakeDamage(actionValue);

                // Update Clip
                player.UseAmmo(-1);

                // Update HUDs
                base.CommenceAction(target);
                playerHUD.UpdateAmmoMask();
                playerHUD.UpdateTotalAmmo();
                playerHUD.UpdateImbueBGPosition();
                playerHUD.UpdateImbueBGContent(player.GetCurrentClip());

                // Play shooting Animation
                player.PistolAnimation();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Determines if an action is available for a gun.
        /// If clip in current gun is greater than 0, it is available.
        /// </summary>
        /// <returns>true if gun can be used, false otherwise</returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Now considers that gun has to be equipped
        /// 2021-06-05  JH  Now checks for ammo in clip rather than inventory
        /// 
        public override bool ActionAvailable()
        {
            PlayerInfo player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
            if (player.GetCurrentClip() > 0 && player.GetGun() == gun)
            {
                return true;
            }
            return false;
        }
    }
}