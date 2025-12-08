using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public abstract class BaseAudioGroup : MonoBehaviour, IAudioController
{
    protected AudioMixer mixer;
    protected AudioMixerGroup group;
    
    public virtual void Init()
    {
        mixer = AudioManager.AudioGroupProvider.GetMixer();
    }
    public abstract void PostInit();

    public abstract void SetAudioPlayerState(AudioPlayerState state);
    public abstract void ResetPlayer(AudioPlayerMode mode);
    public abstract void SetVolume(float volume, float fadeDuration = 0.5F);
    public abstract void SetVolumeZero(bool which);
}
