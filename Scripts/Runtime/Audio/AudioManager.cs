using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

namespace TUFF
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Global Settings")]
        [Range(0f, 1f)] public float globalMusicVolume = 1f;
        [Range(0f, 1f)] public float globalSFXVolume = 1f;
        [Range(0f, 1f)] public float globalAmbienceVolume = 1f;
        [Header("References")]
        public AudioMixerGroup musicMixerGroup;
        public AudioMixerGroup sfxMixerGroup;
        public AudioMixerGroup ambientMixerGroup;
        public SFXManager sfxManager;

        public AudioSource ambsSource;


        [Header("Music Manager")]
        public BGMPlayerHandler bgmPlayer = null;
        [Tooltip("Currently stored Battle BGM")]
        [SerializeField] private BGM currentBattleBGM;
        [Tooltip("Currently playing AMBS")]
        [SerializeField] private AudioClip currentAMBS;
        [Header("AMBS Data")]
        [SerializeField] float ambsVolume = 1f;
        [SerializeField] float ambsPitch = 1f;
        [SerializeField] bool ambsLoop = true;
        [SerializeField] private float ambsTargetVolume = 1f;

        #region Singleton
        public static AudioManager instance;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;

                //if (auIntro) auIntro.outputAudioMixerGroup = musicMixerGroup;
                //if (auLoop) auLoop.outputAudioMixerGroup = musicMixerGroup;
                //if (battleAuIntro) battleAuIntro.outputAudioMixerGroup = musicMixerGroup;
                //if (battleAuLoop) battleAuLoop.outputAudioMixerGroup = musicMixerGroup;
                //if (ambsSource) ambsSource.outputAudioMixerGroup = ambientMixerGroup;
                if (bgmPlayer) bgmPlayer.Initialize();
                if (sfxManager) sfxManager.InitializeSources();
            }
        }
        #endregion

        private void Start()
        {
            UpdateVolumeFromConfig();
        }
        public void UpdateVolumeFromConfig()
        {
            globalMusicVolume = GameManager.instance.configData.globalMusicVolume;
            globalSFXVolume = GameManager.instance.configData.globalSFXVolume;
            globalAmbienceVolume = GameManager.instance.configData.globalAmbienceVolume;
            SetGlobalVolume();
        }
        public void Update()
        {
            SetGlobalVolume();
        }

        public void SetGlobalVolume()
        {
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", LISAUtility.PercentTodB(globalMusicVolume));
            sfxMixerGroup.audioMixer.SetFloat("SFXVolume", LISAUtility.PercentTodB(globalSFXVolume));
            ambientMixerGroup.audioMixer.SetFloat("AmbienceVolume", LISAUtility.PercentTodB(globalAmbienceVolume));
        }
        private void SetAmbience(AudioClip clip)
        {
            currentAMBS = clip;
            ambsSource.loop = ambsLoop;
            ambsSource.clip = clip;
            ambsSource.pitch = ambsPitch;
            ambsSource.Play();
        }
        public void PlayMusic(BGM bgm, float fadeInDuration = 0)
        {
            PlayMusic(new BGMPlayData(bgm), fadeInDuration);
        }
        public void PlayMusic(BGMPlayData bgmPlayData, float fadeInDuration = 0)
        {
            if (bgmPlayer) bgmPlayer.PlayMusic(bgmPlayData, fadeInDuration);
        }
        public void PlayBGMAsPreview(BGMPlayData bgmPlayData)
        {
            if (bgmPlayer) bgmPlayer.PlayBGMAsPreview(bgmPlayData);
        }
        public void PlaySFX(AudioClip clip, float volume, float pitch)
        {
            sfxManager.PlaySFX(clip, volume, pitch);
        }
        public void PlaySFX(SFX sfx)
        {
            if (sfx == null) return;
            sfxManager.PlaySFX(sfx.GetAudioClip(), sfx.GetVolume(), sfx.GetPitch());
        }
        public void PlayAMBS(AMBSPlayData ambsPlayData, float fadeInDuration = 0)
        {
            if (ambsPlayData == null) { StopAmbience(); return; }
            if (ambsPlayData.clip == null) { StopAmbience(); return; }
            if (ambsPlayData.clip == currentAMBS && ambsPlayData.pitch == ambsPitch && ambsPlayData.loop == ambsLoop) return;
            
            ambsVolume = ambsPlayData.volume;
            ambsPitch = ambsPlayData.pitch;
            ambsLoop = ambsPlayData.loop;
            UpdateVolumeFromConfig();

            // Add fade stuff here
            ambsTargetVolume = ambsVolume; //Set Volume
            ambsSource.volume = ambsTargetVolume; // Apply volume
            SetGlobalVolume();
            SetAmbience(ambsPlayData.clip);
        }
        public void PlayGameOverMusic()
        {
            PlayMusic(TUFFSettings.gameOverBGM);
        }
        public void StopMusic()
        {
            if (bgmPlayer) bgmPlayer.StopMusic();
        }
        public void StopSFXs()
        {
            sfxManager?.StopSFXs();
        }
        public void StopAmbience()
        {
            StopAmbienceClips();
            currentAMBS = null;
        }
        private void StopAmbienceClips()
        {
            ambsSource.Stop();
        }
        public void StopMusic(float fadeOutDuration)
        {
            if (bgmPlayer) bgmPlayer.StopMusic(fadeOutDuration);
        }
        public void ChangeAmbienceVolume(float volume, float fadeDuration = 0)
        {
            ambsSource.volume = volume;
        }
        public void RestoreAmbienceVolume()
        {
            ambsSource.volume = ambsTargetVolume;
        }
        public void FadeOutVolume(float timeDuration, bool stopOnComplete = false)
        {
            if (bgmPlayer) bgmPlayer.FadeOutVolume(timeDuration, stopOnComplete);
        }
        public void FadeInVolume(float timeDuration)
        {
            if (bgmPlayer) bgmPlayer.FadeInVolume(timeDuration);   
        }

        public static AudioClip SetClipLoop(AudioClip inputAudio, int startSample, int endSample, string nameSuffix)
        {
            return LISAUtility.CutAudioClip(inputAudio, startSample, endSample, nameSuffix);
        }
    }
}