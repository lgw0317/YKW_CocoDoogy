using System.Collections;
using UnityEngine;

// BGM�� SFX�� �޸� ���⼭ ���������� �ְ� �����ϴ� ����
// Ư�� AudioManager�� DDOL�̴� �� ��ũ��Ʈ�� ���� ������Ʈ�� �ٸ� �� ���� �־ �ٸ� �뷡�� ���� ������ �˾Ƽ� ��� ����.
// SFX�� ��ũ���ͺ� ������Ʈ�� �� ����, �� ��ũ��Ʈ�� �ش� �� ����� �����ϴ� ����

public class SceneAudio : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] BGMKey key;
    [SerializeField] int bgmClipIndex;
    [SerializeField] float fadeInTime;
    [SerializeField] float fadeOutTime;
    [SerializeField] bool loop;

    [Header("IntroBGM")]
    [SerializeField] int introClipIndex;
    [SerializeField] float introDuration;
    [SerializeField] bool useIntro;

    // [Header("SceneMainCamera")]
    // [SerializeField] private Camera cam;

    public void StartBGM()
    {
       if (AudioManager.Instance != null)
       {
           if (useIntro)
           {
               StartCoroutine(PlayIntroBGM());
           }
           else
           {
               AudioManager.Instance.PlayAudio(key, bgmClipIndex, fadeInTime, fadeOutTime, loop);
           }
       }
    }

    private IEnumerator PlayIntroBGM()
    {
       AudioManager.Instance.PlayAudio(key, introClipIndex, fadeInTime, fadeOutTime, false);

       yield return new WaitForSeconds(introDuration);

       AudioManager.Instance.PlayAudio(key, bgmClipIndex, fadeInTime, fadeOutTime, loop);
       //if (cam != null)
       //{
       //    new WaitForSeconds(1f);
       //    var aL = cam.GetComponent<AudioListener>();
       //    aL.enabled = false;
       //}
    }
}


