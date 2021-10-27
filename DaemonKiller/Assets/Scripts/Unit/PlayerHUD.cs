using Action.Info.Imbues;
using System.Collections.Generic;
using Unit.Info.Player;
using UnityEngine;
using UnityEngine.UI;
using Unit.ColorChange;
using Action.Info;
using Action.Info.Guns;

namespace Unit.Player
{
    /// <summary>
    /// PlayerHUD system. Controls player health, energy and ammo
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    ///         Tyson Hoang (TH)
    /// 
    /// Constants               Description
    /// BULLET_RELATIVE_POSX    position of the bullets in the HUD
    /// BULLET_WIDTH            width of a bullet in the HUD
    /// 
    /// Public Vars             Description
    /// healthNumberText        text of health points
    /// healthSlider            image of health bar
    /// healthBackSlider        image of back bar for VFX
    /// healthSliderMask        mask for previewing health resource changes
    /// energyNumberText        text of energy points
    /// energySlider            image of energy bar
    /// energyBackSlider        image of back bar for VFX
    /// energySliderMask        mask for previewing energy resource changes
    /// totalAmmoText           text for total ammo in current gun
    /// ammoBarMask             image for the ammo's bar mask
    /// ammoSprite              ammo's image for the sprite
    /// ammoBarMaskPreview      image mask for previewing ammo resource usage
    /// ammoSpritePreview       ammo's image for the sprite to preview ammo resource usage
    /// ammoBarSilhouette       image of the ammo bar silhouette
    /// player                  holds player information
    /// imbueBGTemplate         game object for the background sprite for bullets
    /// imbueBGContent          GameObject holder for each background sprite for bullets
    /// imbueBGReserves         GameObject holder for each inactive background sprite for bullets
    /// healthFillRate          How fast the health's background bar depletes to match the player's health (per frame)
    /// energyFillRate          How fast the energy's background bar depletes to match the player's energy (per frame)
    /// 
    /// Private Vars
    /// imbueBGList             list of background sprites currently active
    /// imbuePreview            bool to determine whether to preview imbue resource changes
    /// imbuePreviewIndexes     list of the indexes to preview imbuements
    /// alpha                   current alpha for resource previewing
    /// energyExhaustColor      color of energy bar when exhausted
    /// energyNormalColor       regular color of energy bar when not exhausted
    /// 
    public class PlayerHUD : MonoBehaviour
    {
        private const int BULLET_RELATIVE_XPOS = 55;
        private const int BULLET_WIDTH = 16;
        [Header("Health Elements")]
        public Text healthNumberText;
        public Image healthSlider;
        public Image healthBackSlider;
        public Image healthSliderMask;
        [Header("Energy Elements")]
        public Text energyNumberText;
        public Image energySlider;
        public Image energyBackSlider;
        public Image energySliderMask;
        private Color energyExhaustColor = Color.red;
        private Color energyNormalColor;
        [Header("Gun Elements")]
        public Text totalAmmoText;
        public Image ammoBarMask;
        public Image ammoSprite;
        public Image ammoBarMaskPreview;
        public Image ammoSpritePreview;
        public Image ammoBarSilhouette;
        [HideInInspector]
        public PlayerInfo player;
        [Header("Imbue Content")]
        public GameObject imbueBGTemplate; // change sprite later
        public GameObject imbueBGContent;
        public GameObject imbueBGReserves;
        private List<GameObject> imbueBGList;
        private bool imbuePreview;
        private List<int> imbuePreviewIndexes;
        private float alpha;
        [Header("Status Bar Settings")]
        public float healthFillRate = 0.005f;
        public float energyFillRate = 0.010f;

        /// <summary>
        /// Initialize references and variables
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
            imbueBGList = new List<GameObject>();
            energyNormalColor = energySlider.color;
            imbuePreview = false;
            imbuePreviewIndexes = new List<int>();
            alpha = 0f;
        }

        /// <summary>
        /// Initializes value from player info
        /// </summary>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 2021-05-27  JH  Update ammo added
        /// 2021-06-08  JH  Initialize imbueBGList
        /// 
        void Start()
        {              
            UpdateHealth();
            UpdateEnergy();
            UpdateAmmoMask();
        }

        /// <summary>
        /// Updates the Status background bar's fill rate
        /// Updates the resource changes
        /// </summary>
        /// 
        /// 2021-06-24  TH  Initial Implementation
        /// 2021-06-30  JH  Add mask alpha change for overload resource usage
        /// 2021-07-02  JH  Alpha is now the same for each resource.
        ///                 Add resource flashing for imbue and ammo
        /// 2021-07-08  JH  Changed how to change imbue previews
        /// 
        private void Update()
        {
            // fill backslider for resource feedback when using them
            if (healthBackSlider.fillAmount != healthSlider.fillAmount)
                healthBackSlider.fillAmount = Mathf.MoveTowards(healthBackSlider.fillAmount, healthSlider.fillAmount, healthFillRate);

            if (energyBackSlider.fillAmount != energySlider.fillAmount)
                energyBackSlider.fillAmount = Mathf.MoveTowards(energyBackSlider.fillAmount, energySlider.fillAmount, energyFillRate);

            // Any preview enabled, calculate alpha
            if (healthBackSlider.enabled || energySliderMask.enabled || imbuePreview || ammoSpritePreview.enabled)
                alpha = Mathf.Lerp(0f, 1f, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 3)));

            // flash resource changes
            if (healthSliderMask.enabled)
                ModifyColor.ChangeImageAlpha(healthSliderMask, alpha); // health
            if (energySliderMask.enabled)
                ModifyColor.ChangeImageAlpha(energySliderMask, alpha); // energy
            if (imbuePreview)
                foreach (int index in imbuePreviewIndexes)
                    ModifyColor.ChangeImageAlpha(imbueBGList[index].GetComponent<Image>(), alpha);
            if (ammoSpritePreview.enabled)
                ModifyColor.ChangeImageAlpha(ammoSpritePreview, alpha); // ammo
        }

        /// <summary>
        /// Updates the health in the HUD
        /// </summary>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 2021-06-05  JH  Update for more accurate display. Credit to RB
        /// 
        public void UpdateHealth()
        {
            healthSlider.fillAmount = (player.currentHealth / player.maxHealth) <= 0 ? 0f : player.currentHealth / player.maxHealth;
            healthNumberText.text = Mathf.Floor(healthSlider.fillAmount * 100).ToString();
        }

        /// <summary>
        /// Sets up the preview for health change from overload actions
        /// </summary>
        /// <param name="healthChange">amount of health to change</param>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void PreviewHealthChange(float healthChange)
        {
            // reset health bar
            UpdateHealth();

            healthSliderMask.enabled = true;
            float maskFillAmount = ((player.currentHealth + healthChange) / player.maxHealth) <= 0 ? 0f : (player.currentHealth + healthChange) / player.maxHealth;

            if (healthChange >= 0)
                // health will increase
                healthSliderMask.fillAmount = maskFillAmount;
            else
            {
                // health will decrease
                healthSliderMask.fillAmount = healthSlider.fillAmount;
                healthSlider.fillAmount = maskFillAmount;
            }
        }

        /// <summary>
        /// Updates the energy in the HUD
        /// </summary>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 2021-06-05  JH  Update for more accurate display. Credit to RB
        /// 
        public void UpdateEnergy()
        {
            energySlider.fillAmount = (player.currentEnergy / player.maxEnergy) <= 0 ? 0f : player.currentEnergy / player.maxEnergy;
            energyNumberText.text = Mathf.Floor(energySlider.fillAmount * 100).ToString();
        }

        /// <summary>
        /// Sets up the preview for energy change from overload actions
        /// </summary>
        /// <param name="energyChange">amount of energy to change</param>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void PreviewEnergyChange(float energyChange)
        {
            // reset energy bar
            UpdateEnergy();

            energySliderMask.enabled = true;
            float maskFillAmount = ((player.currentEnergy + energyChange) / player.maxEnergy) <= 0 ? 0f : (player.currentEnergy + energyChange) / player.maxEnergy;

            if (energyChange >= 0)
                // energy will increase
                energySliderMask.fillAmount = maskFillAmount;
            else
            {
                // energy will decrease
                energySliderMask.fillAmount = energySlider.fillAmount;
                energySlider.fillAmount = maskFillAmount;
            }
        }

        /// <summary>
        /// Updates the ammo in the HUD by changing the fill amount
        /// </summary>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 2021-05-28  JH  Updated to set fill by the max magazine instead of item limit
        /// 2021-06-05  JH  Changed to access ammo in clip instead of ammo in inventory
        /// 
        public void UpdateAmmoMask()
        {
            if (!player.GetGun())
            {
                // no gun equipped, ammo display goes to 0
                ammoBarMask.fillAmount = 0;
            }
            else
            {
                // otherwise, fill based on ammo in clip vs its maximum magazine
                ammoBarMask.fillAmount = (float) player.GetCurrentClip() /
                                            player.GetGun().maxMag;
            }
        }

        /// <summary>
        /// Changes energy slider color for exhaustion based on the value given.
        /// </summary>
        /// <param name="value">true if exhausted, false if not exhausted</param>
        /// 
        /// 2021-06-30 RB Initial Documentation
        /// 
        public void SetEnergyExhaust(bool value)
        {
            if (value)
            {
                energySlider.color = energyExhaustColor;
            }
            else
            {
                energySlider.color = energyNormalColor;
            }
        }

        /// <summary>
        /// Sets the width of the ammo bar, based on the weapon equipped.
        /// </summary>
        /// 
        /// Resource: http://answers.unity.com/answers/1299314/view.html
        /// 2021-05-28  TH  Initial Implementation
        /// 2021-06-05  JH  Updated references and updates ammo fill amount
        /// 2021-06-10  JH  Also updates total ammo 
        /// 
        public void UpdateAmmoBar()
        {
            if(!player.GetGun())
            {
                // hide the gun bar
                ammoBarSilhouette.gameObject.SetActive(false);
                return;
            }
            // Get max mag for width of ammo bars
            int maxMag = player.GetGun().maxMag;

            // Change sprites
            ammoSprite.sprite = player.GetGun().ammoIcon;
            ammoBarSilhouette.sprite = ammoSprite.sprite;

            // Set size for ammo bars
            ammoBarMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, BULLET_WIDTH * maxMag);
            ammoBarMaskPreview.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, BULLET_WIDTH * maxMag);
            ammoBarSilhouette.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, BULLET_WIDTH * maxMag);

            ammoBarSilhouette.gameObject.SetActive(true);
            UpdateAmmoMask();
            UpdateTotalAmmo();
        }

        /// <summary>
        /// Updates the total ammo amount for the equipped gun
        /// </summary>
        /// 
        /// 2021-06-10  JH  Initial Work
        /// 
        public void UpdateTotalAmmo()
        {
            if (!player.GetGun())
            {
                totalAmmoText.text = "";
                return;
            }
            totalAmmoText.text = player.GetTotalAmmo().ToString();
        }

        /// <summary>
        /// Sets up the preview for ammo change from overload actions
        /// </summary>
        /// <param name="ammoChange">amount of ammo change from actions</param>
        /// 
        /// 2021-07-02  JH  Initial Work
        /// 2021-07-22  JH  Now only called when a gun is equipped
        /// 
        public void PreviewAmmoChange(int ammoChange)
        {
            if (player.GetGun() == null)
                return;
            // Reset ammo masks
            UpdateAmmoMask();
            // Update sprite
            ammoSpritePreview.sprite = ammoSprite.sprite;
            ammoSpritePreview.enabled = true;

            float maskFillAmount = (((float)player.GetCurrentClip() - ammoChange) / player.GetGun().maxMag) <= 0 ? 0f: 
                                        ((float)player.GetCurrentClip() - ammoChange) / player.GetGun().maxMag;

            if (ammoChange > 0)
            {
                // ammo will decrease
                ammoBarMaskPreview.fillAmount = ammoBarMask.fillAmount;
                ammoBarMask.fillAmount = maskFillAmount;
            }
            else
            {
                // ammo will increase
                ammoBarMaskPreview.fillAmount = maskFillAmount;
            }
        }

        /// <summary>
        /// Moves background sprites to the reserve when not in use
        /// Removes from back of the list
        /// </summary>
        /// <param name="size">size to reduce list to</param>
        /// 
        /// 2021-06-08  JH  Initial Work
        /// 2021-06-10  JH  Index to change front to support the FIFO of the imbuement system
        ///                 Also changes alpha to 0
        /// 
        private void ImbueBGToReserve(int size)
        {
            while (imbueBGList.Count > size)
            {
                imbueBGList[0].transform.SetParent(imbueBGReserves.transform);
                imbueBGList[0].SetActive(false); // deactivate object
                ModifyColor.ChangeImageAlpha(imbueBGList[0].GetComponent<Image>(), 0f);
                imbueBGList.RemoveAt(0);
            }
        }

        /// <summary>
        /// Updates the amount of background sprites in the content list
        /// Adds from reserve and creates new ones as necessary
        /// When removing, move to reserve instead of destroying objects
        /// </summary>
        /// <param name="size">size to achieve</param>
        /// 
        /// 2021-06-08  JH  Initial Work
        /// 2021-06-10  JH  BG GameObjects now set to TOP of hierarchy
        ///                 to support the FIFO imbuement system
        /// 2021-06-24  JH  Now updates count when retrieving from the reserves
        /// 
        public void UpdateImbueBGContent(int size)
        {
            int count = imbueBGList.Count;
            // case 1: imbue BG's list < size
            if (count < size)
            {
                // Get from reserves first if any and move to content list
                while (count < size && imbueBGReserves.transform.childCount > 0)
                {
                    GameObject iBG = imbueBGReserves.transform.GetChild(0).gameObject;
                    iBG.SetActive(true); // activate object
                    iBG.transform.SetParent(imbueBGContent.transform);
                    iBG.transform.SetSiblingIndex(0); // Move to top of hierarchy
                    imbueBGList.Add(iBG);
                    count = imbueBGList.Count;
                }
                // Create images until amount matches maximum magazine amout
                for (int i = count; i < size; i++)
                {
                    GameObject newBG = Instantiate(imbueBGTemplate) as GameObject;
                    newBG.transform.SetParent(imbueBGContent.transform);
                    newBG.transform.SetSiblingIndex(0); // Move to top of hierarchy
                    newBG.transform.localScale = new Vector3(1, 1, 1); // fix autoscaling
                    imbueBGList.Add(newBG);
                }
            }
            // case 2: imbue BG's list > size
            else if (count > size)
            {
                ImbueBGToReserve(size);
            }
        }

        /// <summary>
        /// Changes an imbuement background color based on the given index and type
        /// Call when imbuing bullets
        /// </summary>
        /// <param name="index">index of the background sprite to change color</param>
        /// <param name="imbueType">type of imbue effect which determines color</param>
        /// 
        /// 2021-06-08  JH  Initial Work
        /// 
        public void ChangeImbueBGColor(int index, ImbueType imbueType)
        {
            if (index > imbueBGList.Count - 1)
            {
                Debug.LogWarning("Unable to update imbue color, index out of range!");
                return;
            }
            Image imbueBG = imbueBGList[index].GetComponent<Image>();
            // toggle alpha on
            ModifyColor.ChangeImageAlpha(imbueBG, 1f);
            switch (imbueType)
            {
                case ImbueType.Electric:
                    imbueBG.color = Color.magenta;
                    break;
                case ImbueType.Fire:
                    imbueBG.color = Color.red;
                    break;
                case ImbueType.Force:
                    imbueBG.color = Color.yellow;
                    break;
                case ImbueType.Ice:
                    imbueBG.color = Color.cyan;
                    break;
                default:
                    imbueBG.color = new Color(1, 1, 1 ,0);
                    break;
            }
        }

        /// <summary>
        /// Updates the imbue content background position 
        /// Pivot is on right side of image, thus need to move it
        /// for the fill content to go from right to left to match
        /// bullets fired
        /// Call when switching guns / reload / firing
        /// </summary>
        /// 
        /// 2021-06-08  JH  Initial Work
        /// 
        public void UpdateImbueBGPosition()
        {
            RectTransform rT = imbueBGContent.GetComponent<RectTransform>();
            Vector2 newPos = new Vector2(BULLET_RELATIVE_XPOS + (BULLET_WIDTH * player.GetCurrentClip()),
                                rT.anchoredPosition.y);
            rT.anchoredPosition = newPos;
        }

        /// <summary>
        /// Turns all imbuement backgrounds off (alpha = 0),
        /// move them to reserves and clears the background list
        /// Call when switching guns in combat
        /// </summary>
        /// 
        /// 2021-06-08  JH  Initial Work
        /// 
        public void AllImbueBGOff()
        {         
            // revert alpha and move to reserves
            foreach (GameObject gO in imbueBGList)
            {
                // toggle alpha off
                ModifyColor.ChangeImageAlpha(gO.GetComponent<Image>(), 0f);
            }
            ImbueBGToReserve(0); // move all to reserves
            imbueBGList.Clear(); // reset imbuements
        }

        /// <summary>
        /// Sets up the preview for imbue change from overload actions
        /// </summary>
        /// <param name="iTList">list of imbue type's from overload actions</param>
        /// <param name="aIList">list of action info from the overload actions</param>
        /// 
        /// 2021-07-02  JH  Initial Work
        /// 2021-07-08  JH  Now considers attacks when incrementing the index of the
        ///                 imbue preview
        /// 
        public void PreviewImbueChange(List<ImbueType> iTList, List<ActionInfo> aIList)
        {
            int startIndex = player.NumberOfImbuements(); // current amount of imbuements
            int overloadImbueIndex = 0;
            int aIListIndex = 0;
            int attackCount = 0;
            imbuePreviewIndexes.Clear();

            int i = startIndex;
            while (aIListIndex < aIList.Count)
            {
                if (aIList[aIListIndex] is GunActionInfo)
                {
                    if (attackCount < imbueBGList.Count && imbueBGList[attackCount].GetComponent<Image>().color.a == 0)
                    {
                        i++;
                    }
                    attackCount++;
                }
                else if (aIList[aIListIndex] is ImbueActionInfo)
                {
                    imbuePreviewIndexes.Add(i);
                    ChangeImbueBGColor(i++, iTList[overloadImbueIndex++]);

                }
                aIListIndex++;
            }
            if (imbuePreviewIndexes.Count > 0)
                imbuePreview = true;
            else
                imbuePreview = false;
        }

        /// <summary>
        /// Disables the imbue preview changes
        /// </summary>
        /// 
        /// 2021-07-02  JH  Initial Work
        /// 2021-07-08  JH  Now uses the indexes for easier disabling
        /// 
        public void DisableImbuePreviewChange()
        {
            foreach (int index in imbuePreviewIndexes)
                ModifyColor.ChangeImageAlpha(imbueBGList[index].GetComponent<Image>(), 0f);
        }

        /// <summary>
        /// Disables all the resource change previews
        /// </summary>
        /// 
        /// 2021-07-02  JH  Initial Work
        /// 2021-07-08  JH  Now clears indexes of imbue changes
        /// 
        public void DisablePreviews()
        {
            // Health
            healthSliderMask.enabled = false;
            // Energy
            energySliderMask.enabled = false;
            // Ammo
            ammoSpritePreview.enabled = false;
            // Imbue
            imbuePreview = false;
            imbuePreviewIndexes.Clear();
        }

        /// <summary>
        /// Getter method for imbuement background list
        /// </summary>
        /// <returns>list of imbuement background</returns>
        /// 
        /// 2021-07-08  JH  Initial Work
        /// 
        public List<GameObject> GetImbueBGList()
        {
            return imbueBGList;
        }
    }
}
