using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Threading.Tasks;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController Instance;

    public VideoPlayer player;

    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;

        // 중복등록 방지
        player.loopPointReached -= OnFinished;
        player.errorReceived -= OnError;

        // 이벤트 등록
        player.loopPointReached += OnFinished;
        player.errorReceived += OnError;
    }

    //-------------------------------------------
    // 1) StageManager가 요구하는 코루틴 방식
    //-------------------------------------------
    public IEnumerator PlayCutscene(string url, bool waitUntilFinish)
    {
        yield return PlayRoutine(url, waitUntilFinish);
    }

    //-------------------------------------------
    // 2) StageManager가 요구하는 async/await 방식
    //-------------------------------------------
    public async Task PlayAsync(string url)
    {
        var tcs = new TaskCompletionSource<bool>();

        StartCoroutine(PlayRoutine(url, true, () =>
        {
            tcs.SetResult(true);
        }));

        await tcs.Task;
    }

    //-------------------------------------------
    // 공통 실행 로직
    //-------------------------------------------
    IEnumerator PlayRoutine(string url, bool waitUntilFinish, System.Action onFinish = null)
    {
        Debug.Log("[Cutscene] Load: " + url);

        isPlaying = true;

        AudioManager.Instance.StopAllAudioGroup();

        player.Stop();
        player.source = VideoSource.Url;
        player.url = url;
        player.Prepare();

        float timeout = 5f;
        while (!player.isPrepared)
        {
            timeout -= Time.deltaTime;
            if (timeout < 0)
            {
                Debug.LogError("[Cutscene] Prepare Timeout!");
                yield break;
            }
            yield return null;
        }

        Debug.Log("[Cutscene] Playing: " + url);
        player.Play();

        if (waitUntilFinish)
        {
            while (isPlaying)
                yield return null;
        }

        onFinish?.Invoke();
    }

    void OnFinished(VideoPlayer vp)
    {
        Debug.Log("[Cutscene] Finished.");
        isPlaying = false;

        StageUIManager.Instance.videoImage.SetActive(false);
        AudioManager.Instance.ResetAllAudioGroup();
    }

    void OnError(VideoPlayer vp, string msg)
    {
        Debug.LogError("[Cutscene] ERROR: " + msg);
        isPlaying = false;
    }
}