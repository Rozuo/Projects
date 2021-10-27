using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.Info.Player;
using UnityEngine.AI;

namespace CharacterMovement.Enemies
{
    /// <summary>
    /// Zombie motor is responsible for moving and handling attacks of the zombie. It is an extension of the EnemyMotor.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Private Variables:
    /// rightAttackPoint        The transform of the right attack point.
    /// playerLayerMask         The layer mask of the player.
    /// 
    public class ZombieMotor : EnemyMotor
    {
        /// <summary>
        /// NOTE: COULD POSSIBLY EXTEND THE AI PORTION TO IT'S OWN SCRIPT TO MAKE CODE MORE ROBUST
        /// </summary>
        private Transform rightAttackPoint;
        private float attackRadius;
        private LayerMask playerLayerMask = 1 << LayerMask.NameToLayer("Player");

        private float damageTime = 1.5f;
        private float currentTime;

        public ZombieMotor(Transform transform, Rigidbody rigidbody, Animator animator, NavMeshAgent agent,
            float maxXMove, float maxZMove, float overallMaxMove, string deathAnimationString, Transform leftAttackPoint, 
            int attackPower, string attackAnimationName, string damageAnimationName, Transform rightAttackPoint) : 
            base(transform, rigidbody, animator, agent, maxXMove, maxZMove, overallMaxMove, deathAnimationString,
                leftAttackPoint, attackPower, attackAnimationName, damageAnimationName)
        {
            this.rightAttackPoint = rightAttackPoint;
            agent.speed = overallMaxMove;
        }

        /// <summary>
        /// Move the zombie in a direction.
        /// </summary>
        /// <param name="xSpeed">X direction.</param>
        /// <param name="zSpeed">Z direction.</param>
        /// 
        /// 2021-06-04 Initial Documentation.
        /// 
        public override void Move(float xSpeed, float zSpeed)
        {
            base.Move(xSpeed, zSpeed);
        }

        /// <summary>
        /// The attack hit box of the zombies left hand.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation
        /// 2021-07-16 RB Bug fix with radius
        /// 
        public void AttackLeftHitBox()
        {
            Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 0.5f, playerLayerMask);
            //Debug.Log("left: " + colliders[0].name);
            if (colliders.Length != 0)
            {
                colliders[0].GetComponentInParent<PlayerInfo>().TakeDamage(attackPower);
            }
        }

        /// <summary>
        /// The attack hit box of the zombies right hand.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation
        /// 
        public void AttackRightHitBox()
        {
            Collider[] colliders = Physics.OverlapSphere(rightAttackPoint.position, 0.5f, playerLayerMask);
            //Debug.Log("right: " + colliders[0].name);
            if (colliders.Length != 0)
            {
                colliders[0].GetComponentInParent<PlayerInfo>().TakeDamage(attackPower);
            }
        }

        /// <summary>
        /// does nothing instead of performing it's base function.
        /// </summary>
        /// <param name="value">bool value.</param>
        /// 
        /// 2021-06-04 RB Initial Documentation.
        /// 
        public override void SetDamageAnimation(bool value)
        {
            return;
        }
    }
}

