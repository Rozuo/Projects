using Action.Info.Imbues;
using Pool.ElectricParticle;
using Pool.ElectricParticle2;
using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using Unit.Info.Player;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Contains the effect of an electric imbuement.
/// It stuns the target for a brief duration and
/// chains damage to nearby electrified units
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
public class ImbueElectricEffect : ImbueEffect<ImbueElectricData>
{
    /// <summary>
    /// Constructor for this effect
    /// </summary>
    /// <param name="uI">unit info to apply effect to</param>
    /// <param name="iD">data for electric imbue effect</param>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 2021-06-08  JH  Add imbuement type initialization
    /// 
    public ImbueElectricEffect(UnitInfo uI, ImbueElectricData iD) : base(uI, iD)
    {
        imbueType = ImbueType.Electric;
    }

    /// <summary>
    /// Applies the electric effects on the target.
    /// Shock damage will chain to enemies nearby that are also shocked.
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-03  JH  Add base effect and isApplied boolean
    /// 2021-06-10  JH  Add particle application & fixed checking for
    ///                 nearby shocked enemies
    /// 2021-06-12  JH  Adjusted to use object pooling for particles
    /// 2021-06-18  JH  Take damage now uses the no animation version
    /// 2021-06-25  JH  Now spreads to player if they are shocked,
    ///                 doesn't hit themselves, and uses animation hit
    /// 2021-07-26  JH  Player case now checks for layer instead of tag.
    /// 2021-08-04  JH  Effect moved to End. Instead, it keeps a stack count.
    /// 
    public override void ApplyEffects()
    {
        base.ApplyEffects();
        target.TakeDamageNoAnimation(data.damageToAdd);
        if (!isApplied)
            stacks = 1;
        else
            stacks += 1;

        if (!particleEffect)
            switch (target.GetUnitType())
            {
                case UnitType.Enemy:
                    particleEffect = ElectricParticlePool.SetToObject(target.gameObject);
                    break;
                case UnitType.Environment:
                    particleEffect = ElectricParticlePool2.SetToObject(target.gameObject);
                    break;
            }
        isApplied = true;
    }

    /// <summary>
    /// Ends the shock effect
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-03  JH  Reset isApplied boolean
    /// 2021-06-10  JH  Add particle destruction
    /// 2021-06-12  JH  Adjusted to use object pooling for particles
    /// 2021-08-04  JH  Added shock AoE damage effect.
    /// 
    public override void End()
    {
        switch (target.GetUnitType())
        {
            case UnitType.Enemy:
                int enemyMask = 1 << 10;
                int environmentalMask = 1 << 12;
                int finalMask = enemyMask | environmentalMask;
                Collider[] hitColliders = Physics.OverlapSphere(target.gameObject.transform.position, data.chainRange, finalMask);
                foreach (Collider c in hitColliders)
                {
                    UnitInfo uI;
                    if (c.gameObject.tag == "Enemy" || c.gameObject.tag == "Environmental")
                    {
                        uI = c.GetComponent<UnitInfo>();
                        uI.TakeDamage(data.chainDamage * stacks);
                    }
                }
                ElectricParticlePool.ReturnToPool(particleEffect);
                break;
            case UnitType.Environment:
                ElectricParticlePool2.ReturnToPool(particleEffect);
                break;
        }
        particleEffect = null; // remove reference
        isApplied = false;
        return;
    }
    /// <summary>
    /// Effects that occur during a tick of the imbue effect
    /// Ends effect when the time alive is greater than
    /// the buff duration
    /// </summary>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 
    public override void Tick()
    {
        base.Tick();
        if (timeAlive >= data.buffDuration)
            End();
    }
}
