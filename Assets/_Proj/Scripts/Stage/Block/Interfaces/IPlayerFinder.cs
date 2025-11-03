using UnityEngine;

public interface IPlayerFinder
{
    protected Transform Player { get; set; }
    void SetPlayerTransform(GameObject player)
    {
        Player = player.transform;
    }
}
