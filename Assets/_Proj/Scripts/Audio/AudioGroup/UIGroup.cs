using UnityEngine;
using UnityEngine.Rendering;

public class UIGroup : BaseAudioGroup
{
    private UIPlayer player;

    public void PlayUI(AudioClip clip)
    {
        player.PlayAudio(clip);
    }

    // IAudioController 영역
    public override void Init()
    {
        mixer = AudioManager.AudioGroupProvider.GetMixer();
        group = AudioManager.AudioGroupProvider.GetGroup(AudioType.UI);
        Debug.Log($"UIGroup.cs : {group}, SFX 그룹이면 OK");
        player = new UIPlayer(mixer, transform, group);
    }
    public override void PostInit() { }

    public override void SetAudioPlayerState(AudioPlayerState state)
    {
        player.SetAudioPlayerState(state);
    }
    public override void ResetPlayer(AudioPlayerMode mode)
    {
        player.ResetPlayer(mode);
    }
    public override void SetVolume(float volume, float fadeDuration = 0.5F)
    {
        player.SetVolume(volume, fadeDuration);
    }
    public override void SetVolumeZero(bool which)
    {
        player.SetVolumeZero(which);
    }

}
