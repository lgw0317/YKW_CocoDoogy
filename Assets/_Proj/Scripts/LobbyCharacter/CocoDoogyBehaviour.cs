using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 코코두기가 안드로이드와 특정 범위 내에 있거나 랜덤 값으로
// 뽑힌 동물들에게 다가갔을때 로비 매니저에게 이벤트 호출, 로비매니저는
// 상호작용 실행.

public class CocoDoogyBehaviour : BaseLobbyCharacterBehaviour
{
    public Vector3 LastDragEndPos { get; private set; }
    public bool IsDragged { get; private set; }
    public bool IsCMInteract { get; private set; }
    public bool IsCAInteract { get; private set; }
    public bool IsRoutineComplete { get; private set; }

    protected override void InitStates()
    {
        IdleState = new LCocoDoogyIdleState(this, fsm);
        MoveState = new LCocoDoogyMoveState(this, fsm, charAgent, Waypoints);
        InteractState = new LCocoDoogyInteractState(this, fsm);
        ClickSate = new LCocoDoogyClickState(this, fsm, charAnim);
        DragState = new LCocoDoogyDragState(this, fsm);
        EditState = new LCocoDoogyEditState(this, fsm);
        StuckState = new LCocoDoogyStuckState(this, fsm);
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (fsm != null && fsm.CurrentState == MoveState) fsm.ChangeState(IdleState);
        IsDragged = false;
        LastDragEndPos = Waypoints[0].transform.position;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

    // 코코두기 루틴
    public void SetIsRoutineComplete(bool trueorfalse)
    {
        if (trueorfalse)
        {
            IsRoutineComplete = true;
        }
        else
        {
            IsRoutineComplete = false;
        }
    }
    /// <summary>
    /// 코코두기의 상호작용. 0 = 마스터, 1 = 동물들
    /// </summary>
    /// <param name="i"></param>
    public void SetTrueCharInteract(int i)
    {
        if (i == 0)
        {
            IsCMInteract = true;
        }
        else if (i == 1)
        {
            IsCAInteract = true;  
        }
    }
    public void ResetRoutineComplete()
    {
        IsRoutineComplete = false;
        IsCMInteract = false;
        IsCAInteract = false;
    }

    public void SetLastDragEndPos(Vector3 pos)
    {
        LastDragEndPos = pos;
    }
    public void SetIsDragged(bool which)
    {
        IsDragged = which;
    }

    // 인터페이스 영역
    public override void OnCocoAnimalEmotion()
    {
        
    }
    public override void OnCocoMasterEmotion()
    {

    }
    public override void OnLobbyBeginDrag(Vector3 position)
    {
        base.OnLobbyBeginDrag(position);
    }
    public override void OnLobbyDrag(Vector3 position)
    {
        base.OnLobbyDrag(position);
    }
    public override void OnLobbyEndDrag(Vector3 position)
    {
        base.OnLobbyEndDrag(position);
    }
    public override void OnLobbyClick()
    {
        base.OnLobbyClick();
    }
    public override void OnLobbyPress()
    {
        base.OnLobbyPress();
    }
    public override void InNormal()
    {
        base.InNormal();
    }
    public override void InEdit()
    {
        base.InEdit();
    }
    public override void Register()
    {
        base.Register();
    }
    public override void Unregister()
    {
        base.Unregister();
    }
    public override void Init()
    {
        base.Init();
    }
    public override void PostInit()
    {
        base.PostInit();
    }
    public override void LoadInit()
    {
        base.LoadInit();
        IsCMInteract = false;
        IsCAInteract = false;
        agent.avoidancePriority = 20;
    }
    public override void FinalInit()
    {
        base.FinalInit();
    }

}
