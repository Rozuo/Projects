using System.Collections;
using Action.Gauge;
using Action.Info;
using Action.Info.Guns;
using Action.Info.Imbues;
using Action.Info.Items;
using System.Collections.Generic;
using Unit.Info;
using Unit.Info.Player;
using Unit.Player;
using UnityEngine;
using UnityEngine.UI;
using Audio;


namespace Action.Overload
{
    /// <summary>
    /// Overload system for combat. Allows the player to use multiple actions
    /// for a single action at the expense of extra energy. 
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars             Description
    /// actionGauge             action gauge meter for an action
    /// overloadHeader          header for overload UI
    /// overloadContent         content list for the text of overload actions
    /// overloadTextTemplate    overload action template to store enemy name and action info
    /// pHUD                    player HUD that manages player UI
    /// 
    /// Private Vars
    /// actionList              list of actions for overload
    /// targetList              list of targets for overload
    /// overloadTextList        list of overload texts that stores enemy name and action info
    /// inventory               player's inventory to access item information
    /// player                  player data information
    /// maxOverload             maximum amount of overload actions possible in a single action
    /// baseOverloadCost        base cost for overloading (2 or more actions)
    /// 
    public class ActionOverload : MonoBehaviour
    {
        public ActionGauge actionGauge;
        public GameObject overloadHeader;
        public GameObject overloadContent;
        public GameObject overloadTextTemplate;
        public PlayerHUD pHUD;

        private List<ActionInfo> actionList = new List<ActionInfo>();
        private List<UnitInfo> targetList = new List<UnitInfo>();
        private List<GameObject> overloadTextList = new List<GameObject>();

        private Inventory inventory;
        private PlayerInfo player;
        private int maxOverload = 8;
        private float baseOverloadCost = 5;

        /// <summary>
        /// Initialize references
        /// </summary>
        /// 
        /// 2021-07-28  Initial Documentation
        /// 
        void Start()
        {
            inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        }

        /// <summary>
        /// Deactivates the overload header and
        /// disables the resource previews
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void DeactivateOverload()
        {
            //actionState = ActionState.Normal;
            overloadHeader.SetActive(false);
            RemoveAllOverloadActions();
            pHUD.DisablePreviews();
            actionGauge.DisablePreview();
        }

        /// <summary>
        /// Adds an action to the overload list.
        /// Also activates the header if it is not activated yet.
        /// </summary>
        /// <param name="aI">action info to apply on the target</param>
        /// <param name="uI">unit info of the action's target</param>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void AddToList(ActionInfo aI, UnitInfo uI)
        {
            if (actionList.Count == 0 && !overloadHeader.activeSelf)
                overloadHeader.SetActive(true);
            actionList.Add(aI);
            targetList.Add(uI);

            ShowUIChanges();
        }

        /// <summary>
        /// Activates all actions in the overload.
        /// Player resources are expended if actions are used, and 
        /// the overload queue is cleared.
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 2021-07-02  JH  Added overload energy cost to expend
        /// 
        public void ActivateActions()
        {
            pHUD.DisableImbuePreviewChange();
            int actionsUsed = 0;
            for (int i = 0; i < actionList.Count; i++)
            {
                ActionInfo aI = actionList[i];
                UnitInfo target = targetList[i];
                if (aI.CommenceAction(target))
                {
                    actionsUsed += 1;
                }
                Destroy(overloadTextList[i]);
                
            }
            for (int i = 0; i < actionsUsed; i++)
                player.AddEnergy(-(i * baseOverloadCost));


            // Clear the lists
            actionList.Clear();
            targetList.Clear();
            overloadTextList.Clear();

            // Disable resource previews
            pHUD.DisablePreviews();
            actionGauge.DisablePreview();
        }

        /// <summary>
        /// Checks the energy cost of the overload queue 
        /// along with the action info given
        /// </summary>
        /// <param name="aI">action info to check</param>
        /// <returns>true if energy cost is lower than player's current energy
        ///          false otherwise</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 2021-07-02  JH  Now considers the overload energy cost
        /// 
        public bool CheckEnergyCost(ActionInfo aI)
        {
            float aIOverloadEnergyCost = actionList.Count * baseOverloadCost;
            if (OverloadEnergyCost() + aI.energyCost + aIOverloadEnergyCost < player.currentEnergy)
                return true;
            return false;
        }

        /// <summary>
        /// Checks the ammo cost of the overload queue
        /// along with the action info given
        /// </summary>
        /// <param name="aI">action info to check with</param>
        /// <returns>true if ammo cost is lower than player's clip
        ///          false otherwise</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public bool CheckAmmoCost(ActionInfo aI)
        {
            if (aI.actionType == ActionType.Attack)
                if (OverloadAmmoCost() < player.GetCurrentClip())
                    return true;
            return false;
        }

        /// <summary>
        /// Checks the imbuement availablity of the overload along with
        /// the action info given
        /// </summary>
        /// <param name="aI">action info to check with</param>
        /// <returns>true if imbuement is possible. false otherwise</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 2021-07-08  JH  Now considers attacks in queue when checking imbue
        /// 
        public bool CheckImbuements(ActionInfo aI)
        {
            if (aI.actionType == ActionType.Imbue)
            {
                int toImbueIndex = player.NumberOfImbuements();
                int actionListIndex = 0;
                int attackIndex = 0;

                while (actionListIndex < actionList.Count)
                {
                    if (actionList[actionListIndex] is GunActionInfo)
                    {
                        if (attackIndex == toImbueIndex)
                            toImbueIndex++;
                        attackIndex++;
                    }
                    else if (actionList[actionListIndex] is ImbueActionInfo)
                    {
                        toImbueIndex++;
                    }
                    actionListIndex++;
                }
                if (toImbueIndex < player.GetCurrentClip() 
                    && pHUD.GetImbueBGList()[toImbueIndex].GetComponent<Image>().color.a == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks the item amount of the overload queue 
        /// along with the action info given
        /// </summary>
        /// <param name="aI">action info to check with</param>
        /// <returns>true if item amount is lower than amount in the inventory
        ///          false otherwise</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public bool CheckItem(ActionInfo aI)
        {
            if (aI.actionType == ActionType.Item)
            {
                int itemCount = 0;
                foreach (ActionInfo iAI in actionList)
                    if (iAI is ItemActionInfo && iAI.actionName == aI.actionName)
                        itemCount += 1;
                if (inventory.Count(((ItemActionInfo)aI).item) != -1 && itemCount < inventory.Count(((ItemActionInfo)aI).item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the amount of health change to the player from
        /// the overload actions
        /// </summary>
        /// <returns>health change to the player. Positive if healing, negative if damaged</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public float OverloadHealthChange()
        {
            float healthChange = 0;
            foreach (ActionInfo iAI in actionList)
                if (iAI is ItemActionInfo && iAI.targetType == UnitType.Player)
                    healthChange += ((ItemActionInfo)iAI).item.healAmount;
            return healthChange;
        }

        /// <summary>
        /// Gets amount of ammo to be used in the overload actions
        /// </summary>
        /// <returns>amount of ammo to use for the gun equipped</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public int OverloadAmmoCost()
        {
            int bulletCount = 0;
            foreach (ActionInfo gAI in actionList)
                if (gAI is GunActionInfo)
                    bulletCount += 1;
            return bulletCount;
        }

        /// <summary>
        /// Gets the amount of action to be used in the overload actions
        /// </summary>
        /// <returns>amount of action to expend</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public float OverloadActionCost()
        {
            float actionCost = 0;
            foreach (ActionInfo aI in actionList)
                actionCost += aI.actionCost;
            return actionCost;
        }

        /// <summary>
        /// Gets the energy cost of the overload actions
        /// </summary>
        /// <returns>amount of energy to expend</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 2021-07-02  JH  Now considers the overload energy cost
        /// 2021-07-22  JH  Now checks the energy gain from items.
        /// 
        public float OverloadEnergyCost()
        {
            float energyCost = 0;
            float overloadCost = 0;
            for (int i = 0; i < actionList.Count; i++)
            {
                energyCost += actionList[i].energyCost;
                overloadCost += i * baseOverloadCost; // ignore first since index is 0, scales as more actions are put in
            }
            energyCost += overloadCost;

            // check energy recovery items
            foreach (ActionInfo iAI in actionList)
                if (iAI is ItemActionInfo && iAI.targetType == UnitType.Player)
                    energyCost -= ((ItemActionInfo)iAI).item.energyAmount; // -'ve because 'restoring energy'
            return energyCost;
        }

        /// <summary>
        /// Gets the overload energy cost for adding another action.
        /// </summary>
        /// <returns>overload energy cost for the next action</returns>
        /// 
        /// 2021-07-14  JH  Initial Work
        /// 
        public float NextOverloadEnergyCost()
        {
            return actionList.Count * baseOverloadCost;
        }

        /// <summary>
        /// Returns the list of imbue types from all the imbue action infos
        /// in the action list
        /// </summary>
        /// <returns>a list of imbue types from the action list</returns>
        /// 
        /// 2021-07-02  JH  Initial Work
        /// 
        public List<ImbueType> OverloadImbueChange()
        {
            List<ImbueType> iEList = new List<ImbueType>();
            foreach (ActionInfo aI in actionList)
                if (aI is ImbueActionInfo)
                    iEList.Add(((ImbueActionInfo)aI).GetImbuementType());
            return iEList;
        }

        /// <summary>
        /// Adds an action and its target to the overload list
        /// Also adds the text with the action's info to the UI
        /// </summary>
        /// <param name="aI">action info to apply to target</param>
        /// <param name="uI">target of the action</param>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void AddOverloadAction(ActionInfo aI, UnitInfo uI)
        {
            if (actionList.Count < maxOverload)
            {
                GameObject newAction = Instantiate(overloadTextTemplate) as GameObject;
                newAction.transform.SetParent(overloadContent.transform);
                newAction.GetComponent<Text>().text = uI.unitData.name + " | " + aI.actionName;
                newAction.SetActive(true);
                newAction.transform.localScale = new Vector3(1, 1, 1); // fix autoscaling
                overloadTextList.Add(newAction);

                AddToList(aI, uI);
            }
            else Debug.Log("Max overload actions of " + maxOverload + " reached");
        }

        /// <summary>
        /// Removes a single overload action from the queue without using them
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void RemoveOverloadAction()
        {
            int index = actionList.Count - 1;
            if (actionList[index] is ImbueActionInfo)
                pHUD.DisableImbuePreviewChange();
            actionList.RemoveAt(index);
            targetList.RemoveAt(index);
            Destroy(overloadTextList[index]);
            overloadTextList.RemoveAt(index);

            /// Update values
            ShowUIChanges();
        }

        /// <summary>
        /// Removes all overload actions from the queue without using them
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void RemoveAllOverloadActions()
        {
            for (int i = 0; i < overloadTextList.Count; i++)
                Destroy(overloadTextList[i]);
            actionList.Clear();
            targetList.Clear();
            overloadTextList.Clear();

            // Update values
            ShowUIChanges();
        }

        /// <summary>
        /// Checks if overload list of actions is empty
        /// </summary>
        /// <returns>true if list is empty, false otherwise</returns>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public bool IsListEmpty()
        {
            if (actionList.Count == 0)
                return true;
            return false;
        }

        /// <summary>
        /// Show the UI preview changes to the player
        /// Flashes the change in resources periodically
        /// </summary>
        /// 
        /// 2021-06-30  JH  Initial Work
        /// 
        public void ShowUIChanges()
        {
            /// Show change in health
            pHUD.PreviewHealthChange(OverloadHealthChange());
            /// Show change in energy
            pHUD.PreviewEnergyChange(-OverloadEnergyCost());
            /// Show change in action
            actionGauge.PreviewActionChange(-OverloadActionCost());
            /// Show change in ammo
            pHUD.PreviewAmmoChange(OverloadAmmoCost());
            /// Show change in imbuement
            pHUD.PreviewImbueChange(OverloadImbueChange(), actionList);
        }
    }
}