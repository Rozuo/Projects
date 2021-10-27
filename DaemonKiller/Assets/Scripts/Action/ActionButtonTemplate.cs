using UnityEngine;
using UnityEngine.UI;
using Action.HUDListView;
using Action.Info;
using UnityEngine.EventSystems;
using Action.Target;
using Unit.Info.Player;
using Action.Overload;

// reference: http://gregandaduck.blogspot.com/2015/07/unity-ui-dynamic-buttons-and-scroll-view.html
// reference for on highlight/select: https://answers.unity.com/questions/950500/if-button-highlighted.html
namespace Action.HUDButtonTemplate
{
    /// <summary>
    /// Button template for actions to use in battle
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// buttonText      text for button (name)
    /// actionHUD       HUD to add button to
    /// actionInfo      information of action
    /// targeting       targeting system to work with
    /// actionOverload  overload system
    /// 
    /// Private Vars    
    /// name            name of action
    /// 
    public class ActionButtonTemplate : MonoBehaviour, IPointerEnterHandler
    {
        public Text buttonText;
        public ActionHUD actionHUD; // scroll view to add to
        public ActionInfo actionInfo;
        public Targeting targeting;
        public ActionOverload actionOverload;

        private new string name;

        /// <summary>
        /// Initialize targeting
        /// </summary>
        /// 
        /// 2021-06-29  JH  Initial Work
        /// 
        void Start()
        {
            targeting = GameObject.FindGameObjectWithTag("Targeting").GetComponent<Targeting>();
        }

        /// <summary>
        /// Setter and getter method for actionInfo
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public ActionInfo ActionInfo
        {
            get { return actionInfo; }
            set { actionInfo = value; }
        }

        /// <summary>
        /// Sets the name for the button and updates its text UI
        /// </summary>
        /// <param name="newName">name of the action </param>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public void SetName(string newName)
        {
            name = newName;
            buttonText.text = newName;
        }
        /// <summary>
        /// Functionality of button on click. 
        /// Commences action based on its actionInfo,
        /// disables the actionHUD and ends targeting
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-05-28  JH  Added action when clicking a submenu button
        /// 2021-06-04  JH  Add energy check, no behaviour yet
        /// 2021-06-08  JH  Add imbuement number check, no behaviour yet
        /// 2021-06-29  JH  Add behaviour for overload state
        /// 2021-07-02  JH  Remove overload state behaviour. 
        ///                 New behaviour is to add the action to the queue 
        /// 2021-07-04  JH  Added equals to the health check for an edge case
        /// 2021-07-21  JH  ButtonHighlight added at the end to update ActionUI after clicking
        /// 2021-07-22  JH  Energy checking for energy items is now in place.
        /// 
        public void ButtonClick()
        {
            PlayerInfo player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
            if (actionInfo.actionType == ActionType.Menu)
                actionHUD.UpdateHUDSubMenu(actionInfo);
            else if (!actionOverload.CheckEnergyCost(actionInfo))
            {
                // insufficient energy, do nothing (play sound to notify not enough energy)
            }
            else if (actionInfo.actionType == ActionType.Imbue && !actionOverload.CheckImbuements(actionInfo))
            {
                // imbued all bullets, (play sound to notify max imbuements reached)
            }
            else if (actionInfo.actionType == ActionType.Attack && !actionOverload.CheckAmmoCost(actionInfo))
            {
                // no bullets left, cannot attack
            }
            else if (actionInfo.actionType == ActionType.Item && actionInfo.actionValue > 0 && (!actionOverload.CheckItem(actionInfo)
                        || actionOverload.OverloadHealthChange() + player.currentHealth >= player.maxHealth))
            {
                // healing item, no item stock left OR health currently full
            }
            else if (actionInfo.actionType == ActionType.Item && actionInfo.actionValue == 0 && (!actionOverload.CheckItem(actionInfo)
                        || -actionOverload.OverloadEnergyCost() + player.currentEnergy >= player.maxEnergy))
            {
                // energy restoration item, no item stock left OR energy currently full
            }
            else
            {
                // add action to queue
                actionOverload.AddOverloadAction(actionInfo, actionHUD.GetTarget());
                ButtonHighlight();
            }
        }

        /// <summary>
        /// Behaviour when mouse hovers over the button
        /// </summary>
        /// <param name="eventData">pointer event that occurred</param>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public void OnPointerEnter(PointerEventData eventData)
        {
            ButtonHighlight();
        }

        /// <summary>
        /// Highlights the button and displays its action information
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-07-14  JH  Added overload cost
        /// 
        private void ButtonHighlight()
        {
            actionHUD.SetActionDesc(actionInfo);
            actionHUD.SetOverloadCost(actionInfo);
            actionHUD.ActionDescBGOn();
        }
    }
}
