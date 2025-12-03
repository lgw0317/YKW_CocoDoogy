using UnityEngine;

public class EndBlock : Block
{
    IStageManager stage;
    private bool isend = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        isend = false;
    }

    public void Init(IStageManager stage)
    {
        this.stage = stage;
    }


    //???????????
    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("충돌 감지되긴 함");

        if (isend) return;

        if (collision.CompareTag("Player"))
        {
            isend = true;

            if (stage is StageManager stageM)
            {
                stageM.ClearStage();
                
            }
            else if (stage is TutorialStageManager tutorial)
            {
                tutorial.ClearStage();
                
            }
        }
    }

}
