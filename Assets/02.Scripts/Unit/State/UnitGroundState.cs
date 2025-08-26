using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroundState : UnitState
{
    public UnitGroundState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>(); // Move Action에 매핑된 키가 감지되었을 때 입력값을 Vector2 형태로 읽어온다

        if (moveInput.magnitude > 0.1f)
        {
            // Move Action의 이벤트가 감지되었을 경우, groundWalk 상태로 전환한다
            stateMachine.ChangeUnitState(stateMachine.groundWalkState);
        }
        else
        {
            // Move Action의 이벤트가 감지되지 않을 경우, groundIdle 상태로 전환한다
            stateMachine.ChangeUnitState(stateMachine.groundIdleState);
        }
    }
}
