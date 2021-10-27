using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterMovement.AI;
using UnityEngine.AI;
using Audio;

namespace CharacterMovement.Enemies
{
    /// <summary>
    /// The enemy controller is responsible for controlling the enemy, meaning it calls other 
    /// methods related to the enemy.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// UPDATE ME
    /// 
    /// Private Variables:
    /// aggro                   Determines whether the enemy is aggro.
    /// state                   The state of the enemy.
    /// motor                   The motor of that moves the enemy.
    /// currentIndex            Index of the enemies current destination.
    /// maxXMove                Maximum x speed.
    /// maxZMove                Maximum z speed.
    /// maxOverallMove          Maximum overall speed of the enemy.
    /// aiSensor                The sensor of the AI.
    /// target                  The target of the enemy.
    /// gm                      The game manager.
    /// enemyNum                The record index within the game manager.
    /// maxChaseTime            Maximum amount of time the spider will chase the target.
    /// patience                Time since last scene the target.
    /// attackRange             The range that the enemy can attack.
    /// lastWait                The last time we waited at a destination.
    /// combatSpeedModifier     A float the is multiplied when in combat.
    /// waitTime                The amount of time to wait a destination.
    /// attackPower             The attack power of the enemy.
    /// attackAnimationName     The name of the attack animation.
    /// damageAnimationName     The damage animation name.
    /// deathAnimationName      Death animation name.
    /// patrolSounds            The sounds that the enemy makes while patrolling.
    /// hurtSounds              Sounds that are made when taking damage.
    /// deathSounds             Sound made when the enemy dies.
    /// attackSounds            The sounds made when attacking.
    /// 
    /// Public Variables: 
    /// cPoints             The control points of the enemy (The destination points of the enemy.)
    /// attackPoint         The reference to where the attack ray cast should be created.
    /// attackHitBox        The size of the attack hitbox (DEPRECATED)
    /// 
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AiSensor))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        /// <summary>
        /// The possible states of the enemy.
        /// </summary>
        /// 
        /// 2021-05-11 RB Initial documentation.
        /// 2021-05-28 RB Added dead state.
        /// 
        public enum State
        {
            Idle, Patrolling, Combat, Hide, Hunt, Dead
        }
        [SerializeField]
        protected State state = State.Idle;

        [SerializeField]
        protected bool aggro = false;
        protected EnemyMotor motor;
        private protected AiSensor aiSensor;
        protected GameObject target;
        

        protected GameManager gm;
        protected int enemyNum = -1;
        [SerializeField]
        protected Transform[] cPoints;
        protected int currentIndex = 0;

        
        private IEnumerator lastWait;

        [Header("Movement")]
        private protected float maxXMove = 1f;
        private protected float maxZMove = 1f;
        private protected float maxOverallMove = 2f;
        private protected float maxChaseTime = 3f;
        private protected float patience;
        private protected float combatSpeedModifier = 1.5f;
        private protected float waitTime = 1.5f;

        [Header("Attacks")]
        [SerializeField]
        protected GameObject attackHitBox;
        [SerializeField]
        protected Transform attackPoint;
        protected int attackPower = 10;
        [SerializeField]
        private protected string attackAnimationName;
        [SerializeField]
        private protected string damageAnimationName;
        [SerializeField]
        private protected string deathAnimationName;
        protected float attackRange = 1.0f;
        protected LayerMask playerMask;

        [Header("Sounds")]
        protected SoundManager.Sounds patrolSounds;
        protected SoundManager.Sounds hurtSounds;
        protected SoundManager.Sounds deathSounds;
        protected SoundManager.Sounds attackSounds;

        /// <summary>
        /// Finds the game manager.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial documentation.
        /// 
        protected virtual void Awake()
        {
            hurtSounds = SoundManager.Sounds.SpiderHurt;
            deathSounds = SoundManager.Sounds.SpiderDie;
            attackSounds = SoundManager.Sounds.SpiderAttack;
            patrolSounds = SoundManager.Sounds.SpiderMove;
            gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            motor = new EnemyMotor(GetComponent<Transform>(), GetComponent<Rigidbody>(), GetComponent<Animator>(), GetComponent<NavMeshAgent>(),
                maxXMove, maxZMove, maxOverallMove, deathAnimationName, attackPoint, attackPower, attackAnimationName, damageAnimationName);
            
            target = GameObject.FindGameObjectWithTag("Player");     
            if(cPoints.Length == 0)
            {
                state = State.Idle;
            }
            else
            {
                state = State.Patrolling;
            }
            playerMask = 1 << LayerMask.NameToLayer("Player");
        }

        /// <summary>
        /// Check if the Enemy should immediate aggro against the player on becoming active
        /// </summary>
        /// OnEnable method runs every time this GameObject is set to Active
        /// OnDisable is the same but only when set to Inactive
        /// 
        /// 2021-06-23  TH  Initial Implementation
        /// 
        protected virtual void OnEnable()
        {
            if (aggro)
            {
                // enemy goes right for the player
                state = State.Combat;
                motor.Move(target.transform.position);
                return;
            }
            else
            {
                // patrol around the area
                state = State.Patrolling;                
                motor.Move(cPoints[currentIndex].position);
            }
        }

        /// <summary>
        /// Initializes the necessary components of the enemy.
        /// </summary>
        /// 
        /// 2021-05-11 RB Initial documentation.
        /// 
        protected virtual void Start()
        {
            aiSensor = GetComponent<AiSensor>();
            patience = maxChaseTime;
        }

        /// <summary>
        /// Runs the necessary calls for each state.
        /// </summary>
        /// 
        /// 2021-05-11 RB Initial documentation.
        /// 2021-05-21 RB Added handling for combat.
        /// 2021-06-04 RB Adjusted for motor parameter.
        /// 
        private protected virtual void FixedUpdate()
        {
            switch (state)
            {
                case State.Idle:
                    state = State.Patrolling;
                    break;
                case State.Patrolling:
                    HandlePatrolling();
                    break;
                case State.Combat:
                    HandleCombat();
                    break;
            }   
        }


        /// <summary>
        /// Handles what the enemy will do while in the patrolling state.
        /// </summary>
        /// 
        /// 2021-05-18 RB added better heading method.
        /// 2021-06-04 RB Added motor parameter.
        /// 2021-06-18 RB Overhauled the movement of the enemy to 
        ///                 utilize the NavMesh Agent.
        /// 
        private protected virtual void HandlePatrolling()
        {
            motor.CapSpeed();
            
            Vector2 currPosi = new Vector2(transform.position.x, transform.position.z);
            Vector2 destination = new Vector2(cPoints[currentIndex].position.x, cPoints[currentIndex].position.z);
            Vector2 diff = destination - currPosi;

            // near destination
            if (Mathf.Abs(diff.x) < 1f && Mathf.Abs(diff.y) < 1f)
            {
                currentIndex += 1;
                currentIndex = currentIndex >= cPoints.Length ? 0 : currentIndex;
                motor.SetDestination(cPoints[currentIndex].position);
                motor.Move(cPoints[currentIndex].position);
                if (lastWait != null)
                {
                    StopCoroutine(lastWait);
                }

                lastWait = Wait(waitTime);
                StartCoroutine(lastWait);
            }
            if (!motor.GetAgentIsStopped())
            {
                SoundManager.PlaySound(GetPatrolSound(), transform.position);
            }

            
            if(aiSensor.gO.Count != 0)
            {
                EnterCombat(aiSensor.GetGo()[0]);
                if (lastWait != null)
                {
                    StopCoroutine(lastWait);
                }
            }
        }

        /// <summary>
        /// Handles the combat state.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial documentation.
        /// 2021-05-28 RB Added attack animation handling
        /// 2021-06-04 RB Added motor parameter.
        /// 2021-06-18 RB Overhauled the movement to utilize the NavMesh Agent. 
        ///                 And The enemy can now start out tracking the player.
        ///                 Even if the player has not been seen.
        /// 2021-06-25 RB Destination for leaving combat fixed.
        /// 2021-06-26 TH Enemies will now turn towards their target as they attack
        /// 2021-07-04 JH Enemies will now stop moving when in getting damaged animation
        /// 2021-07-07 RB Adjusted so that the enemy is more relentless in attacking.
        /// 2021-07-16 RB Add sounds.
        /// 
        private protected virtual void HandleCombat()
        {
            motor.CapSpeed();
            SoundManager.PlaySound(GetPatrolSound(), transform.position);
            if (!aggro)
            {
                //Debug.Log("nah");
                if (!target)
                {
                    state = State.Patrolling;
                    return;
                }

                if(aiSensor.GetGo().Count == 0 && patience > 0)
                {
                    patience -= Time.deltaTime;
                }
                else if(patience <= 0)
                {
                    state = State.Patrolling;
                    patience = maxChaseTime;
                    motor.SetAttackAnimation(false);
                    motor.SetOverallMaxMove(maxOverallMove);
                    //StopCoroutine(lastAttackRoutine);
                    gm.RemoveEnemyCombat(EnemyNumber, this);
                    motor.Move(cPoints[currentIndex].position);
                    return;
                }
                else
                {
                    patience = maxChaseTime;
                }
            }
            else
            {
                if (aiSensor.GetGo().Count == 0 && patience > 0)
                {
                    patience -= Time.deltaTime;
                }
                else if (patience <= 0)
                {
                    motor.SetAttackAnimation(false);
                    //motor.Move(cPoints[currentIndex].position);
                    motor.SetOverallMaxMove(maxOverallMove);
                    gm.RemoveEnemyCombat(EnemyNumber, this);
                }
                if (aiSensor.GetGo().Count != 0)
                {
                    if(enemyNum == -1)
                    {
                        gm.AddEnemyCombat(gameObject);
                    }
                    motor.SetOverallMaxMove(maxOverallMove * 1.5f);
                    
                    patience = maxChaseTime;
                }
            }
            

            Vector3 heading = DetermineHeading(transform.position, target.transform.position);
            if (!motor.GetDamageAnimatorState() && (Mathf.Abs(heading.x) > attackRange || Mathf.Abs(heading.z) > attackRange))
            {
                motor.SetAttackAnimation(false);
                motor.StartMovement();
                if(Time.time % 0.5 == 0)
                {
                    motor.Move(target.transform.position);
                }
                

            }
            else
            {
                motor.StopMovement();

                if (!motor.GetAttackAnimatorStateAttack() && !motor.GetDamageAnimatorState())
                {
                    motor.SetAttackAnimation(true);
                }
                //else if (motor.GetAttackAnimatorStateAttack())
                //{
                    motor.LookAtTarget(heading, 0.2f); //0.2f
                //}
            }
        }

        /// <summary>
        /// Wait at a position for an amount of time
        /// </summary>
        /// <param name="time">Float of time for waiting.</param>
        /// <returns></returns>
        /// 
        /// 2021-07-25  RB  Initial Documentation.
        /// 
        private IEnumerator Wait(float time)
        {
            motor.StopMovement();
            yield return new WaitForSeconds(time);
            if (state != State.Dead)
                motor.StartMovement();
        }


        /// <summary>
        /// Determine the heading of the enemy.
        /// </summary>
        /// <param name="currentPosition">The enemies current position.</param>
        /// <param name="destination">The enemies destination.</param>
        /// <returns name="heading">The vector3 heading.</returns>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        private protected Vector3 DetermineHeading(Vector3 currentPosition, Vector3 destination)
        {
            Vector3 heading = Vector3.zero;
            heading = destination - currentPosition;
            heading.y = 0;
            return heading;
        }

        /// <summary>
        /// Sets and gets the enemy number.
        /// </summary>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public int EnemyNumber
        {
            set
            {
                enemyNum = value;
            }
            get
            {
                return enemyNum;
            }
        }

        /// <summary>
        /// Returns motor
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-06-16 RB Initial documentation.
        /// 
        public virtual EnemyMotor GetMotor()
        {
            return motor;
        }

        public Transform[] cPointsAll
        {
            get
            {
                return cPoints;
            }
            set
            {
                cPoints = value;
            }
        }

        /// <summary>
        /// Draws the enemy's attackPoint
        /// </summary>
        /// 
        ///             RB  Initial Implementation
        /// 2021-06-26  TH  Added a WireSphere for attackPoint
        /// 
        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 0.42f);
            //Gizmos.DrawWireCube(attackPoint.position + transform.TransformDirection(Vector3.forward) * 0.01f, new Vector3(1f, 1f, 1f));
            //Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(Vector3.forward) * 1f, 0.2f);
            //Gizmos.DrawLine(attackPoint.transform.position, attackPoint.transform.position + transform.TransformDirection(Vector3.forward) * 0.6f);           
            
        }

        /// <summary>
        /// activate the attack collider.
        /// </summary>
        /// 
        /// 2021-05-28 intial documentation.
        /// 
        public void ActiveAttackFrame()
        {
            motor.AttackHitBox();
            SoundManager.PlaySound(GetAttackSound());
        }
        
        /// <summary>
        /// Disable the attack animation boolean.
        /// </summary>
        /// 
        /// 2021-05-28 RB Initial documentation.
        /// 
        public virtual void RecoveryAttackFrame()
        {
            this.motor.SetAttackAnimation(false);
        }

        /// <summary>
        /// If the enemy is damaged.
        /// </summary>
        /// 
        /// 2021-05-28 RB Initial documentation.
        /// 2021-07-29 RB Add sounds to the enemies.
        /// 
        public void Damaged()
        {
            if(enemyNum == -1)
            {
                EnterCombat(target);
            }
            motor.StopMovement();
            motor.SetDamageAnimation(true);
            SoundManager.PlaySound(hurtSounds);
        }

        /// <summary>
        /// Handles how the enemy dies.
        /// </summary>
        /// 
        /// 2021-05-28 RB Initial documentation.
        /// 
        public virtual void Die()
        {
            state = State.Dead;
            gm.RemoveEnemyCombat(enemyNum, this);
            motor.Die();
            
        }

        /// <summary>
        /// Set the damage boolean to false.
        /// </summary>
        /// 
        /// 2021-05-28 RB Initial documentation.
        /// 
        public void RecoveredFromDamage()
        {
            motor.StartMovement();
            motor.SetDamageAnimation(false);
        }

        /// <summary>
        /// Checks if the enemy is still playing the dying animation.
        /// </summary>
        /// <returns>Boolean</returns>
        /// 
        /// 2021-05-28 RB Initial documentation.
        /// 
        public bool DyingOnGoing()
        {
            return motor.GetDeadAnimatorState();
        }

        /// <summary>
        /// Changes the overall speed of the motor by a multiplier
        /// Also affects animaton speed
        /// </summary>
        /// <param name="speedMultiplier">multiplier for speed</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 2021-07-04  JH  Changed to work with NavMeshAgent
        /// 
        public void ChangeOverallSpeed(float speedMultiplier)
        {
            motor.animator.speed = speedMultiplier;
            motor.SlowAgentSpeed(speedMultiplier);
        }

        /// <summary>
        /// Resets the overall speed to the default maximum speed
        /// Speed differs if in combat
        /// </summary>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 2021-07-04  JH  Removed state conditions and changed for NavMeshAgent
        /// 
        public void ResetSpeed()
        {
            motor.animator.speed = 1f;
            motor.ResetAgentSpeed();
        }


        /// <summary>
        /// Puts the enemy in combat.
        /// </summary>
        /// <param name="target">The target</param>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public void EnterCombat(GameObject target)
        {
            state = State.Combat;
            if(lastWait != null)
            {
                StopCoroutine(lastWait);
            }
            this.target = target;
            motor.SetOverallMaxMove(maxOverallMove * combatSpeedModifier);
            gm.AddEnemyCombat(gameObject);
        }

        /// <summary>
        /// On initial collision enter combat.
        /// </summary>
        /// <param name="collision">The object that is colliding with enemy.</param>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 2021-07-26 RB Adjustment to use layers instead of tags.
        /// 
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if(state != State.Dead && enemyNum == -1 && 1 << collision.gameObject.layer == playerMask)
            {
                EnterCombat(collision.collider.gameObject);
            }
        }

        /// <summary>
        /// Gets the hurt sound.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-07-16  RB  Intitial documentation.
        /// 
        public SoundManager.Sounds GetHurtSounds()
        {
            return hurtSounds;
        }
        
        /// <summary>
        /// Gets the death sound.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public SoundManager.Sounds GetDieSounds()
        {
            return deathSounds;
        }

        /// <summary>
        /// Gets the attack sound.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public SoundManager.Sounds GetAttackSound()
        {
            return attackSounds;
        }

        /// <summary>
        /// Gets the patrol sound.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public SoundManager.Sounds GetPatrolSound()
        {
            return patrolSounds;
        }
    }
}

