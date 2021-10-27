using System.Collections;
using System.Collections.Generic;
using Unit.Info.Player;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The User Interface portion of the Inventory
/// </summary>
/// 
/// Author: Tyson Hoang (TH), Jacky Huynh (JH)
/// 
/// public var      desc
/// inv             reference to the Inventory class
/// gridGroup       reference to the grid content list
/// invButton       reference to the button template for the UI
/// scrollbar       reference to the grid content's scroll bar
/// 
/// private var     desc
/// buttonList      holds references to all inventory buttons
/// player          player to access ammo clips
/// 
/// constants       desc
/// MAX_SIZE        max number of buttons held in the grid content list
/// 
public class InventoryUI : MonoBehaviour
{
    public Inventory inv;
    public GameObject gridGroup;
    public GameObject invButton;
    public Scrollbar scrollbar;

    [SerializeField] private List<GameObject> buttonList;
    private PlayerInfo player;

    private const int MAX_SIZE = 5;

    /// <summary>
    /// Initialize reference
    /// </summary>
    /// 
    /// 2021-07-21  JH  Initial Implementation
    /// 
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
    }

    /// <summary>
    /// Refresh information for all inventory buttons
    /// </summary>
    /// 
    /// 2021-05-11  TH  Initial Documentation
    /// 2021-07-30  JH  Scrollbar support added
    /// 
    public void RefreshList()
    {
        foreach (GameObject button in buttonList)
            UpdateSlotData(button);

        if (buttonList.Count > 5)
            scrollbar.gameObject.SetActive(true);
        else
            scrollbar.gameObject.SetActive(false);

        UpdateSBSteps();
    }

    /// <summary>
    /// Updates data for a button
    /// </summary>
    /// <param name="child">GameObject of slot to update</param>
    /// 
    /// 2021-05-11  TH  Initial Documentation
    /// 2021-07-21  JH  Case for guns due to ammo clips being held by the player
    /// 2021-07-26  TH  Added additonal check for PlayerInfo component
    /// 2021-07-30  JH  Switch case used instead of an if-else statement
    /// 
    private void UpdateSlotData(GameObject child)
    {   
        InventoryButton c = child.GetComponent<InventoryButton>();

        // check if PlayerInfo exists, otherwise try to get component again
        if (!player)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();

        if (inv.HasInventoryItem(c.GetIndex()))
        {
            InventorySlot invD = inv.GetInvDetails(c.GetIndex());

            // update slot text
            switch (invD.itemDetails.itemType)
            {
                case ItemType.Weapon:
                    c.SetText("x" + (invD.GetAmount() + player.GetCurrentClip(invD.itemID)) + " " + invD.itemDetails.itemName);
                    break;
                case ItemType.Supply:
                    c.SetText("x" + invD.GetAmount().ToString() + " " + invD.itemDetails.itemName);
                    break;
                default:
                    c.SetText(invD.itemDetails.itemName);
                    break;
            }

            // set slot image and show button
            c.SetImage(invD.itemDetails.itemIcon);
            c.gameObject.SetActive(true);
        }
        else
        {
            // reset and hide the button from view
            c.SetText("");
            c.SetImage(null);
            c.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Adds a new button to the inventory UI.
    /// Only adds button if inventory slot is needed.
    /// </summary>
    /// 
    /// 2021-07-30  JH  Initial Documentation
    /// 
    public void AddButton()
    {
        if (inv.inventory.Count > buttonList.Count)
        {
            GameObject newButton = Instantiate(this.invButton);
            InventoryButton invButton = newButton.GetComponent<InventoryButton>();
            newButton.transform.SetParent(gridGroup.transform);
            newButton.transform.localScale = new Vector3(1, 1, 1);
            buttonList.Add(newButton);
            invButton.SetIndex(buttonList.Count - 1);
        }
    }

    /// <summary>
    /// Updates the number of steps for the scroll bar
    /// </summary>
    /// 
    /// 2021-07-30  JH  Initial Documentation
    /// 
    private void UpdateSBSteps()
    {
        scrollbar.numberOfSteps = buttonList.Count - (MAX_SIZE - 1);
    }
}
