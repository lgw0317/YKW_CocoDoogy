using System.Collections;
using UnityEngine;

public class EnergyNoticePanel : MonoBehaviour
{
    public GameObject EnergyPanel;

    void OnEnable()
    {
        StartCoroutine(SelfDisable());
    }

    IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(2f);
        Destroy(EnergyPanel);
    }
}
