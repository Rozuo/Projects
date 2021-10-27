using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CharacterMovement.Enemies;
using CharacterMovement.AI;
using Unit.Info;
using Audio;

namespace CharacterMovement.Enemies
{
    /// <summary>
    /// Crawler is an enemy of hunter type that will constantly hunt the player until they are dead.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Private Variables:
    /// crawlerMotor                The motor of the crawler
    /// huntSpeed                   Speed of hunt state
    /// pounceRange                 Range of pounce
    /// pounceForce                 The force of the pounce.
    /// hideTime                    Amount of time to hide.
    /// currTime                    Current elapsed time.
    /// waitFloorRange              Lower range of possible wait times.
    /// waitCeilRange               Upper range of possible wait times.
    /// destination                 The destination of the crawler when hiding.
    /// currentEnableKinematic      current IEnumerator that handles enableKinematic.
    /// lastSongReset               last time we reset the song.
    /// resettingSong               Are we resetting the song.
    /// audioSource                 The audio source of the crawler.
    /// 
    [RequireComponent(typeof(AudioSource))]
    public class CrawlerController : EnemyController
    {
        private CrawlerMotor crawlerMotor;

        [Header("Movement")]
        private float huntSpeed = 5f;
        private float pounceRange = 5f;
        private float pounceForce = 6f;
        private float hideTime;
        private float currTime;
        [SerializeField]
        private float waitFloorRange = 1f;
        [SerializeField]
        private float waitCeilRange = 3f;

        private float lastHitOfPlayer = 0f;

        private Vector3 destination;

        private IEnumerator currentEnableKinematic;
        private IEnumerator lastSongReset;

        //private LayerMask playerMask;

        [SerializeField]
        public bool resettingSong = false;

        private AudioSource audioSource;
        /// <summary>
        /// Initialize necessary variables.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 2021-07-17 RB Add audio source initialization.
        /// 
        protected override void Awake()
        {
            base.Awake();
            maxXMove = 5f;
            maxZMove = 5f;
            maxOverallMove = 12f;
            maxChaseTime = 6f;
            attackPower = 10;
            attackRange = 0.95f;
            waitTime = 0.5f;
            patience = 1f;
            aggro = true;
            audioSource = GetComponent<AudioSource>();
            hideTime = Random.Range(waitFloorRange, waitCeilRange);
            crawlerMotor = new CrawlerMotor(transform, GetComponent<Rigidbody>(), GetComponent<Animator>(), GetComponent<NavMeshAgent>(),
                maxXMove, maxZMove, maxOverallMove, deathAnimationName, attackPoint, attackPower, attackAnimationName,
                damageAnimationName, huntSpeed, pounceForce);
            motor = crawlerMotor;
            state = State.Hunt;
        }

        /// <summary>
        /// Disable OnEnable
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation
        /// 
        protected override void OnEnable()
        {
            return;
        }

        /// <summary>
        /// Handle the different states of the crawler.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        private protected override void FixedUpdate()
        {
            switch (state)
            {
                case State.Idle:
                    state = State.Hunt;
                    break;
                case State.Combat:
                    crawlerMotor.BaseCapSpeed();
                    HandleCombat();
                    break;
                case State.Hunt:
                    crawlerMotor.CapSpeed();
                    HandleCombat();
                    break;
                case State.Hide:
                    HandleHide();
                    break;
            }
        }

        /// <summary>
        /// Handles the hide state.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 2021-07-17 RB Add more dynamic music to the crawler when hiding.
        /// 2021-07-25 RB Minor adjustments to audio source and distance to hide.
        /// 
        private void HandleHide()
        {
            crawlerMotor.BaseCapSpeed();
            if (!resettingSong)
            {
                SoundManager.SetCameraTheme(SoundManager.Songs.RunEnd);
                if(lastSongReset != null)
                {
                    StopCoroutine(lastSongReset);
                }
                lastSongReset = SoundManager.DynamicReset(SoundManager.Songs.RunEnd);
                StartCoroutine(lastSongReset);
                resettingSong = true;
            }
            
            
            if (currTime > hideTime)
            {
                resettingSong = false;
                //StartCoroutine(lastSongReset);
                state = State.Hunt;
                currTime = 0f;
                hideTime = Random.Range(waitFloorRange, waitCeilRange);
                audioSource.Play();
                return;
            }
            Vector3 currentPosi = transform.position;
            Vector3 destinPosi = destination;
            currentPosi.y = 0f;
            destinPosi.y = 0f;
            //Vector3 dist = Vector3.Distance(transform.position, destination);
            if (Vector3.Distance(currentPosi, destinPosi) <= 0.05f)
            {
                audioSource.Stop();
                currTime += Time.deltaTime;
                crawlerMotor.StopMovement();
            } 
        }

        /// <summary>
        /// Handles the combat state.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation
        /// 2021-07-17 RB Add dynamic music and more music behavior for the crawler.
        /// 2021-08-05 RB Made crawler a bit more aggressive when player is less then pounce range.
        /// 
        private protected override void HandleCombat()
        {
            //switch (state)
            //{
            //    //case State.Combat:
            //    //    //crawlerMotor.BaseCapSpeed();
            //    //    break;
            //    case State.Hunt:
            //        //SoundManager.DynamicReset(SoundManager.Songs.RunEnd);
            //        //crawlerMotor.CapSpeed();
            //        break;
            //}

            if (aiSensor.GetGo().Count == 0 && patience > 0)
            {
                patience -= Time.deltaTime;
            }
            else if (patience <= 0)
            {
                motor.SetAttackAnimation(false);
                
                state = State.Hunt;

            }
            if (aiSensor.GetGo().Count != 0)
            {
                state = State.Combat;
                patience = maxChaseTime;
            }

            Vector3 heading = DetermineHeading(transform.position, target.transform.position);
            if ((Mathf.Abs(heading.x) > pounceRange || Mathf.Abs(heading.z) > pounceRange))
            {

                motor.SetAttackAnimation(false);
                motor.StartMovement();
                motor.Move(target.transform.position);

            }
            else if(aiSensor.GetGo().Count > 0)
            {
                motor.StopMovement();
                SoundManager.PlaySound(SoundManager.Sounds.CrawlerScream, transform.position);
                SoundManager.SetCameraTheme(SoundManager.Songs.Run);
                crawlerMotor.Pounce(heading.x, heading.z);
                if (currentEnableKinematic != null)
                {
                    StopCoroutine(currentEnableKinematic);
                }
                currentEnableKinematic = crawlerMotor.EnableKinematic(0.5f);
                StartCoroutine(currentEnableKinematic);
                state = State.Hide;
                GoFarthestControlPoint();
                if (motor.GetAgentIsStopped()) { motor.StartMovement(); }
                motor.Move(destination);
                audioSource.Stop();
            }
            else
            {
                motor.StartMovement();
                motor.Move(target.transform.position);
            }
            
        }

        /// <summary>
        /// Sets a random destination for the enemy.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        private void RandomPosition()
        {
            Vector2 random = (Random.insideUnitCircle * 1.5f);
            Vector3 randomDest = (Vector3.forward * random.y) + (Vector3.right * random.x);
            destination += transform.position + randomDest;
            NavMeshHit hit;
            NavMesh.SamplePosition(destination, out hit, 1f, 1 << LayerMask.NameToLayer("Default"));
            destination = hit.position;
        }

        /// <summary>
        /// When attack should be active called by unity animation event.
        /// </summary>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 
        public void ActiveAttack()
        {
            crawlerMotor.ActiveAttackFrame();
        }

        /// <summary>
        /// Handles all collision with the crawler.
        /// </summary>
        /// <param name="collision">object that is colliding.</param>
        /// 
        /// 2021-06-25 RB Initial Documentation.
        /// 2021-07-22 RB Modified so the crawler does not kill the player immediately
        /// 2021-07-26 RB Modified to use layers instead of tags.
        /// 
        protected override void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Crawler Hit: " +  collision.collider.name);
            //collision.collider.tag == "Player"
            if (playerMask == 1 << collision.gameObject.layer && Time.time > lastHitOfPlayer + 1f)
            {
                lastHitOfPlayer = Time.time;
                collision.collider.gameObject.GetComponentInParent<UnitInfo>().TakeDamage(30f);
            }
        }

        /// <summary>
        /// This method will get the farthest control point from the target and set it as a destination.
        /// </summary>
        /// 
        /// 2021-07-26 RB Initial Documentation.
        /// 
        private void GoFarthestControlPoint()
        {
            //Debug.Log("cpoints = " + cPoints.Length);
            if(cPoints.Length == 0)
            {
                RandomPosition();
                return;
            }

            Vector3 farthest = Vector3.zero;
            float currentPoint;
            foreach(Transform t in cPoints)
            {
                if(farthest == Vector3.zero)
                {
                    farthest = t.position;
                    continue;
                }
                currentPoint = Vector3.Distance(target.transform.position, t.position);
                Debug.Log(currentPoint);
                if(currentPoint > Vector3.Distance(target.transform.position, farthest))
                {
                    farthest = t.position;
                }
            }
            farthest.Set(farthest.x, transform.position.y, farthest.z);
            destination = farthest;
        }

        /// <summary>
        /// DEBUGGING ONLY.
        /// </summary>
        //public override void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(attackPoint.position, 0.5f);
        //}
    }
}

