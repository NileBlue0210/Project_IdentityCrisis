using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 체공 상태를 관리하는 상태 클래스
/// </summary>
public class UnitAerialState : UnitState
{
    public UnitAerialState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 유닛이 지면에 닿았을 경우, 지상 상태로 전환
        if (stateMachine.Unit.UnitController.IsGrounded())
        {
            stateMachine.ChangeUnitState(stateMachine.GroundState);
        }
    }
}
