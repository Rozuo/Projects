using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using Unit.Info.Player;
using UnityEngine;
using Audio;

namespace EnvironmentEffect
{
    /// <summary>
    /// Explosion effect which blasts all objects with UnitInfo.
    /// Objects to be affected must have a Rigidbody for physics
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars Description
    /// expDamage   damage from explosion
    /// expForce    force at center of explosion
    /// expRadius   radius of explosion
    /// expLift     upward force of explosion
    /// expTime     time it takes to explode
    /// 
    [CreateAssetMenu(fileName = "NewExplosionEffect", menuName = "Effect/Explosion")]
    public class ExplosionEffect : Effect
    {
        [Header("Explosion Damage")]
        public float expDamage;
        [Header("Explosion Force")]
        public float expForce;
        [Header("Explosion Radius")]
        public float expRadius;
        [Header("Explosion Lift")]
        public float expLift;
        [Header("Explosion Time")]
        public float expTime;

        /// <summary>
        /// Activates the effect of the explosion.
        /// Applies damage
        /// </summary>
        /// <param name="gO">game object with explosion effect</param>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 2021-07-17  RB  Add explosion sound.
        /// 
        public override void ActivateEffect(GameObject gO)
        {
            base.ActivateEffect(gO);
            int enemyLayerMask = 1 << 10; // used to hit only enemies
            int environmentalLayerMask = 1 << 12; // used to hit environmental objects
            int finalLayerMask = enemyLayerMask | environmentalLayerMask;

            Collider[] hitColliders = Physics.OverlapSphere(gO.transform.position, expRadius, finalLayerMask);
            foreach (Collider c in hitColliders)
            {
                // do not hit self
                if (c.gameObject != gO)
                {
                    UnitInfo unit = c.GetComponent<UnitInfo>();
                    if (unit != null)
                    {
                        unit.TakeDamage(expDamage);
                        Rigidbody rb = c.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            unit.ApplySpecialAffliction(0.5f);
                            rb.AddExplosionForce(expForce, gO.transform.position, expRadius, expLift, ForceMode.Impulse);
                        }
                        
                    }
                }
            }
            SoundManager.PlaySound(SoundManager.Sounds.BarrelExplosion);
            // player case, since no collider
            ApplyOnPlayer(gO);
        }

        /// <summary>
        /// Explosion effect for the player case
        /// </summary>
        /// <param name="gO">game object that caused the explosion</param>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        private void ApplyOnPlayer(GameObject gO)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float distanceFromPlayer = Vector3.Distance(gO.GetComponent<Collider>().ClosestPoint(player.transform.position), player.transform.position);
            if (distanceFromPlayer <= expRadius)
            {
                PlayerInfo pInfo = player.GetComponent<PlayerInfo>();
                pInfo.TakeDamage(expDamage);
                Rigidbody pRigidbody = player.GetComponent<Rigidbody>();
                if (pRigidbody != null)
                {
                    // Disable root motion
                    pInfo.ApplyRootMotionAffliction(0.1f);
                    pRigidbody.AddExplosionForce(expForce * 1.5f, gO.transform.position, expRadius, expLift, ForceMode.VelocityChange);
                }
            }
        }

        /// <summary>
        /// Returns the range of the explosion
        /// </summary>
        /// <returns>range of explosion</returns>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        public override float GetRange()
        {
            return expRadius;
        }

        /// <summary>
        /// Returns the delay time of the explosion
        /// </summary>
        /// <returns>delay time of explosion</returns>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        public override float GetDelayTime()
        {
            return expTime;
        }
    }
}
