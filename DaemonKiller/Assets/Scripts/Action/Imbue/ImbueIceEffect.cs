using Action.Info.Imbues;
using CharacterMovement.Enemies;
using ObserverPattern.Clock;
using Pool.IceParticle;
using Unit.Info;
using UnityEngine;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Contains the effect of an ice imbuement.
/// It slows the target for the duration.
/// If a certain amount of stacks is reached,
/// it freezes the target instead
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Private Vars    Description
/// isFrozen        boolean to determine if target is frozen
/// frozenTime      time since frozen
/// eC              enemy controller of target
/// 
public class ImbueIceEffect : ImbueEffect<ImbueIceData>
{
    private bool isFrozen;
    private float frozenTime;
    private EnemyController eC;


    /// <summary>
    /// Constructor for this effect
    /// </summary>
    /// <param name="uI">unit info to apply effect to</param>
    /// <param name="iD">data for ice imbue effect</param>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 2021-06-08  JH  Add imbuement type initialization
    /// 
    public ImbueIceEffect(UnitInfo uI, ImbueIceData iD) : base(uI, iD)
    { 
        imbueType = ImbueType.Ice;
    }

    /// <summary>
    /// Slows target when effects are applied.
    /// Each application increases the stacks
    /// At max stacks, the target freezes.
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-02  JH  Added slow and freezing mechanics
    /// 2021-06-03  JH  Add base effects and damage 
    /// 2021-06-10  JH  Add particle application
    /// 2021-06-12  JH  Adjusted to use object pooling for particles
    /// 2021-06-16  JH  Adjusted for environmental targets
    /// 2021-06-18  JH  Take damage now uses the no animation version
    /// 
    public override void ApplyEffects()
    {
        base.ApplyEffects(); // reset timer on application
        target.TakeDamageNoAnimation(data.damageToAdd);
        // works for only enemy for now
        switch (target.GetUnitType())
        {
            case UnitType.Enemy:
                eC = target.GetComponent<EnemyController>();
                if (eC)
                {
                    if (!isApplied)
                    {
                        // first application of slow
                        stacks = 1; // reset stacks
                        eC.ChangeOverallSpeed(data.slowMultiplier); // slow movement & animation by multiplier
                    }
                    else
                    {
                        stacks += 1; // increment stacks for freezing
                    }
                    if (stacks >= data.stacksToFreeze && !isFrozen)
                    {
                        // freeze if stacks acquired
                        isFrozen = true;
                        frozenTime = 0f;
                        eC.GetMotor().StopMovement();
                        eC.ChangeOverallSpeed(0f); // movement & animation to 0
                    }
                    if (!particleEffect)
                    {
                        particleEffect = IceParticlePool.SetToObject(target.gameObject);
                    }
                }
                break;
            case UnitType.Player:
            case UnitType.Environment:
                // only adds particle effect
                if (!particleEffect)
                {
                    particleEffect = IceParticlePool.SetToObject(target.gameObject);
                }
                break;
        }
        isApplied = true;
    }
    /// <summary>
    /// Effects that occur during a tick of the imbue effect
    /// Freeze effect is checked separately from the slow effect
    /// </summary>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 
    public override void Tick()
    {
        base.Tick();
        switch (target.GetUnitType())
        {
            case UnitType.Enemy:
                if (isFrozen)
                {
                    frozenTime += Clock.tickTime; // increment frozen time
                }
                if (frozenTime >= data.freezeDuration)
                {
                    // thaw once freeze time met
                    isFrozen = false;
                    frozenTime = 0f;
                    eC.ResetSpeed(); // motor to original speed
                    eC.ChangeOverallSpeed(data.slowMultiplier); // motor back to slowed speed
                }
                break;
        }
        if (timeAlive >= data.buffDuration)
        {
            End(); // end of buff, reset values
        }
    }

    /// <summary>
    /// Resets the target's move speed to default
    /// and the target's stacks
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 2021-06-02  JH  Resets more values
    /// 2021-06-10  JH  Add particle destruction
    /// 2021-06-12  JH  Adjusted to use object pooling for particles
    /// 
    public override void End()
    {
        IceParticlePool.ReturnToPool(particleEffect);
        particleEffect = null; // remove reference
        stacks = 0;
        isApplied = false;
        isFrozen = false;
        if (target.GetUnitType() == UnitType.Enemy)
        {
            eC.ResetSpeed();
            eC = null;
        }
        return;
    }
}
