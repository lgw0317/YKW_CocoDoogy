using UnityEngine;

/// <summary>
/// 이 오브젝트가 어떤 데코 데이터(decoId)에서 생성된 건지 표시해주는 태그.
/// 저장/복원/보관/확정할 때 이 값으로 다시 DB를 찾는다.
/// </summary>
[DisallowMultipleComponent]
public class PlaceableTag_Deco : MonoBehaviour
{
    [Tooltip("어떤 DecoData(id)에서 생성됐는지")]
    public int decoId;
}
