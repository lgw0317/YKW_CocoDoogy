using UnityEngine;

public class AudioGroupController
{
    private readonly BaseAudioGroup[] audioGroups;

    public AudioGroupController(BaseAudioGroup[] audioGroups)
    {
        this.audioGroups = audioGroups;
    }

    public void SetAllStateAudioGroup(AudioPlayerState state)
    {
        foreach (IAudioController aG in audioGroups)
        {
            aG.SetAudioPlayerState(state);
        }
    }

    public void ResetAllAudioGroup(AudioPlayerMode mode)
    {
        foreach (IAudioController aG in audioGroups)
        {
            aG.ResetPlayer(mode);
        }
    }

    /// <summary>
    /// true = 볼륨 0, false = 볼륨 1
    /// </summary>
    /// <param name="isEntering"></param>
    public void SetEnterChapterPanel(bool isEntering)
    {
        foreach (IAudioController aG in audioGroups)
        {
            if (aG is BGMGroup) continue;
            if (aG is UIGroup) continue;
            if (isEntering) aG.SetVolumeZero(true);
            else if (!isEntering) aG.SetVolumeZero(false);
        }
    }
    /// <summary>
    /// true = 다이얼로그 입장, false = 다이얼로그 퇴장
    /// </summary>
    /// <param name="isEntering"></param>
    public void SetDialogueState(bool isEntering)
    {
        foreach (IAudioController aG in audioGroups)
        {
                if (aG is UIGroup) continue;

            if (isEntering)
            {
                if (aG is DialogueGroup) aG.SetVolumeZero(false);
                aG.SetVolume(0.1f, 0.2f);
            } 
            else
            {
                if (aG is DialogueGroup) aG.SetVolumeZero(true);
                aG.SetVolumeZero(false);
            }
        }
    }

    /// <summary>
    /// true = 컷신 입장, false = 컷신 퇴장
    /// </summary>
    /// <param name="isEntering"></param>
    public void SetCutsceneState(bool isEntering)
    {
        foreach (IAudioController aG in audioGroups)
        {

            if (isEntering)
            {
                if (aG is CutsceneGroup) aG.SetVolumeZero(false);
                else aG.SetVolumeZero(true); 
            }
            else
            {
                if (aG is CutsceneGroup) aG.SetVolumeZero(true);
                else aG.SetVolumeZero(false);  
            }
        }
    }
}
