using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using UnityEngine;

namespace EnvironmentEffect
{
    /// <summary>
    /// Manages the DoT effect on an environment unit.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// How to use:
    /// 1. Attach environment effect with the DoTEffect as its effect. 
    ///    Ensure a toggle for effect is on.
    /// 2. Attach DoTEffectManager onto it as well
    /// 3. Change collider to your liking for the area of DoT.
    /// 4. Enable IsTrigger for the collider
    /// 
    /// Note: The area will be unable to be targeted after enabled
    /// 
    /// Public Vars     Description
    /// tickRate        time for a tick in seconds
    /// damage          damage to apply per tick
    /// isActivated     bool for activation of effect
    /// 
    /// Private Vars    Description
    /// unitsInside     hash set of units insider the collider
    /// tickIEnumerator IEnumerator for a tick for this class
    /// 
    /// 
    [RequireComponent(typeof(Collider))]
    public class DoTEffectManager : MonoBehaviour
    {
        public float tickRate { get; set; }
        public float damagePerTick { get; set; }
        public bool isActivated { get; set; }

        private HashSet<UnitInfo> unitsInside = new HashSet<UnitInfo>();
        // public GameObject particleEffect;

        private IEnumerator tickIEnumerator;

        /// <summary>
        /// Disable script and set activation to false
        /// </summary>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 
        void Start()
        {
            this.enabled = false;
            isActivated = false;
        }

        /// <summary>
        /// Adds units into the hash set on the enter of the trigger
        /// </summary>
        /// <param name="other">other collider that enters the trigger</param>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 2021-07-26  JH  Player case updated to layer rather than tags
        /// 
        void OnTriggerEnter(Collider other)
        {
            UnitInfo uI;
            int playerLayerMask = 1 << 9;
            // player case
            if (1 << other.gameObject.layer == playerLayerMask)
                uI = GameObject.FindWithTag("Player").GetComponent<UnitInfo>();
            else
                uI = other.GetComponent<UnitInfo>();
            if (uI != null)
                unitsInside.Add(uI);
        }

        /// <summary>
        /// Removes the unit from the hashset upon leaving the trigger
        /// </summary>
        /// <param name="other">other collider that leaves the trigger</param>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 2021-07-26  JH  Player case updated to layer rather than tags
        /// 
        void OnTriggerExit(Collider other)
        {
            UnitInfo uI;
            int playerLayerMask = 1 << 9;
            // player case
            if (1 << other.gameObject.layer == playerLayerMask)
                uI = GameObject.FindWithTag("Player").GetComponent<UnitInfo>();
            else
                uI = other.GetComponent<UnitInfo>();
            if (uI != null)
                unitsInside.Remove(uI);
        }

        /// <summary>
        /// Activates the DoT effect.
        /// Also makes the object untargetable.
        /// </summary>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 2021-07-21  JH  Now takes time and starts a parallel coroutine
        /// 
        public void ActivateTick(float time = 0.0f)
        {
            // change layer to level so it is no longer targetable and doesnt block targeting
            gameObject.layer = 11;

            if (tickIEnumerator != null)
            {
                StopCoroutine(tickIEnumerator);
            }

            isActivated = true;

            tickIEnumerator = DoTTick();
            StartCoroutine(tickIEnumerator);
            if (time > 0.0f)
                StartCoroutine(Duration(time));
        }

        /// <summary>
        /// Deactivates the DoT effect.
        /// Also makes the object targetable again.
        /// </summary>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 
        public void DisableTick()
        {
            if (tickIEnumerator != null)
            {
                // change layer back to environmental so it is targetable.
                gameObject.layer = 12;

                StopCoroutine(tickIEnumerator);
                tickIEnumerator = null;
                isActivated = false;
                this.enabled = false;
            }
        }

        /// <summary>
        /// Damages all units inside the object each tick.
        /// </summary>
        /// <returns>IEnumerator for the coroutine</returns>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 2021-07-20  JH  Now checks if health > 0 when trying to tick
        /// 
        public IEnumerator DoTTick()
        {
            HashSet<UnitInfo> removeSet = new HashSet<UnitInfo>();
            while (isActivated)
            {
                removeSet.Clear();
                foreach (UnitInfo uI in unitsInside)
                {
                    if (uI != null && uI.currentHealth > 0)
                        uI.TakeDamageNoAnimation(damagePerTick);
                    if (uI == null || !uI.isActiveAndEnabled || uI.currentHealth <= 0)
                        removeSet.Add(uI);
                }
                unitsInside.ExceptWith(removeSet);

                yield return new WaitForSeconds(tickRate);
            }
        }

        /// <summary>
        /// Dsiables the DoT effect after the given time in seconds.
        /// </summary>
        /// <param name="time">time in seconds until DoT effect ends</param>
        /// <returns>IEnumerator, yields the given time</returns>
        /// 
        /// 2021-07-26  JH  Initial Documentation
        /// 
        public IEnumerator Duration(float time)
        {
            yield return new WaitForSeconds(time);
            DisableTick();
        }

        /// <summary>
        /// Disables the tick on the disable
        /// </summary>
        /// 
        /// 2021-07-26  JH  Initial Documentation
        /// 
        void OnDisable()
        {
            DisableTick();
        }
    }
}
