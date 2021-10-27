using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using CharacterMovement.Player;
using Interactable;

/// <summary>
/// The main script that overlooks the Game's User Interface (UI) and Heads Up Display (HUD)
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// public var      desc
/// fadeRate        How fast the screen's alpha changes
/// textObj         reference to general dialogue Text component
/// examineObj      reference to pop-up examination Text component
/// 
/// private var     desc
/// fadeObj         Reference to the Screen Fade Image Component
/// blackFade       If the screen should be fading black or not
/// textCoroutine   Used for the timed text
/// 
public class GameUI : MonoBehaviour
{
    public float fadeRate = 0.15f;
  
    [HideInInspector] public Text textObj;
    [HideInInspector] public Text examineObj;
    private Image fadeObj;
    private bool blackFade = false;
    private IEnumerator textCoroutine;

    /// <summary>
    /// Toggles if the screen should fade to black or not
    /// </summary>
    /// Context Menu methods can be activated through the Inspector (right-click the component).
    /// 
    /// 2021-07-27  TH  Initial Documentation
    /// 
    [ContextMenu("ToggleFade")]
    public void ToggleFade()
    {
        blackFade = !blackFade;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.gameObject.GetComponent<PlayerController>().ToggleState();
    }

    /// <summary>
    /// Starts the Fade timeline (for testing purposes)
    /// </summary>
    /// Context Menu methods can be activated through the Inspector (right-click the component).
    /// 
    /// 2021-07-27  TH  Initial Documentation
    /// 
    [ContextMenu("InvokeFade")]
    public void InvokeFade() {
        gameObject.GetComponent<PlayableDirector>().Play();      
    }

    /// <summary>
    /// Get components and references
    /// </summary>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 
    void Awake()
    {
        fadeObj = transform.Find("Fade").gameObject.GetComponent<Image>();
        textObj = transform.Find("Text").gameObject.GetComponent<Text>();
        examineObj = transform.Find("InteractActionText").gameObject.GetComponent<Text>();

        textObj.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets a permanent text onto the Game HUD. Use ClearText() to remove text.
    /// </summary>
    /// <param name="newText"></param>
    /// 
    /// 2021-06-01  TH  Initial Implementation
    /// 
    public void SetText(string newText)
    {
        textObj.text = newText;
        textObj.gameObject.SetActive(true);           
    }

    /// <summary>
    /// Clears existing text, and sets Text Component inactive.
    /// </summary>
    /// 
    /// 2021-06-02  TH  Initial Implementation
    /// 
    public void ClearText()
    {
        textObj.text = "";
        textObj.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the text and invokes the Coroutine.
    /// </summary>
    /// <param name="newText">The text to display</param>
    /// 
    /// 2021-05-24  TH  Initial Implementation
    /// 2021-05-28  TH  Modified to have Overload duration
    /// 
    public void ChangeTextTimed(string newText, float duration = 3.0f)
    {
        SetText(newText);

        if (textCoroutine != null)
            StopCoroutine(textCoroutine);

        textCoroutine = TextTimer(duration);
        StartCoroutine(textCoroutine);
    }

    /// <summary>
    /// Updates the pop-up interaction text when near interactable objects
    /// </summary>
    /// <param name="interactObj">The Interactable object</param>
    /// 
    /// 2021-07-27  TH  Initial Documentation, added support for Save objects
    /// 
    public void ChangeInteractText(GameObject interactObj)
    {
        if(interactObj == null || !examineObj.gameObject.activeSelf)
        {
            examineObj.text = "";
            return;
        }
            
        if(interactObj.GetComponent<ItemInteract>())
        {
            examineObj.text = ("< E > Pick Up");
        }
        else if (interactObj.GetComponent<DoorInteract>())
        {
            examineObj.text = ("< E > Open Door");
        }
        else if (interactObj.GetComponent<SaveInteract>())
        {
            examineObj.text = ("< E > Save Game");
        }
        else
        {
            examineObj.text = ("< E > Examine");
        }
    }

    /// <summary>
    /// Change the alpha of the Screen Fade Image
    /// </summary>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 
    private void LateUpdate()
    {
        if(blackFade && fadeObj.color.a < 1) //fade to black
        {
            Color currentAlpha = fadeObj.color;
            currentAlpha.a += fadeRate;
            fadeObj.color = currentAlpha;
        }
        else if (!blackFade && fadeObj.color.a > 0) //return to normal
        {
            Color currentAlpha = fadeObj.color;
            currentAlpha.a -= fadeRate;
            fadeObj.color = currentAlpha;
        }
    }

    /// <summary>
    /// Display the text for a set amount of time
    /// </summary>
    /// <returns></returns>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 
    private IEnumerator TextTimer(float duration)
    {       
        yield return new WaitForSeconds(duration);
        ClearText();
    }
}
