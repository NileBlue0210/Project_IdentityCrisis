using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroundWalkState : UnitGroundState
{
    public UnitGroundWalkState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundWalkState Enter");
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundWalkState Exit");
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>();

        // 이동 입력이 없을 경우, groundIdle상태로 변경
        if (moveInput.magnitude < 0.1f)
        {
            stateMachine.ChangeUnitState(stateMachine.GroundIdleState);

            return;
        }

        Vector2 moveDirection = new Vector2(moveInput.x * stateMachine.Unit.UnitController.MoveSpeed, stateMachine.Unit.UnitController.Velocity.y);
        stateMachine.transform.Translate(moveDirection);
    }
}
