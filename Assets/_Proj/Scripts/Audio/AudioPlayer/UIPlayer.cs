using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class UIPlayer : AudioPlayerControl
{
    private readonly AudioMixer mixer;
    private readonly Transform myTrans;
    private AudioSource currentSource;

    public UIPlayer(AudioMixer mixer, Transform myTrans, AudioMixerGroup group)
    {
        this.mixer = mixer;
        this.myTrans = myTrans;
        GameObject gObj = new GameObject($"UIPlayer");
        gObj.transform.parent = myTrans;
        currentSource = gObj.AddComponent<AudioSource>();
        activeSources.Add(currentSource);
        currentSource.outputAudioMixerGroup = group;
        currentSource.loop = false;
        currentSource.volume = 0.5f;

        setVolume = currentSource.volume;
        ingameVolume = 1f;
        outgameVolume = 0.5f;
    }

    public void PlayAudio(AudioClip clip)
    {
        //if (currentSource.isPlaying && currentSource.clip == clip) return;
        currentSource.PlayOneShot(clip);
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
