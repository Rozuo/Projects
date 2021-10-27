using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Electric imbuement data that can be generated.
/// Values can be changed by changing the scriptable
/// object's values
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public Vars     Description
/// chainDamage     damage from chain electric damage
/// chainRange      range of the chain 
/// 
[CreateAssetMenu(fileName = "ImbueElectricData", menuName = "Imbuements/Electric")]
public class ImbueElectricData : ImbueData
{
    [Header("Electric Data")]
    public float chainDamage;
    public float chainRange;
}
