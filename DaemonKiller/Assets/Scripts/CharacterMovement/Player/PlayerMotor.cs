using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;

namespace CharacterMovement.Player{
    /// <summary>
    /// The player motor is responsible for performing any physic related changes or actions that the player can take.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Private Variables:
    /// interactPoint       The point where the boxcast for interactions will based off of.
    /// stepRayLower        The minimum height for an object to be climbed over.
    /// stepRayUpper        The maximum height for an object to be climbed over.
    /// stepHeight          How high the character can step up.
    /// stepSmooth          The step height of the character.
    /// reloadAnimationName string of the reload animation name
    /// gO                  The gameObject of the player.
    /// 
    public class PlayerMotor : CharacterMotor
    {
        private Vector3 previousSpeed;
        private Vector3 interactBoxSize = new Vector3(1f, 1.8f, 0.5f);
        private Transform interactPoint;

        [Header("Stair climb")]
        private GameObject stepRayLower;
        private GameObject stepRayUpper;
        [SerializeField]
        private float stepHeight;
        [SerializeField]
        private float stepSmooth;

        private string reloadAnimationName;

        private GameObject gO;
        /// <summary>
        /// The constructor that is responsible for initializing mutiple essential variables.
        /// </summary>
        /// <param name="transform">Transform of the player</param>
        /// <param name="rigidbody">Rigidbody of the player</param>
        /// <param name="animator">Animator of the player</param>
        /// <param name="maxXMove">max x speed of the player</param>
        /// <param name="maxZMove">max z speed of the player</param>
        /// <param name="overallMaxMove">overall max speed of the player</param>
        /// <param name="deathAnimationString">death animation's name</param>
        /// <param name="reloadAnimationName">reload animation's name</param>
        /// <param name="stepSmooth">The step height of the character.</param>
        /// <param name="stepHeight">How high the character can step up.</param>
        /// <param name="stepRayLower">The minimum height for an object to be climbed over.</param>
        /// <param name="stepRayUpper">The maximum height for an object to be climbed over.</param>
        /// <param name="interactPoint">The point where the boxcast for interactions will based off of.</param>
        /// <param name="gO">The gameObject of the player.</param>
        /// 
        /// 2021-05-06 RB Initial Documentation
        /// 2021-06-10 JH Add reload animation name to constructor
        /// 
        public PlayerMotor(Transform transform, Rigidbody rigidbody, Animator animator,
            float maxXMove, float maxZMove, float overallMaxMove, string deathAnimationString, string reloadAnimationName,
            float stepSmooth, float stepHeight, GameObject stepRayLower, GameObject stepRayUpper, Transform interactPoint, GameObject gO) : 
            base(transform, rigidbody, animator, maxXMove, maxZMove, overallMaxMove, deathAnimationString)
        {
            this.reloadAnimationName = reloadAnimationName;
            this.stepRayLower = stepRayLower;
            this.stepRayUpper = stepRayUpper;
            this.stepSmooth = stepSmooth;
            this.stepHeight = stepHeight;
            this.interactPoint = interactPoint;
            this.gO = gO;

            previousSpeed = Vector3.zero;
            //this.stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, this.stepHeight, stepRayUpper.transform.position.z);
        }

        /// <summary>
        /// Move the players gameobject in the direction of the inputs. While also telling the animator to animate the player.
        /// </summary>
        /// <param name="xSpeed">X input of the player.</param>
        /// <param name="zSpeed">Z input of the player.</param>
        /// 
        /// 2021-05-06 RB initial documentation.
        /// 2021-06-10 JH Now stops movement when in reloading animation - Reference: EnemyMotor.Move from RB
        /// 
        public override void Move(float xSpeed, float zSpeed)
        {
            if (GetReloadAnimatorState())
            {
                // prevent player from moving if reloading
                return;
            }

            Vector3 speed = Vector3.forward * zSpeed * maxZMove;
            speed += Vector3.right * xSpeed * maxXMove;
            // Clamp speed.
            if (speed.magnitude > overallMaxMove)
            {
                speed = speed.normalized * overallMaxMove;
            }

            // Keep rotation.
            if(speed.magnitude != 0)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(speed), 0.20f);

            rigidbody.AddForce(speed, ForceMode.VelocityChange);

            animator.SetFloat("Speed", speed.magnitude / overallMaxMove);

            previousSpeed = speed;
        }

        /// <summary>
        /// Interact with a interactable game object if it is in range of the boxcast.
        /// </summary>
        /// <param name="interactInput">Interact input key</param>
        /// 
        /// 2021-05-06 RB Inital Documentation
        /// 2021-06-04 RB Changed box cast to overlapbox for more reliable interactions.
        /// 2021-07-07 RB Patched out bug where players animation moves while interacting with an object.
        /// 
        public void Interact(float interactInput)
        {
            //RaycastHit hit;
            //Physics.BoxCast(interactPoint.position, interactBoxSize, transform.forward, out hit, transform.rotation, 0.3f)
            //Debug.Log(1 << LayerMask.NameToLayer("Interactable"));

            Collider[] colliders = Physics.OverlapBox(interactPoint.position, interactBoxSize, transform.rotation, 1 << LayerMask.NameToLayer("Interactable"));
            //Debug.Log(colliders.Length);
            if(colliders.Length != 0)
            {
                //foreach (Collider coll in colliders)
                //{
                //    Debug.Log(coll.name);
                //}
                //Debug.Log("END");
                Move(0f, 0f);
                colliders[0].GetComponent<Interactables>().Interact(gO);
                //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable") && interactInput == 1f)
                //    hit.collider.GetComponent<Interactables>().Interact(gO);
            }
        }

        /// <summary>
        /// Gets an interactable object.
        /// </summary>
        /// <returns>The interactable object else it returns null no interactable object could be found.</returns>
        /// 
        /// 2021-07-07 RB Initial documentation.
        /// 
        public GameObject GetInteractObject()
        {
            Collider[] colliders = Physics.OverlapBox(interactPoint.position, interactBoxSize, transform.rotation, 1 << LayerMask.NameToLayer("Interactable"));

            if (colliders.Length > 0) return colliders[0].gameObject;
            else return null;
        }

        /// <summary>
        /// Reload behaviour for the player
        /// </summary>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        public void Reload()
        {
            SetReloadAnimation();
        }

        // based on https://www.youtube.com/watch?v=DrFk5Q_IwG0&ab_channel=Brackeys
        /// <summary>
        /// Climb stairs based on if the lower ray intersect an object but the upper ray does not.
        /// </summary>
        /// 
        /// 2021-05-06 Initial Documentation.
        /// 
        public void StairClimb()
        {
            RaycastHit hitLower;
            if(Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
            {
                RaycastHit hitUpper;
                if(!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
                {
                    rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                }
            }
            Debug.DrawRay(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward) * 0.2f, Color.red);
            Debug.DrawRay(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward) * 0.1f, Color.blue);


            RaycastHit hitLower45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
            {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
                {
                    rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                }
            }
            Debug.DrawRay(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1) * 0.2f, Color.red);
            Debug.DrawRay(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1) * 0.1f, Color.blue);

            RaycastHit hitLowerMinus45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
            {
                RaycastHit hitUpperMinus45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
                {
                    rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                }
            }
            Debug.DrawRay(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1) * 0.2f, Color.red);
            Debug.DrawRay(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1) * 0.1f, Color.blue);
        }

        /// <summary>
        /// Sets the combat animation bool.
        /// </summary>
        /// <param name="combat">the new bool we want to set to the player.</param>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public void SetCombatAnimation(bool combat)
        {
            animator.SetBool("Combat", combat);
        }

        /// <summary>
        /// Sets the blocking animation of the animator.
        /// Note: this will not work unless the combat animation is set to true.
        /// </summary>
        /// <param name="value"></param>
        /// 
        /// 2021-06-30 RB Initial Documentation.
        /// 
        public void SetBlockingAnimation(bool value)
        {
            animator.SetBool("Block", value);
        }

        /// <summary>
        /// Sets the overallMaxMove.
        /// </summary>
        /// <param name="overallMaxMove">The new value of the overall max move.</param>
        /// 
        /// 2021-05-21 RB Initial Documentation.
        /// 
        public void SetOverallMaxMove(float overallMaxMove)
        {
            this.overallMaxMove = overallMaxMove;
        }

        /// <summary>
        /// Sets the reload animation
        /// </summary>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        private void SetReloadAnimation()
        {
            animator.SetTrigger("Reload");
        }

        /// <summary>
        /// Gets the state of the reload animator state
        /// </summary>
        /// <returns>true if animation is occuring, false otherwise</returns>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        public bool GetReloadAnimatorState()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(reloadAnimationName);
        }

        /// <summary>
        /// Sets the animator mode to the given mode.
        /// </summary>
        /// <param name="mode">mode to set the player animator</param>
        /// 
        /// 2021-07-25  JH Initial Documentation
        /// 
        public void SetAnimatorUpdateMode(AnimatorUpdateMode mode)
        {
            animator.updateMode = mode;
        }
    }
}

