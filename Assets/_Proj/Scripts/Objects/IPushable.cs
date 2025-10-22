using UnityEngine;

public interface IPushable
{
    bool IsMoving { get; }
    Vector3 originPos { get; }
    Vector3 targetPos { get; }
}
