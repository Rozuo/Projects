using System.Collections.Generic;
using UnityEngine;
using CharacterMovement.Player;
using Unit.Player;
using System.Collections;
using Audio;

namespace Unit.Info.Player
{
    /// <summary>
    /// Player information, based on UnitInfo.
    /// Holds information that is exclusively for the player
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars         Description
    /// maxEnergy           maximum energy of the player
    /// currentEnergy       curreny energy of the player
    /// 
    /// Private Vars        Description
    /// animator            animator to control animations
    /// equippedGun         scriptable object of gun that is equipped
    /// ammoClips           list of ammo clips for guns the player currently has
    /// inventory           inventory of the player
    /// imbueQueue          queue of imbued bullets in case of multiple of bullets
    /// controller          controller that controls the player's movement
    /// pHUD                hud to update for player statistics
    /// blocking            bool to check if player is blocking or not
    /// 
    public class PlayerInfo : UnitInfo
    {
        public float maxEnergy = 100;
        public float currentEnergy;
        private float energyRegenRate = 1f;
        private Animator animator;
        private InventoryItem equippedGun = null;
        private List<AmmoClip> ammoClips;
        private Inventory inventory;
        private Queue<ImbueEffect> imbueQueue = new Queue<ImbueEffect>();

        private PlayerController controller;
        private PlayerHUD pHUD;
        private bool blocking = false;

        private IEnumerator curRootMotionAffliction;

        /// <summary>
        /// Get/Set functions for the currentHealth variable
        /// </summary>
        /// 
        /// 2021-06-07  TH  Initial Implementation
        /// 
        public float CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; pHUD.UpdateHealth(); }
        }

        /// <summary>
        /// Get/Set functions for ammoClips list
        /// </summary>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        public List<AmmoClip> AmmoClips
        {
            get { return ammoClips; }
            set { ammoClips = value; }
        }

        /// <summary>
        /// Initializes values for player information
        /// </summary>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 2021-05-21  JH  Add Animator
        /// 2021-06-05  JH  Add ammo clip and player HUD 
        /// 
        void Awake()
        {
            rend = transform.Find("Blake").gameObject.GetComponent<Renderer>(); // Change upon changing character models
            currentEnergy = maxEnergy;
            startColor = rend.material.color;
            maxHealth = unitData.maxHP;
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            ammoClips = new List<AmmoClip>();
            inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
            pHUD = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<PlayerHUD>();
        }

        /// <summary>
        /// Initialize necessary variables.
        /// </summary>
        /// 
        /// 2021-06-04 RB Initial Documentation.
        /// 
        private void Start()
        {
            controller = GetComponent<PlayerController>();
            characterMotor = controller.GetMotor();
        }

        /// <summary>
        /// Passively regenrates the player's energy
        /// If player is in combat, it will regenerate slower
        /// </summary>
        /// 
        /// 2021-06-04  JH  Initial Work
        /// 2021-06-30  JH  Energy regen now only works when time isn't stopped
        /// 
        private void Update()
        {
            if (currentEnergy < maxEnergy && Time.timeScale != 0)
            {
                if (controller.PlayerState == PlayerController.State.Combat)
                {
                    PassiveEnergyRegen(0.5f);
                }
                else
                {
                    PassiveEnergyRegen(1f);
                }
            }
        }

        /// <summary>
        /// Passively regenerates the player's energy depending on the multiplier
        /// Also updates the UI to correspond with the new energy values
        /// </summary>
        /// <param name="multiplier">multiplier for the regeneration rate</param>
        /// 
        /// 2021-06-04  JH  Initial Work
        /// 
        private void PassiveEnergyRegen(float multiplier)
        {
            currentEnergy += energyRegenRate * multiplier * Time.deltaTime;
            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            pHUD.UpdateEnergy();
        }

        /// <summary>
        /// The player will take damage.
        /// </summary>
        /// <param name="damage"></param>
        /// 
        /// 2021-05-31  RB  Initial Documentation
        /// 2021-06-01  TH  Player dying now notifies the Game Manager it's game over
        /// 2021-06-04  RB  Added visuals for health
        /// 2021-06-30  RB  Add reduce damage for blocking.
        /// 2021-07-06  JH  Blocking state is now checked upon taking damage instead of in update.
        /// 2021-07-16  RB  Add sounds.
        /// 
        public override void TakeDamage(float damage)
        {
            blocking = controller.GetBlockingAnimation();
            if(blocking)
            {
                currentHealth -= (damage/2f);
                AddEnergy(-5f);
            }
            else
            {
                currentHealth -= damage;
            }
            
            pHUD.UpdateHealth();
            if (currentHealth <= 0)
            {
                controller.PlayerState = PlayerController.State.Dead;
                characterMotor.Die();
                int randomNum = Random.Range(1, 3);
                switch (randomNum)
                {
                    case (1):
                        SoundManager.PlaySound(SoundManager.Sounds.PlayerDie1);
                        break;
                    case (2):
                        SoundManager.PlaySound(SoundManager.Sounds.PlayerDie2);
                        break;
                }
                

                // Inform Game Manager (if it exists)
                GameObject dir = GameObject.FindGameObjectWithTag("Game Manager");
                if (dir)
                    StartCoroutine(dir.GetComponent<GameManager>().GameOverTimer());
                return;
            }
        }

        /// <summary>
        /// Take damage without animation. However, player has no
        /// hit animation, so it calls regular taking damage method
        /// </summary>
        /// <param name="damage">damage to deal to player</param>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        public override void TakeDamageNoAnimation(float damage)
        {
            TakeDamage(damage);
        }

        /// <summary>
        /// Applies affliction status by removing 
        /// root motion temporarily, allowing for 
        /// physics on the player
        /// </summary>
        /// <param name="time">time to disable root motion</param>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        public void ApplyRootMotionAffliction(float time)
        {
            if (animator == null)
            {
                return;
            }

            if (curRootMotionAffliction != null)
            {
                StopCoroutine(curRootMotionAffliction);
            }

            curRootMotionAffliction = RootMotionAffliction(time, animator);
            StartCoroutine(curRootMotionAffliction);
        }
        /// <summary>
        /// IEnumerator for the root motion affliction
        /// </summary>
        /// <param name="time">time to disable the root motion</param>
        /// <param name="animator">animator to remove root motion from</param>
        /// <returns>IEnumerator for coroutine</returns>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        private IEnumerator RootMotionAffliction(float time, Animator animator)
        {
            animator.applyRootMotion = false;
            yield return new WaitForSeconds(time);
            animator.applyRootMotion = true;
        }

        /// <summary>
        /// Adds an imbued bullet effect to the queue
        /// </summary>
        /// <param name="imbuement">imbuement effect to add</param>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 2021-06-02  JH  Changed from action info to imbue effect type
        /// 2021-06-08  JH  Add imbuement background color change
        /// 2021-06-10  JH  Change indexing to front
        /// 
        public void AddImbuement(ImbueEffect imbuement)
        {
            imbueQueue.Enqueue(imbuement);
            pHUD.ChangeImbueBGColor(NumberOfImbuements() - 1, imbuement.imbueType);
        }
        /// <summary>
        /// Removes the first imbuement in the queue
        /// </summary>
        /// <returns>type of imbuement to use</returns>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public ImbueEffect GetImbuement()
        {
            return imbueQueue.Dequeue();
        }
        /// <summary>
        /// Clears the list of imbuements in the queue
        /// </summary>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public void ClearImbuements()
        {
            // use for swapping guns maybe...
            imbueQueue.Clear();
        }
        /// <summary>
        /// Returns the amount of imbuements in the queue
        /// </summary>
        /// <returns>the number of bullets imbued</returns>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public int NumberOfImbuements()
        {
            // use for limiting amount of imbuements (all bullets
            // in magazine are imbued)
            return imbueQueue.Count;
        }

        /// <summary>
        /// Adds energy to the current amount.
        /// If energy surplus, then current amount is set to max
        /// If energy deficit, then current amount is set to zero
        /// </summary>
        /// <param name="energy">amount of energy to add</param>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 2021-05-27  JH  Removed SubtractEnergy and moved functions here
        /// 
        public void AddEnergy(float energy)
        {
            currentEnergy += energy;
            if (currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            else if (currentEnergy <= 0)
            {
                currentEnergy = 0;
            }
        }

        /// <summary>
        /// Getter method for equipped gun
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public InventoryItem GetGun()
        {
            return equippedGun;
        }
        /// <summary>
        /// Setter method for equipped gun
        /// </summary>
        /// <param name="gun">gun to equip to player</param>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public void SetGun(InventoryItem gun)
        {
            equippedGun = gun;
        }

        /// <summary>
        /// Adds a new ammo clip into the list of clips
        /// Only add if that clip does not exist
        /// </summary>
        /// <param name="ammoClip">ammo clip to add to the list</param>
        /// 
        /// 2021-06-05  JH  Initial Work
        /// 2021-07-20  JH  Fixed to check ammoClip rather than equipped gun ammo type
        /// 
        public void AddAmmoClip(AmmoClip ammoClip)
        {
            if (!ammoClips.Exists(x => x.gunID.Equals(ammoClip.gunID)))
            {
                ammoClips.Add(ammoClip);
            }
        }

        /// <summary>
        /// Reloads a single bullet into the clip
        /// Takes an ammo out of the inventory, 
        /// and adds it to the currently equipped gun's clip
        /// </summary>
        /// 
        /// 2021-06-05  JH  Initial Work
        /// 2021-06-10  JH  Now updates ammo portion of HUD
        /// 
        public void ReloadOnce()
        {
            if (IsReloadAvailable())
            {
                int index = ammoClips.FindIndex(x => x.gunDetails.Equals(equippedGun));
                AmmoClip newClip = ammoClips[index];
                // reload only if ammo available and clip smaller than max mag
                inventory.Use(1, equippedGun);
                newClip.AddAmmo(1);
                ammoClips[index] = newClip;

                pHUD.UpdateAmmoMask();
                pHUD.UpdateImbueBGPosition();
                pHUD.UpdateImbueBGContent(GetCurrentClip());
                inventory.updateInventory.Invoke();
            }
        }
        /// <summary>
        /// Reloads the gun to its maximum magazine.
        /// Takes ammo out of the inventory, 
        /// and adds it to the currently equipped gun's clip
        /// until it reaches the maximum clip size
        /// </summary>
        /// 
        /// 2021-06-05  JH  Initial Work
        /// 2021-06-10  JH  Now updates ammo portion of HUD
        /// 
        public void ReloadAll()
        {
            int index = ammoClips.FindIndex(x => x.gunDetails.Equals(equippedGun));
            AmmoClip newClip = ammoClips[index];
            while (IsReloadAvailable())
            {
                // reload only if ammo available and clip smaller than max mag
                inventory.Use(1, equippedGun);
                newClip.AddAmmo(1);
                ammoClips[index] = newClip;

                pHUD.UpdateAmmoMask();
                pHUD.UpdateImbueBGPosition();
                pHUD.UpdateImbueBGContent(GetCurrentClip());
                inventory.updateInventory.Invoke();
            }
        }

        /// <summary>
        /// Checks if reloading is available
        /// Depends on ammo in clip and ammo in inventory
        /// </summary>
        /// <returns>true if can reload, false otherwise</returns>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        public bool IsReloadAvailable()
        {
            if (!equippedGun)
            {
                // no gun equipped
                return false;
            }
            if (inventory.Count(equippedGun) > 0 &&
                    GetCurrentClip() < equippedGun.maxMag) 
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the amount of bullets in the clip
        /// of the equipped gun.
        /// </summary>
        /// <returns>amount of bullets in the clip of the equipped gun</returns>
        /// 
        /// 2021-06-05  JH  Initial Work
        /// 
        public int GetCurrentClip()
        {
            return ammoClips.Find(x => x.gunDetails.Equals(equippedGun)).GetAmount();
        }

        /// <summary>
        /// Gets the amount of bullets in the specified gun clip
        /// </summary>
        /// <param name="gunID">gun ID to check</param>
        /// <returns>amount of bullets in the clip of the specified gun</returns>
        /// 
        /// 2021-07-21  JH  Initial Documentation
        /// 
        public int GetCurrentClip(Item gunID)
        {
            return ammoClips.Find(x => x.gunID.Equals(gunID)).GetAmount();
        }

        /// <summary>
        /// Gets the total amount of ammo in the clip
        /// and inventory of the equipped gun
        /// </summary>
        /// <returns>total ammo for the equipped gun</returns>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        public int GetTotalAmmo()
        {
            return GetCurrentClip() + inventory.Count(equippedGun);
        }

        /// <summary>
        /// Uses the amount of ammo from the clip
        /// Only if the amount is available
        /// </summary>
        /// <param name="amount">amount to use from the clip</param>
        /// 
        /// 2021-07-05  JH  Initial Work
        /// 
        public void UseAmmo(int amount)
        {
            if (GetCurrentClip() >= amount)
            {
                int index = ammoClips.FindIndex(x => x.gunDetails.Equals(equippedGun));
                AmmoClip newClip = ammoClips[index];
                newClip.AddAmmo(amount);
                ammoClips[index] = newClip;

                inventory.updateInventory.Invoke();
            }
        }

        /// <summary>
        /// Changes the gun equipped on the player
        /// Puts clip ammo back into the inventory
        /// Imbuements removed upon switching
        /// 
        /// </summary>
        /// <param name="gun">new gun to equip</param>
        /// 
        /// 2021-06-05  JH  Initial Work
        /// 2021-06-08  JH  Add imbuement background changes
        /// 2021-06-10  JH  Now checks if gun is already equipped
        /// 
        public void ChangeGun(InventoryItem gun)
        {
            if (!gun)
                return; // null
            //if (gun == equippedGun)
            //{
            //    Debug.Log("Already Equipped");
            //    // same gun, do not swap
            //    return;
            //}
            ClearImbuements(); // switching guns removes imbuement from bullets
            pHUD.AllImbueBGOff(); // clear imbuements from
            SetGun(gun); // change to new gun
            pHUD.UpdateAmmoBar();
            pHUD.UpdateImbueBGPosition();
            pHUD.UpdateImbueBGContent(GetCurrentClip());
        }

        /// <summary>
        /// Swaps the players gun to the next in the inventory
        /// Depends on negative or positive input for direction
        /// Negative is previous weapon in inventory
        /// Positive is next weapon in inventory
        /// </summary>
        /// <param name="direction">the direction in inventory to get the gun</param>
        /// 
        /// 2021-06-10  JH  Inital Work
        /// 
        public void NextGun(float direction)
        {
            if (!GetGun())
            {
                return;
            }
            int index = ammoClips.FindIndex(x => x.gunDetails.Equals(equippedGun));
            if (direction > 0)
            {
                // next gun
                index += 1;
                if (index > ammoClips.Count - 1)
                {
                    // index out of bounds, move to first item
                    index = 0;
                }
            }
            else if (direction < 0)
            {
                // previous gun
                index -= 1;
                if (index < 0)
                {
                    // index out of bounds, move to last item
                    index = ammoClips.Count - 1;
                }
            }
            ChangeGun(ammoClips[index].gunDetails);
        }

        /// <summary>
        /// Triggers the pistol shooting animation
        /// </summary>
        /// 
        /// 2021-05-22  JH  Initial Work
        /// 
        public void PistolAnimation()
        {
            animator.SetTrigger("Pistol");
        }
    }
}
