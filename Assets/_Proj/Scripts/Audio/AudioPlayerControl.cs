using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioPlayerMode
{
    InGame = 0,
    OutGame
}

public enum AudioPlayerState
{
    Play = 0,
    Pause,
    Resume,
    Stop
}

public abstract class AudioPlayerControl
{
    protected MonoBehaviour mono; // 각 상속 받을 오디오 플레이어 mono를 각 오디오 그룹으로 잡아줄 거임.
    public List<AudioSource> activeSources = new List<AudioSource>();
    protected float setVolume;
    protected float ingameVolume;
    protected float outgameVolume;

    protected AnimationCurve fadeInCurve;
    protected bool doFadeIn;
    protected AnimationCurve fadeOutCurve;
    protected bool doFadeOut;
    protected AnimationCurve fadeCurve;

    // 본래 DOTween 구조는 맨 밑에 주석으로 백업은 했고 이제 어떻게 DOTween을 도려내고 적용할까
    public virtual void SetAudioPlayerState(AudioPlayerState state)
    {
        switch (state)
        {
            case AudioPlayerState.Play:
            PlayAll();
            break;
            case AudioPlayerState.Pause:
            PauseAll();
            break;
            case AudioPlayerState.Resume:
            ResumeAll();
            break;
            case AudioPlayerState.Stop:
            StopAll();
            break;
        }
    }

    public virtual void SetVolume(float volume, float fadeDuration = 0.5f)
    {
        foreach (var src in activeSources)
        {
            if (DOTween.IsTweening(src, true))
            {
                src.DOKill();
            }
            src.DOFade(volume, fadeDuration);
        }
    }

    public virtual void SetVolumeZero(bool which)
    {
        if (which)
        {
            foreach (var src in activeSources)
            {
                src.volume = 0f;
            }
        }
        else
        {
            foreach (var src in activeSources)
            {
                src.volume = setVolume;
            }
        }
    }
    
    public virtual void ResetPlayer(AudioPlayerMode mode)
    {
        foreach (var src in activeSources)
        {
            if (src != null)
            {
                if (DOTween.IsTweening(src, true)) src.DOKill();
                if (src.isPlaying) src.Stop();
                src.volume = 0f;
                src.clip = null;
                src.loop = false;
                src.pitch = 1f;
                switch (mode)
                {
                    case AudioPlayerMode.InGame:
                    setVolume = ingameVolume;
                    break;
                    case AudioPlayerMode.OutGame:
                    setVolume = outgameVolume;
                    break;
                }
                src.volume = setVolume;
            }
        }
    }
    
    protected void PlayAll()
    {
        foreach (var src in activeSources)
        {
            if (src != null && !src.isPlaying)
            {
                if (DOTween.IsTweening(src, true))
                {
                    src.DOKill();
                }
                if (src.volume != setVolume)
                {
                    src.volume = setVolume;
                    src.Play();
                }
                else
                {
                    src.Play();
                }
            }
        }
    }

    protected void PauseAll()
    {
        foreach (var src in activeSources)
        {
            if (src != null && src.isPlaying)
            {
                src.Pause();
            }
        }
    }

    protected void ResumeAll()
    {
        foreach (var src in activeSources)
        {
            if (src != null && !src.isPlaying)
            {
                src.UnPause();
            }
        }
    }

    protected void StopAll()
    {
        foreach (var src in activeSources)
        {
            if (src != null && src.isPlaying) 
            {
                src.Stop();
            }
        }
    }


    // DOTween을 사용하지 않을 시 페이드인 페이드아웃을 애니메이션 커브와 코루틴으로?

    /// <summary>
    /// mode : 0 = 일반, 1 = UnPause, 2 = Play()
    /// 후 페이드인 시작. hasVolumeValue : 0 == initVolume, 0 != hasVolumeValue 값으로 오디오 볼륨 값 설정
    /// </summary>
    /// <param name="fadeInTime"></param>
    /// <returns></returns>
    protected IEnumerator VolumeFadeIn(AudioSource src, float hasVolumeValue = 0, float fadeInTime = 0.5f, int mode = 0)
    {
        float timer = 0f;

        switch (mode)
        {
            case 0:
            break;
            case 1:
            src.volume = 0f;
            src.UnPause();
            break;
            case 2:
            src.volume = 0f;
            src.Play();
            break;
            default: throw new Exception("Mode 값은 0~2 만 있습니다.");
        }

        while (timer < fadeInTime)
        {
            src.volume = fadeInCurve.Evaluate(timer / fadeInTime) * setVolume;

            timer += Time.deltaTime;
            yield return null;
        }
        if (hasVolumeValue == 0)
        {
            src.volume = setVolume;
        }
        else if (hasVolumeValue != 0)
        {
            src.volume = hasVolumeValue;
        }
    }

    /// <summary>
    /// 현재 값에서 페이드 아웃 후
    /// hasVolumeValue : 해당 값으로 mode : 0 = 일반, 1 = Pause, 2 = Stop.
    /// </summary>
    /// <param name="fadeOutTime"></param>
    /// <returns></returns>
    protected IEnumerator VolumeFadeOut(AudioSource src, float hasVolumeValue = 0, float fadeOutTime = 0.5f, int mode = 0)
    {
        float currentVolume = src.volume;
        float timer = 0f;

        while (timer < fadeOutTime)
        {
            src.volume = fadeOutCurve.Evaluate(timer / fadeOutTime) * currentVolume;

            timer += Time.deltaTime;
            yield return null;
        }
        src.volume = hasVolumeValue;

        switch (mode)
        {
            case 0:
            break;
            case 1:
            src.Pause();
            break;
            case 2:
            src.Stop();
            break;
            default: throw new Exception("Mode 값은 0~2 만 있습니다.");
        }
    }

    protected IEnumerator VolumeAToB(AudioSource src, float targetVolume, float changeTime = 0.5f)
    {
        float timer = 0f;
        float startVolume = src.volume;
        float endVolume = targetVolume;

        while (timer < changeTime)
        {
            float evaluated = fadeCurve.Evaluate(timer / changeTime);
            src.volume = Mathf.Lerp(startVolume, endVolume, evaluated);
            timer += Time.deltaTime;
            yield return null;
        }
        src.volume = endVolume;
    }
}

// public virtual void ResetAll(float volumeValue)
// {
//     foreach (var src in activeSources)
//     {
//         if (src != null)
//         {
//             if (src.isPlaying) 
//             {
//                 if (DOTween.IsTweening(src, true))
//                 {
//                     src.DOKill();
//                 }
//                 src.DOFade(0, 0.2f).OnComplete(() => 
//                 {
//                     src.Stop();
//                     src.loop = false; 
//                     src.volume = initVolume;
//                     src.pitch = 1f;
//                     src.clip = null; 
//                 });
//             }
//             else
//             {
//                 src.loop = false;
//                 src.volume = initVolume;
//                 src.pitch = 1f;
//                 src.clip = null; 
//             }
//         }
//     }
// }