using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Fire imbuement data that can be generated.
/// Values can be changed by changing the scriptable
/// object's values
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public Vars     Description
/// tickRate        the rate to burn the taret
/// damagePerTick   damage to burn the target per tick
/// 
[CreateAssetMenu(fileName = "ImbueFireData", menuName = "Imbuements/Fire")]
public class ImbueFireData : ImbueData
{
    [Header("Fire Data")]
    public float tickRate; // limited by clock, change Clock tick rate if needs to be quicker
    public float damagePerTick;
}
