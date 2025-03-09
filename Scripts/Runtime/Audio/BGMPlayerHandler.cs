using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TUFF
{
    public class BGMPlayerHandler : MonoBehaviour
    {
        [System.Serializable]
        public class BGMData
        {
            public float volume = 1f;
            public float pitch = 1f;
            public bool loop = true;
            public AudioClip introClip = null;
            public AudioClip loopClip = null;
            public float targetVolume = 1;
            public bool queueLoopMusic = false;
        }
        [Header("References")]
        public AudioSource sourceIntro = null;
        public AudioSource sourceLoop = null;
        [Header("BGM Data")]
        public BGM currentBGM = null;
        public BGMData data = new BGMData();

        private Tween tweenIntro;
        private Tween tweenLoop;

        

        public void Initialize()
        {
            if (sourceIntro) sourceIntro.outputAudioMixerGroup = AudioManager.instance.musicMixerGroup;
            if (sourceLoop) sourceLoop.outputAudioMixerGroup = AudioManager.instance.musicMixerGroup;
        }
        public void Update()
        {
            CheckQueuedMusic();
        }
        private void CheckQueuedMusic()
        {
            if (!data.queueLoopMusic) return;
            double introStart = AudioSettings.dspTime;
            sourceIntro.PlayScheduled(introStart);
            double introLength = (double)data.introClip.samples / data.introClip.frequency;
            introLength /= (data.pitch <= 0 ? 0.01f : data.pitch);
            sourceIntro.SetScheduledEndTime(introStart + introLength);
            sourceLoop.PlayScheduled(introStart + introLength);

            data.queueLoopMusic = false;
        }
        public void PlayBGMAsPreview(BGMPlayData bgmPlayData)
        {
            if (bgmPlayData == null) { return; }
            if (bgmPlayData.bgm == null) { return; }
            data.volume = bgmPlayData.volume;
            data.pitch = bgmPlayData.pitch;
            data.loop = bgmPlayData.loop;
            SetAndApplyVolume();
            SetMusic(bgmPlayData.bgm, true);
        }
        public void PlayMusic(BGMPlayData bgmPlayData, float fadeInDuration = 0)
        {
            if (bgmPlayData == null) { StopMusic(); return; }
            if (bgmPlayData.bgm == null) { StopMusic(); return; }
            if (bgmPlayData.bgm == currentBGM && bgmPlayData.pitch == data.pitch && bgmPlayData.loop == data.loop) return;
            data.volume = bgmPlayData.volume;
            data.pitch = bgmPlayData.pitch;
            data.loop = bgmPlayData.loop;
            if (AudioManager.instance) AudioManager.instance.UpdateVolumeFromConfig();
            if (fadeInDuration <= 0) SetAndApplyVolume();
            else FadeInVolume(fadeInDuration);
            SetMusic(bgmPlayData.bgm);
        }
        private void SetMusic(BGM bgm, bool forceInstantPlay = false)
        {
            currentBGM = bgm;
            StopClips();
            SetupMusicLoop(forceInstantPlay);
        }
        private void SetupMusicLoop(bool forceInstantPlay = false)
        {
            if (currentBGM.loopStart < 0 || currentBGM.loopEnd < 0)
            {
                Debug.LogWarning("Loop Start and Loop End must be 0 or higher.");
                return;
            }
            if (currentBGM.loopStart == currentBGM.loopEnd || !data.loop) // If BGM loops itself, or song doesn't loop, set the song to play from start to finish
            {
                data.introClip = null;
                data.loopClip = currentBGM.clip;
                sourceLoop.loop = data.loop;
                sourceLoop.clip = data.loopClip;
                sourceLoop.pitch = data.pitch;
                sourceLoop.Play();
            }
            else // If it loops dynamically
            {
                // Intro
                if (currentBGM.prebakedLoopIntro != null)
                {
                    data.introClip = currentBGM.prebakedLoopIntro;
                }
                else if (currentBGM.loopStart > 0)
                {
                    data.introClip = AudioManager.SetClipLoop(currentBGM.clip, 0, currentBGM.loopStart, "_introI");
                }
                if (data.introClip != null)
                {
                    sourceIntro.clip = data.introClip;
                    sourceIntro.loop = false;
                    sourceIntro.pitch = data.pitch;
                }

                // Loop
                if (currentBGM.prebakedLoopMain != null)
                {
                    data.loopClip = currentBGM.prebakedLoopMain;
                }
                else data.loopClip = AudioManager.SetClipLoop(currentBGM.clip, currentBGM.loopStart, currentBGM.loopEnd, "_loopI");
                if (data.loopClip != null)
                {
                    sourceLoop.clip = data.loopClip;
                    sourceLoop.loop = true;
                    sourceLoop.pitch = data.pitch;

                    if (currentBGM.loopStart > 0 || currentBGM.prebakedLoopMain != null)
                    {
                        data.queueLoopMusic = true;
                        if (forceInstantPlay) CheckQueuedMusic();
                    }
                    else sourceLoop.Play();
                }
            }
        }
        public void StopMusic()
        {
            StopClips();
            currentBGM = null;
        }
        public void StopMusic(float fadeOutDuration)
        {
            if (fadeOutDuration <= 0)
            {
                StopMusic();
                return;
            }
            FadeOutVolume(fadeOutDuration, true);
        }
        private void StopClips()
        {
            data.queueLoopMusic = false;
            sourceIntro.Stop();
            sourceLoop.Stop();
        }
        private void SetAndApplyVolume()
        {
            KillTweens();
            SetVolume();
            ApplyVolume();
        }
        private void KillTweens()
        {
            tweenIntro.Kill();
            tweenIntro = null;
            tweenLoop.Kill();
            tweenLoop = null;
        }
        private void SetVolume()
        {
            data.targetVolume = data.volume;
            if (AudioManager.instance) AudioManager.instance.SetGlobalVolume();
        }
        private void ApplyVolume()
        {
            float vol = data.targetVolume;
            sourceIntro.volume = vol;
            sourceLoop.volume = vol;
        }
        public void FadeInVolume(float timeDuration)
        {
            KillTweens();
            SetVolume();
            tweenIntro = sourceIntro.DOFade(data.targetVolume, timeDuration).From(0f).SetAutoKill();
            tweenLoop = sourceLoop.DOFade(data.targetVolume, timeDuration).From(0f).SetAutoKill();
        }
        public void FadeOutVolume(float timeDuration, bool stopOnComplete = false)
        {
            KillTweens();
            SetVolume();
            tweenIntro = sourceIntro.DOFade(0f, timeDuration).From(data.targetVolume).SetAutoKill();
            tweenLoop = sourceLoop.DOFade(0f, timeDuration).From(data.targetVolume).SetAutoKill().OnComplete(() => { if (stopOnComplete) StopMusic(); });
        }
    }

}
