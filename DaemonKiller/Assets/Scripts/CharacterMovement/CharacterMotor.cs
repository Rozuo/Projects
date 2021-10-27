using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterMovement
{
    /// <summary>
    /// Character motor is responsible for the creation of all character motors that will move an Actor.
    /// </summary>
    /// 
    /// Author: Rozario (RB) Beaudin
    /// 
    /// Public variables:
    /// transform           The transform of the actor
    /// rigidbody           The rigidbody of the actor
    /// animator            The animator of the actor
    /// maxXMove            Max movement on the x axis.
    /// maxZMove            Max movement on the z axis.
    /// overallMaxMove      Max overall speed.
    /// 
    public abstract class CharacterMotor
    {

        public Transform transform;
        public Rigidbody rigidbody;
        public Animator animator;

        public float maxXMove;
        public float maxZMove;
        public float overallMaxMove;

        protected string deathAnimationString;
        /// <summary>
        /// The constructor to set inital values.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rigidbody"></param>
        /// <param name="animator"></param>
        /// <param name="maxXMove"></param>
        /// <param name="maxZMove"></param>
        /// <param name="overallMaxMove"></param>
        /// 
        /// 2021-05-28 Initial documentation.
        /// 
        public CharacterMotor(Transform transform, Rigidbody rigidbody, Animator animator, 
            float maxXMove, float maxZMove, float overallMaxMove, string deathAnimationString)
        {
            this.transform = transform;
            this.rigidbody = rigidbody;
            this.animator = animator;

            this.maxXMove = maxXMove;
            this.maxZMove = maxZMove;
            this.overallMaxMove = overallMaxMove;
            this.deathAnimationString = deathAnimationString;
        }

        /// <summary>
        /// This method will move the actor.
        /// </summary>
        /// <param name="xSpeed">x axis speed.</param>
        /// <param name="zSpeed">z axis speed.</param>
        /// 
        /// 2021-05-28 RB Inital docuementation.
        /// 
        public abstract void Move(float xSpeed, float zSpeed);

        /// <summary>
        /// Set the damage boolean in the animator.
        /// </summary>
        /// <param name="value">boolean.</param>
        /// 
        /// 2021-05-28 RB Initial Documenation.
        /// 
        public virtual void SetDamageAnimation(bool value)
        {
            animator.SetTrigger("Damage");
            Move(0f, 0f);
        }

        /// <summary>
        /// Sets the Dead boolean to true.
        /// </summary>
        /// 
        /// 2021-05-28 RB Initial Documentation.
        /// 
        public virtual void Die()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(deathAnimationString))
            {
                Move(0f, 0f);
                animator.SetTrigger("Dead");
            }
        }
    }
}

