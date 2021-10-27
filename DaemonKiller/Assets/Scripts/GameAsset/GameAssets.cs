using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace GameAsset
{
    /// <summary>
    /// Based on https://www.youtube.com/watch?v=QL29aTa7J5Q&ab_channel=CodeMonkey
    /// GameAssets is used mostly to preset game asset references between scenes.
    /// </summary>
    /// 
    /// Private Variable:
    /// _instanceAssets             The instance of the game assets.
    /// 
    /// Public Variables:
    /// arraySoundAudioClips        The array of all possible sounds.
    /// cameraAudio                 Audio source of the camera.
    /// arraySongClip               The array of all possible songs.
    /// 
    public class GameAssets : MonoBehaviour
    {
        protected static GameAssets _instanceAssets;

        /// <summary>
        /// Gets an instance of the GameAsset Prefab. It will instantiate the 
        /// Prefab if it doesn't already exist in the scene.
        /// </summary>
        /// 
        /// 2021-07-17 Initial Documentation.
        /// 
        public static GameAssets instanceAssets
        {
            get
            {
                if (_instanceAssets == null)
                    _instanceAssets = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _instanceAssets;
            }
        }

        //public static AudioClip[] GetArraySoundAudioClips()
        //{
        //    return SoundAssets.;
        //}
        [Header("Sounds")]
        public SoundAudioClip[] arraySoundAudioClips;

        /// <summary>
        /// A class that simplies stores data of a sound type and audio clip.
        /// </summary>
        /// 
        /// soundType       Type of sound.
        /// audioClip       The clip of audio.
        /// 
        [System.Serializable]
        public class SoundAudioClip
        {
            public SoundManager.Sounds soundType;
            public AudioClip audioClip;
        }

        [Header("Songs")]
        public AudioSource CameraAudio;
        public SongClip[] arraySongClips;

        /// <summary>
        /// A class that simplies stores data of all song types and it's repective audio clips.
        /// </summary>
        /// 
        /// Author: Rozario (Ross) Beaudin
        /// 
        /// Public Variable: 
        /// songType        Type of song.
        /// audioClip       The audio clip.
        /// 
        [System.Serializable]
        public class SongClip
        {
            public SoundManager.Songs songType;
            public AudioClip audioClip;
        }
    }
}

