using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Action.HUDListView;
using Unit.Info;
using Unit.ColorChange;

namespace Action.Gauge
{
    /// <summary>
    /// Manages the action gauge regeneration rate and its UI elements.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars             Description
    /// actionBar               action bar (fillable) of HUD
    /// actionBarBackground     action bar background of HUD
    /// actionBarShell          outline of action bar of HUD
    /// actionBarMask           action bar mask for flashing preview
    /// chargeIcon              action icon image
    /// chargingSprite          sprite for when charging action bar
    /// chargedSprite           sprite for when action bar is full
    /// chargeText              text for action bar
    /// interactText            text with instructions for user
    /// actionHUD               HUD to call when starting targeting
    /// 
    /// Private Vars        
    /// gameManager             gameManager to determine game state
    /// changedHUD              bool if HUD has changed
    /// maxAction               maximum amount of action
    /// currentAction           current amount of action
    /// actionRegenRate         regeneration rate of action
    /// barDefaultColor         default color of action bar
    /// barBGDefaultColor       default color of BG action bar
    /// barShellDefaultColor    default color of shell of action bar
    /// barReadyColor           color fillable bar when action bar is charged
    /// barBGReadyColor         color of BG action bar when action bar is charged
    /// barShellReadyColor      color of shell when action bar is charged
    ///
    public class ActionGauge : MonoBehaviour
    {
        [Header("HUD Elements")]
        public Image actionBar;
        public Image actionBarBackground;
        public Image actionBarShell;
        public Image actionBarMask;
        public Image chargeIcon;
        public Sprite chargingSprite;
        public Sprite chargedSprite;
        public Text chargeText;
        public Text interactText;
        [Header("ActionHUD")]
        public ActionHUD actionHUD;

        private GameManager gameManager;

        private bool changedHUD = false;

        private float maxAction = 100f;
        private float currentAction;
        private float actionRegenRate = 20f;

        private Color barDefaultColor;
        private Color barBGDefaultColor;
        private Color barShellDefaultColor;

        private Color barReadyColor = new Color(0, 0.8039216f, 0.8039216f);
        private Color barBGReadyColor = new Color(0, 0.5f, 0.5f);
        private Color barShellReadyColor = new Color(0.2f, 1f, 1f);

        /// <summary>
        /// Initializes values
        /// </summary>
        ///
        /// 2021-05-14  JH  Initial work
        /// 2021-05-20  JH  Add targeting reference
        /// 2021-06-16  JH  Removed targeting reference (moved to Targeting)
        ///                 Add game manager reference
        ///
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            actionHUD.gameObject.SetActive(false);
            currentAction = 0f;
            barDefaultColor = actionBar.color;
            barBGDefaultColor = actionBarBackground.color;
            barShellDefaultColor = actionBarShell.color;
        }
        /// <summary>
        /// Activates targeting and sets up actions
        /// </summary>
        /// 
        /// 2021-05-26  JH  Initial Work
        /// 2021-06-16  JH  Removed targeting activation
        /// 
        void OnEnable()
        {
            actionHUD.SetUpActions();
        }

        /// <summary>
        /// Reset action gauge values and disables the targeting object
        /// </summary>
        /// 2021-05-21  JH  Initial Work
        /// 2021-05-26  JH  Add disabling of targeting object
        /// 2021-06-16  JH  Removed targeting deactivation
        /// 
        void OnDisable()
        {
            ResetAction(); // restart action and HUD upon leaving combat
        }

        /// <summary>
        /// Determines if action gauge is full
        /// </summary>
        /// <returns>true if action gauge is full, false otherwise</returns>
        /// 
        /// 2021-05-20  JH  Inital Work
        /// 
        public bool IsActionFull()
        {
            if (currentAction >= maxAction)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Add action value to action gauge
        /// </summary>
        /// <param name="value">amount of action to add</param>
        /// 
        /// 2021-05-20  JH  Inital Work
        /// 2021-05-27  JH  Removed Subtract Action, moved its content here
        /// 
        public void AddAction(float value)
        {
            currentAction += value;
            if (currentAction >= maxAction)
            {
                currentAction = maxAction;
            }
            else if (currentAction <= 0f)
            {
                currentAction = 0f;
            }
        }

        /// <summary>
        /// Called once per frame.
        /// Regenerates action bar when not full and once full, changes the HUDs appearance.
        /// </summary>
        ///
        /// 2021-05-14  JH  Initial work
        /// 2021-05-20  JH  Moved actionHUD activation to targeting
        /// 2021-06-30  JH  Add time scale to state checking and 
        ///                 action bar flashing for overload
        ///
        void Update()
        {
            if (gameManager.GameStatus == GameState.Danger && Time.timeScale != 0)
            {
                if (!IsActionFull())
                {
                    if (changedHUD)
                        ChangeHUD();

                    PassiveActionRegen();
                    actionBar.fillAmount = currentAction / maxAction; // update UI
                }
                else if (IsActionFull() && changedHUD == false)
                {
                    ChangeHUD();
                }
            }

            if (actionBarMask.enabled)
            {
                float alpha = Mathf.Lerp(0f, 1f, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 3)));

                ModifyColor.ChangeImageAlpha(actionBarMask, alpha);
            }
        }
        /// <summary>
        /// Regenerates action based on the regeneration rate
        /// </summary>
        ///
        /// 2021-05-14  JH  Initial work
        ///
        private void PassiveActionRegen()
        {
            currentAction += actionRegenRate * Time.deltaTime;
            if (currentAction > maxAction)
            {
                currentAction = maxAction;
            }
        }

        /// <summary>
        /// Changes HUD to the alternate version,
        /// blue if charged, red if not charged
        /// </summary>
        ///
        /// 2021-05-14 JH Initial work
        ///
        private void ChangeHUD()
        {
            if (actionBar.color == barDefaultColor)
            {
                actionBar.color = barReadyColor;
                actionBarBackground.color = barBGReadyColor;
                actionBarShell.color = barShellReadyColor;
                chargeIcon.sprite = chargedSprite;
                chargeText.text = "CHARGED";
                interactText.gameObject.SetActive(true);
                changedHUD = true;
            }
            else if (actionBar.color != barDefaultColor)
            {
                actionBar.color = barDefaultColor;
                actionBarBackground.color = barBGDefaultColor;
                actionBarShell.color = barShellDefaultColor;
                chargeIcon.sprite = chargingSprite;
                chargeText.text = "CHARGING";
                interactText.gameObject.SetActive(false);
                changedHUD = false;
            }
        }

        /// <summary>
        /// Resets action bar to zero and changes HUD if necessary
        /// </summary>
        ///
        /// 2021-05-20 JH Initial work
        ///
        private void ResetAction()
        {
            currentAction = 0;
            if (changedHUD == true)
            {
                ChangeHUD();
            }
        }

        /// <summary>
        /// Activates the action HUD and updates the 
        /// HUD with the unit info
        /// </summary>
        /// <param name="unitInfo">unit information to set to HUD</param>
        ///
        /// 2021-05-14  JH  Initial work
        /// 2021-05-20  JH  Add HUD update with unit info
        ///
        public void ActivateActionHUD(UnitInfo unitInfo, float unitDistance)
        {
            if (actionHUD.gameObject.activeSelf == false)
            {
                actionHUD.gameObject.SetActive(true);
            }
            actionHUD.UpdateHUD(unitInfo, unitDistance);
            actionHUD.ActionDescBGOff();
        }

        /// <summary>
        /// Deactivates the action HUD 
        /// </summary>
        ///
        /// 2021-05-14  JH  Initial work
        ///
        public void DeactivateActionHUD()
        {
            actionHUD.gameObject.SetActive(false);
        }

        /// <summary>
        /// Previews the action bar change for overload
        /// </summary>
        /// <param name="actionChange">amount of action to add to the bar</param>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void PreviewActionChange(float actionChange)
        {
            // reset action bar
            actionBar.fillAmount = currentAction / maxAction;

            actionBarMask.enabled = true;
            float maskFillAmount = ((currentAction + actionChange) / maxAction) <= 0 ? 0f : (currentAction + actionChange) / maxAction;


            if (actionChange >= 0)
                // energy will increase
                actionBarMask.fillAmount = maskFillAmount;
            else
            {
                // energy will decrease
                actionBarMask.fillAmount = actionBar.fillAmount;
                actionBar.fillAmount = maskFillAmount;
            }
        }

        /// <summary>
        /// Disables the action bar mask for 
        /// previewing action resource changes
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void DisablePreview()
        {
            actionBarMask.enabled = false;
        }
    }
}