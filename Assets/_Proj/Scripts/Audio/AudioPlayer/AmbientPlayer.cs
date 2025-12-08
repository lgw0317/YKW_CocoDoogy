using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientPlayer : AudioPlayerControl
{
    private readonly AudioMixer mixer;
    private readonly Transform myTrans;
    private readonly AudioPool audioPool;
    private MonoBehaviour coroutineHost;
    public AudioSource currentSource;

    public AmbientPlayer(AudioMixer mixer, Transform myTrans, AudioPool pool)
    {
        this.mixer = mixer;
        this.myTrans = myTrans;
        audioPool = pool;
        coroutineHost = myTrans.GetComponent<MonoBehaviour>();
        
        setVolume = 1f;
        ingameVolume = 1f;
        outgameVolume = 1f;
    }

    public void PlayAudio(AudioClip clip, AudioMixerGroup group, bool loop, bool pooled, Vector3? pos = null)
    {
        if (pooled)
        {
            currentSource = audioPool.GetSource();
            currentSource.spread = 0f; // 환경음은 좌우에 따라 다른 소리가 더 어울리지 않을까?
        }
        else
        {
            GameObject gObj = new GameObject($"AmbientPlayer");
            gObj.transform.parent = myTrans;
            currentSource = gObj.AddComponent<AudioSource>();
            currentSource.dopplerLevel = 0f;
            currentSource.reverbZoneMix = 0f;
            activeSources.Add(currentSource);
            Debug.Log($"{activeSources}�� {currentSource.name} ���� ����");
        }
        currentSource.outputAudioMixerGroup = group;
        currentSource.clip = clip;
        currentSource.loop = loop;

        // ��ġ�� �ִٸ� 3D �����
        if (pos.HasValue)
        {
            currentSource.transform.position = pos.Value;
            currentSource.spatialBlend = 1f;

            #region Ŀ����
            //src.rolloffMode = AudioRolloffMode.Custom;
            //AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.9f, 0.5f), new Keyframe(0.3f, 0.3f), new Keyframe(1f, 0f));
            //src.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            //src.minDistance = 0.3f;  // float �� �̳��� �׻� �ִ� ����
            //src.maxDistance = 5f; // float �� �̻��� �� �鸲
            #endregion

            #region �븻
            // currentSource.rolloffMode = AudioRolloffMode.Logarithmic; // �ڿ������� ����
            // currentSource.minDistance = 1f;  // float �� �̳��� �׻� �ִ� ����
            // currentSource.maxDistance = 50f; // float �� �̻��� �� �鸲
            #endregion
        }
        else currentSource.spatialBlend = 0f;

        // currentSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        // currentSource.volume = UnityEngine.Random.Range(0.95f, 1f);

        // ��� �� ����
        if (pooled)
        {
            currentSource.Play();
            coroutineHost.StartCoroutine(audioPool.ReturnAfterDelay(currentSource, clip.length));
        }
        if (!pooled)
        {
            //play
            currentSource.Play();
            // loop�϶� ���� �� ���°� �߰� �ؾ���
            if (loop) { }
            else NewDestroy(currentSource.gameObject, clip.length);
        }
    }

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

    private void NewDestroy(GameObject gObj, float length)
    {
        AudioSource aS = gObj.GetComponent<AudioSource>();
        UnityEngine.Object.Destroy (gObj, length);
        if (gObj.IsDestroyed())
        {
            Debug.Log($"{activeSources}�� {aS.name} ������");
            activeSources.Remove(aS);
        }
    }

}
