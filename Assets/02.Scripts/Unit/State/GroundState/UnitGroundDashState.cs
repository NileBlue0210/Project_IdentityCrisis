using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// to do : 대시 스테이트 쪽에서 델리게이트에 대시 이벤트를 묶는 처리를 넣도록 하고, 대시 감지를 SequenceController에서 받아 처리
/// 연속 입력이 발생했을 때, InputManager에서 SequenceController에 이벤트를 발생시키도록 하거나
/// 특정 키(방향키 등)에 한해서만 이벤트를 호출하도록 구현해 볼 것
public class UnitGroundDashState : UnitGroundState
{
    public Action DashAction;   // 대시 입력 시 동작시킬 델리게이트
    private int dashDirection;

    public UnitGroundDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundDashState Enter");
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundDashState Exit");
    }

    public override void Update()
    {
        base.Update();
    }

    public void SetDashDirection(int direction)
    {
        dashDirection = direction;
    }
}
