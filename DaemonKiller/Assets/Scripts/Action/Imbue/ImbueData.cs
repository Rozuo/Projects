using Action.Info.Imbues;
using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Abstract class that contains generic information for an imbuement
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public Vars         Description
/// damageToAdd         the amount of damage to add when imbuing a bullet
/// debuffDuration      status effect application duration
/// isStackable         determines if the buff is stackable
/// imbueType           the type of imbuement for the imbue effect
/// 
public abstract class ImbueData : ScriptableObject
{
    [Header("Base Damage on application")]
    public float damageToAdd;
    [Header("Base Duration of Buff")]
    public float buffDuration; 
    [Header("Determines if buff can be stacked")]
    public bool isStackable;
    [Header("Type of Imbuement")]
    public ImbueType imbueType;
}
