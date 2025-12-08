using UnityEngine;
using UnityEngine.Audio;

public class AmbientGroup : BaseAudioGroup
{
    [Header("Pooling Settings")]
    [SerializeField] private int poolSize = 5;

    private AmbientPlayer player;
    private AudioPool audioPool;

    public override void Init()
    {
        base.Init();
        Debug.Log($"AmbientGroup Mixer : {mixer.name}");
        group = AudioManager.AudioGroupProvider.GetGroup(AudioType.Ambient);
        Debug.Log($"AmbientGroup MixerGroup : {group}");
        audioPool = new AudioPool(transform, group, poolSize);
        player = new AmbientPlayer(mixer, transform, audioPool);
    }

    public override void PostInit() { }
    // ����� ����
    public void PlayAmbient(AudioClip clip, bool loop, bool pooled, Vector3? pos = null)
    {
        player.PlayAudio(clip, group, loop, pooled, pos);
    }


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
    
    

    public void CustomPlayerControl()
    {
        // ?필요없음
    }

}
