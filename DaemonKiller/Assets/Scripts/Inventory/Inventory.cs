using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unit.Player;
using Unit.Info.Player;
using Audio;

/// <summary>
/// The System responsible for managing the Player's Inventory
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
///         Jacky Huynh (JH)
/// 
/// public var          desc
/// inventory           list of InventorySlot structs that holds inventory data
/// updateInventory     UnityEvent that refreshes the Inventory UI         
/// giveStartingGear    UnityEvent specifically for the first level that gives items and activates text
/// 
/// private var         desc
/// playerInfo          Reference to the Player's PlayerInfo component
/// playerHud           Reference to the GameHUD's PlayerHUD component
/// gameUi              Reference to GameUI component
/// 
[System.Serializable]
public class Inventory : MonoBehaviour
{
    [HideInInspector] public List<InventorySlot> inventory;
    public UnityEvent updateInventory;
    public UnityEvent giveStartingGear;
    private PlayerInfo playerInfo;
    private PlayerHUD playerHud;
    private GameUI gameUi;

    /// <summary>
    /// Initialize the inventory List
    /// </summary>
    /// 
    /// 2021-05-03  TH  Initializes the Inventory List
    /// 2021-06-27  TH  Added component references
    /// 
    private void Awake()
    {
        inventory = new List<InventorySlot>();
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        playerHud = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<PlayerHUD>();
        gameUi = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<GameUI>();
    }

    /// <summary>
    /// Returns the amount of an Item currently in the Inventory.
    /// </summary>
    /// <param name="itm">The Item to look for.</param>
    /// <returns>Current amount in the Inventory.</returns>
    /// 
    /// 2021-05-03  TH  Checks for existing item, then the amount
    ///
    public int Count(InventoryItem itm)
    {
        try
        {
            return inventory.Find(x => x.itemDetails.itemName.Equals(itm.itemName)).GetAmount();
        }
        catch
        {
            return -1; //return negative if the item doesn't exist
        }

    }

    /// <summary>
    /// Checks if an item exists in the inventory.
    /// </summary>
    /// <param name="itm">The Inventory Item to look for</param>
    /// <returns>boolean: if the item exists</returns>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 
    public bool ItemExists(InventoryItem itm)
    {
        return inventory.Exists(x => x.itemDetails.itemName == itm.itemName);
    }

    /// <summary>
    /// Adds an item to the Inventory, or an amount to an existing item.
    /// </summary>
    /// <param name="addedAmount">Amount to add (must be greater than 0)</param>
    /// <param name="itm">The Item being added</param>
    /// 
    /// 2021-05-03  TH  Adds an item to the List, or modifies an existing item with updated value
    /// 2021-06-11  JH  Add exception for weapons
    ///
    public void AddItem(int addedAmount, InventoryItem itm)
    {
        if ((addedAmount < 1 && itm.itemType != ItemType.Weapon) || itm == null)
            return; // don't do anything if the amount is less than 1 or item parameter doesn't exist

        if (!ItemExists(itm))
        {            
            inventory.Add(new InventorySlot(itm, addedAmount));
        }
        else
        {         
            // creates a new struct with updated data, then replaces it in the list
            int index = inventory.FindIndex(x => x.itemDetails.itemName.Equals(itm.itemName));
            InventorySlot newitm = inventory[index];
            newitm.AddAmount(addedAmount);
            inventory[index] = newitm;
        }

        updateInventory.Invoke();
    }

    /// <summary>
    /// Remove an amount to an existing item. Removes the item if the amount is 0 afterwards.
    /// </summary>
    /// <param name="usedAmount">Amount to use (must be greater than 0 and less than currently held)</param>
    /// <param name="itm">The Item being used</param>
    /// 
    /// 2021-05-03  TH  Modifies an existing item with updated value, or removes item from the list
    /// 2021-05-27  JH  Weapon count reaching zero does not remove it from the inventory
    /// 
    public void Use(int usedAmount, InventoryItem itm)
    {
        if (usedAmount < 1 || itm == null)
            return; // don't do anything if amount is less than 1      

        if (ItemExists(itm))
        {
            // creates a new struct with updated data, then replaces it in the list
            int index = inventory.FindIndex(x => x.itemDetails.itemName.Equals(itm.itemName));
            InventorySlot newitm = inventory[index];
            newitm.AddAmount(-usedAmount);
            inventory[index] = newitm;

            // If the item stack is now 0 or lower, remove the item from the Inventory (if not a weapon)
            if (inventory[index].GetAmount() < 1 && itm.itemType != ItemType.Weapon)
            {
                inventory.RemoveAt(index);
            }
        }

        updateInventory.Invoke();
    }

    /// <summary>
    /// Uses an item at an index in the inventory
    /// </summary>
    /// <param name="index">Location of the item being used</param>
    /// 
    /// 2021-05-03  TH  Initial Documentation
    /// 2021-05-25  TH  Integrated healing items with the PlayerInfo and PlayerHUD components
    /// 2021-05-28  TH  Add support for equipping different weapons
    /// 2021-07-21  RB  Sounds added for energy drink and med kits.
    /// 2021-07-22  JH  Add support for energy drink
    /// 2021-07-30  JH  Using guns will now use a new method, ChangeGun
    ///
    public void UseItem(int index)
    {
        if (!HasInventoryItem(index)) 
            return; // index out of range, do not proceed

        InventoryItem itm = inventory[index].itemDetails;
        //Debug.Log("Item was used at index " + index);

        switch(itm.itemID)
        {
            case Item.sup_medkit:
                if (playerInfo.CurrentHealth > playerInfo.maxHealth - 1)
                {
                    gameUi.ChangeTextTimed("Your health is full.");
                    break;
                }
                playerInfo.HealHealth(itm.healAmount);
                playerHud.GetComponent<PlayerHUD>().UpdateHealth();
                Use(1, inventory[index].itemDetails);
                SoundManager.PlaySound(SoundManager.Sounds.MedKit);
                break;
            case Item.sup_energydrink:
                if (playerInfo.currentEnergy > playerInfo.maxEnergy - 1)
                {
                    gameUi.ChangeTextTimed("Your energy is full.");
                    break;
                }
                playerInfo.currentEnergy = Mathf.Min(playerInfo.currentEnergy + itm.energyAmount, playerInfo.maxEnergy);
                playerHud.GetComponent<PlayerHUD>().UpdateEnergy();
                Use(1, inventory[index].itemDetails);
                SoundManager.PlaySound(SoundManager.Sounds.MonsterEnergyDrink);
                break;
            case Item.wep_handgun:
            case Item.wep_shotgun:
                playerInfo.ChangeGun(itm);
                break;           
            default:
                Debug.Log("This item doesn't have a usage yet!");
                break;
        }
    }

    /// <summary>
    /// Returns the InventorySlot struct at an index in the inventory
    /// </summary>
    /// <param name="index">Location of the item being fetched</param>
    /// <returns>InventorySlot</returns>
    /// 
    /// 2021-05-11  TH  Initial Documentation
    /// 
    public InventorySlot GetInvDetails(int index)
    {
        return inventory[index];
    }

    /// <summary>
    /// Returns if an item exists at an index
    /// </summary>
    /// <param name="index">Location of the item being fetched</param>
    /// <returns>Boolean</returns>
    /// 
    /// 2021-05-11  TH  Initial Documentation
    /// 
    public bool HasInventoryItem(int index)
    {
        return index < inventory.Count;
    }
}

/// <summary>
/// Structure containing information for the inventory item.
/// </summary>
/// 
/// Public Vars     Description
/// itemDetails     the inventory item for the slot
/// itemID          the item's ID
/// 
/// Private Vars    Description
/// amount          amount of the item for the inventory slot
/// 
/// 2021-05-03  TH  Added
/// 2021-07-20  JH  ItemID is also saved into the struct.
/// 
[System.Serializable]
public struct InventorySlot
{
    public InventoryItem itemDetails;
    public Item itemID;
    [SerializeField] private int amount;

    /// <summary>
    /// Constructor of the inventory struct
    /// </summary>
    /// <param name="itm">item for the inventory slot</param>
    /// <param name="number">initial amount of the item</param>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public InventorySlot (InventoryItem itm, int number)
    {
        itemDetails = itm;
        itemID = itm.itemID;
        amount = number;
    }

    /// <summary>
    /// Adds the amount to the inventory slot
    /// </summary>
    /// <param name="number">number to add to the inventory slot</param>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public void AddAmount (int number)
    {
        amount += number;      
    }

    /// <summary>
    /// Gets the amount of the inventory slot
    /// </summary>
    /// <returns>amount of the inventory slot</returns>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public int GetAmount()
    {
        return amount;
    }
}

/// <summary>
/// Structure containing information for an ammo clip
/// </summary>
/// 
/// Public Vars     Description
/// gunDetails      the inventory item for the gun
/// gunID           the gun's ID
/// 
/// Private Vars    Description
/// ammo            amount of ammo in the ammo clip for the gun
/// 
/// 2021-06-05  JH  Initial Work
/// 2021-07-20  JH  gunID is also saved into the struct
/// 
[System.Serializable]
public struct AmmoClip
{
    public InventoryItem gunDetails;
    public Item gunID;
    [SerializeField] private int ammo;

    /// <summary>
    /// Constructor for ammo clip
    /// </summary>
    /// <param name="gun">gun to store clip</param>
    /// <param name="number">ammo of the clip</param>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public AmmoClip (InventoryItem gun, int number)
    {
        gunDetails = gun;
        gunID = gun.itemID;
        ammo = number;
    }

    /// <summary>
    /// Add amount of ammo to clip
    /// </summary>
    /// <param name="number">amount of ammo to add</param>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public void AddAmmo(int number)
    {
        ammo = ammo + number;
    }

    /// <summary>
    /// Gets the amount of ammo in the clip
    /// </summary>
    /// <returns></returns>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public int GetAmount()
    {
        return ammo;
    }

    /// <summary>
    /// Checks if the clip is empty 
    /// </summary>
    /// <returns>true if clip is empty, false otherwise</returns>
    /// 
    /// 2021-08-05  JH  Initial Documentation
    /// 
    public bool IsEmpty()
    {
        if (ammo == 0)
        {
            return true;
        }
        return false;
    }
}
