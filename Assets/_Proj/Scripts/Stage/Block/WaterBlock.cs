using UnityEngine;
using Water;
public class WaterBlock : Block
{
    Flow flow;

   void Awake()
    {
        flow = GetComponentInChildren<Flow>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        //flow.enabled = false;
    }
}
