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
        float jumpDirection = 0f;
        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>(); // 대각선 점프를 위한 플레이어 좌 우 이동 입력 감지

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            jumpDirection = Mathf.Sign(moveInput.x) * stateMachine.Unit.UnitController.HorizontalJumpSpeed;   // 좌 우 입력에 따른 값 보정
        }

        stateMachine.Unit.UnitController.Velocity = new Vector3(
            jumpDirection,
            stateMachine.Unit.UnitController.JumpForce,
            stateMachine.Unit.UnitController.Velocity.z
        );
    }
}
