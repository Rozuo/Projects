using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// A custom Unity Signal that can send strings
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// NOTE: Original source is linked below, project-specific additions made by TH.
/// https://github.com/Unity-Technologies/Timeline-MessageMarker/blob/master/Assets/MessageMarker/Message.cs
/// 
/// public var      desc
/// dialogueText    The string of text to send
/// retroactive     Equivalent to Signal's Retroactive
/// emitOnce        Equivalent to Signal's Emit Once
/// 
[System.Serializable, System.ComponentModel.DisplayName("Dialogue Marker")]
public class DialogueMarker : Marker, INotification, INotificationOptionProvider
{
    public string dialogueText;
    public bool retroactive;
    public bool emitOnce = true;

    public PropertyName id { get { return new PropertyName(); } }   

    NotificationFlags INotificationOptionProvider.flags
    {
        get
        {
            return (retroactive ? NotificationFlags.Retroactive : default(NotificationFlags)) |
                (emitOnce ? NotificationFlags.TriggerOnce : default(NotificationFlags));
        }
    }
}
