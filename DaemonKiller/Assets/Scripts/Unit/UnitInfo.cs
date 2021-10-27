using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.Data;
using CharacterMovement;
using CharacterMovement.Enemies;
using CharacterMovement.Player;
using ObserverPattern;
using ObserverPattern.Clock;
using Audio;

namespace Unit.Info
{
    /// <summary>
    /// Holds the unit information, based on their scriptable object, UnitData
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH), Rozario (Ross) Beaudin
    /// 
    /// Public Vars                 Description
    /// unitData                    scriptable object with unit's information
    /// maxHealth                   maximum health of unit
    /// currentHealth               current health of unit
    /// rend                        renderer of unit
    /// startColor                  default color of unit
    /// 
    /// Private Vars
    /// enemyController             The controller of the enemy
    /// rb                          rigidbody of the unit 
    /// currentSpecialAfflication   IEnumerator for a special debuff
    /// buffs                       hash set of buffs for the unit        
    ///
    public class UnitInfo : MonoBehaviour, Observer
    {
        public UnitData unitData;
        [HideInInspector]
        public float maxHealth;
        [Tooltip("Change only in debugging")]
        public float currentHealth;
        [HideInInspector]
        public Renderer rend;
        [HideInInspector]
        public Color startColor;

        private EnemyController enemyController;
        
        private Rigidbody rb;
        private IEnumerator currentSpecialAffliction;

        protected CharacterMotor characterMotor;

        protected List<ImbueEffect> buffs = new List<ImbueEffect>();

        /// <summary>
        /// Initializes values
        /// </summary>
        ///
        /// 2021-05-14 JH Initial work
        /// 2021-05-28 RB added enemy controller to determine if the unit is an enemy.
        /// 2021-06-04 RB Adjusted so that the enemy and player are separate.
        ///
        void Start()
        {
            rend = GetComponentInChildren<Renderer>();
            startColor = rend.material.color;
            maxHealth = unitData.maxHP;
            currentHealth = maxHealth;

            rb = GetComponent<Rigidbody>();

            switch (unitData.unitType)
            {
                case (UnitType.Enemy):
                    characterMotor = GetComponent<EnemyController>().GetMotor();
                    enemyController = GetComponent<EnemyController>();
                    break;
            }
        }

        /// <summary>
        /// Sets the game object to inactive if the unit is an enemy and is dead.
        /// </summary>
        /// 
        /// 2021-08-04 JH Initial Implementation
        /// 
        void OnDisable()
        {
            if (gameObject.layer == 13)
            {
                switch (unitData.unitType)
                {
                    case UnitType.Enemy:
                        gameObject.SetActive(false);
                        break;
                }
            }
        }

        /// <summary>
        /// Take damage based on given value
        /// </summary>
        /// <param name="damage">amount of damage to take</param>
        ///
        /// 2021-05-14  JH  Initial work
        /// 2021-05-28  RB  Added handle for enemy animations and death routine.
        /// 2021-05-28  JH  Parameter changed from int to float
        /// 2021-07-20  JH  Checks if object is active in hierarchy
        ///
        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0 && gameObject.activeInHierarchy)
            {
                Death();
                return;
            }
            enemyController.Damaged();
        }

        /// <summary>
        /// Applies a temporary affliction for a set amount of time to allow
        /// physics to work on the target, disabling isKinematic on its rigidbody.
        /// </summary>
        /// <param name="time">duration of affliction</param>
        /// 
        /// 2021-07-14  JH  Initial Documentation. Initial Work by RB
        /// 
        public void ApplySpecialAffliction(float time)
        {
            if(rb == null)
            {
                return;
            }

            if(currentSpecialAffliction != null)
            {
                StopCoroutine(currentSpecialAffliction);
            }

            currentSpecialAffliction = SpecialAffliction(time, rb);
            StartCoroutine(currentSpecialAffliction);
        }

        /// <summary>
        /// Special affliction to apply for a set amount of time, disabling
        /// the rigidbody's isKinematic temporarily.
        /// </summary>
        /// <param name="time">duration of affliction</param>
        /// <param name="rigidbody">rigidbody of the target</param>
        /// <returns>duration of affliction</returns>
        /// 
        /// 2021-07-14  JH  Initial Documentation. Initial Work by RB
        /// 
        private IEnumerator SpecialAffliction(float time, Rigidbody rigidbody)
        {
            rigidbody.isKinematic = false;
            yield return new WaitForSeconds(time);
            rigidbody.isKinematic = true;
        }

        /// <summary>
        /// Take damage based on given value
        /// No damage animation is played.
        /// </summary>
        /// <param name="damage">amount of damage to take</param>
        /// 
        /// 2021-06-04  JH  Initial Work
        /// 2021-06-18  JH  Now virtual
        /// 2021-07-20  JH  Checks if object is active in hierarchy
        /// 
        public virtual void TakeDamageNoAnimation(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0 && gameObject.activeInHierarchy)
            {
                Death();
                return;
            }
        }
        /// <summary>
        /// Heal health based on given value
        /// </summary>
        /// <param name="health">amount to heal</param>
        /// 
        /// 2021-05-14 JH Initial work
        /// 
        public void HealHealth(int health)
        {
            currentHealth += health;
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        /// <summary>
        /// Unit dies and its game object is destroyed
        /// </summary>
        /// 
        /// 2021-05-14  JH  Initial work
        /// 2021-06-02  JH  Unsubscribe clock added 
        /// 2021-06-12  JH  Now applies end of buff effects before unsubscribing (for pooling)
        /// 2021-06-16  JH  End of buff moved to DeathRoutine
        /// 2021-07-14  JH  Enemy layer moved after dying to prevent targeting on death.
        /// 2021-07-16  RB  Add sounds.
        ///
        private void Death()
        {
            gameObject.layer = 13; // layer to "dead" prevents targeting
            UnsubscribeToClock();
            // play death animation
            StartCoroutine(DeathRoutine());
            SoundManager.Sounds sound = enemyController.GetDieSounds();
            SoundManager.PlaySound(sound, enemyController.transform.position);
            
            
            return;
        }

        /// <summary>
        /// A rountine that is performed whent the unit dies.
        /// </summary>
        /// 
        /// 2021-05-28 RB Initial Documentation
        /// 2021-06-16 JH Removes buff after animation
        /// 2021-07-14 JH Enemies are now set to inactive rather than being destroyed.
        /// 
        /// <returns></returns>
        private IEnumerator DeathRoutine()
        {
            enemyController.Die();
            yield return new WaitForSeconds(1.5f);

            // remove all buffs
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                if (buffs[i].isApplied == true)
                {
                    buffs[i].End();
                }
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Getter method for the unit's type
        /// </summary>
        /// <returns>returns either Player or Enemy</returns>
        /// 
        /// 2021-05-16 JH Initial work
        /// 
        public UnitType GetUnitType()
        {
            return unitData.unitType;
        }
        /// <summary>
        /// Getter method for the unit's buffs
        /// </summary>
        /// <returns>hash set of buffs for the unit</returns>
        /// 
        /// 2021-05-30  JH  Initial Work
        /// 
        public List<ImbueEffect> GetBuffs()
        {
            return buffs;
        }

        /// <summary>
        /// Adds an imbuement effect to the buff list
        /// Only adds if the buff is unique
        /// </summary>
        /// <param name="iE">imbuement effect to add to the list</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public void AddBuff(ImbueEffect iE)
        {
            if (buffs.Contains(iE) == false)
            {
                buffs.Add(iE);
            }
        }

        /// <summary>
        /// Subscribes this unit to the clock
        /// Only if the unit has buffs and if it is currently not subscribed
        /// </summary>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public void SubscribeToClock()
        {
            if (buffs.Count > 0 && Clock.CheckSubscription(this) == false)
            {
                Clock.Subscribe(this);
            }
        }
        /// <summary>
        /// Unsubscribes this unit to the clock
        /// Only if this unit is currently subscribed
        /// </summary>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public void UnsubscribeToClock()
        {
            if (Clock.CheckSubscription(this) == true)
            {
                Clock.Unsubscribe(this);
            }
        }

        /// <summary>
        /// Actions to do upon an update from the clock
        /// Goes through each buff and applies its tick effect,
        /// then removes the buff and unsubscribes if the
        /// buff duration is over
        /// </summary>
        /// <param name="obj">info to use from the clock</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        void Observer.UpdateObserver(object obj)
        {
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                buffs[i].Tick();
                if (buffs[i].isApplied == false)
                {
                    
                    buffs.RemoveAt(i);
                }
            }
            if (buffs.Count == 0)
                UnsubscribeToClock(); // only unsubscribe if ALL buffs expired
        }
    }
}

