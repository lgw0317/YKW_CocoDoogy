using UnityEngine;
using UnityEngine.UI;

public class CheatButtonMethod : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(async () => { await FirebaseManager.Instance.UnlockAllStages(); Destroy(gameObject); });
    }
}
