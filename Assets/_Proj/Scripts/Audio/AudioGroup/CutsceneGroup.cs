using UnityEngine;

// 영상으로 대체. 이제 오디오소스만 관리해야함

public class CutsceneGroup : BaseAudioGroup
{
    private CutscenePlayer player;

    public AudioSource GetCutsceneSource()
    {
        return player.GetCutsceneAS();
    }

    // IAudioController 영역
    public override void Init()
    {
        base.Init();
        group = AudioManager.AudioGroupProvider.GetGroup(AudioType.Cutscene);
        Debug.Log($"CutsceneGroup : {group}");
        player = new CutscenePlayer(mixer, transform, group);
    }
    public override void PostInit() { }

    public override void ResetPlayer(AudioPlayerMode mode)
    {
        player.ResetPlayer(mode);
    }

    public override void SetAudioPlayerState(AudioPlayerState state)
    {
        player.SetAudioPlayerState(state);
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
