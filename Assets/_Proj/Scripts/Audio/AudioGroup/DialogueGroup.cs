using UnityEngine;
using UnityEngine.Audio;

public class DialogueGroup : BaseAudioGroup
{
    private AudioMixerGroup bgm;
    private AudioMixerGroup sfx;
    private DialoguePlayer player;
    private AudioSource audioS;

    public void PlayDialogue(AudioType type, string audioFileName)
    {
        player.PlayDialogueAudio(type, audioFileName);
    }

    public override void Init()
    {
        base.Init();
        bgm = AudioManager.AudioGroupProvider.GetGroup(AudioType.DialogueBGM);
        sfx = AudioManager.AudioGroupProvider.GetGroup(AudioType.DialogueSFX);
        Debug.Log($"BGMGroup : {bgm}, SFXGroup : {sfx}");
        player = new DialoguePlayer(mixer, transform, bgm, sfx);
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
