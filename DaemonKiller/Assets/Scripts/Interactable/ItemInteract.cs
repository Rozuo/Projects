using System.Collections;
using System.Collections.Generic;
using Unit.Info.Player;
using Unit.Player;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// Manages any interactions involving item pickups.
    /// </summary>
    ///
    /// Author: Tyson Hoang (TH)
    ///         Rozario (Ross) Beaudin (RB)
    /// 
    /// public var      desc
    /// item            The inventory item that will be given
    /// amount          How many of an item that will be given
    /// 
    /// private var     desc
    /// playHUD         reference to the PlayerHUD component
    /// playInfo        reference to the PlayerInfo component
    /// 
    [RequireComponent(typeof(Collider))]
    public class ItemInteract : Interactables
    {
        public InventoryItem item;
        public int amount = 1;

        private PlayerHUD playHUD;
        private PlayerInfo playInfo;

        /// <summary>
        /// Do initial checks and set up component references
        /// </summary>
        /// 
        /// Author: Tyson Hoang (TH)
        /// 
        /// 2021-05-11  TH  Initial Implementation
        /// 2021-07-06  RB  Add variables to initialize
        /// 2021-07-13  JH  Changed method from Start to Awake for loading data
        /// 
        void Awake()
        {
            if(item == null)
            {
                gameObject.SetActive(false);
                Debug.LogWarning("An interactable item does not have an assigned InventoryItem!");
            }
            gui = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<GameUI>();
            inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
            playHUD = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<PlayerHUD>();
            playInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        }

        /// <summary>
        /// Add item to the inventory, if possible
        /// </summary>
        /// 
        /// Author: Tyson Hoang (TH)
        /// 
        /// 2021-05-11  TH  Initial Implementation
        /// 2021-05-25  TH  Updated to support HUD text
        ///                 GameObject is now destroyed when added to Inventory
        /// 2021-05-27  JH  Update to equip gun immediately if gun is picked up
        ///                 Also updates the HUD for ammo
        /// 2021-06-01  TH  Weapon pickup text updated for first-time pickups
        /// 2021-06-05  JH  Ammo added to clip instead of the item as needed
        /// 2021-06-08  JH  HUD updates for imbue indicators
        /// 2021-06-10  JH  Add support for changing guns. Only equip if new weapon.
        /// 
        public override void Interact(GameObject gO)
        {
            bool itemExists = inventory.ItemExists(item);

            // Add the item to the Inventory
            inventory.AddItem(amount, item);
            
            // Update player data and HUD
            if(item.itemType == ItemType.Weapon)
            {
                if (itemExists) // weapon already exists, update ammo count
                {
                    playHUD.UpdateTotalAmmo();
                    gui.ChangeTextTimed(
                        "Found " + amount.ToString() + "x <color=lime>" + item.itemName + " ammo</color>");
                }
                else // new weapon picked up, equip weapon immediately
                {             
                    playInfo.ChangeGun(item);                       // Equip the new weapon                   
                    playInfo.AddAmmoClip(new AmmoClip(item, 0));    // Add a new AmmoClip class for the weapon                  
                    playInfo.ReloadAll();                           // Reload the new weapon

                    // Update ammo HUD
                    playHUD.UpdateAmmoBar();
                    playHUD.UpdateImbueBGPosition();
                    playHUD.UpdateImbueBGContent(playInfo.GetCurrentClip());

                    gui.ChangeTextTimed(
                        "You got the <color=lime>" + item.itemName + "</color>!");
                }
            }
            else
                gui.ChangeTextTimed(
                    "Found " + amount.ToString() + "x <color=lime>" + item.itemName + "</color>");
            
            Destroy(gameObject); // If the item is successfully added, remove/inactive the item.
        }
    }
}
