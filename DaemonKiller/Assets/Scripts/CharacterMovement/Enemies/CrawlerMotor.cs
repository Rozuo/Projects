using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unit.Info.Player;

namespace CharacterMovement.Enemies
{
    /// <summary>
    /// The crawler motor handles how the crawler moves around.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin RB
    /// 
    /// Private Variables:
    /// huntSpeed           The speed of hunting.
    /// pounceForce         Force of the pounce.
    /// 
    public class CrawlerMotor : EnemyMotor
    {

        private float huntSpeed;
        private float pounceForce;

        /// <summary>
        /// Initialize all necessary variables.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rigidbody"></param>
        /// <param name="animator"></param>
        /// <param name="agent"></param>
        /// <param name="maxXMove"></param>
        /// <param name="maxZMove"></param>
        /// <param name="overallMaxMove"></param>
        /// <param name="deathAnimationString"></param>
        /// <param name="attackPoint"></param>
        /// <param name="attackPower"></param>
        /// <param name="attackAnimationName"></param>
        /// <param name="damageAnimationName"></param>
        /// <param name="huntSpeed"></param>
        /// <param name="pounceForce"></param>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public CrawlerMotor(Transform transform, Rigidbody rigidbody, Animator animator, NavMeshAgent agent,
                float maxXMove, float maxZMove, float overallMaxMove, string deathAnimationString, Transform attackPoint,
                int attackPower, string attackAnimationName, string damageAnimationName, float huntSpeed, float pounceForce) :
                base(transform, rigidbody, animator, agent, maxXMove, maxZMove, overallMaxMove, deathAnimationString, 
                    attackPoint, attackPower, attackAnimationName, damageAnimationName)
        {
            agent.speed = overallMaxMove;
            this.huntSpeed = huntSpeed;
            this.pounceForce = pounceForce;
        }

        /// <summary>
        /// Apply a impulse force towards a direction.
        /// </summary>
        /// <param name="xDirect">xDirection force</param>
        /// <param name="zDirect">zDirection force</param>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public void Pounce(float xDirect, float zDirect)
        {
            Vector3 direction = Vector3.forward * zDirect + Vector3.right * xDirect;
            //LookAtTarget(direction, 0.50f);
            rigidbody.isKinematic = false;
            rigidbody.AddForce(direction * pounceForce, ForceMode.Impulse);
        }

        /// <summary>
        /// Enable kinematic rigidbody.
        /// </summary>
        /// <param name="time">The time to enable kinematic.</param>
        /// <returns></returns>
        /// 
        /// 2021-06-25 RB Initial Documentation 
        /// 
        public IEnumerator EnableKinematic(float time)
        {
            yield return new WaitForSeconds(time);
            rigidbody.isKinematic = true;
        }

        /// <summary>
        /// Call the base CapSpeed.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public void BaseCapSpeed()
        {
            base.CapSpeed();
        }

        /// <summary>
        /// Cap the speed of the crawler using hunt speed.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public override void CapSpeed()
        {
            agent.velocity = SpeedCapMax(agent.velocity, huntSpeed);
            animator.SetFloat("Speed", agent.velocity.magnitude / overallMaxMove);
        }

        /// <summary>
        /// Frame where the attack hitbox is active.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public void ActiveAttackFrame()
        {
            Collider[] coll = Physics.OverlapSphere(attackPoint.position, 0.5f, playerMask);
            if (coll.Length != 0)
            {
                coll[0].GetComponentInParent<PlayerInfo>().TakeDamage(attackPower);
            }
        }

    }
}

