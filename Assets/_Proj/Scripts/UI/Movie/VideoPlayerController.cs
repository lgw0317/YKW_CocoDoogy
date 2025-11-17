using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Threading.Tasks;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController Instance;

    public VideoPlayer player;

    //LSH 1117 추가
    private AudioSource cutsceneSource;

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

        cutsceneSource = AudioManager.Instance.GetAudioSourceForVideoPlayer();

        isPlaying = true;

        AudioManager.Instance.EnterCutscene();

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
        //LSH 1117 추가 좀더 생각해 보겠습니다.
        // for (ushort i = 0; i < player.audioTrackCount; i++)
        // {
        //     player.EnableAudioTrack(i, true);
        //     player.SetDirectAudioMute(i, true);
        // }
        // player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        // player.SetTargetAudioSource(0, cutsceneSource);
        // int count = player.audioTrackCount;
        //Debug.Log($"오디오 트랙 카운트 : {count}"):

        Debug.Log("[Cutscene] Playing: " + url);
        player.Play();
        Debug.Log("Is track enabled: " + player.IsAudioTrackEnabled(0));


        Debug.Log("Can set audio source? " + player.GetTargetAudioSource(0));

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
        //AudioManager.Instance.ResetAllAudioGroup();
    }

    void OnError(VideoPlayer vp, string msg)
    {
        Debug.LogError("[Cutscene] ERROR: " + msg);
        isPlaying = false;
    }
}