using UnityEngine;

public class BGMGroup : BaseAudioGroup
{
    private BGMPlayer player;
    private AudioSource audioS;

    public void PlayBGM(AudioClip clip, float fadeIn, float fadeOut, bool loop, bool forcePlay = false)
    {
        player.PlayAudio(clip, fadeIn, fadeOut, loop, forcePlay);
    }

    public void PlayBGMForResources(AudioClip clip, float fadeIn, float fadeOut, bool loop)
    {
        player.PlayBGMForResources(clip, fadeIn, fadeOut, loop);
    }

    // IAudioController 영역
    public override void Init()
    {
        base.Init();
        group = AudioManager.AudioGroupProvider.GetGroup(AudioType.BGM);
        Debug.Log($"BGMGroup : {group}");
        player = new BGMPlayer(mixer, transform, group);
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
