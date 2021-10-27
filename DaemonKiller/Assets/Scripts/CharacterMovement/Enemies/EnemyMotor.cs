using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.Info.Player;
using UnityEngine.AI;

namespace CharacterMovement.Enemies
{
    /// <summary>
    /// Enemy motor controls how the enemy moves around the environment.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Private Variables:
    /// 
    /// attackPoint             The point that we spawn the attack collider.
    /// attackPointSize         The size of the attack.
    /// attackCooldown          The cooldown of the attack.
    /// currentCooldown         The current cooldown.
    /// playerMask              Layer mask of the player.
    /// attackAnimationName     Name of the attack animation.
    /// damageAnimationName     Name of the damage animation.
    /// agent                   NavMesh agent of the enemy.
    /// agentBaseSoeed          The agents base speed.
    /// 
    public class EnemyMotor : CharacterMotor
    {
        private Vector3 destination = Vector3.zero;

        private protected Transform attackPoint;
        //private Vector3 attackPointSize = new Vector3(1f, 1f, 1f);
        //private LayerMask ignoreMask = LayerMask.NameToLayer("Default");
        protected LayerMask playerMask = 1 << LayerMask.NameToLayer("Player");
        protected float attackCooldown = 1f;
        protected float currentCooldown;
        protected bool attackOnGoing = false;
        //private Collider attackHitBox;
        protected int attackPower;

        protected string attackAnimationName;
        protected string damageAnimationName;

        protected NavMeshAgent agent;
        protected float agentBaseSpeed;

        /// <summary>
        /// Initialize the neccessary variables.
        /// </summary>
        /// <param name="transform">Enemy transform</param>
        /// <param name="rigidbody">Enemy rigidbody</param>
        /// <param name="animator">Enemy animator</param>
        /// <param name="maxXMove">Enemy maxXMove</param>
        /// <param name="maxZMove">Enemy maxZMove</param>
        /// <param name="overallMaxMove">Enemy overallMaxMove</param>
        /// 
        /// 2021-05-11 RB Initial documentation.
        /// 
        public EnemyMotor(Transform transform, Rigidbody rigidbody, Animator animator, NavMeshAgent agent,
            float maxXMove, float maxZMove, float overallMaxMove, string deathAnimationString, Transform attackPoint, 
            int attackPower, string attackAnimationName, string damageAnimationName) : 
            base(transform, rigidbody, animator, maxXMove, maxZMove, overallMaxMove, deathAnimationString)
        {
            this.attackPoint = attackPoint;
            currentCooldown = 0;
            this.attackPower = attackPower;
            this.attackAnimationName = attackAnimationName;
            this.damageAnimationName = damageAnimationName;
            this.agent = agent;
            agentBaseSpeed = agent.speed;
            //this.attackHitBox = attackHitBox.GetComponent<Collider>();
            //this.attackHitBox.enabled = false;
        }

        /// <summary>
        /// Moves our enemy in a specified x direction and z direction.
        /// </summary>
        /// <param name="xSpeed">Movement on the x axis.</param>
        /// <param name="zSpeed">Movement on the z axis.</param>
        /// 
        /// 2021-05-11 RB Initial documentation.
        /// 2021-05-28 RB Added extra condition to bool so the enemy doesn't move if damaged.
        /// 
        public override void Move(float xSpeed, float zSpeed)
        {
            if (animator.GetBool("Attack") || GetDamageAnimatorState())
                return;
            //Debug.Log("yep");
            Vector3 speed = (Vector3.right * xSpeed) + (Vector3.forward * zSpeed);

            speed = SpeedCapMax(speed, overallMaxMove);

            if (speed.magnitude != 0f)
                LookAtTarget(speed, 0.20f);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(speed), 0.20f);

            if (rigidbody.velocity.magnitude < overallMaxMove)
                rigidbody.AddForce(speed, ForceMode.VelocityChange);

            animator.SetFloat("Speed", speed.magnitude / overallMaxMove);
        }

        /// <summary>
        /// Move the enemy using a vector3 destination. 
        /// </summary>
        /// <param name="destination">The location of the destination we are moving to.</param>
        /// 
        /// 2021-06-18 RB Initial Documentation.
        /// 
        public void Move(Vector3 destination)
        {
            agent.destination = destination;
        }

        /// <summary>
        /// Cap the movement speed of the agent.
        /// </summary>
        /// 
        /// 2021-06-18 RB Initial Documentation
        /// 
        public virtual void CapSpeed()
        {
            agent.velocity = SpeedCapMax(agent.velocity, overallMaxMove);
            animator.SetFloat("Speed", agent.velocity.magnitude / overallMaxMove);
        }

        /// <summary>
        /// Stop the agents movements.
        /// </summary>
        /// 
        /// 2021-06-18 RB   Initial Documentation.
        /// 2021-06-26 TH   Velocity now goes to 0, fixing agents 
        ///                 sliding while attacking
        /// 
        public void StopMovement()
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        /// <summary>
        /// Start the agents movements.
        /// </summary>
        /// 
        /// 2021-06-18 RB Initial Documentation.
        /// 
        public void StartMovement()
        {
            agent.isStopped = false;
        }

        /// <summary>
        /// Returns true if the agent has been stopped.
        /// </summary>
        /// <returns>boolean of if the agent has been stopped.</returns>
        /// 
        /// 2021-06-18 RB Initial Documentation.
        /// 
        public bool GetAgentIsStopped()
        {
            return agent.isStopped;
        }

        /// <summary>
        /// Sets angular speed.
        /// </summary>
        /// <param name="speed">Desired speed.</param>
        /// 
        /// 2021-06-25 RB Initial Documentation
        /// 
        public void SetAngularSpeed(float speed)
        {
            agent.angularSpeed = speed;
        }

        /// <summary>
        /// Gets and sets the agents enabled value.
        /// </summary>
        /// 
        /// 2021-06-18 RB Initial Documentation.
        /// 
        public bool IsAgentActive
        {
            set
            {
                agent.enabled = value;
            }

            get
            {
                return agent.enabled;
            }
        }

        /// <summary>
        /// Clamps speed to the max.
        /// </summary>
        /// <param name="speed">The current speed.</param>
        /// <returns></returns>
        /// 
        /// 2021-06-09 RB Initial documentation
        /// 
        public Vector3 SpeedCapMax(Vector3 speed, float maxMove)
        {
            if (speed.magnitude > maxMove)
                speed = speed.normalized * maxMove;
            return speed;
        }

        /// <summary>
        /// Rotate the enemy to look at a direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        /// 
        /// 2021-06-18 RB Initial Documentation.
        /// 
        public void LookAtTarget(Vector3 direction, float lookSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), lookSpeed);
        }

        /// <summary>
        /// Sends an attack in directly in front of the enemy.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public void Attack()
        {
            if(currentCooldown >= attackCooldown && !GetAttackAnimatorStateAttack())
            {
                animator.SetBool("Attack", true);
            }
            else
            {
                if(currentCooldown >= attackCooldown)
                {
                    animator.SetBool("Attack", false);
                }
                currentCooldown += Time.deltaTime;
                //if (currentCooldown >= attackCooldown * 0.1f)
                //{
                //AttackHitBox();
                //}
            }
            
        }

        /// <summary>
        /// Sets the attack animation bool in the animator.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public virtual void SetAttackAnimation(bool value)
        {
            animator.SetBool("Attack", value);
        }

        /// <summary>
        /// Returns true if the animator is playing the attack animation.
        /// </summary>
        /// <returns>Boolean</returns>
        /// 
        /// 2021-05-28 RB Initial Documentation
        /// 
        public bool GetAttackAnimatorStateAttack()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName);
        }

        /// <summary>
        /// Returns true if the animator is playing the damage animation.
        /// </summary>
        /// <returns>Boolean</returns>
        /// 
        /// 2021-05-28 RB Initial Documentation
        /// 
        public bool GetDamageAnimatorState()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnimationName);
        }

        /// <summary>
        /// Returns true if the animator is playing the death animation.
        /// </summary>
        /// <returns>Boolean</returns>
        /// 
        /// 2021-05-28 RB Initial Documentation
        /// 
        public bool GetDeadAnimatorState()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("die");
        }

        /// <summary>
        /// POSSIBLY DEPRECATED 
        /// </summary>
        /// <returns></returns>
        public IEnumerator AttackRoutine()
        {
            //Debug.Log("yuh");
            attackOnGoing = true;
            animator.SetBool("Attack", true);
            yield return new WaitForSeconds(1f);
            //Debug.Log("Fire");
            AttackHitBox();
            yield return new WaitForSeconds(1f);
            animator.SetBool("Attack", false);
            attackOnGoing = false;
        }

        /// <summary>
        /// Spawns a small box collider that will damage the player.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 2021-05-28 RB Added raycast instead of boxcast/sphere cast as it was more consistent.
        /// 2021-06-26 TH Uses the method from ZombieController.cs now
        /// 
        public void AttackHitBox()
        {
            Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 0.42f, playerMask);
            foreach (Collider other in colliders)
            {
                // Enemy found a target in their collider, deal damage to them
                other.GetComponentInParent<PlayerInfo>().TakeDamage(attackPower);
            }
        }

        /// <summary>
        /// The enemy dies and stops all agent movements.
        /// </summary>
        /// 
        /// 2021-06-18 RB Initial Documentation.
        /// 
        public override void Die()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(deathAnimationString))
            {
                agent.isStopped = true;
                animator.SetTrigger("Dead");
            }
        }
        
        /// <summary>
        /// Resets the agent speed to default values
        /// </summary>
        /// 
        /// 2021-07-04  JH  Initial Work
        /// 
        public void ResetAgentSpeed()
        {
            agent.speed = agentBaseSpeed;
        }

        /// <summary>
        /// Decreases the agents speed by the multiplier given
        /// </summary>
        /// <param name="speedMultiplier">multiplier for agent's speed, speed multiplier >= 0</param>
        /// 
        /// 2021-07-04  JH  Initial Work
        /// 
        public void SlowAgentSpeed(float speedMultiplier)
        {
            if (speedMultiplier < 0)
                return;
            agent.speed = agentBaseSpeed * speedMultiplier;
        }

        /// <summary>
        /// Sets the overallMaxMove.
        /// </summary>
        /// <param name="overallMaxMove">The new value of the overallMaxMove.</param>
        /// 
        /// 2021-05-21 RB Initial documentation.
        /// 
        public void SetOverallMaxMove(float overallMaxMove)
        {
            this.overallMaxMove = overallMaxMove;
        }

        /// <summary>
        /// Sets the destination of the enemy.
        /// </summary>
        /// <param name="destination">The new destination of the enemy.</param>
        /// 
        /// 2021-05-21 RB Initial documentation.
        /// 
        public void SetDestination(Vector3 destination)
        {
            this.destination = destination;
        }

        /// <summary>
        /// Returns the bool attack on going.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-05-28 RB Initial Documentation.
        /// 
        public bool GetAttackOnGoing()
        {
            return attackOnGoing;
        }

    }
}

