using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitJumpState : UnitAerialState
{
    public UnitJumpState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitJumpState Enter");

        Jump();
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitJumpState Exit");
    }

    public override void Update()
    {
        base.Update();
    }

    // to do : 만약 현재 stateMachine.transform.Translate값이 존재할 경우, 점프 방향에 x값을 추가한다
    private void Jump()
    {
        Debug.Log("Do Jump");

        stateMachine.Unit.UnitController.Velocity = new Vector3(
            stateMachine.Unit.UnitController.Velocity.x,
            stateMachine.Unit.UnitController.JumpForce,
            stateMachine.Unit.UnitController.Velocity.z
        );
    }
}
