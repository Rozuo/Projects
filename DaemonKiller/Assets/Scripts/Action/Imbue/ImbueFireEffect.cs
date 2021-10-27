using Action.Info.Imbues;
using ObserverPattern.Clock;
using Pool.FireParticle;
using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using UnityEngine;
//using UnityEngine.XR.WSA.Input;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Contains the effect of a fire imbuement.
/// It burns the target over time a damage per
/// tick.
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public Vars     Description
/// activeTime      time buff is up
/// 
public class ImbueFireEffect : ImbueEffect<ImbueFireData>
{
    private float tickTime = 0.0f;

    /// <summary>
    /// Constructor for this effect
    /// </summary>
    /// <param name="uI">unit info to apply effect to</param>
    /// <param name="iD">data for fire imbue effect</param>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 2021-06-08  JH  Add imbuement type initialization
    /// 2021-06-20  JH  Add active time initialization
    /// 
    public ImbueFireEffect(UnitInfo uI, ImbueFireData iD) : base(uI, iD) 
    {
        tickTime = 0.0f;
        imbueType = ImbueType.Fire;
    }

    /// <summary>
    /// Applies the burn effect to the target
    /// Refreshes duration rather than stacking
    /// When a new source of burn is applied
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-02  JH  Changed to remove monobehaviour functions and using observer clock
    /// 2021-06-10  JH  Add particle application
    /// 2021-06-12  JH  Adjusted to use object pooling for particles
    /// 2021-06-18  JH  Take damage now uses the no animation version
    /// 
    public override void ApplyEffects()
    {
        base.ApplyEffects();
        target.TakeDamageNoAnimation(data.damageToAdd);
        if (!particleEffect)
        {
            particleEffect = FireParticlePool.SetToObject(target.gameObject);
        }
        isApplied = true;
    }

    /// <summary>
    /// Ends the burn effect
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-02  JH  Changed to remove monobehaviour functions
    /// 2021-06-10  JH  Add particle destruction
    /// 2021-06-12  JH  Adjusted to use object pooling for particles
    /// 
    public override void End()
    {
        FireParticlePool.ReturnToPool(particleEffect);
        tickTime = 0f;
        particleEffect = null; // remove reference
        isApplied = false;
        return;
    }

    /// <summary>
    /// Effects that occur during a tick of the imbue effect
    /// Burns the target for a set amount of damage per tick
    /// Ends effect when time alive is greater than the buff duration
    /// </summary>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 2021-06-20  JH  Now tick is based on its scriptable object rather than the clock.
    ///                 However, still limited by the clock's tick rate.
    /// 2021-06-21  JH  Tick time is now added properly
    /// 
    public override void Tick()
    {
        base.Tick();
        tickTime += Clock.tickTime;
        if (tickTime >= data.tickRate)
        {
            tickTime = 0f;
            BurnEffect();
        }
        if (timeAlive >= data.buffDuration)
            End();
    }

    /// <summary>
    /// Applies damage to enemy based on damage per tick
    /// Used in InvokeRepeating to simulate burn effect
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-04  JH  No longer flinches enemy per burn tick
    /// 2021-06-16  JH  Adjusted to consider environment objects
    /// 2021-06-21  JH  Flag checking is adjusted properly
    /// 
    public void BurnEffect()
    {
        if (!target)
        {
            return;
        }

        if (target.GetUnitType() == UnitType.Enemy 
            || target.GetUnitType() == UnitType.Player
            || target.GetUnitType() == UnitType.Environment)
            target.TakeDamageNoAnimation(data.damagePerTick);
    }
}
