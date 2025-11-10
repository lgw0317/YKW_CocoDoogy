using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Audio Key = 5����
/// Audio Type = 5���� 
/// Ű�� �Է� ������ ����� Ÿ���� �������°� ���� �� ����.
/// </summary>

public interface IAudioLibrary
{
    AudioClip GetClipByEnum(Enum key, int index = -1);
}

public interface IAudioLibraryProvider
{
    AudioClip GetClip(AudioType type, Enum key, int index = -1);
}

public interface IAudioState
{
    void Enter(AudioManager audio);
    void Exit(AudioManager audio);
    void Update(AudioManager audio);
}

public interface IAudioGroupSetting
{
    AudioMixer GetMixer();
    AudioMixerGroup GetGroup(AudioType type);
}

public interface IAudioController
{
    void Init();
    void PostInit();

    void PlayPlayer();
    void PausePlayer();
    void ResumePlayer();
    void StopPlayer();
    void ResetPlayer();

}

