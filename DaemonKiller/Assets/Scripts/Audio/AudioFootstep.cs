using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays footsteps made by a GameObejct
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// Source: https://youtu.be/Bnm8mzxnwP8
/// 
/// private var     desc
/// footstepAudio   array of audio for footstep sounds
/// source          reference to the AudioSource component
/// 
//[RequireComponent(typeof(AudioSource))]
public class AudioFootstep : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepAudio;
    private AudioSource source;

    /// <summary>
    /// Set up component references
    /// </summary>
    /// 
    /// 2021-07-27  TH  Initial Documentation
    /// 
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play a random sound from the array of sounds available
    /// </summary>
    /// NOTE: This is activated through signals in the Animations
    /// 
    /// 2021-07-27  TH  Initial Documentation
    /// 
    private void MakeStepSound()
    {
        source.PlayOneShot(footstepAudio[Random.Range(0, footstepAudio.Length)]);
    }
}
