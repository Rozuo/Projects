using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Functions related to the inventory button
/// </summary>
/// 
/// Author: Tyson Hoang (TH), Jacky Huynh (JH)
/// 
/// public var      desc
/// buttonText      reference to this button's text component
/// buttonTypeImage reference to this button's image component
/// 
/// private var     desc
/// buttonIndex     this button's index in the UI's list of buttons
/// 
public class InventoryButton : MonoBehaviour
{
    private int buttonIndex;
    public Text buttonText;
    public Image buttonTypeImage;

    /// <summary>
    /// Notifies the GameDirector that this button is used
    /// </summary>
    /// 
    /// 2021-05-11  TH  Inital Documentation
    /// 
    public void UseSlotItem()
    {
        // Attempt to use the item at index
        GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>().UseItem(buttonIndex);
    }

    /// <summary>
    /// Returns the button's index
    /// </summary>
    /// <returns>int</returns>
    /// 
    /// 2021-05-11  TH  Inital Documentation
    /// 
    public int GetIndex()
    {
        return buttonIndex;
    }

    /// <summary>
    /// Sets this button's index
    /// </summary>
    /// <param name="newIndex">The index to set the button to</param>
    /// 
    /// 2021-05-11  TH  Inital Documentation
    /// 
    public void SetIndex(int newIndex)
    {
        buttonIndex = newIndex;
    }

    /// <summary>
    /// Sets this button's text
    /// </summary>
    /// <param name="newText">New text to apply</param>
    /// 
    /// 2021-05-11  TH  Inital Documentation
    /// 
    public void SetText(string newText)
    {
        buttonText.text = newText;
    }

    /// <summary>
    /// Sets this button's image
    /// </summary>
    /// <param name="newImage">New sprite to apply</param>
    /// 
    /// 2021-05-11  TH  Inital Documentation
    /// 
    public void SetImage(Sprite newImage)
    {
        buttonTypeImage.sprite = newImage;
    }
}
