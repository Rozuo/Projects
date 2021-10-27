using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data for specific Items for the Inventory
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// 
/// Public Vars     Description
/// itemID          item's unique ID
/// itemName        The human-readable name of the Item
/// itemType        The type of Item (Weapon, Supply, or Keys)
/// itemIcon        The 2D image representing the Item
/// itemLimit       The maximum possible amount that can be carried at one time (unused)
/// itemPrefab      The 3D Prefab representing the Item (unused)
/// 
/// maxMag          (Weapon Only) The weapon's maximum capacity before having to reload
/// ammoIcon        (Weapon Only) The weapon's ammo sprite (used for the Ammo bar)
/// damage          (Weapon Only) The weapon's damage
/// 
/// healAmount      (Supply Only) How much health this will restore when used
/// energyAmount    (Supply Only) How much energy this will restore when used
/// 
[CreateAssetMenu(fileName = "NewInventoryObject", menuName = "Inventory/Item")]
[System.Serializable]
public class InventoryItem : ScriptableObject
{
    [Header ("Universal Settings")]
    public Item itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public int itemLimit;
    public GameObject itemPrefab;

    [Header("Weapon Settings")]
    public int maxMag;
    public Sprite ammoIcon;
    public int damage;

    [Header("Supply Settings")]
    public int healAmount;
    public int energyAmount;
}

/// <summary>
/// Specifies the type for this item
/// Weapon: Used for attacking enemies (ex. Firearms, melee)
/// Supply: A consumable item (ex. healing)
/// Special: Story-specific items (ex. keys)
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// 
public enum ItemType { Weapon, Supply, Special }

/// <summary>
/// A numerical identifier for items that's used in other functions
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// 
public enum Item
{
    wep_handgun,
    wep_shotgun,
    sup_medkit,
    sup_energydrink,
    sup_grenade,
    key_book,
    key_chapelA,
    key_chapelB,
    key_chapelC,
    key_garden1,
    key_gate1,
    key_gate2,
    key_gate3,
    key_gate4,
    key_gate5,
    ket_gate6,
    total
}