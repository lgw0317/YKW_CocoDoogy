using UnityEngine;
public class Turret : MonoBehaviour, ISignalSender
{
    [Header("Detection Settings")]
    public float detectRadius = 4f;
    [Range(30f, 180f)] public float fov = 90f;
    public LayerMask targetMask;
    public LayerMask occluderMask;
    public bool useOcclusion = true;

    [Header("Signal Settings")]
    [SerializeField] private MonoBehaviour receiverObject;
    public ISignalReceiver Receiver { get; set; }

    private bool wasDetected;

    [Header("Visual Range")]
    [SerializeField] private RingRange ring;
    [Tooltip("감지 중일 때 색상")] public Color detectedColor = new(1f, 0.3f, 0.3f, 0.4f);
    [Tooltip("기본 상태 색상")] public Color idleColor = new(0.3f, 1f, 0.3f, 0.2f);

    void Awake()
    {
        if (receiverObject is ISignalReceiver recv)
            Receiver = recv;
    }

    void Start()
    {
        ring.visibleAngle = fov;
        ring.forwardDir = transform.forward;
        ring.SetRadius(detectRadius);
    }

    void Update()
    {
        bool detected = DetectTarget();

        if (detected != wasDetected)
        {
            SendSignal();
            wasDetected = detected;

            // 색상 전환
            if (ring != null)
            {
                ring.fillColor = detected ? detectedColor : idleColor;
                ring.lineColor = detected ? detectedColor : idleColor;
                ring.RebuildAll();
            }
        }
    }

    public void ConnectReceiver(ISignalReceiver receiver)
    {
        Receiver = receiver;
        receiverObject = receiver as MonoBehaviour;
    }

    public void SendSignal()
    {
        if (Receiver != null)
            Receiver.ReceiveSignal();
        else
            Debug.Log("[Turret] 연결된 수신자 없음.");
    }

    bool DetectTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, detectRadius, targetMask, QueryTriggerInteraction.Ignore);
        if (cols.Length == 0) return false;

        foreach (var c in cols)
        {
            Vector3 dir = (c.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle > fov * 0.5f) continue;

            if (useOcclusion && IsLineBlocked(transform.position, c, occluderMask))
                continue;

            return true;
        }
        return false;
    }

    bool IsLineBlocked(Vector3 origin, Collider target, LayerMask blockMask)
    {
        Vector3 dir = target.bounds.center - origin;
        float dist = dir.magnitude;
        if (dist < 0.01f) return false;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, blockMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != target)
                return true;
        }
        return false;
    }
}
