using Action.Info.Imbues;
using ObserverPattern.Clock;
using Unit.Info;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// references: https://straypixels.net/statuseffects-framework/
/// <summary>
/// Class to allow polymerization without specifying a generic type
/// Imbue effect will handle effects of the imbuement
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// Public vars     Description
/// imbueType       imbuement element of the effect
/// target          unit info of the target to apply effects to
/// effectApplier   unit info of the applier of this effect
/// timeAlive       time alive of imbuement
/// stacks          amount of stacks of the buff (if applicable)
/// isApplied       determines if buff application is first applied
/// particleEffect  particle effect that it instantiates on the target
/// 
public abstract class ImbueEffect 
{
    public ImbueType imbueType;
    public UnitInfo target;
    public UnitInfo effectApplier;
    public float timeAlive = 0;
    public int stacks = 0;
    public bool isApplied = false;
    public GameObject particleEffect = null;

    /// <summary>
    /// Checks if this imbue effect is in the target's buff list
    /// </summary>
    /// <param name="target">target to check</param>
    /// <returns>true if this imbue effect is in target's buff list, false otherwise</returns>
    /// 
    /// 2021-07-04  JH  Initial Work
    /// 
    public bool CheckEffectInBuffs(UnitInfo target)
    {
        List<ImbueEffect> buffList = target.GetBuffs();
        if (buffList.Contains(this))
            return true;
        return false;
    }

    /// <summary>
    /// Reapply effects of a buff in the target's buff list
    /// </summary>
    /// <param name="target">target to check</param>
    /// 
    /// 2021-07-04  JH  Initial Work
    /// 
    public void ReapplyEffects(UnitInfo target)
    {
        if (CheckEffectInBuffs(target))
        {
            List<ImbueEffect> buffList = target.GetBuffs();
            ImbueEffect iE = buffList.Find(x => x.Equals(this));
            if (iE != null)
            {
                iE.ApplyEffects();
                return;
            }
        }
        Debug.Log("Failed to reapply, buff does not exist");
    }

    /// <summary>
    /// Sets up the imbue effect, setting the imbue's target and applier
    /// and adding this effect to the target if it is not in the target's
    /// buff list. 
    /// If this imbue effect is in the target's buff list,
    /// then reapply the effect instead.
    /// </summary>
    /// <param name="target">target to check</param>
    /// <param name="applier">applier of the effect</param>
    /// 
    /// 2021-07-04  JH  Initial Work
    /// 
    public void SetupEffect(UnitInfo target, UnitInfo applier)
    {
        if (CheckEffectInBuffs(target))
        {
            // buff of type already exist, refresh its duration and reapply effects
            ReapplyEffects(target);
            return;
        }
        // buff of type doesn't exist
        // apply effects
        target.AddBuff(this);
        SetTarget(target);
        SetApplier(applier);
        ApplyEffects();
    }

    /// <summary>
    /// Setter for the effect applier
    /// </summary>
    /// <param name="applier">applier to set to</param>
    /// 
    /// 2021-06-03  JH  Initial Work
    /// 
    public void SetApplier(UnitInfo applier)
    {
        effectApplier = applier;
    }

    /// <summary>
    /// Creates a shallow copy of the imbue effect
    /// References are copied.
    /// </summary>
    /// <returns>shallow copy of ImbueEffect</returns>
    public ImbueEffect ShallowCopy()
    {
        return (ImbueEffect)this.MemberwiseClone();
    }

    /// <summary>
    /// Applies the effect of the imbue bullet to the target
    /// </summary>
    /// 
    /// 2021-05-28  JH  Initial Work
    /// 2021-06-02  JH  Changed to virtual for polymerization
    /// 
    public virtual void ApplyEffects()
    {
        timeAlive = 0f;
    }
    
    /// <summary>
    /// Set the target of the effect
    /// </summary>
    /// <param name="newTarget">target to for the effect</param>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 
    public void SetTarget(UnitInfo newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// Ends the effect of the buff
    /// </summary>
    /// 
    /// 2021-05-30  JH  Initial Work
    /// 
    public abstract void End();

    /// <summary>
    /// Determines what happens on a tick of the buff
    /// Time alive should always go up on the tick
    /// </summary>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 
    public virtual void Tick()
    {
        timeAlive += Clock.tickTime;
    }

    /// <summary>
    /// Determines if another object is equal to this one.
    /// Checks if it is of type ImbueEffect
    /// </summary>
    /// <param name="obj">object to compare to</param>
    /// <returns>true if object is the same, false otherwise</returns>
    /// 
    /// 2021-07-04  JH  Initial Work
    /// 
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        ImbueEffect objIE = obj as ImbueEffect;
        if (objIE == null) return false;
        else return Equals(objIE);
    }

    /// <summary>
    /// Determines if another ImbueEffect is equal to this one.
    /// Checks if they hold the same imbue type.
    /// </summary>
    /// <param name="iE">other imbue effect to compare to</param>
    /// <returns>true if they hold the same imbue type, false otherwise</returns>
    /// 
    /// 2021-07-04  JH  Initial Work
    /// 
    public bool Equals(ImbueEffect iE)
    {
        if (iE == null) return false;
        return (this.imbueType == iE.imbueType);
    }

    /// <summary>
    /// Returns the hashcode of the object. Based on the imbue type
    /// </summary>
    /// <returns>hashcode of the imbue effect</returns>
    /// 
    /// 2021-07-04  JH  Initial Work
    /// 
    public override int GetHashCode()
    {
        return this.imbueType.GetHashCode();
    }
}
/// <summary>
/// Generic version extending the class above.
/// This allows data that is specific to a imbuement
/// to be referred to easier.
/// </summary>
/// <typeparam name="DataType">data specific to an imbuement</typeparam>
/// 
/// Author: Jacky Huynh (JH)
/// 
/// public vars     Description
/// data            data specific to the effect
/// 
public abstract class ImbueEffect<DataType> : ImbueEffect
{
    public DataType data;

    /// <summary>
    /// Constructor class for imbue effect of a data type
    /// Allows construcing of polymerized class
    /// </summary>
    /// <param name="uI">unit info to apply effect to</param>
    /// <param name="imbueData">data for a specific imbue effect</param>
    /// 
    /// 2021-06-02  JH  Initial Work
    /// 
    public ImbueEffect(UnitInfo uI, DataType imbueData)
    {
        target = uI;
        data = imbueData;
    }
}