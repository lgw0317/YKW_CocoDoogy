using UnityEngine;

public class MasterBehaviour : BaseLobbyCharacterBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        agent.avoidancePriority = 98;
    }
    // 인터페이스 영역
    public override void OnCocoAnimalEmotion()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCocoMasterEmotion()
    {
        throw new System.NotImplementedException();
    }

    public override void OnLobbyEndDrag(Vector3 position)
    {
        base.OnLobbyEndDrag(position);
    }
    public override void OnLobbyInteract()
    {
        base.OnLobbyInteract();
    }
    public override void InNormal()
    {
        base.InNormal();
    }
    public override void InUpdate()
    {
        throw new System.NotImplementedException();
    }
    public override void StartScene()
    {
        throw new System.NotImplementedException();
    }
    public override void ExitScene()
    {
        throw new System.NotImplementedException();
    }
}
