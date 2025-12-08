using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class BGMPlayer : AudioPlayerControl
{
    private readonly AudioMixer mixer;
    private readonly Transform myTrans;
    public AudioSource currentSource;

    //private const string BGMFolderPath = "Sound/BGM/";

    public BGMPlayer(AudioMixer mixer, Transform myTrans, AudioMixerGroup group)
    {
        this.mixer = mixer;
        this.myTrans = myTrans;

        setVolume = 0.9f;
        ingameVolume = 0.32f;
        outgameVolume = 0.9f;
        
        GameObject gObj = new GameObject($"BGMPlayer");
        gObj.transform.parent = myTrans;
        currentSource = gObj.AddComponent<AudioSource>();
        activeSources.Add(currentSource);
        currentSource.outputAudioMixerGroup = group;
        currentSource.volume = setVolume;
        setVolume = currentSource.volume;
    }

    public void PlayAudio(AudioClip clip, float fadeIn, float fadeOut, bool loop, bool forcePlay = false)
    {

        if (!forcePlay && currentSource.isPlaying && currentSource.clip == clip) return;

        if (DOTween.IsTweening(currentSource, true)) currentSource.DOKill();
        currentSource.volume = 0f;
        Debug.Log($"BGMPlayer : DoTween fadeOut 끝 재생 시작");
        currentSource.clip = clip;
        currentSource.loop = loop;
        //currentSource.volume = 0f;
        currentSource.Play();
        currentSource.DOFade(setVolume, fadeIn);
    }

    public void PlayBGMForResources(AudioClip clip, float fadeIn, float fadeOut, bool loop)
    {
        if (currentSource.clip == clip && currentSource.isPlaying)
        {
            Debug.Log($"BGMPlayer : 리턴시킨다");
            return;
        }
        if (clip != null)
        {
            if (DOTween.IsTweening(currentSource, true)) currentSource.DOKill();
            currentSource.volume = 0f;
            currentSource.clip = clip;
            currentSource.loop = loop;
            //currentSource.volume = 0f;
            currentSource.Play();
            currentSource.DOFade(setVolume, fadeIn);
        }
        else
        {
            Debug.Log($"BGMPlayer : AudioClip 없음");
        }
    }
    
    public override void SetAudioPlayerState(AudioPlayerState state)
    {
        base.SetAudioPlayerState(state);
    }

    public override void ResetPlayer(AudioPlayerMode mode)
    {
        base.ResetPlayer(mode);
    }

    public override void SetVolume(float volume, float fadeDuration = 0.5F)
    {
        base.SetVolume(volume, fadeDuration);
    }

    public override void SetVolumeZero(bool which)
    {
        base.SetVolumeZero(which);
    }

}

