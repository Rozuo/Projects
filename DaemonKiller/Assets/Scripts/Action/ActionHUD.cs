using System.Collections.Generic;
using UnityEngine;
using Action.HUDButtonTemplate;
using System;
using UnityEngine.UI;
using Unit.Info;
using Action.Info;
using Action.Info.Items;
using Action.Overload;

// reference: http://gregandaduck.blogspot.com/2015/07/unity-ui-dynamic-buttons-and-scroll-view.html

namespace Action.HUDListView
{
    /// <summary>
    /// ActionHUD, displays the information needed to commence an action
    /// and the action's description information.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars         Description
    /// buttonTemplate      template for all buttons created
    /// listViewTransform   rectangle transform of list of actions
    /// canvas              canvas of action HUD
    /// unitNameBG          image background of unit name
    /// unitNameText        text of unit name
    /// listViewBG          image background of list of actions
    /// actionDescriptionBG image background for action info
    /// costText            text for cost of ability
    /// infoText            text for action description
    /// actionsObject       game object to hold the list of actions
    /// scrollbar           scrollbar for the list view of actions
    /// buttonContentList   content list to put button of actions in
    /// buttonReserves      reserve list for button of actions in
    /// actionOverload      overload system manager for actions
    /// 
    /// Private Vars        Description
    /// maxSize             maximum size of listView
    /// target              current selected target for list of actions
    /// actionList          list of all possible actions
    /// buttonList          list of all buttons made from possible actions
    /// arrayOfAllActions   list of all usable actions (even unobtained ones)
    /// blueHUDColor        blue color for the HUD
    /// redHUDColor         red color for the HUD
    /// yellowHUDColor      yellow color for the HUD
    /// blueButtonColor     blue color for the buttons
    /// redButtonColor      red color for the buttons
    /// yellowButtonColor   yellow color for the buttons
    /// whiteButtonColor    white color for the buttons
    /// 
    public class ActionHUD : MonoBehaviour
    {
        [Header("Button Template")]
        public GameObject buttonTemplate;
        [Header("List View Transform")]
        public RectTransform listViewTransform;
        [Header("HUD Elements")]
        public Canvas canvas;
        public Image unitNameBG;
        public Text unitNameText;
        public Image listViewBG;
        public Image actionDescriptionBG;
        public Text costText;
        public Text infoText;
        [Header("Action Object")]
        public GameObject actionsObject;
        [Header("Scrollbar")]
        public Scrollbar scrollbar;
        [Header("Button Content List")]
        public GameObject buttonContentList;
        [Header("Button Reserves")]
        public GameObject buttonReserves;
        [Header("Action Overload")]
        public ActionOverload actionOverload;

        private const int maxSize = 3; // max size of listView
        private UnitInfo target;
        private List<ActionInfo> actionList = new List<ActionInfo>();
        private List<GameObject> buttonList = new List<GameObject>();
        private ActionInfo[] arrayOfAllActions;

        private Color blueHUDColor = new Color(0.0627451f, 0.1333333f, 0.1882353f, 0.7843137f);
        private Color redHUDColor = new Color(0.1960784f, 0, 0, 0.7843137f);
        private Color yellowHUDColor = new Color(0.193658f, 0.1960784f, 0, 0.7843137f);

        private Color blueButtonColor = new Color(0, 0.2901961f, 0.4980392f, 0.7843137f);
        private Color redButtonColor = new Color(1, 0, 0, 0.7843137f); 
        private Color yellowButtonColor = new Color(0.6611321f, 0.6886792f, 0, 0.7843137f); 
        private Color whiteButtonColor = new Color(1, 1, 1, 0.7843137f);

        /// <summary>
        /// Setter method for the target to base list of actions on.
        /// </summary>
        /// <param name="newTarget">new target's unit info to assign</param>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public void SetTarget(UnitInfo newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Getter method for target
        /// </summary>
        /// <returns>Unit Info of the current target</returns>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public UnitInfo GetTarget()
        {
            return target;
        }

        /// <summary>
        /// Clears the button list by putting them in the reserves list
        /// and setting them inactive.
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-06-14  JH  Changed to move buttons to reserve instead of destroying them
        /// 
        private void ClearButtonList()
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].transform.SetParent(buttonReserves.transform);
                buttonList[i].SetActive(false);
            }

            buttonList.Clear();
        }

        /// <summary>
        /// Clears the action list
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        private void ClearActionList()
        {
            actionList.Clear();
        }

        /// <summary>
        /// Stores the array of all actions
        /// </summary>
        /// 
        /// 2021-05-25  JH  Initial Work
        /// 
        private void ObtainAllActions()
        {
            arrayOfAllActions = actionsObject.GetComponentsInChildren<ActionInfo>();
        }

        /// <summary>
        /// Returns a list of all possible actions based on availability such as
        /// if item is acquired, bullets available if it is a gun.
        /// </summary>
        /// <returns>List of all possible actions</returns>
        /// 
        /// 2021-05-25  JH  Initial Work
        /// 2021-05-27  JH  Added checks for each action if they are available to use
        /// 
        private List<ActionInfo> GetAvailableActions()
        {
            // Get actions
            if (arrayOfAllActions.Length == 0)
                ObtainAllActions();

            List<ActionInfo> possibleActions = new List<ActionInfo>();
            // Check each action info if they are available to use
            foreach (ActionInfo aI in arrayOfAllActions)
                if (aI.ActionAvailable())
                    possibleActions.Add(aI);

            return possibleActions;
        }

        /// <summary>
        /// Returns the maximum possible range from the player's list of actions
        /// </summary>
        /// <returns>maximum range from list of actions that are available. Minimum is zero</returns>
        /// 
        /// 2021-05-25  JH  Initial Work
        /// 2021-05-27  JH  Changed to get max range from possible actions
        /// 
        public float GetMaxRange()
        {
            List<ActionInfo> possibleActions = GetAvailableActions();
            float maxRange = 0;
            for (int i = 0; i < possibleActions.Count; i++)
                maxRange = Mathf.Max(possibleActions[i].GetRange(), maxRange);

            return maxRange;
        }
        
        /// <summary>
        /// Updates the action list with possible action based on target
        /// </summary>
        /// <param name="unitType">unit type to determine actions possible</param>
        /// <param name="unitDistance">distance between the player and the target</param>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-05-25  JH  Add checking distance for possible actions
        /// 2021-05-27  JH  Changed to get list from available actions
        /// 2021-06-16  JH  Enum changed to flags, checking updated
        /// 
        private void UpdateActionList(UnitType unitType, float unitDistance)
        {
            List<ActionInfo> possibleActions = GetAvailableActions();
            for (int i = 0; i < possibleActions.Count; i++)
            {
                if (possibleActions[i].GetTargetType().HasFlag(unitType)
                    && possibleActions[i].GetRange() >= unitDistance)
                {
                    actionList.Add(possibleActions[i]);
                }
            }
        }

        /// <summary>
        /// Updates button list by creating a button
        /// for each action in the action list
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-05-21  JH  Add naming for items
        /// 2021-05-27  JH  Add naming scheme for action type ITEM
        /// 2021-05-28  JH  Uses parameter to allow for generic use
        /// 2021-06-14  JH  Now uses reserve system for the buttons instead of
        ///                 always creating a new button
        /// 2021-06-16  JH  Now updates item names again
        /// 
        private void UpdateButtonList(List<ActionInfo> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                // Reset button reserve 
                int reserveCount = buttonReserves.transform.childCount;
                bool buttonMatch = false;

                // Check reserves if button already made for button
                if (reserveCount > 0)
                {
                    for (int j = 0; j < reserveCount; j++)
                    {
                        ActionButtonTemplate reserveButton = buttonReserves.transform.GetChild(j).GetComponent<ActionButtonTemplate>();
                        if (reserveButton.actionInfo == actions[i])
                        {
                            // button match found, add to list
                            buttonMatch = true;
                            if (actions[i].actionType == ActionType.Item)
                            {
                                reserveButton.SetName(actions[i].GetComponent<ItemActionInfo>().ToString());
                            }
                            reserveButton.transform.SetParent(buttonContentList.transform);
                            reserveButton.gameObject.SetActive(true);
                            buttonList.Add(reserveButton.gameObject);
                            break;
                        }
                    }
                }

                // No match, create new button
                if (buttonMatch == false)
                {
                    GameObject newButton = Instantiate(buttonTemplate) as GameObject;
                    newButton.SetActive(true);
                    ActionButtonTemplate buttonT = newButton.GetComponent<ActionButtonTemplate>();
                    if (actions[i].actionType == ActionType.Item)
                        buttonT.SetName(actions[i].GetComponent<ItemActionInfo>().ToString());
                    else
                        buttonT.SetName(actions[i].actionName);
                    buttonT.ActionInfo = actions[i];
                    newButton.transform.SetParent(buttonContentList.transform);
                    newButton.transform.localScale = new Vector3(1, 1, 1); // buttons autoscale, this fixes that
                    buttonList.Add(newButton);
                }
            }
        }
        /// <summary>
        /// Updates the action HUD with its actions based on the 
        /// target unit's UnitInfo.
        /// Fills the action and button lists for all possible actions based on
        /// target type and range within player.
        /// </summary>
        /// <param name="target">target to base possible actions from</param>
        /// <param name="targetDistance">distance between the player and the target</param>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-05-21  JH  Moved HUD color changes to separate function
        /// 2021-05-25  JH  Add new parameter for distance checking of enemy
        /// 2021-05-28  JH  UpdateButtonList now passes actionList
        /// 2021-06-03  JH  UpdateScrollbar is added
        /// 2021-06-16  JH  Button color now updates
        /// 
        public void UpdateHUD(UnitInfo target, float targetDistance)
        {
            ClearActionList();
            ClearButtonList();
            unitNameText.text = target.unitData.name;
            ChangeHUDColor(target.GetUnitType());

            UpdateActionList(target.GetUnitType(), targetDistance);
            UpdateButtonList(actionList);
            ChangeButtonColor(target.GetUnitType());
            UpdateScrollbar(actionList.Count);

            ChangeListSize();
            SetTarget(target);
        }

        /// <summary>
        /// Updates the action HUD with contents of a submenu.
        /// </summary>
        /// <param name="actionInfo">action info of button to determine its type</param>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public void UpdateHUDSubMenu(ActionInfo actionInfo)
        {
            ClearButtonList();
            SubButtonList((ActionType) Enum.Parse(typeof(ActionType), actionInfo.actionName)); // get ActionType from its name
            ChangeListSize();
        }

        /// <summary>
        /// Creates buttons for the submenu based on the action type.
        /// Note, this does not consider availability of the action.
        /// </summary>
        /// <param name="actionType">action type to make sub menu from</param>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 2021-06-03  JH  UpdateScrollbar is added
        /// 2021-06-16  JH  Button color now updates
        /// 
        private void SubButtonList(ActionType actionType)
        {
            List<ActionInfo> newList = new List<ActionInfo>();
            foreach (ActionInfo aI in arrayOfAllActions)
                if (aI.actionType == actionType)
                    newList.Add(aI);

            UpdateButtonList(newList);
            ChangeButtonColor(target.GetUnitType());
            UpdateScrollbar(newList.Count);
        }

        /// <summary>
        /// Changes HUD color based on the unit info of the target
        /// </summary>
        /// <param name="targetType">unit type of the target</param>
        /// 
        /// 2021-05-21  JH  Initial Work
        /// 2021-06-16  JH  Add environment target color
        /// 
        private void ChangeHUDColor(UnitType targetType)
        {
            switch (targetType)
            {
                case UnitType.Player:
                    listViewBG.color = blueHUDColor;
                    unitNameBG.color = blueHUDColor;
                    actionDescriptionBG.color = blueHUDColor;
                    break;
                case UnitType.Enemy:
                    listViewBG.color = redHUDColor;
                    unitNameBG.color = redHUDColor;
                    actionDescriptionBG.color = redHUDColor;
                    break;
                case UnitType.Environment:
                    listViewBG.color = yellowHUDColor;
                    unitNameBG.color = yellowHUDColor;
                    actionDescriptionBG.color = yellowHUDColor;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Changes button color in the button list.
        /// Blue for player, red for enemy, yellow for environment.
        /// </summary>
        /// <param name="targetType">target's unit type</param>
        /// 
        /// 2021-06-16  JH  Initial Work
        /// 
        private void ChangeButtonColor(UnitType targetType)
        {
            Color color;  
            switch (targetType)
            {
                case UnitType.Player:
                    color = blueButtonColor;
                    break;
                case UnitType.Enemy:
                    color = redButtonColor;
                    break;
                case UnitType.Environment:
                    color = yellowButtonColor;
                    break;
                default:
                    // for new UnitTypes until a color is assigned
                    color = whiteButtonColor;
                    break;
            }
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].GetComponent<Image>().color = color;
            }
        }

        /// <summary>
        /// Changes the list size based on possilbe actions
        /// with a max size. So HUD doesn't cover unnecessary space.
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        private void ChangeListSize()
        {
            listViewTransform.sizeDelta = new Vector2(130, Math.Min(buttonList.Count, maxSize) * 25);
        }

        /// <summary>
        /// Activates action description background
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public void ActionDescBGOn()
        {
            actionDescriptionBG.gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivates action description background
        /// </summary>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        public void ActionDescBGOff()
        {
            actionDescriptionBG.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the action description information with
        /// the action's cost and description.
        /// </summary>
        /// <param name="action">action to set description with</param>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 2021-05-28  JH  Add case for sub menus (no cost text at all)
        /// 2021-07-21  JH  Changed to show action cost first, then energy cost
        /// 
        public void SetActionDesc(ActionInfo action)
        {
            if (action.actionCost == 0 && action.energyCost != 0)
            {
                costText.text = "COST: " + action.energyCost + " ENRG";
            }
            else if (action.actionCost != 0 && action.energyCost == 0)
            {
                costText.text = "COST: " + action.actionCost + "% ACTION";
            }
            else if (action.actionCost == 0 && action.energyCost == 0)
            {
                // for sub menus
                costText.text = "";
            }
            else
            {
                // action costs both
                costText.text = "COST: " + action.actionCost + "% ACTION / " + 
                    action.energyCost + " ENRG";
            }
            infoText.text = " INFO: " + action.actionDescription;
        }

        /// <summary>
        /// Sets the overload cost in the action description
        /// </summary>
        /// <param name="action">action to check if it is a sub menu</param>
        /// 
        /// 2021-07-14  JH  Initial Work.
        /// 
        public void SetOverloadCost(ActionInfo action)
        {
            if (!actionOverload.IsListEmpty() && action.actionType != ActionType.Menu)
                costText.text += " (+" + actionOverload.NextOverloadEnergyCost() + " ENRG)";
        }

        /// <summary>
        /// Sets up each action to be made into a button
        /// </summary>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public void SetUpActions()
        {
            ObtainAllActions();
            foreach (ActionInfo aI in arrayOfAllActions)
            {
                aI.SetUpAction();
            }
        }

        /// <summary>
        /// Sets the number of steps in the scroll bar
        /// </summary>
        /// <param name="numOfActions">number of actions in the list</param>
        /// 
        /// 2021-06-03  JH  Initial Work
        /// 
        private void UpdateScrollbar(int numOfActions)
        {
            scrollbar.numberOfSteps = numOfActions - 2;
        }
    }
}

