using System.Collections;
using UnityEngine;

/// <summary>
/// 코코두기와 안드로이드 루틴 담당
/// 상호작용도 파츠 클래스로 만들까?
/// </summary>
public class LMCharacterRoutineControl
{
    private CocoDoogyBehaviour coco;
    private MasterBehaviour master;
    //private ILobbyCharactersEmotion emotion;
    private WaitForSeconds delay;
    //private float loading = 0f;
    //private float RoutineDelay = 5f;

    public LMCharacterRoutineControl(CocoDoogyBehaviour coco, MasterBehaviour master)
    {
        this.coco = coco;
        this.master = master;
        Debug.Log($"coco : {this.coco.gameObject.name}");
        Debug.Log($"master : {this.master.gameObject.name}");
    }

    public IEnumerator MainCharRoutineLoop()
    {
        while (true)
        {
            if (!coco.gameObject.activeSelf) yield return CocoRoutine();
            // {
            //     yield return delay = new (5f); // 로비 시작 후 좀 있다가 생성하는게 이쁘지 않을까?
            //     coco.gameObject.SetActive(true);
            //     coco.ResetRoutineComplete();
            // }
            if (!master.gameObject.activeSelf) yield return MasterRoutine();
            // {
            //     yield return delay = new (12f);
            //     master.gameObject.SetActive(true);
            // }
            yield return null;
        }
    }

    private IEnumerator CocoRoutine()
    {
        yield return delay = new (5f); // 로비 시작 후 좀 있다가 생성하는게 이쁘지 않을까?
        coco.gameObject.SetActive(true);
        coco.ResetRoutineComplete();
        yield break;
    }
    private IEnumerator MasterRoutine()
    {
        yield return delay = new (10f);
        master.gameObject.SetActive(true);
        yield break;
    }
}

// private IEnumerator MainCharRoutineLoop()
//     {
//         // while (true)
//         // {
//         //     // 루틴 시작
//         //     float activeDelay = UnityEngine.Random.Range(2, 5);

//         //     if (!coco.gameObject.activeSelf) coco.gameObject.SetActive(true);
//         //     yield return new WaitForSeconds(activeDelay);
//         //     if (!master.gameObject.activeSelf) master.gameObject.SetActive(true);

//         //     if (coco.IsRoutineComplete) coco.gameObject.SetActive(false);
//         //     if (master.IsRoutineComplete) master.gameObject.SetActive(false);
//         //     yield return new WaitUntil(() => coco.IsRoutineComplete);
//         // }
//         StartCoroutine(CocoRoutine());
//         //StartCoroutine(MasterRoutine());

//         yield return new WaitForSeconds(1f);
//     }

//     private IEnumerator CocoRoutine()
//     {
//         while (true)
//         {
//             if (!coco.gameObject.activeSelf) coco.gameObject.SetActive(true);
//             yield return new WaitUntil(() => coco.IsCMRoutineComplete);

//             coco.gameObject.SetActive(false);
//             yield return new WaitForSeconds(routineDelay);

//             coco.ResetRoutine();
//             coco.ResetInteract(0);
//             coco.ResetInteract(1);
//         }
//     }

//     private IEnumerator MasterRoutine()
//     {
//         while (true)
//         {
//             float activeDelay = UnityEngine.Random.Range(2, 5);
//             yield return new WaitForSeconds(activeDelay);
//             if (!master.gameObject.activeSelf) master.gameObject.SetActive(true);
//             yield return new WaitUntil(() => master.IsCMRoutineComplete);

//             master.gameObject.SetActive(false);
//             yield return new WaitForSeconds(routineDelay);

//             master.ResetRoutine();
//             master.ResetInteract(0);
//         }
//     }
