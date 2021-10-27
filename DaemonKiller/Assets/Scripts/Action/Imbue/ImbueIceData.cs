using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Ice imbuement data that can be generated.
/// Values can be changed by changing the scriptable
/// object's values
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public Vars     Description
/// slowMultiplier  slow factor multiplier
/// stacksToFreeze  number of stacks to freeze unit
/// freezeDuration  duration of freeze
/// 
[CreateAssetMenu(fileName = "ImbueIceData", menuName = "Imbuements/Ice")]
public class ImbueIceData : ImbueData
{
    [Header("Ice Data")]
    public float slowMultiplier;
    public int stacksToFreeze;
    public float freezeDuration;
}
