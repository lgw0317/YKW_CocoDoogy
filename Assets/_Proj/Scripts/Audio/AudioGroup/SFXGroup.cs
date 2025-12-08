using UnityEngine;
using UnityEngine.Audio;

public class SFXGroup : BaseAudioGroup
{
    [Header("Pooling Settings")]
    [SerializeField] private int poolSize = 10;

    private SFXPlayer player;
    private AudioPool audioPool;

    public void PlaySFX(AudioClip clip, bool loop, bool pooled, Vector3? pos = null)
    {
        player.PlayAudio(clip, loop, pooled, pos);
    }

    /// <summary>
    /// 특정 클립을 재생 중인 Pool 안에 상태를 제어하는 메서드
    /// int mode : 0 = 재생, 1 = 멈춤, 2 = 재개, 3 = 멈추고 재생 위치를 처음으로, 4 = 멈추고 클립 빼기, 5 = 볼륨 조절
    /// mode 4, 5는 float volumeValue 파라미터를 넣을 수 있음
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="mode"></param>
    /// <param name="volumeValue"></param>
    public void CustomPlayerControl(AudioClip clip, int mode, float? volumeValue = null)
    {
        player.CustomPoolSourceInPool(clip, mode, volumeValue);
    }

    // IAudioController 영역
    public override void Init()
    {
        base.Init();
        group = AudioManager.AudioGroupProvider.GetGroup(AudioType.SFX);
        Debug.Log($"SFXGroup : {group}");
        audioPool = new AudioPool(transform, group, poolSize);
        player = new SFXPlayer(mixer, group, transform, audioPool);
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
