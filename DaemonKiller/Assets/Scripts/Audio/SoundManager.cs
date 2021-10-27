using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAsset;

namespace Audio
{
    /// <summary>
    /// Based on https://www.youtube.com/watch?v=QL29aTa7J5Q&ab_channel=CodeMonkey
    /// The purpose of this script is to play various sounds of the game.
    /// Whether it's an enemies sounds or players it will play them.
    /// It will also change what song plays in the overworld.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB)
    /// 
    /// dictSoundTimer              A dictionary that stores all the sounds repective cooldowns if they have any.
    /// cameraSong                  The initial song of a scene.
    /// dictSongReset               Reset times of a song.
    /// dictSoundSpecification      Specifications of a specific sound.
    /// 
    [RequireComponent(typeof(AudioSource))]
    public static class SoundManager
    {
        /// <summary>
        /// Sound specification purpose is solely to hold data that sounds or songs need to be played for audio sources.
        /// </summary>
        /// 
        /// Author: Rozario (Ross) Beaudin
        /// 
        /// Public Variables:
        /// mute                Determine if we need to mute the sound.
        /// byPassEffect        byPassEffect variable
        /// byPassListener      byPassListener variable
        /// byPassReverb        byPassReverb variable
        /// playOnAwake         Whether to play on awake
        /// loop                determines if a sound loops
        /// priority            Priority of a sound
        /// volume              Volume of a sound
        /// pitch               Pitch of a sound
        /// stereoPan           pan of a sound
        /// spatialBlend        Whether the sound is in 3D space or 2D space
        /// reverbZoneMix       reverb zone of the sound
        /// 
        private class SoundSpecifications
        {
            public bool mute = false;
            public bool byPassEffect = false;
            public bool byPassListener = false;
            public bool byPassReverb = false;
            public bool playOnAwake = false;
            public bool loop = false;

            [Range(0f, 256f)]
            public float priority = 0f;
            [Range(0f, 1f)]
            public float volume = 0f;
            [Range(-3f, 3f)]
            public float pitch = 0f;
            [Range(-1f, 1f)]
            public float stereoPan = 0f;
            [Range(0f, 1f)]
            public float spatialBlend = 0f;
            [Range(0f, 1.1f)]
            public float reverbZoneMix = 0f;

            /// <summary>
            /// Initialize variables.
            /// </summary>
            /// <param name="mute"></param>
            /// <param name="byPassEffect"></param>
            /// <param name="byPassListener"></param>
            /// <param name="byPassReverb"></param>
            /// <param name="playOnAwake"></param>
            /// <param name="loop"></param>
            /// <param name="priority"></param>
            /// <param name="volume"></param>
            /// <param name="pitch"></param>
            /// <param name="stereoPan"></param>
            /// <param name="spatialBlend"></param>
            /// <param name="reverbZoneMix"></param>
            /// 
            /// 2021-07-16 RB Initial documentation.
            /// 
            public SoundSpecifications(bool mute, bool byPassEffect, bool byPassListener, bool byPassReverb, 
                bool playOnAwake, bool loop, float priority, float volume, float pitch, float stereoPan, 
                float spatialBlend, float reverbZoneMix)
            {
                this.mute = mute;
                this.byPassEffect = byPassEffect;
                this.byPassListener = byPassListener;
                this.byPassReverb = byPassReverb;
                this.playOnAwake = playOnAwake;
                this.loop = loop;

                this.priority = priority;
                this.volume = volume;
                this.pitch = pitch;
                this.stereoPan = stereoPan;
                this.spatialBlend = spatialBlend;
                this.reverbZoneMix = reverbZoneMix;
            }
        }

        /// <summary>
        /// The enum of various sounds that could appear in the game.
        /// </summary>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public enum Sounds
        {
            PlayerAttack,
            PlayerForce,
            PlayerFire,
            PlayerElectric,
            PlayerIce,
            PlayerHurt,
            PlayerDie1,
            PlayerDie2,
            MagicForce,
            MagicFire,
            MagicIce,
            MagicElectricity,

            MedKit,
            MonsterEnergyDrink,

            SpiderDie,
            SpiderHurt,
            SpiderAttack,
            SpiderMove,

            CrawlerScream,
            CrawlerTheme,
            CrawlerAttack,
            CrawlerMove,

            ZombieHurt,
            ZombieDeath,
            ZombieAttack,
            ZombiePatrol,
            ZombieMove,

            BarrelExplosion,
            SlideGate,
            DoorOpen,
            DoorClose,
            DoorLock,

            StartGame,
            ExitGame,
            ReturnMainMenu
        }

        /// <summary>
        /// The enum of various so9unds that could appear in the game.
        /// </summary>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public enum Songs
        {
            Run,
            RunEnd
        }

        private static Dictionary<Sounds, float> dictSoundTimer;

        private static AudioClip cameraSong;
        private static Dictionary<Songs, float> dictSongReset;

        private static Dictionary<Sounds, SoundSpecifications> dictSoundSpecification;

        /// <summary>
        /// Initializes variables
        /// </summary>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static void Init()
        {
            dictSoundTimer = new Dictionary<Sounds, float>();
            dictSoundTimer[Sounds.PlayerAttack] = 0f;
            dictSoundTimer[Sounds.CrawlerScream] = 0f;
            dictSoundTimer[Sounds.SpiderMove] = 0f;
            dictSoundTimer[Sounds.ZombiePatrol] = 0f;
            dictSoundTimer[Sounds.ZombieAttack] = 0f;
            dictSoundTimer[Sounds.MedKit] = 0f;
            dictSoundTimer[Sounds.MonsterEnergyDrink] = 0f;
            dictSoundTimer[Sounds.MagicFire] = 0f;
            dictSoundTimer[Sounds.MagicElectricity] = 0f;
            dictSoundTimer[Sounds.MagicForce] = 0f;
            dictSoundTimer[Sounds.MagicIce] = 0f;
            dictSoundTimer[Sounds.ZombieHurt] = 0f;

            GameAssets.instanceAssets.CameraAudio = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
            cameraSong = GameAssets.instanceAssets.CameraAudio.clip;
            dictSongReset = new Dictionary<Songs, float>();
            dictSongReset[Songs.RunEnd] = 0f;

            dictSoundSpecification = new Dictionary<Sounds, SoundSpecifications>();
            dictSoundSpecification[Sounds.CrawlerScream] = 
                new SoundSpecifications(false, false, false, false, false, false, 0f, 0.67f, 0f, 0f, 0f, 0f);
            dictSoundSpecification[Sounds.BarrelExplosion] =
                new SoundSpecifications(false, false, false, false, false, false, 0f, 0.40f, 0f, 0f, 0f, 0f);
            dictSoundSpecification[Sounds.MedKit] = 
                new SoundSpecifications(false, false, false, false, false, false, 0f, 0.40f, 0f, 0f, 0f, 0f);
            dictSoundSpecification[Sounds.MonsterEnergyDrink] = 
                new SoundSpecifications(false, false, false, false, false, false, 0f, 0.40f, 0f, 0f, 0f, 0f);
        }

        /// <summary>
        /// Plays a sound in 2D Space.
        /// </summary>
        /// <param name="sound">Type of sound.</param>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static void PlaySound(Sounds sound)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundObj = new GameObject("Sound");
                AudioSource audioSource = soundObj.AddComponent<AudioSource>();
                audioSource = SetSoundSpecifications(sound, audioSource);
                AudioClip clip = GetAudioClip(sound);
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                    Object.Destroy(soundObj, clip.length);
                }
                else
                {
                    Object.Destroy(soundObj);
                }
                
            }
        }

        /// <summary>
        /// Delays an audio queue by X amount of seconds.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="sound"></param>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static IEnumerator DelayPlaySound(float delay, Sounds sound)
        {
            yield return new WaitForSeconds(delay);
            PlaySound(sound);
        }

        /// <summary>
        /// Play a sound in 3D space.
        /// </summary>
        /// <param name="sound">Type of sound.</param>
        /// <param name="position">Position of the sound.</param>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static void PlaySound(Sounds sound, Vector3 position)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundObj = new GameObject("Sound");
                soundObj.transform.position = position;
                AudioSource audioSource = soundObj.AddComponent<AudioSource>();
                AudioSource setAudio = SetSoundSpecifications(sound, audioSource);
                audioSource = setAudio == null ? audioSource : setAudio;
                AudioClip clip = GetAudioClip(sound);
                if(clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.maxDistance = 25f;
                    audioSource.spatialBlend = 1f;
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                    audioSource.dopplerLevel = 0f;
                    audioSource.Play();
                    Object.Destroy(soundObj, clip.length);
                }
                else
                {
                    Object.Destroy(soundObj);
                }
            }
        }

        /// <summary>
        /// Determines if we can play a sound if is present in the dictSoundTimer.
        /// </summary>
        /// <param name="sound">Type of sound.</param>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        private static bool CanPlaySound(Sounds sound)
        {
            switch (sound)
            {
                case Sounds.PlayerAttack:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float attackIntervals = 0.01f;
                        if (lastPlayed + attackIntervals < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.CrawlerScream:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float attackIntervals = 2f;
                        if (lastPlayed + attackIntervals < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.ZombiePatrol:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float attackIntervals = 16f;
                        if (lastPlayed + attackIntervals < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.ZombieAttack:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float attackIntervals = 0.5f;
                        if (lastPlayed + attackIntervals < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.SpiderMove:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float attackIntervals = 0.4f;
                        if (lastPlayed + attackIntervals < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.MedKit:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float interval = 0.1f;
                        if (lastPlayed + interval < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.MonsterEnergyDrink:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float interval = 0.1f;
                        if (lastPlayed + interval < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.ZombieHurt:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float interval = 0.01f;
                        if (lastPlayed + interval < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                case Sounds.MagicIce:
                case Sounds.MagicFire:
                case Sounds.MagicForce:
                case Sounds.MagicElectricity:
                    if (dictSoundTimer.ContainsKey(sound))
                    {
                        float lastPlayed = dictSoundTimer[sound];
                        float interval = 0.1f;
                        if (lastPlayed + interval < Time.time)
                        {
                            dictSoundTimer[sound] = Time.time;
                            return true;
                        }
                    }
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Repeats a sound NOT FULLY TESTED
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="times"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static IEnumerator RepeatSounds(Sounds sound, int times, float wait)
        {
            for (int i = 0; i < times; i++)
            {
                GameObject soundObj = new GameObject("Sound");
                AudioSource audioSource = soundObj.AddComponent<AudioSource>();
                audioSource.PlayOneShot(GetAudioClip(sound));
                yield return new WaitForSeconds(wait);
            }
        }

        /// <summary>
        /// Get an audio clip for sounds.
        /// </summary>
        /// <param name="sound">Type of sound.</param>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        private static AudioClip GetAudioClip(Sounds sound)
        {
            foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.instanceAssets.arraySoundAudioClips)
            {
                if(soundAudioClip.soundType == sound)
                {
                    return soundAudioClip.audioClip;
                }
            }
            Debug.LogError("Sound " + sound + " not found.");
            return null;
        }

        /// <summary>
        /// Set the sound specification of an audio source.
        /// </summary>
        /// <param name="sound">Type of sound</param>
        /// <param name="source">source of a sound.</param>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        private static AudioSource SetSoundSpecifications(Sounds sound, AudioSource source)
        {
            switch (sound)
            {
                case (Sounds.CrawlerScream):
                    if (dictSoundSpecification.ContainsKey(sound))
                    {
                        source.volume = dictSoundSpecification[sound].volume;
                    }
                    return source;
                case (Sounds.BarrelExplosion):
                    if (dictSoundSpecification.ContainsKey(sound))
                    {
                        source.volume = dictSoundSpecification[sound].volume;
                    }
                    return source;
                case (Sounds.MedKit):
                    if (dictSoundSpecification.ContainsKey(sound))
                    {
                        source.volume = dictSoundSpecification[sound].volume;
                    }
                    return source;
                case (Sounds.MonsterEnergyDrink):
                    if (dictSoundSpecification.ContainsKey(sound))
                    {
                        source.volume = dictSoundSpecification[sound].volume;
                    }
                    return source;
                default:
                    return source;
            }
        }

        /// <summary>
        /// Sets the overworld theme.
        /// </summary>
        /// <param name="song">Type of song.</param>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static void SetCameraTheme(Songs song)
        {
            AudioSource source = GameAssets.instanceAssets.CameraAudio;
            if(source.clip == GetAudioClip(song))
            {
                return;
            }
            source.Stop();
            source.clip = GetAudioClip(song);
            source.Play();
        }


        /// <summary>
        /// Resets the song to the original overworld song.
        /// </summary>
        /// <param name="song">Type of song.</param>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 2021-07-17 RB Updated it to be an Coroutine.
        /// 
        //public static void DynamicReset(Songs song)
        //{
        //    AudioClip currentClip = GameAssets.instanceAssets.CameraAudio.clip;
        //    if (dictSongReset.ContainsKey(song) && cameraSong != currentClip)
        //    {
        //        switch (song)
        //        {
        //            case (Songs.RunEnd):
        //                float lastPlayTime = dictSongReset[song];
        //                float duration = currentClip.length;
        //                if(lastPlayTime + duration < Time.time)
        //                {
        //                    dictSongReset[song] = Time.time;
        //                    ResetMusic();
        //                    return;
        //                }
        //                break;
        //        }
        //    }
        //}
        public static IEnumerator DynamicReset(Songs song)
        {
            AudioClip currentClip = GameAssets.instanceAssets.CameraAudio.clip;
            if (dictSongReset.ContainsKey(song) && cameraSong != currentClip)
            {
                switch (song)
                {
                    case (Songs.RunEnd):
                        //float lastPlayTime = dictSongReset[song];
                        //float duration = currentClip.length;
                        //if(lastPlayTime + duration < Time.time)
                        //{
                        //    dictSongReset[song] = Time.time;
                        //    ResetMusic();
                        //    return;
                        //}
                        // offset to end song faster.
                        yield return new WaitForSeconds(currentClip.length - 3f);
                        ResetMusic();
                        break;
                }
            }
        }

        /// <summary>
        /// Resets the music of to the original overworld song.
        /// </summary>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static void ResetMusic()
        {
            if(cameraSong == null)
            {
                return;
            }

            AudioSource source = GameAssets.instanceAssets.CameraAudio;
            source.Stop();
            source.clip = cameraSong;
            source.Play();
        }

        /// <summary>
        /// Get the audio clip of a song.
        /// </summary>
        /// <param name="song">Song Type.</param>
        /// <returns></returns>
        /// 
        /// 2021-07-16 RB Initial Documentation.
        /// 
        public static AudioClip GetAudioClip(Songs song)
        {
            foreach (GameAssets.SongClip songAudioClip in GameAssets.instanceAssets.arraySongClips)
            {
                if (songAudioClip.songType == song)
                {
                    return songAudioClip.audioClip;
                }
            }
            Debug.LogError("Sound " + song + " not found.");
            return null;
        }

    }
}

