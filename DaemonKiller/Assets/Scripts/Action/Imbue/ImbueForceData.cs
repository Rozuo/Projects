using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Force imbuement data that can be generated.
/// Values can be changed by changing the scriptable
/// object's values.
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public Vars     Description
/// force           amount of force to push the target away
/// 
[CreateAssetMenu(fileName = "ImbueForceData", menuName = "Imbuements/Force")]
public class ImbueForceData : ImbueData
{
    [Header("Force Data")]
    public float force;
}
