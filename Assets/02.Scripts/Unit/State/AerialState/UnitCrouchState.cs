using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCrouchState : UnitGroundState
{
    public UnitCrouchState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundSitState::Enter");
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundSitState::Exit");
    }

    public override void Update()
    {
        base.Update();
    }
}
