using Action.Info.Imbues;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unit.Info;
using UnityEngine;
//using UnityEngine.XR.WSA.Input;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Contains the effect of an force imbuement.
/// It pushes enemies away by adding force to
/// the target's rigid body.
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Private Vars    Description
/// rigidbody       rigidbody of the target
/// 
public class ImbueForceEffect : ImbueEffect<ImbueForceData>
{
    private Rigidbody rigidbody;

    /// <summary>
    /// Constructor for the force effect
    /// </summary>
    /// <param name="uI">unit info to apply effect to</param>
    /// <param name="iD">data for the force imbue effect</param>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 2021-06-08  JH  Add imbuement type initialization
    /// 
    public ImbueForceEffect(UnitInfo uI, ImbueForceData iD) : base(uI, iD)
    {
        imbueType = ImbueType.Force;
    }

    /// <summary>
    /// Pushes target away from pusher when effects are applied.
    /// Small cooldown of being pushed, controlled by tick rate.
    /// </summary>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 2021-06-16  JH  Now checks if rigidbody exists
    /// 2021-06-18  JH  Take damage now uses the no animation version
    /// 
    public override void ApplyEffects()
    {
        base.ApplyEffects();
        target.TakeDamageNoAnimation(data.damageToAdd);
        rigidbody = target.GetComponent<Rigidbody>();
        if (rigidbody)
            rigidbody.AddForce(GetPushVector() * data.force, ForceMode.Impulse);
        isApplied = true;
    }


    /// <summary>
    /// Effects that occur during a tick of the imbue effect.
    /// Ends the effect of force.
    /// </summary>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 
    public override void Tick()
    {
        base.Tick();
        if (timeAlive >= data.buffDuration)
        {
            End(); // end of buff
        }
    }

    /// <summary>
    /// End of buff. Resets application boolean
    /// </summary>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 
    public override void End()
    {
        rigidbody = null;
        isApplied = false;
        return;
    }

    /// <summary>
    /// Gets the push vector from the pusher and the target
    /// Y-Component removed, making push in the X-Z plane
    /// </summary>
    /// <returns>direction to push the target, away from the pusher</returns>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 
    private Vector3 GetPushVector()
    {
        Vector3 pushVector = (target.transform.position -
                                effectApplier.transform.position).normalized;
        pushVector = Vector3.ProjectOnPlane(pushVector, Vector3.up); // remove Y component
        return pushVector;
    }
}
