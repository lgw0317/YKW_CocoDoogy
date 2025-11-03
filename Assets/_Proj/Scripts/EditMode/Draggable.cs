using UnityEngine;

/// <summary>
/// - 드래그 가능한 오브젝트에 붙는 공통 컴포넌트
/// - 하이라이트 / 무효 표시 색상
/// - 위치/회전 PlayerPrefs 저장/로드
/// - 렌더러 자동 수집
/// </summary>
[DisallowMultipleComponent]
public class Draggable : MonoBehaviour
{
    #region === Inspector ===

    [Header("Highlight (선택/보관 상태 표현)")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color highlightColor = new(1f, 0.9f, 0.3f, 1f);
    [SerializeField] private Color invalidColor = new(1f, 0.2f, 0.2f, 1f);

    [Header("Transform 저장/로드")]
    [Tooltip("Start 시 PlayerPrefs에서 자동 로드할지 여부")]
    [SerializeField] private bool loadSavedTransformOnStart = true;

    [Tooltip("수동으로 저장 키를 지정하고 싶을 때")]
    [SerializeField] private string customSaveKey = "";

    [Header("고급")]
    [Tooltip("머티리얼 인스턴스를 안 만들고 색 변경할 때 권장")]
    [SerializeField] private bool useMaterialPropertyBlock = true;

    [Tooltip("셰이더 색상 프로퍼티명")]
    [SerializeField] private string colorPropName = "_Color";

    [Tooltip("저장 키에 전체 트랜스폼 경로를 포함할지 여부")]
    [SerializeField] private bool useFullTransformPathForKey = false;

    #endregion


    #region === State ===

    private Color[] originalColors;   // 렌더러별 원래 색
    private bool highlighted;
    private bool invalid;

    // PropertyBlock
    private MaterialPropertyBlock mpb;
    private int colorPropId = -1;

    // 셰이더 색상 후보들
    private static readonly string[] kColorPropCandidates = { "_BaseColor", "_Color", "_TintColor" };

    #endregion


    #region === Unity ===

    private void Awake()
    {
        EnsureRenderers();
        CacheOriginalColors();
        InitPropertyBlock();

        if (loadSavedTransformOnStart)
            LoadTransformIfAny();

        ApplyVisual();
    }

    private void OnValidate()
    {
        EnsureRenderers();
        if (colorPropId < 0)
            colorPropId = Shader.PropertyToID(colorPropName);
        ApplyVisual();
    }

    private void OnDestroy()
    {
        // PropertyBlock 으로 덮어쓴 거 제거
        if (useMaterialPropertyBlock && renderers != null)
        {
            foreach (var r in renderers)
            {
                if (!r) continue;
                r.SetPropertyBlock(null);
            }
        }
    }

    #endregion


    #region === Public API (Highlight & Validity) ===

    /// <summary>선택 등으로 하이라이트 켤 때</summary>
    public void SetHighlighted(bool on)
    {
        highlighted = on;
        ApplyVisual();
    }

    /// <summary>배치 불가일 때 빨간색</summary>
    public void SetInvalid(bool on)
    {
        invalid = on;
        ApplyVisual();
    }

    #endregion


    #region === Public API (Persistence) ===

    public void SavePosition() => SaveTransform();

    /// <summary>현재 위치/회전을 PlayerPrefs에 저장</summary>
    public void SaveTransform()
    {
        Vector3 p = transform.position;
        Quaternion q = transform.rotation;

        PlayerPrefs.SetFloat(BuildKey("x"), p.x);
        PlayerPrefs.SetFloat(BuildKey("y"), p.y);
        PlayerPrefs.SetFloat(BuildKey("z"), p.z);

        PlayerPrefs.SetFloat(BuildKey("qx"), q.x);
        PlayerPrefs.SetFloat(BuildKey("qy"), q.y);
        PlayerPrefs.SetFloat(BuildKey("qz"), q.z);
        PlayerPrefs.SetFloat(BuildKey("qw"), q.w);

        PlayerPrefs.Save();
    }

    public void LoadPositionIfAny() => LoadTransformIfAny();

    /// <summary>저장된 위치/회전이 있으면 불러온다.</summary>
    public void LoadTransformIfAny()
    {
        // 위치
        if (PlayerPrefs.HasKey(BuildKey("x")))
        {
            float x = PlayerPrefs.GetFloat(BuildKey("x"), transform.position.x);
            float y = PlayerPrefs.GetFloat(BuildKey("y"), transform.position.y);
            float z = PlayerPrefs.GetFloat(BuildKey("z"), transform.position.z);
            transform.position = new Vector3(x, y, z);
        }

        // 회전 (Quat 우선)
        if (PlayerPrefs.HasKey(BuildKey("qx")) &&
            PlayerPrefs.HasKey(BuildKey("qy")) &&
            PlayerPrefs.HasKey(BuildKey("qz")) &&
            PlayerPrefs.HasKey(BuildKey("qw")))
        {
            var q = new Quaternion(
                PlayerPrefs.GetFloat(BuildKey("qx"), transform.rotation.x),
                PlayerPrefs.GetFloat(BuildKey("qy"), transform.rotation.y),
                PlayerPrefs.GetFloat(BuildKey("qz"), transform.rotation.z),
                PlayerPrefs.GetFloat(BuildKey("qw"), transform.rotation.w)
            );
            transform.rotation = q;
        }
        else
        {
            // 예전 방식(Euler) 호환
            if (PlayerPrefs.HasKey(BuildKey("ry")))
            {
                var euler = new Vector3(
                    PlayerPrefs.GetFloat(BuildKey("rx"), transform.eulerAngles.x),
                    PlayerPrefs.GetFloat(BuildKey("ry"), transform.eulerAngles.y),
                    PlayerPrefs.GetFloat(BuildKey("rz"), transform.eulerAngles.z)
                );
                transform.eulerAngles = euler;
            }
        }

        Physics.SyncTransforms();
    }

    /// <summary>저장된 위치/회전 삭제</summary>
    public void DeleteSavedPosition()
    {
        PlayerPrefs.DeleteKey(BuildKey("x"));
        PlayerPrefs.DeleteKey(BuildKey("y"));
        PlayerPrefs.DeleteKey(BuildKey("z"));
        PlayerPrefs.DeleteKey(BuildKey("qx"));
        PlayerPrefs.DeleteKey(BuildKey("qy"));
        PlayerPrefs.DeleteKey(BuildKey("qz"));
        PlayerPrefs.DeleteKey(BuildKey("qw"));
        PlayerPrefs.DeleteKey(BuildKey("rx"));
        PlayerPrefs.DeleteKey(BuildKey("ry"));
        PlayerPrefs.DeleteKey(BuildKey("rz"));
        PlayerPrefs.Save();
    }

    #endregion


    #region === Visual ===

    /// <summary>현재 상태(highlight/invalid)에 맞춰 렌더러 색을 갱신.</summary>
    private void ApplyVisual()
    {
        if (renderers == null || renderers.Length == 0) return;

        bool useOriginal = !invalid && !highlighted;
        Color color = useOriginal
            ? Color.white
            : (invalid ? invalidColor : highlightColor);

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            // 사용할 프로퍼티 이름 결정
            string propToUse;
            var matForCheck = r.sharedMaterial ? r.sharedMaterial : (Application.isPlaying ? r.material : null);
            if (matForCheck && matForCheck.HasProperty(colorPropName))
                propToUse = colorPropName;
            else
                propToUse = DetectColorProp(r);

            if (useMaterialPropertyBlock && propToUse != null)
            {
                EnsureMPB();
                int pid = Shader.PropertyToID(propToUse);

                if (useOriginal && originalColors != null && i < originalColors.Length)
                    mpb.SetColor(pid, originalColors[i]);
                else
                    mpb.SetColor(pid, color);

                r.SetPropertyBlock(mpb);
            }
            else
            {
                // fallback
                bool isPlaying = Application.isPlaying;
                var mat = isPlaying ? r.material : r.sharedMaterial;
                if (!mat) continue;

                if (useOriginal && originalColors != null && i < originalColors.Length)
                    mat.color = originalColors[i];
                else
                    mat.color = color;
            }
        }
    }

    /// <summary>렌더러에서 쓸 수 있는 색상 프로퍼티 탐색</summary>
    private string DetectColorProp(Renderer r)
    {
        var mat = r ? (r.sharedMaterial ? r.sharedMaterial : (Application.isPlaying ? r.material : null)) : null;
        if (!mat) return null;
        foreach (var p in kColorPropCandidates)
        {
            if (mat.HasProperty(p)) return p;
        }
        return null;
    }

    private void EnsureRenderers()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    private void CacheOriginalColors()
    {
        if (renderers == null || renderers.Length == 0)
        {
            originalColors = null;
            return;
        }

        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r)
            {
                originalColors[i] = Color.white;
                continue;
            }

            var mat = r.sharedMaterial ? r.sharedMaterial : (Application.isPlaying ? r.material : null);
            originalColors[i] = mat && mat.HasProperty(colorPropName)
                ? mat.GetColor(colorPropName)
                : Color.white;
        }
    }

    private void InitPropertyBlock()
    {
        colorPropId = Shader.PropertyToID(colorPropName);
        if (useMaterialPropertyBlock && mpb == null)
            mpb = new MaterialPropertyBlock();
    }

    private void EnsureMPB()
    {
        if (mpb == null) mpb = new MaterialPropertyBlock();
        if (colorPropId < 0) colorPropId = Shader.PropertyToID(colorPropName);
    }

    #endregion


    #region === Key Helpers ===

    /// <summary>PlayerPrefs 저장 키 생성</summary>
    private string BuildKey(string suffix)
    {
        string baseKey;

        if (!string.IsNullOrEmpty(customSaveKey))
        {
            baseKey = customSaveKey;
        }
        else
        {
            var sceneName = gameObject.scene.name;
            var objPath = useFullTransformPathForKey ? GetFullPath(transform) : gameObject.name;
            baseKey = $"{sceneName}/{objPath}";
        }

        return $"{baseKey}:{suffix}";
    }

    private static string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = $"{t.name}/{path}";
        }
        return path;
    }

    #endregion
}
