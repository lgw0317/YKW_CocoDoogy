using UnityEngine;

public class CamControl_Temp : MonoBehaviour
{
    Vector3 offset;
    public GameObject playerObj;
    void Start()
    {
        offset = transform.position;
    }

    void Update()
    {
        if (!playerObj) return;

        transform.position = playerObj.transform.position + offset;
    }
}
