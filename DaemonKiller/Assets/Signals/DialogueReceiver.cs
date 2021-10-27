using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// The component that receives DialogueMarker signals
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// NOTE: Original source is linked below, project-specific additions made by TH.
/// https://github.com/Unity-Technologies/Timeline-MessageMarker/blob/master/Assets/MessageMarker/MessageReceiver.cs
/// 
/// private var     desc
/// gameUi          Component reference to the GameUI
/// 
public class DialogueReceiver : MonoBehaviour, INotificationReceiver
{
    private GameUI gameUi;

    /// <summary>
    /// Set up component references
    /// </summary>
    /// 
    /// 2021-07-02  TH  Initial Implementation
    /// 
    void Awake()
    {
        gameUi = GetComponent<GameUI>();
    }

    /// <summary>
    /// Receive text from Signal and send it to GameUI
    /// </summary>
    /// 
    /// 2021-07-02  TH  Initial Implementation
    /// 
    /// <param name="origin">Not used</param>
    /// <param name="notification">The Unity Signal</param>
    /// <param name="context">Not used</param>
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        var message = notification as DialogueMarker;
        if (message == null)             
            return; // do not proceed if the message doesn't exist       

        gameUi.SetText(message.dialogueText);
        //Debug.Log("Message Fired!");
    }
}
