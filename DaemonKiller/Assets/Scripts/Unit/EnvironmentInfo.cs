using Action.Info.Imbues;
using EnvironmentEffect;
using System.Collections;
using UnityEngine;

namespace Unit.Info.Environment
{
    /// <summary>
    /// Holds the environment unit information, based on their scriptable object, UnitData
    /// </summary>
    /// 
    /// Ensure the environmnet layer is applied to these objects
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// effect          effect to apply on a condition
    /// onHit           determines if effect is applied on taking damage
    /// onDeath         determines if effect is applied on death
    /// onImbue         determines if effect is applied on imbuement
    /// imbueCondition  type of imbuement for onImbueEffect
    /// permParticle    makes particle applied permanently on object
    /// timedParticle   makes partice on the object timed (for onImbue)
    /// particleTimer   timer for particle to disappear
    /// activeParticle  determines if particle with correct ImbueType is on
    /// effectUsed      determines if effect has been used
    /// 
    /// Private Vars    Description
    /// endParticle     coroutine to end the particle
    /// deathApplied    determines if effect on death has been used
    /// lastImbueEffect last imbue effect that was applied to the object
    /// coroutine       coroutine for effect delay
    /// effectCooldown  effect cooldown for all objects to prevent infinite looping
    /// nextEffectTime  time until effect can be activated
    ///        
    public class EnvironmentInfo : UnitInfo
    {
        [Header("Effect to apply. May be left blank")]
        public Effect effect;
        [Header("Toggles for effect occurrece. (Select One)")]
        public bool onHit = false;
        public bool onDeath = false;
        public bool onImbue = false;
        [Header("Type of imbue for onImbueEffect")]
        [Tooltip("Must have onImbueEffect toggled on. Do NOT select Menu")]
        public ImbueType imbueCondition;
        [Header("Toggle permanent particle effect (for onImbue)")]
        public bool permParticle = false;
        [Header("Toggle timed (for onImbue)")]
        public bool timedParticle = false;
        [Header("Particle Active Time (for onImbue)")]
        public float particleTimer = 0;

        [HideInInspector] private Coroutine endParticle;
        [HideInInspector] public bool activeParticle = false;
        [HideInInspector] public bool effectUsed = false; // for single use effects
        private bool deathApplied = false;
        private ImbueEffect lastImbueEffect;
        private Coroutine delayCoroutine;

        private const float effectCooldown = 0.5f; // for repeated use effects
        private float nextEffectTime = 0.0f;

        /// <summary>
        /// Initializes values
        /// </summary>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        void Start()
        {
            rend = GetComponent<Renderer>();
            startColor = rend.material.color;
            maxHealth = unitData.maxHP;
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Enables bools to ensure proper behaviour on next effect.
        /// </summary>
        /// 
        /// 2021-08-04  JH  Initial Implementation
        /// 
        void OnEnable()
        {
            // remove timed particle
            if (timedParticle)
            {
                if (lastImbueEffect != null)
                    lastImbueEffect.End();
                if (endParticle != null)
                    StopCoroutine(endParticle);

                activeParticle = false;
                lastImbueEffect = null;
                endParticle = null;
            }
            // stop delayed effect
            if (delayCoroutine != null)
                StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        /// <summary>
        /// Disables bools to ensure proper behaviour on next effect
        /// </summary>
        /// 
        /// 2021-07-21  JH  Initial Implementation.
        /// 2021-08-04  JH  Now removes timed particles properly
        /// 
        void OnDisable()
        {
        }

        /// <summary>
        /// Take damage based on given value
        /// Also applies any effects on hit or on 
        /// imbue particle
        /// </summary>
        /// <param name="damage">amount of damage to take</param>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 2021-06-19  JH  Add timed particles
        /// 2021-06-20  JH  Effects now have a cooldown
        /// 2021-06-24  JH  Now stores coroutine to stop later
        ///                 Stores previous ImbueEffect to prevent null reference
        /// 2021-06-25  JH  Adds coroutine check to prevent multiple effects while
        ///                 waiting on the activation of one
        /// 
        public override void TakeDamage(float damage)
        {
            if (unitData.isDestroyable)
                currentHealth -= damage;

            if (onHit && effect)
            {
                if (!EffectSingleAndUsed() && IsOffCooldown()) 
                {
                    effectUsed = true;
                    if (delayCoroutine == null && effect.GetDelayTime() > 0)
                        delayCoroutine = StartCoroutine(EffectDelay(effect.GetDelayTime()));
                    else if (delayCoroutine == null)
                        effect.ActivateEffect(gameObject);
                }
            }

            if (onImbue && effect)
            {
                if (buffs.Count > 0)
                {
                    ImbueEffect iE = buffs[buffs.Count - 1];
                    if (lastImbueEffect != iE)
                    {
                        lastImbueEffect = iE;
                        UnsubscribeToClock();
                        if (iE.imbueType == imbueCondition && !activeParticle)
                        {
                            if (!EffectSingleAndUsed() && IsOffCooldown())
                            {
                                // right effect & first of its type
                                effectUsed = true;
                                activeParticle = true;
                                if (delayCoroutine == null && effect.GetDelayTime() > 0)
                                    delayCoroutine = StartCoroutine(EffectDelay(effect.GetDelayTime()));
                                else
                                    effect.ActivateEffect(gameObject);
                                if (permParticle)
                                    // particle is permanent
                                    return;
                                else if (timedParticle)
                                    // turn off after timer
                                    endParticle = StartCoroutine(EndActiveParticle(iE, particleTimer));
                                else
                                    // turn off particle quickly
                                    StartCoroutine(EndActiveParticle(iE, 1f));
                            }
                            else
                            {
                                // right effect, but already active, remove it immediately
                                StartCoroutine(EndParticle(iE, 0f));
                            }
                        }
                        else
                        {
                            // wrong effect or particle is already active
                            StartCoroutine(EndParticle(iE, 1f));
                        }
                    }
                }
            }
            if (unitData.isDestroyable && currentHealth <= 0)
            {
                if (onDeath)
                    if (effect != null && delayCoroutine == null && effect.GetDelayTime() > 0)
                        delayCoroutine = StartCoroutine(DeathDelay(effect.GetDelayTime()));
                    else Death();
                else Death();
            }
        }

        /// <summary>
        /// Take damage without animation
        /// </summary>
        /// <param name="damage">amount of damage to take</param>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 2021-06-25  JH  Add coroutine
        /// 2021-07-20  JH  onHit effects will now apply here (for DoTEffect)
        /// 
        public override void TakeDamageNoAnimation(float damage)
        {
            if (unitData.isDestroyable)
                currentHealth -= damage;

            if (onHit && effect)
            {
                if (!EffectSingleAndUsed() && IsOffCooldown())
                {
                    effectUsed = true;
                    if (delayCoroutine == null && effect.GetDelayTime() > 0)
                        delayCoroutine = StartCoroutine(EffectDelay(effect.GetDelayTime()));
                    else if (delayCoroutine == null)
                        effect.ActivateEffect(gameObject);
                }
            }

            if (unitData.isDestroyable && currentHealth <= 0)
                if (onDeath)
                    if (effect != null && delayCoroutine == null && effect.GetDelayTime() > 0)
                        delayCoroutine = StartCoroutine(DeathDelay(effect.GetDelayTime()));
                    else if (delayCoroutine != null) { }
                    else Death();
                else Death();
        }

        /// <summary>
        /// Unit dies and its game object is destroyed
        /// Unsubscribes to the clock and removes its buffs
        /// On death effects applied if enabled
        /// </summary>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        private void Death()
        {
            UnsubscribeToClock();
            // remove buffs - move to after animation if animation available
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                if (buffs[i].isApplied == true)
                {
                    buffs[i].End();
                }
            }
            if (onDeath && effect && !deathApplied)
            {
                if (!EffectSingleAndUsed() && IsOffCooldown())
                {
                    Debug.Log("On Death");
                    effectUsed = true;
                    deathApplied = true;
                    effect.ActivateEffect(gameObject);
                }
            }
            Destroy(gameObject);
        }


        /// <summary>
        /// IEnumerator for ending the active particle's effect
        /// on the object after a certain time.
        /// Should be stored as a coroutine endParticle
        /// </summary>
        /// <param name="iE">the imbue effect to remove</param>
        /// <param name="time">the time to remove its particle</param>
        /// <returns>IEnumerator for a coroutine</returns>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 2021-06-19  JH  Add support for timed particle
        /// 
        private IEnumerator EndActiveParticle(ImbueEffect iE, float time)
        {
            yield return new WaitForSeconds(time);
            iE.End();
            activeParticle = false;
            lastImbueEffect = null;
        }

        /// <summary>
        /// IEnumerator for ending a particle's effect
        /// on the object after a certain time
        /// </summary>
        /// <param name="iE">the imbue effect to remove</param>
        /// <param name="time">the time to remove its particle</param>
        /// <returns>IEnumerator for a coroutine</returns>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 2021-06-19  JH  Add support for timed particle
        /// 
        private IEnumerator EndParticle(ImbueEffect iE, float time)
        {
            yield return new WaitForSeconds(time);
            iE.End();
        }

        /// <summary>
        /// Stops the end particle coroutine.
        /// Used for keeping the particle on the object
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public void StopEndParticle()
        {
            if (endParticle != null)
                StopCoroutine(endParticle);
        }

        /// <summary>
        /// Determines if the effect is single used and
        /// if it has been used
        /// </summary>
        /// <returns>true if single use and used, false otherwise</returns>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        private bool EffectSingleAndUsed()
        {
            return (effect.IsSingleUse() && effectUsed);
        }

        /// <summary>
        /// Determines if the effect is off cooldown.
        /// Sets next time for effect.
        /// </summary>
        /// <returns>true if off cooldown. false otherwise</returns>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 
        private bool IsOffCooldown()
        {
            if (Time.time > nextEffectTime)
            {
                nextEffectTime = Time.time + effectCooldown;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delays the effect by the given time
        /// </summary>
        /// <param name="time">time to delay</param>
        /// <returns>IEnumerator for coroutine</returns>
        private IEnumerator EffectDelay(float time)
        {
            // make untargetable
            gameObject.layer = 0;
            yield return new WaitForSeconds(time);
            effect.ActivateEffect(gameObject);
            // make targetable
            gameObject.layer = 12;
            delayCoroutine = null;
        }
        /// <summary>
        /// Delays death by the given time
        /// </summary>
        /// <param name="time">time to delay</param>
        /// <returns>IEnumerator for coroutine</returns>
        private IEnumerator DeathDelay(float time)
        {
            gameObject.layer = 0;
            yield return new WaitForSeconds(time);
            Death();
            delayCoroutine = null;
        }
    }
}