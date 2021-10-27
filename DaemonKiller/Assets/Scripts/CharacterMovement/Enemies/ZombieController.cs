using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterMovement.AI;
using UnityEngine.AI;
using Audio;

namespace CharacterMovement.Enemies
{
    /// <summary>
    /// The zombie controller is an extension of the enemy controller and is used to control the zombie.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Private Variables: 
    /// motor               The zombie motor.
    /// rightAttackPoint    The transform of the right attack point.
    /// 
    public class ZombieController : EnemyController
    {

        private ZombieMotor zombieMotor;

        [Header("Attacks")]
        [SerializeField]
        private Transform rightAttackPoint;
        
        /// <summary>
        /// Set initial variables.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation.
        /// 2021-07-16 RB Add sounds for the zombie.
        /// 
        protected override void Awake()
        {
            base.Awake();
            hurtSounds = SoundManager.Sounds.ZombieHurt;
            deathSounds = SoundManager.Sounds.ZombieDeath;
            patrolSounds = SoundManager.Sounds.ZombiePatrol;
            attackSounds = SoundManager.Sounds.ZombieAttack;
            maxXMove = 1f;
            maxZMove = 1f;
            maxOverallMove = 1f;
            maxChaseTime = 4f;
            attackPower = 5;
            attackRange = 0.95f;
            waitTime = 2.5f;
            gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            target = GameObject.FindGameObjectWithTag("Player");
            zombieMotor = new ZombieMotor(transform, GetComponent<Rigidbody>(), GetComponent<Animator>(), GetComponent<NavMeshAgent>(),
                maxXMove, maxZMove, maxOverallMove, deathAnimationName, attackPoint, attackPower, attackAnimationName, 
                damageAnimationName, rightAttackPoint);
            base.motor = zombieMotor;
            //motor.Move(cPoints[currentIndex].position);
        }

        /// <summary>
        /// Spawn the left hand hit box.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation.
        /// 
        public void attackHitBoxLeft()
        {
            zombieMotor.AttackLeftHitBox();
        }

        /// <summary>
        /// Spawn the right hand hit box.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation.
        /// 2021-07-16 RB Add sounds
        /// 
        public void attackHitBoxRight()
        {
            SoundManager.PlaySound(GetAttackSound(), transform.position);
            zombieMotor.AttackRightHitBox();
        }

        /// <summary>
        /// Sets the attack animation to false when zombie is done attacking.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation.
        /// 
        //public override void RecoveryAttackFrame()
        //{
        //    motor.SetAttackAnimation(false);
        //}

        /// <summary>
        /// Returns motor
        /// </summary>
        /// <returns></returns>
        public override EnemyMotor GetMotor()
        {
            return motor;
        }

        //public override void Die()
        //{
        //    motor.Die();
        //    gm.RemoveEnemyCombat(enemyNum, this);
        //    state = State.Dead;
        //}

        /// <summary>
        /// DEBUG ONLY
        /// </summary>
        public override void OnDrawGizmos()
        {
            //base.OnDrawGizmos();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 0.15f);

            Gizmos.DrawWireSphere(rightAttackPoint.position, 0.15f);
        }
    }
}

