using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class CutscenePlayer : AudioPlayerControl
{
    private readonly AudioMixer mixer;
    private readonly Transform myTrans;
    private AudioSource currentSource;

    public CutscenePlayer(AudioMixer mixer, Transform myTrans, AudioMixerGroup group)
    {
        this.mixer = mixer;
        this.myTrans = myTrans;
        GameObject gObj = new GameObject($"CutscenePlayer");
        gObj.transform.parent = myTrans;
        currentSource = gObj.AddComponent<AudioSource>();
        activeSources.Add(currentSource);
        currentSource.outputAudioMixerGroup = group;
        currentSource.volume = 0.7f;

        setVolume = currentSource.volume;
        ingameVolume = 0.7f;
        outgameVolume = 0.7f;
    }

    public AudioSource GetCutsceneAS()
    {
        return currentSource;
    }

    public override void ResetPlayer(AudioPlayerMode mode)
    {
        base.ResetPlayer(mode);
    }
    public override void SetAudioPlayerState(AudioPlayerState state)
    {
        base.SetAudioPlayerState(state);
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
