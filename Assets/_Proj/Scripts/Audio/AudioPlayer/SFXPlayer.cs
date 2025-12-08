using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SFXPlayer : AudioPlayerControl
{
    private readonly AudioMixer mixer;
    private AudioMixerGroup group;
    private readonly Transform myTrans;
    private readonly AudioPool audioPool;
    private MonoBehaviour coroutineHost;
    private AudioSource currentPooledSource;
    private AudioSource currentNotPooledSource;

    public SFXPlayer(AudioMixer mixer, AudioMixerGroup group, Transform myTrans, AudioPool pool)
    {
        this.mixer = mixer;
        this.group = group;
        this.myTrans = myTrans;
        audioPool = pool;
        coroutineHost = myTrans.GetComponent<MonoBehaviour>();

        GameObject gObj = new GameObject($"NotPooledAudio");
        gObj.transform.parent = myTrans;
        currentNotPooledSource = gObj.AddComponent<AudioSource>();
        activeSources.Add(currentNotPooledSource);
        currentNotPooledSource.outputAudioMixerGroup = group;
        currentNotPooledSource.volume = 1f;
        currentNotPooledSource.dopplerLevel = 0f;
        currentNotPooledSource.reverbZoneMix = 0f;

        setVolume = currentNotPooledSource.volume;
        ingameVolume = 1f;
        outgameVolume = 1f;
        
        currentNotPooledSource.pitch = 1f;
        currentNotPooledSource.rolloffMode = AudioRolloffMode.Custom;
        AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(50f, 0.877f), new Keyframe(60f, 0.59f), new Keyframe(80f, 0.34f), new Keyframe(128f, 0.125f), new Keyframe(200f, 0.002f));
        currentNotPooledSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
        currentNotPooledSource.minDistance = 40f;
        currentNotPooledSource.maxDistance = 200f;
    }

    public void PlayAudio(AudioClip clip, bool loop, bool pooled, Vector3? pos = null)
    {
        if (pooled)
        {
            currentPooledSource = audioPool.GetSource();
            //currentPooledSource.outputAudioMixerGroup = group;
            //  3D 옵션
            if (pos.HasValue)
            {
                currentPooledSource.transform.position = pos.Value;
                currentPooledSource.spatialBlend = 1f;
            }
            else currentPooledSource.spatialBlend = 0f;

            currentPooledSource.clip = clip;
            currentPooledSource.loop = loop;

            // currentSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            // currentSource.volume = UnityEngine.Random.Range(0.95f, 1f);

            currentPooledSource.Play();
            coroutineHost.StartCoroutine(audioPool.ReturnAfterDelay(currentPooledSource, clip.length));
        }
        else
        {
            //if (currentNotPooledSource.clip == clip && currentNotPooledSource.isPlaying) return;
            //currentNotPooledSource.clip = clip;
            currentNotPooledSource.loop = loop;

            //  3D 옵션
            if (pos.HasValue)
            {
                currentNotPooledSource.transform.position = pos.Value;
                currentNotPooledSource.spatialBlend = 1f;
            }
            else currentNotPooledSource.spatialBlend = 0f;

            //play
            currentNotPooledSource.PlayOneShot(clip);
            // loop�϶� ���� �� ���°� �߰� �ؾ���
            if (loop) { }
        }
    }

    // 제어
    public override void ResetPlayer(AudioPlayerMode mode)
    {
        base.ResetPlayer(mode);
        audioPool.ResetPool();
        audioPool.SettingPool(mode);
    }
    public override void SetAudioPlayerState(AudioPlayerState state)
    {
        base.SetAudioPlayerState(state);
        audioPool.SetAudioPoolState(state);
    }
    public override void SetVolume(float volume, float fadeDuration = 0.5F)
    {
        base.SetVolume(volume, fadeDuration);
        audioPool.SetPoolVolume(volume, fadeDuration);
    }
    public override void SetVolumeZero(bool which)
    {
        base.SetVolumeZero(which);
        audioPool.SetVolumeZero(which);
    }
    //

    public void CustomPoolSourceInPool(AudioClip clip, int mode, float? volumeValue = null)
    {
        audioPool.CustomPoolSourceInPool(clip, mode, volumeValue);
    }

    private void NewDestroy(GameObject gObj, float length)
    {
        AudioSource aS = gObj.GetComponent<AudioSource>();
        UnityEngine.Object.Destroy(gObj, length);
        if (gObj.IsDestroyed())
        {
            activeSources.Remove(aS);
        }
    }
}
